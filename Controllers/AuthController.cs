using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;
using BarberShopAPI.Server.Helpers;
using BarberShopAPI.Server.Services;
using BCrypt.Net;

namespace BarberShopAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BarberShopContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordPolicyService _passwordPolicyService;
        private readonly PasswordStrengthService _passwordStrengthService;

        public AuthController(BarberShopContext context, IConfiguration configuration, 
            PasswordPolicyService passwordPolicyService, PasswordStrengthService passwordStrengthService)
        {
            _context = context;
            _configuration = configuration;
            _passwordPolicyService = passwordPolicyService;
            _passwordStrengthService = passwordStrengthService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TenantLoginResponse>> Login(TenantLoginRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var tenantName = TenantHelper.GetCurrentTenantName(HttpContext);
            var isDefaultTenant = tenantName?.ToLower() == "default" || tenantName?.ToLower() == "clean cuts";
            
            var admin = await _context.Admins
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(a => a.Username.ToLower() == request.Username.ToLower() && 
                                        a.TenantId == tenantId && 
                                        a.IsActive);

            if (admin == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // For default tenant, allow default credentials
            if (isDefaultTenant && request.Username.ToLower() == "admin" && request.Password == "admin123")
            {
                var token = GenerateJwtToken(admin);
                return new TenantLoginResponse
                {
                    Token = token,
                    Username = admin.Username,
                    Name = admin.Name,
                    RequiresPasswordSetup = false,
                    TenantName = tenantName ?? "Clean Cuts",
                    IsDefaultTenant = true
                };
            }

            // For all other cases, verify password hash
            if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            var jwtToken = GenerateJwtToken(admin);
            return new TenantLoginResponse
            {
                Token = jwtToken,
                Username = admin.Username,
                Name = admin.Name,
                RequiresPasswordSetup = !admin.HasCustomPassword && !isDefaultTenant,
                TenantName = tenantName ?? "Unknown",
                IsDefaultTenant = isDefaultTenant
            };
        }

        private string GenerateJwtToken(Admin admin)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("TenantId", admin.TenantId.ToString()),
                new Claim("TenantSubdomain", admin.Tenant?.Subdomain ?? "default")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("setup")]
        public async Task<ActionResult> SetupAdmin()
        {
            // Check if admin already exists
            var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == "admin");
            if (existingAdmin != null)
            {
                return BadRequest(new { message = "Admin user already exists" });
            }

            // Create admin user
            var admin = new Admin
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Name = "Barber Admin",
                Email = "admin@barbershop.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok("Admin user created successfully");
        }

        [HttpPost("validate-password")]
        public ActionResult<PasswordValidationResponse> ValidatePassword([FromBody] string password)
        {
            var validationResult = _passwordPolicyService.ValidatePassword(password);
            var strength = _passwordStrengthService.CalculateStrength(password);

            return new PasswordValidationResponse
            {
                IsValid = validationResult.IsValid,
                Errors = validationResult.Errors,
                Strength = (PasswordStrengthLevel)strength,
                StrengthDescription = _passwordStrengthService.GetStrengthDescription(strength),
                StrengthColor = _passwordStrengthService.GetStrengthColor(strength)
            };
        }

        [HttpPost("generate-password")]
        public ActionResult<string> GeneratePassword([FromQuery] int length = 12)
        {
            if (length < 8 || length > 128)
            {
                return BadRequest(new { message = "Password length must be between 8 and 128 characters." });
            }

            var password = _passwordPolicyService.GenerateSecurePassword(length);
            return Ok(password);
        }

        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var adminId))
            {
                return Unauthorized("Invalid user context");
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Id == adminId && a.TenantId == tenantId && a.IsActive);

            if (admin == null)
            {
                return NotFound("Admin user not found");
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, admin.PasswordHash))
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            // Validate new password
            var validationResult = _passwordPolicyService.ValidatePassword(request.NewPassword);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Password validation failed", errors = validationResult.Errors });
            }

            // Update password
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("create-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> CreatePassword(CreatePasswordRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var adminId))
            {
                return Unauthorized("Invalid user context");
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Id == adminId && a.TenantId == tenantId && a.IsActive);

            if (admin == null)
            {
                return NotFound("Admin user not found");
            }

            // Validate password
            var validationResult = _passwordPolicyService.ValidatePassword(request.Password);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Password validation failed", errors = validationResult.Errors });
            }

            // Update password
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password created successfully" });
        }

        [HttpGet("password-requirements")]
        public ActionResult GetPasswordRequirements()
        {
            return Ok(new
            {
                minLength = 8,
                maxLength = 128,
                requireUppercase = true,
                requireLowercase = true,
                requireDigit = true,
                requireSpecialChar = true,
                minSpecialChars = 1,
                maxConsecutiveChars = 3,
                forbiddenPatterns = new[] { "password", "123456", "admin", "qwerty" }
            });
        }

        [HttpGet("auth-status")]
        public async Task<ActionResult<TenantAuthStatus>> GetAuthStatus()
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var tenantName = TenantHelper.GetCurrentTenantName(HttpContext);
            var isDefaultTenant = tenantName?.ToLower() == "default" || tenantName?.ToLower() == "clean cuts";

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.IsActive);

            if (admin == null)
            {
                return Ok(new TenantAuthStatus
                {
                    RequiresPasswordSetup = !isDefaultTenant,
                    IsDefaultTenant = isDefaultTenant,
                    TenantName = tenantName ?? "Unknown",
                    Message = isDefaultTenant ? "Use default credentials: admin/admin123" : "Admin account not found"
                });
            }

            return Ok(new TenantAuthStatus
            {
                RequiresPasswordSetup = !admin.HasCustomPassword && !isDefaultTenant,
                IsDefaultTenant = isDefaultTenant,
                TenantName = tenantName ?? "Unknown",
                Message = isDefaultTenant ? "Use default credentials: admin/admin123" : 
                          admin.HasCustomPassword ? "Password already set" : "Password setup required"
            });
        }

        [HttpPost("setup-password")]
        public async Task<ActionResult> SetupPassword(SetupPasswordRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var tenantName = TenantHelper.GetCurrentTenantName(HttpContext);
            var isDefaultTenant = tenantName?.ToLower() == "default" || tenantName?.ToLower() == "clean cuts";

            if (isDefaultTenant)
            {
                return BadRequest(new { message = "Password setup not allowed for default tenant" });
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.IsActive);

            if (admin == null)
            {
                return NotFound("Admin account not found");
            }

            if (admin.HasCustomPassword)
            {
                return BadRequest(new { message = "Password has already been set. Use password recovery if needed." });
            }

            // Validate password
            var validationResult = _passwordPolicyService.ValidatePassword(request.Password);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Password validation failed", errors = validationResult.Errors });
            }

            // Set password
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            admin.HasCustomPassword = true;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password set successfully" });
        }

        [HttpPost("recover-password")]
        public async Task<ActionResult> RecoverPassword(PasswordRecoveryRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var tenantName = TenantHelper.GetCurrentTenantName(HttpContext);
            var isDefaultTenant = tenantName?.ToLower() == "default" || tenantName?.ToLower() == "clean cuts";

            if (isDefaultTenant)
            {
                return BadRequest(new { message = "Password recovery not available for default tenant" });
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.IsActive && a.Email == request.Email);

            if (admin == null)
            {
                // Don't reveal if email exists or not for security
                return Ok(new { message = "If the email exists, a recovery link has been sent" });
            }

            // TODO: Implement email sending for password recovery
            // For now, just return success
            return Ok(new { message = "If the email exists, a recovery link has been sent" });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var tenantName = TenantHelper.GetCurrentTenantName(HttpContext);
            var isDefaultTenant = tenantName?.ToLower() == "default" || tenantName?.ToLower() == "clean cuts";

            if (isDefaultTenant)
            {
                return BadRequest(new { message = "Password reset not available for default tenant" });
            }

            // Validate JWT token
            var adminId = await ValidatePasswordResetTokenAsync(request.Token, tenantId);
            if (adminId == null)
            {
                return BadRequest(new { message = "Invalid, expired, or already used reset token" });
            }

            // Get admin
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Id == adminId && a.TenantId == tenantId && a.IsActive);

            if (admin == null)
            {
                return BadRequest(new { message = "Invalid reset token" });
            }

            // Validate new password
            var validationResult = _passwordPolicyService.ValidatePassword(request.NewPassword);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Password validation failed", errors = validationResult.Errors });
            }

            // Update password
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            admin.HasCustomPassword = true;
            admin.UpdatedAt = DateTime.UtcNow;

            // Save password changes to database
            await _context.SaveChangesAsync();

            // Mark token as used
            var tokenHash = BCrypt.Net.BCrypt.HashPassword(request.Token);
            var usedToken = new UsedPasswordResetToken
            {
                TokenHash = tokenHash,
                AdminId = adminId.Value, // adminId is guaranteed to be non-null here
                TenantId = tenantId,
                UsedAt = DateTime.UtcNow
            };

            _context.UsedPasswordResetTokens.Add(usedToken);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully" });
        }

        private string GeneratePasswordResetJwtToken(Admin admin, int tenantId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim("TenantId", tenantId.ToString()),
                new Claim("TokenType", "PasswordReset"),
                new Claim("Email", admin.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // 1 hour expiration for password reset
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<int?> ValidatePasswordResetTokenAsync(string token, int tenantId)
        {
            try
            {
                Console.WriteLine($"ValidatePasswordResetTokenAsync - Token: {token.Substring(0, Math.Min(50, token.Length))}...");
                Console.WriteLine($"ValidatePasswordResetTokenAsync - TenantId: {tenantId}");
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // No clock skew for password reset tokens
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                // Check if this is a password reset token
                var tokenType = principal.FindFirst("TokenType")?.Value;
                Console.WriteLine($"ValidatePasswordResetTokenAsync - TokenType: {tokenType}");
                if (tokenType != "PasswordReset")
                {
                    Console.WriteLine("ValidatePasswordResetTokenAsync - Invalid token type");
                    return null;
                }

                // Check tenant ID
                var tokenTenantId = principal.FindFirst("TenantId")?.Value;
                Console.WriteLine($"ValidatePasswordResetTokenAsync - TokenTenantId: {tokenTenantId}");
                if (!int.TryParse(tokenTenantId, out var parsedTenantId) || parsedTenantId != tenantId)
                {
                    Console.WriteLine("ValidatePasswordResetTokenAsync - Tenant ID mismatch");
                    return null;
                }

                // Get admin ID
                var adminIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"ValidatePasswordResetTokenAsync - AdminId: {adminIdClaim}");
                if (!int.TryParse(adminIdClaim, out var adminId))
                {
                    Console.WriteLine("ValidatePasswordResetTokenAsync - Invalid admin ID");
                    return null;
                }

                // Check if this specific token has already been used
                var usedTokens = await _context.UsedPasswordResetTokens
                    .Where(t => t.AdminId == adminId && t.TenantId == tenantId)
                    .ToListAsync();

                Console.WriteLine($"ValidatePasswordResetTokenAsync - Found {usedTokens.Count} used tokens for admin {adminId}");

                foreach (var usedToken in usedTokens)
                {
                    if (BCrypt.Net.BCrypt.Verify(token, usedToken.TokenHash))
                    {
                        Console.WriteLine("ValidatePasswordResetTokenAsync - Token already used");
                        return null; // This specific token already used
                    }
                }

                Console.WriteLine("ValidatePasswordResetTokenAsync - Token is valid and not used");
                return adminId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ValidatePasswordResetTokenAsync - Exception: {ex.Message}");
                return null;
            }
        }
    }
}