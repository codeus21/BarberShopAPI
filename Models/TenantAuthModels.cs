using System.ComponentModel.DataAnnotations;

namespace BarberShopAPI.Server.Models
{
    public class TenantLoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class TenantLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool RequiresPasswordSetup { get; set; } = false;
        public string TenantName { get; set; } = string.Empty;
        public bool IsDefaultTenant { get; set; } = false;
    }

    public class SetupPasswordRequest
    {
        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password confirmation does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class TenantAuthStatus
    {
        public bool RequiresPasswordSetup { get; set; }
        public bool IsDefaultTenant { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class PasswordRecoveryRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Password confirmation does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
