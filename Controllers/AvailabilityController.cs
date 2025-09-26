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
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime.ToString(@"hh\:mm"),
                EndTime = s.EndTime.ToString(@"hh\:mm"),
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Ok(response);
        }

        // GET: api/Availability/day/Monday
        [HttpGet("day/{dayOfWeek}")]
        public async Task<ActionResult<IEnumerable<AvailabilityScheduleResponse>>> GetAvailabilityByDay(string dayOfWeek)
        {
            var schedules = await _availabilityRepository.GetByDayOfWeekAsync(dayOfWeek);
            var response = schedules.Select(s => new AvailabilityScheduleResponse
            {
                Id = s.Id,
                DayOfWeek = s.DayOfWeek,
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
            // Validate day of week
            var validDays = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            if (!validDays.Contains(request.DayOfWeek))
            {
                return BadRequest("Invalid day of week. Must be one of: Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday");
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

            // Check if there's already a schedule for this day and time
            var existingSchedule = await _availabilityRepository.GetByDayAndTimeAsync(request.DayOfWeek, startTime);
            if (existingSchedule != null)
            {
                return BadRequest("A schedule already exists for this day and start time.");
            }

            var schedule = new AvailabilitySchedule
            {
                DayOfWeek = request.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsAvailable = request.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            var createdSchedule = await _availabilityRepository.AddAsync(schedule);

            var response = new AvailabilityScheduleResponse
            {
                Id = createdSchedule.Id,
                DayOfWeek = createdSchedule.DayOfWeek,
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
                DayOfWeek = updatedSchedule.DayOfWeek,
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

        // POST: api/Availability/initialize-default
        [HttpPost("initialize-default")]
        public async Task<ActionResult> InitializeDefaultSchedule()
        {
            var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            var defaultSchedules = new List<AvailabilitySchedule>();

            foreach (var day in daysOfWeek)
            {
                // Check if schedules already exist for this day
                var existingSchedules = await _availabilityRepository.GetByDayOfWeekAsync(day);
                if (existingSchedules.Any())
                {
                    continue; // Skip if schedules already exist
                }

                // Create default schedule (9 AM to 6 PM for weekdays, 9 AM to 4 PM for weekends)
                var startTime = TimeSpan.FromHours(9);
                var endTime = (day == "Saturday" || day == "Sunday") ? TimeSpan.FromHours(16) : TimeSpan.FromHours(18);

                var schedule = new AvailabilitySchedule
                {
                    DayOfWeek = day,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                };

                defaultSchedules.Add(schedule);
            }

            // Add all new schedules
            foreach (var schedule in defaultSchedules)
            {
                await _availabilityRepository.AddAsync(schedule);
            }

            return Ok(new { message = $"Initialized default schedules for {defaultSchedules.Count} days." });
        }
    }
}
