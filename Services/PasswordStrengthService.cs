using System.Text.RegularExpressions;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Server.Services
{
    public class PasswordStrengthService
    {
        public PasswordStrengthLevel CalculateStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return PasswordStrengthLevel.VeryWeak;

            int score = 0;
            var feedback = new List<string>();

            // Length scoring
            if (password.Length >= 8) score += 1;
            if (password.Length >= 12) score += 1;
            if (password.Length >= 16) score += 1;
            if (password.Length >= 20) score += 1;

            // Character variety scoring
            if (password.Any(char.IsLower)) score += 1;
            if (password.Any(char.IsUpper)) score += 1;
            if (password.Any(char.IsDigit)) score += 1;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score += 1;

            // Complexity scoring
            if (password.Length >= 8 && password.Any(char.IsLower) && password.Any(char.IsUpper) && 
                password.Any(char.IsDigit) && password.Any(c => !char.IsLetterOrDigit(c)))
            {
                score += 2; // Bonus for meeting all basic requirements
            }

            // Penalty for common patterns
            if (HasCommonPatterns(password))
            {
                score -= 2;
                feedback.Add("Avoid common keyboard patterns");
            }

            // Penalty for repeated characters
            if (HasRepeatedCharacters(password))
            {
                score -= 1;
                feedback.Add("Avoid repeated characters");
            }

            // Penalty for sequential characters
            if (HasSequentialCharacters(password))
            {
                score -= 1;
                feedback.Add("Avoid sequential characters");
            }

            // Bonus for entropy
            var entropy = CalculateEntropy(password);
            if (entropy > 4.0) score += 1;
            if (entropy > 5.0) score += 1;

            // Determine strength level
            return score switch
            {
                <= 1 => PasswordStrengthLevel.VeryWeak,
                <= 2 => PasswordStrengthLevel.Weak,
                <= 3 => PasswordStrengthLevel.Good,
                _ => PasswordStrengthLevel.Strong
            };
        }

        public string GetStrengthDescription(PasswordStrengthLevel strength)
        {
            return strength switch
            {
                PasswordStrengthLevel.VeryWeak => "Very Weak - Easily guessable",
                PasswordStrengthLevel.Weak => "Weak - Vulnerable to attacks",
                PasswordStrengthLevel.Good => "Good - Reasonably secure",
                PasswordStrengthLevel.Strong => "Strong - Very secure",
                _ => "Unknown"
            };
        }

        public string GetStrengthColor(PasswordStrengthLevel strength)
        {
            return strength switch
            {
                PasswordStrengthLevel.VeryWeak => "#ff4444",
                PasswordStrengthLevel.Weak => "#ff8800",
                PasswordStrengthLevel.Good => "#88cc00",
                PasswordStrengthLevel.Strong => "#00aa00",
                _ => "#666666"
            };
        }

        private bool HasCommonPatterns(string password)
        {
            var commonPatterns = new[]
            {
                "qwerty", "asdfgh", "zxcvbn", "123456", "654321",
                "abcdef", "fedcba", "qazwsx", "wsxedc", "password",
                "admin", "letmein", "welcome", "monkey", "dragon"
            };

            var lowerPassword = password.ToLower();
            return commonPatterns.Any(pattern => lowerPassword.Contains(pattern));
        }

        private bool HasRepeatedCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i] == password[i + 2])
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasSequentialCharacters(string password)
        {
            var sequences = new[]
            {
                "abcdefghijklmnopqrstuvwxyz",
                "zyxwvutsrqponmlkjihgfedcba",
                "0123456789",
                "9876543210"
            };

            var lowerPassword = password.ToLower();
            return sequences.Any(seq => 
                Enumerable.Range(0, seq.Length - 3)
                    .Any(i => lowerPassword.Contains(seq.Substring(i, 4)))
            );
        }

        private double CalculateEntropy(string password)
        {
            var charSet = new HashSet<char>(password);
            var charsetSize = charSet.Count;
            
            if (charsetSize == 0) return 0;
            
            return Math.Log(Math.Pow(charsetSize, password.Length), 2);
        }
    }

}
