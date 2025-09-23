using System.ComponentModel.DataAnnotations;

namespace BarberShopAPI.Server.Models
{
    public class UsedPasswordResetToken
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string TokenHash { get; set; } = string.Empty;
        
        [Required]
        public int AdminId { get; set; }
        
        [Required]
        public int TenantId { get; set; }
        
        [Required]
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Admin Admin { get; set; } = null!;
        public BarberShop Tenant { get; set; } = null!;
    }
}
