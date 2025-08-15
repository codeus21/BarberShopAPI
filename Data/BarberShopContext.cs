using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Models;

namespace BarberShopAPI.Data
{
    public class BarberShopContext : DbContext
    {
        public BarberShopContext(DbContextOptions<BarberShopContext> options)
            : base(options)
        {
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Service entity
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DurationMinutes).IsRequired();
            });

            // Configure Appointment entity
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AppointmentDate).HasColumnType("date");
                entity.Property(e => e.AppointmentTime).HasColumnType("time");
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CustomerEmail).HasMaxLength(200);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

                // Foreign key relationship
                entity.HasOne(e => e.Service)
                    .WithMany(s => s.Appointments)
                    .HasForeignKey(e => e.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed initial services
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    Name = "Classic Haircut",
                    Price = 35.00m,
                    DurationMinutes = 50,
                    Description = "Professional haircut with consultation, wash, cut, and style.",
                    IsActive = true
                },
                new Service
                {
                    Id = 2,
                    Name = "Haircut with Designs",
                    Price = 40.00m,
                    DurationMinutes = 60,
                    Description = "Creative haircut with custom designs, fades, and artistic styling.",
                    IsActive = true
                },
                new Service
                {
                    Id = 3,
                    Name = "Beard & Mustache Trim",
                    Price = 10.00m,
                    DurationMinutes = 10,
                    Description = "Professional beard and mustache trimming with shaping and styling.",
                    IsActive = true
                },
                new Service
                {
                    Id = 4,
                    Name = "Eyebrow Shaping",
                    Price = 5.00m,
                    DurationMinutes = 5,
                    Description = "Precision eyebrow shaping and grooming for a polished look.",
                    IsActive = true
                }
            );

            // In OnModelCreating method, add:
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
            });

            // Seed admin user (password: admin123)
            //modelBuilder.Entity<Admin>().HasData(
            //new Admin
            //{
            //    Id = 1,
            //    Username = "admin",
            //    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            //    Name = "Barber Admin",
            //    Email = "admin@barbershop.com",
            //    IsActive = true,
            //    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            //}
            //);
        }
    }
}