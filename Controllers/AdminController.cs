using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;
using BarberShopAPI.Server.Helpers;
using BarberShopAPI.Server.Attributes;

namespace BarberShopAPI.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [TenantAuthorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly BarberShopContext _context;

        public AdminController(BarberShopContext context)
        {
            _context = context;
        }

        // GET: api/Admin/appointments
        [HttpGet("appointments")]
        public async Task<ActionResult<IEnumerable<object>>> GetAppointments()
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Where(a => a.TenantId == tenantId) // Filter by tenant
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Select(a => new
                {
                    a.Id,
                    a.CustomerName,
                    a.CustomerPhone,
                    a.CustomerEmail,
                    ServiceName = a.Service.Name,
                    a.AppointmentDate,
                    a.AppointmentTime,
                    a.Status,
                    a.Notes,
                    a.CreatedAt
                })
                .ToListAsync();

            return appointments;
        }

        // PUT: api/Admin/appointments/5/cancel
        [HttpPut("appointments/{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);
            
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/Admin/appointments/5/reschedule
        [HttpPut("appointments/{id}/reschedule")]
        public async Task<IActionResult> RescheduleAppointment(int id, [FromBody] RescheduleRequest request)
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);
            
            if (appointment == null)
            {
                return NotFound();
            }

            // Parse the time string to TimeSpan
            if (!TimeSpan.TryParse(request.NewTime, out TimeSpan newTime))
            {
                return BadRequest("Invalid time format. Use HH:MM:SS format.");
            }

            // Check if new time slot is available (within same tenant)
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a =>
                    a.TenantId == tenantId &&
                    a.AppointmentDate == request.NewDate.Date &&
                    a.AppointmentTime == newTime &&
                    a.Status != "Cancelled" &&
                    a.Id != id);

            if (existingAppointment != null)
            {
                return BadRequest("This time slot is already booked.");
            }

            appointment.AppointmentDate = request.NewDate.Date;
            appointment.AppointmentTime = newTime;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        // POST: api/Admin/cleanup-completed
        [HttpPost("cleanup-completed")]
        public async Task<IActionResult> CleanupCompletedAppointments()
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            var yesterday = DateTime.Today.AddDays(-1);

            // Get all confirmed appointments from past dates (within tenant)
            var pastAppointments = await _context.Appointments
                .Where(a => a.TenantId == tenantId && 
                           a.AppointmentDate <= yesterday && 
                           a.Status == "Confirmed")
                .ToListAsync();

            if (pastAppointments.Any())
            {
                foreach (var appointment in pastAppointments)
                {
                    appointment.Status = "Completed";
                    appointment.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = $"Marked {pastAppointments.Count} past appointments as completed" });
            }

            return Ok(new { message = "No past appointments to mark as completed" });
        }
        // DELETE: api/Admin/clear-past-appointments
        [HttpDelete("clear-past-appointments")]
        public async Task<IActionResult> ClearPastAppointments()
        {
            try
            {
                var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
                
                // Get all completed and cancelled appointments (within tenant)
                var pastAppointments = await _context.Appointments
                    .Where(a => a.TenantId == tenantId && 
                               (a.Status == "Completed" || a.Status == "Cancelled"))
                    .ToListAsync();

                if (!pastAppointments.Any())
                {
                    return Ok(new { message = "No past appointments found to clear." });
                }

                // Remove all past appointments
                _context.Appointments.RemoveRange(pastAppointments);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Successfully cleared {pastAppointments.Count} past appointment(s)."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error clearing past appointments: " + ex.Message });
            }
        }

        // GET: api/Admin/availability
        [HttpGet("availability")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailabilitySchedules()
        {
            var tenantId = TenantHelper.GetCurrentTenantId(HttpContext);
            
            try
            {
                var schedules = await _context.AvailabilitySchedules
                    .Where(a => a.TenantId == tenantId)
                    .OrderBy(a => a.ScheduleDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();

                Console.WriteLine($"Found {schedules.Count} schedules for tenant {tenantId}");
                foreach (var schedule in schedules)
                {
                    Console.WriteLine($"Schedule ID: {schedule.Id}, StartTime: {schedule.StartTime} (type: {schedule.StartTime.GetType()}), EndTime: {schedule.EndTime} (type: {schedule.EndTime.GetType()})");
                }

                var response = schedules.Select(a => new
                {
                    Id = a.Id,
                    ScheduleDate = a.ScheduleDate,
                    StartTime = $"{a.StartTime.Hours:D2}:{a.StartTime.Minutes:D2}",
                    EndTime = $"{a.EndTime.Hours:D2}:{a.EndTime.Minutes:D2}",
                    IsAvailable = a.IsAvailable,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();

                return Ok(response);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"FormatException in GetAvailabilitySchedules: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // If there's invalid TimeSpan data, clear all availability schedules for this tenant
                var invalidSchedules = await _context.AvailabilitySchedules
                    .Where(a => a.TenantId == tenantId)
                    .ToListAsync();
                
                Console.WriteLine($"Found {invalidSchedules.Count} invalid schedules to clear for tenant {tenantId}");
                
                _context.AvailabilitySchedules.RemoveRange(invalidSchedules);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Cleared all invalid schedules");
                return Ok(new List<object>()); // Return empty list
            }
        }

    }


    public class RescheduleRequest
    {
        public DateTime NewDate { get; set; }
        public string NewTime { get; set; } = string.Empty; // Changed from TimeSpan to string
    }
}