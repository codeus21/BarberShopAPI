using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShopAPI.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [StringLength(200)]
        public string? CustomerEmail { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Confirmed";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public Service? Service { get; set; }
    }
}