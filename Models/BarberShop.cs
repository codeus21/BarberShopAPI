// Add this to your Models folder in the C# backend
using System.ComponentModel.DataAnnotations;

namespace BarberShopAPI.Server.Models
{
    public class BarberShop
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Subdomain { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string AdminEmail { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string AdminPasswordHash { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? BusinessPhone { get; set; }
        
        [MaxLength(255)]
        public string? BusinessAddress { get; set; }
        
        [MaxLength(500)]
        public string? BusinessHours { get; set; }
        
        [MaxLength(255)]
        public string? LogoUrl { get; set; }
        
        [MaxLength(7)]
        public string ThemeColor { get; set; } = "#D4AF37";
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();
    }
    
    // Interface for tenant entities
    public interface ITenantEntity
    {
        int TenantId { get; set; }
    }
}
