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
                entity.Property(e => e.SecondaryColor).HasMaxLength(7).HasDefaultValue("#000000");
                entity.Property(e => e.FontFamily).HasMaxLength(50).HasDefaultValue("Arial, sans-serif");
                entity.Property(e => e.CustomCss).HasMaxLength(1000);
                
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
            // Seed barber shops
            modelBuilder.Entity<BarberShop>().HasData(
                new BarberShop
                {
                    Id = 1,
                    Name = "Clean Cuts",
                    Subdomain = "default",
                    AdminEmail = "CleanCuts@thebarberbook.com",
                    AdminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Default password
                    BusinessPhone = "(123) 456-7890",
                    BusinessAddress = "123 Main Street",
                    BusinessHours = "Mon-Fri: 9AM-6PM, Sat: 9AM-4PM, Sun: Closed",
                    ThemeColor = "#D4AF37",
                    SecondaryColor = "#000000",
                    FontFamily = "Arial, sans-serif",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
           new BarberShop
           {
               Id = 2,
               Name = "Elite Cuts",
               Subdomain = "elite",
               AdminEmail = "admin@elitecuts.com",
               AdminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
               BusinessPhone = "(555) 123-4567",
               BusinessAddress = "456 Oak Street",
               BusinessHours = "Mon-Fri: 9AM-7PM, Sat: 9AM-5PM, Sun: Closed",
               ThemeColor = "#8B5CF6",
               SecondaryColor = "#F8FAFC",
               FontFamily = "Inter, -apple-system, BlinkMacSystemFont, sans-serif",
               CustomCss = @"
                   .elite-premium { 
                       background: linear-gradient(135deg, #8B5CF6 0%, #A855F7 50%, #C084FC 100%);
                       color: white;
                       border-radius: 12px;
                       padding: 20px;
                       box-shadow: 0 20px 40px rgba(139, 92, 246, 0.3);
                   }
                   .elite-gold { 
                       background: linear-gradient(45deg, #F59E0B, #FCD34D);
                       -webkit-background-clip: text;
                       -webkit-text-fill-color: transparent;
                       font-weight: 700;
                   }
                   .elite-card {
                       background: rgba(255, 255, 255, 0.95);
                       backdrop-filter: blur(10px);
                       border: 1px solid rgba(139, 92, 246, 0.2);
                       border-radius: 16px;
                       box-shadow: 0 8px 32px rgba(139, 92, 246, 0.1);
                   }
                   .elite-button {
                       background: linear-gradient(135deg, #8B5CF6, #A855F7);
                       border: none;
                       border-radius: 8px;
                       color: white;
                       font-weight: 600;
                       transition: all 0.3s ease;
                       box-shadow: 0 4px 15px rgba(139, 92, 246, 0.4);
                   }
                   .elite-button:hover {
                       transform: translateY(-2px);
                       box-shadow: 0 8px 25px rgba(139, 92, 246, 0.6);
                   }
                   .elite-nav {
                       background: rgba(255, 255, 255, 0.95);
                       backdrop-filter: blur(20px);
                       border-bottom: 1px solid rgba(139, 92, 246, 0.1);
                   }
               ",
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
                    Price = 5.00m,
                    DurationMinutes = 10,
                    IsActive = true
                },
                new Service
                {
                    Id = 3,
                    TenantId = 1,
                    Name = "Beard Trimming",
                    Description = "Professional beard trimming and shaping",
                    Price = 5.00m,
                    DurationMinutes = 10,
                    IsActive = true
                },
                new Service
                {
                    Id = 4,
                    TenantId = 1,
                    Name = "Eyebrows",
                    Description = "Eyebrow trimming and shaping",
                    Price = 5.00m,
                    DurationMinutes = 5,
                    IsActive = true
                },
                // Elite Cuts Services
                new Service
                {
                    Id = 5,
                    TenantId = 2,
                    Name = "Signature Elite Cut",
                    Description = "Our signature premium haircut with precision styling and luxury treatment",
                    Price = 75.00m,
                    DurationMinutes = 60,
                    IsActive = true
                },
                new Service
                {
                    Id = 6,
                    TenantId = 2,
                    Name = "Royal Beard Sculpting",
                    Description = "Master-level beard sculpting with premium oils and precision tools",
                    Price = 65.00m,
                    DurationMinutes = 45,
                    IsActive = true
                },
                new Service
                {
                    Id = 7,
                    TenantId = 2,
                    Name = "Executive Hair Styling",
                    Description = "Professional executive styling for business and special occasions",
                    Price = 45.00m,
                    DurationMinutes = 30,
                    IsActive = true
                },
                new Service
                {
                    Id = 8,
                    TenantId = 2,
                    Name = "Luxury Eyebrow Design",
                    Description = "Artisanal eyebrow shaping with precision and attention to detail",
                    Price = 35.00m,
                    DurationMinutes = 25,
                    IsActive = true
                },
                new Service
                {
                    Id = 9,
                    TenantId = 2,
                    Name = "Complete Gentleman's Package",
                    Description = "Full service: Haircut, beard sculpting, styling, and eyebrow design",
                    Price = 150.00m,
                    DurationMinutes = 90,
                    IsActive = true
                }
            );

            // Seed admins
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
                },
                new Admin
                {
                    Id = 2,
                    TenantId = 2,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Name = "Elite Cuts Admin",
                    Email = "admin@elitecuts.com",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}