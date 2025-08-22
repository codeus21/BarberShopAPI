using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;
using BCrypt.Net;

namespace BarberShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BarberShopContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(BarberShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == request.Username && a.IsActive);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(admin);

            return new LoginResponse
            {
                Token = token,
                Username = admin.Username,
                Name = admin.Name
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
                new Claim(ClaimTypes.Role, "Admin")
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
                return BadRequest("Admin user already exists");
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
    }
}