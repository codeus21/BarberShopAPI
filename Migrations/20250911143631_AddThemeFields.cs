using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomCss",
                table: "barbershops",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FontFamily",
                table: "barbershops",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Arial, sans-serif");

            migrationBuilder.AddColumn<string>(
                name: "SecondaryColor",
                table: "barbershops",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "#000000");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Vo0QxbBc.CRvly5oM2jiQuE.IrDSszxAKTj6uy4LXdj7ZdGD6rpW6");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdminEmail", "AdminPasswordHash", "BusinessAddress", "BusinessHours", "BusinessPhone", "CustomCss", "FontFamily", "Name", "SecondaryColor" },
                values: new object[] { "CleanCuts@thebarberbook.com", "$2a$11$bD6t7GoMlV9JL.uRQMrAGehVxbhTgPGTMEhDEO.VQ9.glMrMJOelS", "123 Main Street", "Mon-Fri: 9AM-6PM, Sat: 9AM-4PM, Sun: Closed", "(123) 456-7890", null, "Arial, sans-serif", "Clean Cuts", "#000000" });

            migrationBuilder.InsertData(
                table: "barbershops",
                columns: new[] { "Id", "AdminEmail", "AdminPasswordHash", "BusinessAddress", "BusinessHours", "BusinessPhone", "CreatedAt", "CustomCss", "FontFamily", "IsActive", "LogoUrl", "Name", "SecondaryColor", "Subdomain", "ThemeColor", "UpdatedAt" },
                values: new object[] { 2, "admin@elitecuts.com", "$2a$11$Uz/LB1tXmgTT32OBye1C2.EB.UFqEHUYDCf8VuhrQAJdRb5Vw/r92", "456 Oak Street", "Mon-Fri: 9AM-7PM, Sat: 9AM-5PM, Sun: Closed", "(555) 123-4567", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Georgia, serif", true, null, "Elite Cuts", "#FFFFFF", "elite", "#1E40AF", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 1,
                column: "Price",
                value: 30.00m);

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 10, 5.00m });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 10, 5.00m });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 5, 5.00m });

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "TenantId", "Username" },
                values: new object[] { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@elitecuts.com", true, "Elite Cuts Admin", "$2a$11$RiVEXpCPVsxHWrsdcK55MOfQf1EVwprH44bQ4Ke/YCZHEzRz/.Ks2", 2, "admin" });

            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "Id", "Description", "DurationMinutes", "IsActive", "Name", "Price", "TenantId" },
                values: new object[,]
                {
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
                keyValue: 2);

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
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "CustomCss",
                table: "barbershops");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "barbershops");

            migrationBuilder.DropColumn(
                name: "SecondaryColor",
                table: "barbershops");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$gHuYzx30Zsk4pty/0YfGXe1JvTk/tnPGRmlenxhSAGwDR4.VVwjwy");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AdminEmail", "AdminPasswordHash", "BusinessAddress", "BusinessHours", "BusinessPhone", "Name" },
                values: new object[] { "admin@thebarberbook.com", "$2a$11$bjVrMiS1DSPN0ADHcJQj3ODqEeKgMrcbYG426GFUUA07ivxSJVnFa", null, null, null, "The Barber Book" });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 1,
                column: "Price",
                value: 25.00m);

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 10.00m });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 15.00m });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 15, 8.00m });
        }
    }
}
