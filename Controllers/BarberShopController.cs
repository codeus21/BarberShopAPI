// Add this to your Controllers folder in the C# backend
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;
using BarberShopAPI.Server.Helpers;
using BCrypt.Net;

namespace BarberShopAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarberShopController : ControllerBase
    {
        private readonly BarberShopContext _context;
        private readonly ILogger<BarberShopController> _logger;

        public BarberShopController(BarberShopContext context, ILogger<BarberShopController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/BarberShop/current
        [HttpGet("current")]
        public async Task<ActionResult<BarberShop>> GetCurrentBarberShop()
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            var barberShop = await _context.BarberShops
                .FirstOrDefaultAsync(b => b.Id == tenantId);

            if (barberShop == null)
            {
                return NotFound();
            }

            return Ok(barberShop);
        }

        // PUT: api/BarberShop/current
        [HttpPut("current")]
        public async Task<IActionResult> UpdateCurrentBarberShop([FromBody] UpdateBarberShopRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            var barberShop = await _context.BarberShops
                .FirstOrDefaultAsync(b => b.Id == tenantId);

            if (barberShop == null)
            {
                return NotFound();
            }

            // Update properties
            barberShop.Name = request.Name ?? barberShop.Name;
            barberShop.BusinessPhone = request.BusinessPhone ?? barberShop.BusinessPhone;
            barberShop.BusinessAddress = request.BusinessAddress ?? barberShop.BusinessAddress;
            barberShop.BusinessHours = request.BusinessHours ?? barberShop.BusinessHours;
            barberShop.LogoUrl = request.LogoUrl ?? barberShop.LogoUrl;
            barberShop.ThemeColor = request.ThemeColor ?? barberShop.ThemeColor;
            barberShop.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(barberShop);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
        }

        // POST: api/BarberShop/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            var barberShop = await _context.BarberShops
                .FirstOrDefaultAsync(b => b.Id == tenantId);

            if (barberShop == null)
            {
                return NotFound();
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, barberShop.AdminPasswordHash))
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            // Hash new password
            barberShop.AdminPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            barberShop.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Password changed successfully" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }
        }
    }

    // DTOs for requests
    public class UpdateBarberShopRequest
    {
        public string? Name { get; set; }
        public string? BusinessPhone { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessHours { get; set; }
        public string? LogoUrl { get; set; }
        public string? ThemeColor { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
