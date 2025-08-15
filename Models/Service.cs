using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShopAPI.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}