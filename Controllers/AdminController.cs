using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Controllers
{
    [Authorize(Roles = "Admin")]
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
            var appointments = await _context.Appointments
                .Include(a => a.Service)
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
            var appointment = await _context.Appointments.FindAsync(id);
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
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Parse the time string to TimeSpan
            if (!TimeSpan.TryParse(request.NewTime, out TimeSpan newTime))
            {
                return BadRequest("Invalid time format. Use HH:MM:SS format.");
            }

            // Check if new time slot is available
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a =>
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
            var yesterday = DateTime.Today.AddDays(-1);

            // Get all confirmed appointments from past dates (including today but past times)
            var pastAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate <= yesterday && a.Status == "Confirmed")
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
    }


    public class RescheduleRequest
    {
        public DateTime NewDate { get; set; }
        public string NewTime { get; set; } = string.Empty; // Changed from TimeSpan to string
    }
}