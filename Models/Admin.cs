using System.ComponentModel.DataAnnotations;

namespace BarberShopAPI.Server.Models
{
    public class Admin : ITenantEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Multi-tenant support
        public int TenantId { get; set; }

        // Navigation property
        public virtual BarberShop Tenant { get; set; } = null!;
    }
}