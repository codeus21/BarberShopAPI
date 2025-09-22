using System.ComponentModel.DataAnnotations;

namespace BarberShopAPI.Server.Models
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Password confirmation does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CreatePasswordRequest
    {
        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 128 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Password confirmation does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }


    public class PasswordValidationResponse
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public PasswordStrengthLevel Strength { get; set; }
        public string StrengthDescription { get; set; } = string.Empty;
        public string StrengthColor { get; set; } = string.Empty;
    }

    public enum PasswordStrengthLevel
    {
        VeryWeak = 0,
        Weak = 1,
        Fair = 2,
        Good = 3,
        Strong = 4
    }
}
