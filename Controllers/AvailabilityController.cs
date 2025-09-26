using Microsoft.AspNetCore.Mvc;
using BarberShopAPI.Server.Repositories;
using BarberShopAPI.Server.Models;
using BarberShopAPI.Server.Attributes;

namespace BarberShopAPI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [TenantAuthorize]
    public class AvailabilityController : ControllerBase
    {
        private readonly AvailabilityScheduleRepository _availabilityRepository;

        public AvailabilityController(AvailabilityScheduleRepository availabilityRepository)
        {
            _availabilityRepository = availabilityRepository;
        }

        // GET: api/Availability
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvailabilityScheduleResponse>>> GetAvailabilitySchedules()
        {
            var schedules = await _availabilityRepository.GetAllSchedulesAsync();
            var response = schedules.Select(s => new AvailabilityScheduleResponse
            {
                Id = s.Id,
                ScheduleDate = s.ScheduleDate,
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // GET: api/Availability/date/2024-01-15
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<AvailabilityScheduleResponse>>> GetAvailabilityByDate(DateTime date)
        {
            var schedules = await _availabilityRepository.GetByDateAsync(date);
            var response = schedules.Select(s => new AvailabilityScheduleResponse
            {
                Id = s.Id,
                ScheduleDate = s.ScheduleDate,
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // GET: api/Availability/available-slots/2024-01-15
        [HttpGet("available-slots/{date}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableSlots(DateTime date)
        {
            var timeSlots = await _availabilityRepository.GetAvailableTimeSlotsAsync(date);
            return Ok(timeSlots);
        }

        // POST: api/Availability
        [HttpPost]
        public async Task<ActionResult<AvailabilityScheduleResponse>> CreateAvailabilitySchedule(CreateAvailabilityScheduleRequest request)
        {
            // Parse times
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            {
                return BadRequest("Invalid start time format. Please use HH:MM format.");
            }

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
            {
                return BadRequest("Invalid end time format. Please use HH:MM format.");
            }

            if (startTime >= endTime)
            {
                return BadRequest("Start time must be before end time.");
            }

            // Check if there's already a schedule for this date and time
            var existingSchedule = await _availabilityRepository.GetByDateAndTimeAsync(request.ScheduleDate, startTime);
            if (existingSchedule != null)
            {
                return BadRequest("A schedule already exists for this day and start time.");
            }

            var schedule = new AvailabilitySchedule
            {
                ScheduleDate = request.ScheduleDate.Date,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = request.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            var createdSchedule = await _availabilityRepository.AddAsync(schedule);

            var response = new AvailabilityScheduleResponse
            {
                Id = createdSchedule.Id,
                ScheduleDate = createdSchedule.ScheduleDate,
                StartTime = createdSchedule.StartTime.ToString(@"hh\:mm"),
                EndTime = createdSchedule.EndTime.ToString(@"hh\:mm"),
                IsAvailable = createdSchedule.IsAvailable,
                CreatedAt = createdSchedule.CreatedAt,
                UpdatedAt = createdSchedule.UpdatedAt
            };

            return CreatedAtAction(nameof(GetAvailabilitySchedules), new { id = createdSchedule.Id }, response);
        }

        // PUT: api/Availability/5
        [HttpPut("{id}")]
        public async Task<ActionResult<AvailabilityScheduleResponse>> UpdateAvailabilitySchedule(int id, UpdateAvailabilityScheduleRequest request)
        {
            var schedule = await _availabilityRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            // Parse times
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            {
                return BadRequest("Invalid start time format. Please use HH:MM format.");
            }

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
            {
                return BadRequest("Invalid end time format. Please use HH:MM format.");
            }

            if (startTime >= endTime)
            {
                return BadRequest("Start time must be before end time.");
            }

            schedule.StartTime = startTime;
            schedule.EndTime = endTime;
            schedule.IsAvailable = request.IsAvailable;
            schedule.UpdatedAt = DateTime.UtcNow;

            var updatedSchedule = await _availabilityRepository.UpdateAsync(schedule);

            var response = new AvailabilityScheduleResponse
            {
                Id = updatedSchedule.Id,
                ScheduleDate = updatedSchedule.ScheduleDate,
                StartTime = updatedSchedule.StartTime.ToString(@"hh\:mm"),
                EndTime = updatedSchedule.EndTime.ToString(@"hh\:mm"),
                IsAvailable = updatedSchedule.IsAvailable,
                CreatedAt = updatedSchedule.CreatedAt,
                UpdatedAt = updatedSchedule.UpdatedAt
            };

            return Ok(response);
        }

        // DELETE: api/Availability/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailabilitySchedule(int id)
        {
            var result = await _availabilityRepository.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
