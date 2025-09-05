using Microsoft.AspNetCore.Mvc;
using BarberShopAPI.Server.Repositories;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentRepository _appointmentRepository;
        private readonly ServiceRepository _serviceRepository;

        public AppointmentsController(AppointmentRepository appointmentRepository, ServiceRepository serviceRepository)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _appointmentRepository.GetUpcomingAppointmentsAsync();
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

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
            var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                appointment.AppointmentDate, 
                appointment.AppointmentTime);

            if (!isAvailable)
            {
                return BadRequest("This time slot is already booked.");
            }

            // Validate service exists
            var service = await _serviceRepository.GetByIdAsync(appointment.ServiceId);
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
                CreatedAt = DateTime.UtcNow,
                TenantId = 0 // This will be set by the repository
            };

            var createdAppointment = await _appointmentRepository.AddAsync(newAppointment);
            return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointment.Id }, createdAppointment);
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

            // Get appointments for the date
            var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date);
            var bookedSlots = appointments
                .Where(a => a.Status != "Cancelled")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm\:ss"))
                .ToList();

            // Return available slots
            var availableSlots = timeSlots.Where(slot => !bookedSlots.Contains(slot)).ToList();

            return availableSlots;
        }
    }
}