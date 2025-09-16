using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "barbershops",
                columns: new[] { "Id", "AdminEmail", "AdminPasswordHash", "BusinessAddress", "BusinessHours", "BusinessPhone", "CreatedAt", "fontfamily", "IsActive", "LogoUrl", "Name", "secondarycolor", "Subdomain", "themecolor", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CleanCuts@thebarberbook.com", "$2a$11$cNXTY1r.MR2oxNC7uf40WOtd8SKtFNdclonZCqPN2n3DzkjwoncQO", "123 Main Street", "Mon-Fri: 9AM-6PM, Sat: 9AM-4PM, Sun: Closed", "(123) 456-7890", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arial, sans-serif", true, null, "Clean Cuts", "#000000", "default", "#D4AF37", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "admin@elitecuts.com", "$2a$11$7u6..PbuHvPIieEYCnt/dOWhDoVbOCdBC7PUoaZ50ZHi8an68hcsm", "456 Oak Street", "Mon-Fri: 9AM-7PM, Sat: 9AM-5PM, Sun: Closed", "(555) 123-4567", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arial, sans-serif", true, null, "Elite Cuts", "#000000", "elite", "#D4AF37", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "TenantId", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@thebarberbook.com", true, "Barber Admin", "$2a$11$vSii9inmg7FkB4pdRrph9u9GS/9VIvAiysJ1aCrn1aMy1GXusTojy", 1, "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@elitecuts.com", true, "Elite Cuts Admin", "$2a$11$dQItxJzr.kPWZj3fJm8ITOAh/qRSoEPgAsjbvC3ccGofM1yW2JAMO", 2, "admin" }
                });

            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "Id", "Description", "DurationMinutes", "IsActive", "Name", "Price", "TenantId" },
                values: new object[,]
                {
                    { 1, "Professional haircut with styling", 60, true, "Classic Haircut", 30.00m, 1 },
                    { 2, "Creative hair design and styling", 10, true, "Design", 5.00m, 1 },
                    { 3, "Professional beard trimming and shaping", 10, true, "Beard Trimming", 5.00m, 1 },
                    { 4, "Eyebrow trimming and shaping", 5, true, "Eyebrows", 5.00m, 1 },
                    { 5, "Luxury haircut with premium styling", 75, true, "Premium Haircut", 45.00m, 2 },
                    { 6, "Professional beard sculpting and design", 20, true, "Beard Sculpting", 15.00m, 2 },
                    { 7, "Professional hair styling and finishing", 15, true, "Hair Styling", 10.00m, 2 },
                    { 8, "Precision eyebrow design and shaping", 10, true, "Eyebrow Design", 8.00m, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
