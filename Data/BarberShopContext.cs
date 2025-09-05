using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Server.Data
{
    public class BarberShopContext : DbContext
    {
        public BarberShopContext(DbContextOptions<BarberShopContext> options)
            : base(options)
        {
        }

        // Master table for barber shops
        public DbSet<BarberShop> BarberShops { get; set; }

        // Existing tables (now with tenant support)
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BarberShop entity
            modelBuilder.Entity<BarberShop>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Subdomain).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AdminEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AdminPasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.BusinessPhone).HasMaxLength(20);
                entity.Property(e => e.BusinessAddress).HasMaxLength(255);
                entity.Property(e => e.BusinessHours).HasMaxLength(500);
                entity.Property(e => e.LogoUrl).HasMaxLength(255);
                entity.Property(e => e.ThemeColor).HasMaxLength(7).HasDefaultValue("#D4AF37");
                
                entity.HasIndex(e => e.Subdomain).IsUnique();
                entity.ToTable("barbershops"); // Lowercase for PostgreSQL
            });

            // Configure Service entity with tenant relationship
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                
                // Tenant relationship
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Services)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.ToTable("services");
            });

            // Configure Appointment entity with tenant relationship
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Confirmed");
                
                // Tenant relationship
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Appointments)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Service relationship
                entity.HasOne(e => e.Service)
                      .WithMany(s => s.Appointments)
                      .HasForeignKey(e => e.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.ToTable("appointments");
            });

            // Configure Admin entity with tenant relationship
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(100);
                
                // Tenant relationship
                entity.HasOne(e => e.Tenant)
                      .WithMany(t => t.Admins)
                      .HasForeignKey(e => e.TenantId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                entity.ToTable("admins");
            });

            // Seed default tenant and data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default barber shop
            modelBuilder.Entity<BarberShop>().HasData(
                new BarberShop
                {
                    Id = 1,
                    Name = "The Barber Book",
                    Subdomain = "default",
                    AdminEmail = "admin@thebarberbook.com",
                    AdminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Default password
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed default services
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    TenantId = 1,
                    Name = "Classic Haircut",
                    Description = "Professional haircut with styling",
                    Price = 30.00m,
                    DurationMinutes = 60,
                    IsActive = true
                },
                new Service
                {
                    Id = 2,
                    TenantId = 1,
                    Name = "Design",
                    Description = "Creative hair design and styling",
                    Price = 10.00m,
                    DurationMinutes = 0,
                    IsActive = true
                },
                new Service
                {
                    Id = 3,
                    TenantId = 1,
                    Name = "Beard Trimming",
                    Description = "Professional beard trimming and shaping",
                    Price = 5.00m,
                    DurationMinutes = 0,
                    IsActive = true
                },
                new Service
                {
                    Id = 4,
                    TenantId = 1,
                    Name = "Eyebrows",
                    Description = "Eyebrow trimming and shaping",
                    Price = 5.00m,
                    DurationMinutes = 0,
                    IsActive = true
                }
            );

            // Seed default admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    TenantId = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Name = "Barber Admin",
                    Email = "admin@thebarberbook.com",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}