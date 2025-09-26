using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShopAPI.Server.Models
{
    public class AvailabilitySchedule : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime ScheduleDate { get; set; } // Specific date instead of day of week

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Multi-tenant support
        public int TenantId { get; set; }

        // Navigation properties
        public virtual BarberShop Tenant { get; set; } = null!;
    }

    public class CreateAvailabilityScheduleRequest
    {
        [Required]
        public DateTime ScheduleDate { get; set; } // Specific date

        [Required]
        public string StartTime { get; set; } = string.Empty; // Format: "HH:MM"

        [Required]
        public string EndTime { get; set; } = string.Empty; // Format: "HH:MM"

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateAvailabilityScheduleRequest
    {
        [Required]
        public string StartTime { get; set; } = string.Empty;

        [Required]
        public string EndTime { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;
    }

    public class AvailabilityScheduleResponse
    {
        public int Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
