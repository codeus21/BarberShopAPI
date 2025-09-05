using System.ComponentModel.DataAnnotations;

namespace BarberShopAPI.Server.Models
{
    public class CreateAppointmentRequest
    {
        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; } = string.Empty;

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

        public string Status { get; set; } = "Confirmed";
    }
}
