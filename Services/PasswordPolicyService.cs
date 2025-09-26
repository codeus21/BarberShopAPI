using System.Text.RegularExpressions;

namespace BarberShopAPI.Server.Services
{
    public class PasswordPolicyService
    {
        private readonly PasswordPolicy _policy;

        public PasswordPolicyService()
        {
            _policy = new PasswordPolicy
            {
                MinLength = 8,
                MaxLength = 128,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireDigit = true,
                RequireSpecialChar = true,
                MinSpecialChars = 1,
                ForbiddenPatterns = new[] { "password", "123456", "admin", "qwerty" },
                MaxConsecutiveChars = 3
            };
        }

        public PasswordValidationResult ValidatePassword(string password)
        {
            var result = new PasswordValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password is required.");
                return result;
            }

            // Check minimum length
            if (password.Length < _policy.MinLength)
            {
                result.IsValid = false;
                result.Errors.Add($"Password must be at least {_policy.MinLength} characters long.");
            }

            // Check for spaces
            if (password.Contains(' '))
            {
                result.IsValid = false;
                result.Errors.Add("Password cannot contain spaces.");
            }

            // Check maximum length
            if (password.Length > _policy.MaxLength)
            {
                result.IsValid = false;
                result.Errors.Add($"Password must be no more than {_policy.MaxLength} characters long.");
            }

            // Check for uppercase
            if (_policy.RequireUppercase && !password.Any(char.IsUpper))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one uppercase letter.");
            }

            // Check for lowercase
            if (_policy.RequireLowercase && !password.Any(char.IsLower))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one lowercase letter.");
            }

            // Check for digits
            if (_policy.RequireDigit && !password.Any(char.IsDigit))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one digit.");
            }

            // Check for special characters
            if (_policy.RequireSpecialChar)
            {
                var specialCharCount = password.Count(c => !char.IsLetterOrDigit(c));
                if (specialCharCount < _policy.MinSpecialChars)
                {
                    result.IsValid = false;
                    result.Errors.Add($"Password must contain at least {_policy.MinSpecialChars} special character(s).");
                }
            }

            // Check for forbidden patterns (case-insensitive)
            foreach (var pattern in _policy.ForbiddenPatterns)
            {
                if (password.ToLower().Contains(pattern.ToLower()))
                {
                    result.IsValid = false;
                    result.Errors.Add($"Password cannot contain common words like '{pattern}'.");
                }
            }

            // Check for consecutive characters
            if (HasConsecutiveCharacters(password, _policy.MaxConsecutiveChars))
            {
                result.IsValid = false;
                result.Errors.Add($"Password cannot have more than {_policy.MaxConsecutiveChars} consecutive identical characters.");
            }

            // Check for common patterns
            if (HasCommonPatterns(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password contains common patterns that are easy to guess.");
            }

            return result;
        }

        public string GenerateSecurePassword(int length = 12)
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var random = new Random();
            var password = new List<char>();

            // Ensure at least one character from each required category
            password.Add(uppercase[random.Next(uppercase.Length)]);
            password.Add(lowercase[random.Next(lowercase.Length)]);
            password.Add(digits[random.Next(digits.Length)]);
            password.Add(specialChars[random.Next(specialChars.Length)]);

            // Fill the rest with random characters
            var allChars = uppercase + lowercase + digits + specialChars;
            for (int i = 4; i < length; i++)
            {
                password.Add(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password
            for (int i = password.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (password[i], password[j]) = (password[j], password[i]);
            }

            return new string(password.ToArray());
        }

        private bool HasConsecutiveCharacters(string password, int maxConsecutive)
        {
            for (int i = 0; i <= password.Length - maxConsecutive; i++)
            {
                var substring = password.Substring(i, maxConsecutive);
                if (substring.All(c => c == substring[0]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasCommonPatterns(string password)
        {
            // Check for keyboard patterns
            var keyboardPatterns = new[]
            {
                "qwerty", "asdfgh", "zxcvbn", "123456", "654321",
                "abcdef", "fedcba", "qazwsx", "wsxedc"
            };

            var lowerPassword = password.ToLower();
            return keyboardPatterns.Any(pattern => lowerPassword.Contains(pattern));
        }
    }

    public class PasswordPolicy
    {
        public int MinLength { get; set; } = 8;
        public int MaxLength { get; set; } = 128;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireDigit { get; set; } = true;
        public bool RequireSpecialChar { get; set; } = true;
        public int MinSpecialChars { get; set; } = 1;
        public string[] ForbiddenPatterns { get; set; } = Array.Empty<string>();
        public int MaxConsecutiveChars { get; set; } = 3;
    }

    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StrengthScore { get; set; }
    }
}
