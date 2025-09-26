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
        private readonly AvailabilityScheduleRepository _availabilityRepository;

        public AppointmentsController(AppointmentRepository appointmentRepository, ServiceRepository serviceRepository, AvailabilityScheduleRepository availabilityRepository)
        {
            _appointmentRepository = appointmentRepository;
            _serviceRepository = serviceRepository;
            _availabilityRepository = availabilityRepository;
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
        public async Task<ActionResult<Appointment>> CreateAppointment(CreateAppointmentRequest request)
        {
            // Parse the time string to TimeSpan
            if (!TimeSpan.TryParse(request.AppointmentTime, out var appointmentTime))
            {
                return BadRequest("Invalid time format. Please use HH:MM:SS format.");
            }

            // Check if the time slot is available in the schedule
            var isInSchedule = await _availabilityRepository.IsTimeSlotAvailableAsync(
                request.AppointmentDate, 
                appointmentTime);

            if (!isInSchedule)
            {
                return BadRequest("This time slot is not available in your schedule.");
            }

            // Check if the time slot is already booked
            var isBooked = await _appointmentRepository.IsTimeSlotAvailableAsync(
                request.AppointmentDate, 
                appointmentTime);

            if (!isBooked)
            {
                return BadRequest("This time slot is already booked.");
            }

            // Validate service exists
            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
            {
                return BadRequest("Invalid service selected.");
            }

            // Create a new appointment object to avoid validation issues
            var newAppointment = new Appointment
            {
                ServiceId = request.ServiceId,
                AppointmentDate = request.AppointmentDate,
                AppointmentTime = appointmentTime,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,
                Notes = request.Notes,
                Status = request.Status,
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
            // Get available time slots from the availability schedule
            var availableTimeSlots = await _availabilityRepository.GetAvailableTimeSlotsAsync(date);
            
            // If no availability schedule is set, return empty list (no times available)
            if (!availableTimeSlots.Any())
            {
                return Ok(new List<string>());
            }

            // Get appointments for the date
            var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date);
            var bookedSlots = appointments
                .Where(a => a.Status != "Cancelled")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm\:ss"))
                .ToList();

            // Return available slots that are not booked
            var availableSlots = availableTimeSlots.Where(slot => !bookedSlots.Contains(slot)).ToList();

            return availableSlots;
        }

        // GET: api/Appointments/booked-slots/2024-01-15
        [HttpGet("booked-slots/{date}")]
        public async Task<ActionResult<IEnumerable<string>>> GetBookedSlots(DateTime date)
        {
            // Get appointments for the date
            var appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date);
            var bookedSlots = appointments
                .Where(a => a.Status != "Cancelled")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm\:ss"))
                .ToList();

            return bookedSlots;
        }
    }
}