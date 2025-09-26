using System.Net;
using System.Net.Mail;

namespace BarberShopAPI.Server.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordRecoveryEmailAsync(string toEmail, string tenantName, string resetToken, string subdomain = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendPasswordRecoveryEmailAsync(string toEmail, string tenantName, string resetToken, string subdomain = null)
        {
            try
            {
                // For development/testing, we'll use a simple SMTP configuration
                // In production, you'd want to use a service like SendGrid, AWS SES, etc.
                
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@thebarberbook.com";
                var fromName = _configuration["Email:FromName"] ?? "The Barber Book";

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email configuration is missing. Password recovery email not sent.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                Console.WriteLine($"EmailService - Subdomain: {subdomain}");
                Console.WriteLine($"EmailService - TenantName: {tenantName}");
                
                var tenantParam = subdomain ?? tenantName.ToLower().Replace(" ", "-");
                Console.WriteLine($"EmailService - Final tenant param: {tenantParam}");
                
                var resetUrl = $"{_configuration["App:BaseUrl"]}/admin/reset-password?token={resetToken}&tenant={tenantParam}";
                Console.WriteLine($"EmailService - Reset URL: {resetUrl}");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = $"Password Recovery - {tenantName}",
                    Body = CreatePasswordRecoveryEmailBody(tenantName, resetUrl),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation($"Password recovery email sent to {toEmail} for tenant {tenantName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password recovery email to {toEmail}");
                return false;
            }
        }

        private string CreatePasswordRecoveryEmailBody(string tenantName, string resetUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2c3e50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f9f9f9; }}
        .button {{ display: inline-block; padding: 12px 30px; background-color: #3498db; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ padding: 20px; text-align: center; color: #666; font-size: 12px; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔐 Password Recovery</h1>
            <h2>{tenantName}</h2>
        </div>
        
        <div class='content'>
            <h3>Password Reset Request</h3>
            <p>We received a request to reset your password for your {tenantName} admin account.</p>
            
            <p>Click the button below to reset your password:</p>
            
            <a href='{resetUrl}' class='button'>Reset My Password</a>
            
            <div class='warning'>
                <strong>⚠️ Security Notice:</strong>
                <ul>
                    <li>This link will expire in 1 hour</li>
                    <li>If you didn't request this, please ignore this email</li>
                    <li>For security, this link can only be used once</li>
                </ul>
            </div>
            
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <p style='word-break: break-all; background-color: #eee; padding: 10px; border-radius: 3px;'>
                {resetUrl}
            </p>
        </div>
        
        <div class='footer'>
            <p>This email was sent from The Barber Book system.</p>
            <p>If you have any questions, please contact support.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
