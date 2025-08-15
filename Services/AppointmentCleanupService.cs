using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Data;
using BarberShopAPI.Models;

namespace BarberShopAPI.Services
{
    public class AppointmentCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentCleanupService> _logger;

        public AppointmentCleanupService(IServiceProvider serviceProvider, ILogger<AppointmentCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessCompletedAppointments();

                    // Wait until next day at 12:01 AM
                    var tomorrow = DateTime.Today.AddDays(1);
                    var nextRun = tomorrow.AddMinutes(1);
                    var delay = nextRun - DateTime.Now;

                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing completed appointments");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Wait 1 hour before retrying
                }
            }
        }

        private async Task ProcessCompletedAppointments()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BarberShopContext>();

            var today = DateTime.Today;

            // Get all confirmed appointments from past dates (including today but past times)
            var pastAppointments = await context.Appointments
                .Where(a =>
                    (a.AppointmentDate < today) ||
                    (a.AppointmentDate == today && a.AppointmentTime < DateTime.Now.TimeOfDay) &&
                    a.Status == "Confirmed")
                .ToListAsync();

            if (pastAppointments.Any())
            {
                foreach (var appointment in pastAppointments)
                {
                    appointment.Status = "Completed";
                    appointment.UpdatedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
                _logger.LogInformation($"Marked {pastAppointments.Count} past appointments as completed");
            }
        }
    }
}