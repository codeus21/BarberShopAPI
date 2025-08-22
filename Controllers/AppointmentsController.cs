using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly BarberShopContext _context;

        public AppointmentsController(BarberShopContext context)
        {
            _context = context;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            // Check if the time slot is available
            var existingAppointment = await _context.Appointments
                .FirstOrDefaultAsync(a =>
                    a.AppointmentDate == appointment.AppointmentDate &&
                    a.AppointmentTime == appointment.AppointmentTime &&
                    a.Status != "Cancelled");

            if (existingAppointment != null)
            {
                return BadRequest("This time slot is already booked.");
            }

            // Validate service exists and load it
            var service = await _context.Services.FindAsync(appointment.ServiceId);
            if (service == null)
            {
                return BadRequest("Invalid service selected.");
            }

            // Create a new appointment object to avoid validation issues
            var newAppointment = new Appointment
            {
                ServiceId = appointment.ServiceId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                CustomerName = appointment.CustomerName,
                CustomerPhone = appointment.CustomerPhone,
                CustomerEmail = appointment.CustomerEmail,
                Notes = appointment.Notes,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            // Return the appointment with service details
            var createdAppointment = await _context.Appointments
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == newAppointment.Id);

            return CreatedAtAction(nameof(GetAppointment), new { id = newAppointment.Id }, createdAppointment);
        }

        // GET: api/Appointments/available-slots/2024-01-15
        [HttpGet("available-slots/{date}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableSlots(DateTime date)
        {
            // Define business hours (8 AM to 8 PM, Monday-Friday)
            var timeSlots = new List<string>
            {
            "08:00:00", "09:00:00", "10:00:00", "11:00:00", "12:00:00",
            "13:00:00", "14:00:00", "15:00:00", "16:00:00", "17:00:00", "18:00:00", "19:00:00"
            };

            // Get booked slots for the date
            var bookedSlots = await _context.Appointments
                .Where(a => a.AppointmentDate == date && a.Status != "Cancelled")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm\:ss"))
                .ToListAsync();

            // Return available slots
            var availableSlots = timeSlots.Where(slot => !bookedSlots.Contains(slot)).ToList();

            return availableSlots;
        }
    }
}