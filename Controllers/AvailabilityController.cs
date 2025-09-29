using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarberShopAPI.Server.Repositories;
using BarberShopAPI.Server.Models;
using BarberShopAPI.Server.Attributes;

namespace BarberShopAPI.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [TenantAuthorize]
    [ApiController]
    [Route("api/[controller]")]
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
            try
            {
                var schedules = await _availabilityRepository.GetAllSchedulesAsync();
                var response = schedules.Select(s => new AvailabilityScheduleResponse
                {
                    Id = s.Id,
                    ScheduleDate = s.ScheduleDate,
                    StartTime = $"{s.StartTime.Hours:D2}:{s.StartTime.Minutes:D2}",
                    EndTime = $"{s.EndTime.Hours:D2}:{s.EndTime.Minutes:D2}",
                    IsAvailable = s.IsAvailable,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList();

                return Ok(response);
            }
            catch (FormatException ex)
            {
                // If there's invalid TimeSpan data, return empty list
                return Ok(new List<AvailabilityScheduleResponse>());
            }
        }

        // GET: api/Availability/date/2024-01-15
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<AvailabilityScheduleResponse>>> GetAvailabilityByDate(DateTime date)
        {
            try
            {
                var schedules = await _availabilityRepository.GetByDateAsync(date);
                var response = schedules.Select(s => new AvailabilityScheduleResponse
                {
                    Id = s.Id,
                    ScheduleDate = s.ScheduleDate,
                    StartTime = $"{s.StartTime.Hours:D2}:{s.StartTime.Minutes:D2}",
                    EndTime = $"{s.EndTime.Hours:D2}:{s.EndTime.Minutes:D2}",
                    IsAvailable = s.IsAvailable,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList();

                return Ok(response);
            }
            catch (FormatException ex)
            {
                // If there's invalid TimeSpan data, return empty list
                return Ok(new List<AvailabilityScheduleResponse>());
            }
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
            Console.WriteLine($"CreateAvailabilitySchedule called with: ScheduleDate={request.ScheduleDate}, StartTime={request.StartTime}, EndTime={request.EndTime}, IsAvailable={request.IsAvailable}");
            
            // Parse times
            if (!TimeSpan.TryParse(request.StartTime, out var startTime))
            {
                Console.WriteLine($"Invalid start time format: {request.StartTime}");
                return BadRequest("Invalid start time format. Please use HH:MM format.");
            }

            if (!TimeSpan.TryParse(request.EndTime, out var endTime))
            {
                Console.WriteLine($"Invalid end time format: {request.EndTime}");
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

            AvailabilitySchedule? createdSchedule = null;
            try
            {
                createdSchedule = await _availabilityRepository.AddAsync(schedule);
                Console.WriteLine($"Created schedule with ID: {createdSchedule.Id}");
                Console.WriteLine($"StartTime type: {createdSchedule.StartTime.GetType()}, value: {createdSchedule.StartTime}");
                Console.WriteLine($"EndTime type: {createdSchedule.EndTime.GetType()}, value: {createdSchedule.EndTime}");

                var response = new AvailabilityScheduleResponse
                {
                    Id = createdSchedule.Id,
                    ScheduleDate = createdSchedule.ScheduleDate,
                    StartTime = $"{createdSchedule.StartTime.Hours:D2}:{createdSchedule.StartTime.Minutes:D2}",
                    EndTime = $"{createdSchedule.EndTime.Hours:D2}:{createdSchedule.EndTime.Minutes:D2}",
                    IsAvailable = createdSchedule.IsAvailable,
                    CreatedAt = createdSchedule.CreatedAt,
                    UpdatedAt = createdSchedule.UpdatedAt
                };

                Console.WriteLine($"Returning response with ID: {response.Id}");
                return Ok(response);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"FormatException caught: {ex.Message}");
                Console.WriteLine($"StartTime: {createdSchedule?.StartTime}, EndTime: {createdSchedule?.EndTime}");
                // If there's a TimeSpan formatting issue, return a simple response
                return Ok(new AvailabilityScheduleResponse
                {
                    Id = 0,
                    ScheduleDate = request.ScheduleDate.Date,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    IsAvailable = request.IsAvailable,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null
                });
            }
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
                StartTime = $"{updatedSchedule.StartTime.Hours:D2}:{updatedSchedule.StartTime.Minutes:D2}",
                EndTime = $"{updatedSchedule.EndTime.Hours:D2}:{updatedSchedule.EndTime.Minutes:D2}",
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
            Console.WriteLine($"DeleteAvailabilitySchedule called with ID: {id}");
            var result = await _availabilityRepository.DeleteAsync(id);
            if (!result)
            {
                Console.WriteLine($"Failed to delete schedule with ID: {id}");
                return NotFound();
            }

            Console.WriteLine($"Successfully deleted schedule with ID: {id}");
            return NoContent();
        }

        // DELETE: api/Availability/clear-all
        [HttpDelete("clear-all")]
        public async Task<ActionResult> ClearAllAvailabilitySchedules()
        {
            try
            {
                var allSchedules = await _availabilityRepository.GetAllSchedulesAsync();
                foreach (var schedule in allSchedules)
                {
                    await _availabilityRepository.DeleteAsync(schedule.Id);
                }
                return Ok(new { message = "All availability schedules cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error clearing schedules: " + ex.Message });
            }
        }

    }
}
