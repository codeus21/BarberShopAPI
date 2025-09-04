using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenantSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admins",
                table: "Admins");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "services");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "appointments");

            migrationBuilder.RenameTable(
                name: "Admins",
                newName: "admins");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ServiceId",
                table: "appointments",
                newName: "IX_appointments_ServiceId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "services",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "appointments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Confirmed",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "appointments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "admins",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "admins",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "admins",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "admins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_services",
                table: "services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_appointments",
                table: "appointments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admins",
                table: "admins",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "barbershops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Subdomain = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AdminEmail = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AdminPasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BusinessPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BusinessAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    BusinessHours = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ThemeColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#D4AF37"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barbershops", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "barbershops",
                columns: new[] { "Id", "AdminEmail", "AdminPasswordHash", "BusinessAddress", "BusinessHours", "BusinessPhone", "CreatedAt", "IsActive", "LogoUrl", "Name", "Subdomain", "ThemeColor", "UpdatedAt" },
                values: new object[] { 1, "admin@thebarberbook.com", "$2a$11$bjVrMiS1DSPN0ADHcJQj3ODqEeKgMrcbYG426GFUUA07ivxSJVnFa", null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "The Barber Book", "default", "#D4AF37", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DurationMinutes", "Price", "TenantId" },
                values: new object[] { "Professional haircut with styling", 60, 25.00m, 1 });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "DurationMinutes", "Name", "Price", "TenantId" },
                values: new object[] { "Creative hair design and styling", 30, "Design", 10.00m, 1 });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "DurationMinutes", "Name", "Price", "TenantId" },
                values: new object[] { "Professional beard trimming and shaping", 30, "Beard Trimming", 15.00m, 1 });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "DurationMinutes", "Name", "Price", "TenantId" },
                values: new object[] { "Eyebrow trimming and shaping", 15, "Eyebrows", 8.00m, 1 });

            migrationBuilder.InsertData(
                table: "admins",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "TenantId", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@thebarberbook.com", true, "Barber Admin", "$2a$11$gHuYzx30Zsk4pty/0YfGXe1JvTk/tnPGRmlenxhSAGwDR4.VVwjwy", 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_services_TenantId",
                table: "services",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_TenantId",
                table: "appointments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_admins_TenantId",
                table: "admins",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_barbershops_Subdomain",
                table: "barbershops",
                column: "Subdomain",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_admins_barbershops_TenantId",
                table: "admins",
                column: "TenantId",
                principalTable: "barbershops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_barbershops_TenantId",
                table: "appointments",
                column: "TenantId",
                principalTable: "barbershops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_services_ServiceId",
                table: "appointments",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_services_barbershops_TenantId",
                table: "services",
                column: "TenantId",
                principalTable: "barbershops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admins_barbershops_TenantId",
                table: "admins");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_barbershops_TenantId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_services_ServiceId",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_services_barbershops_TenantId",
                table: "services");

            migrationBuilder.DropTable(
                name: "barbershops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_services",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_services_TenantId",
                table: "services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_appointments",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "IX_appointments_TenantId",
                table: "appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admins",
                table: "admins");

            migrationBuilder.DropIndex(
                name: "IX_admins_TenantId",
                table: "admins");

            migrationBuilder.DeleteData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "services");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "admins");

            migrationBuilder.RenameTable(
                name: "services",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "appointments",
                newName: "Appointments");

            migrationBuilder.RenameTable(
                name: "admins",
                newName: "Admins");

            migrationBuilder.RenameIndex(
                name: "IX_appointments_ServiceId",
                table: "Appointments",
                newName: "IX_Appointments_ServiceId");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Confirmed");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "Appointments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Admins",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Admins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Admins",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admins",
                table: "Admins",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DurationMinutes", "Price" },
                values: new object[] { "Professional haircut with consultation, wash, cut, and style.", 50, 35.00m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "DurationMinutes", "Name", "Price" },
                values: new object[] { "Creative haircut with custom designs, fades, and artistic styling.", 60, "Haircut with Designs", 40.00m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "DurationMinutes", "Name", "Price" },
                values: new object[] { "Professional beard and mustache trimming with shaping and styling.", 10, "Beard & Mustache Trim", 10.00m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "DurationMinutes", "Name", "Price" },
                values: new object[] { "Precision eyebrow shaping and grooming for a polished look.", 5, "Eyebrow Shaping", 5.00m });

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId",
                table: "Appointments",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
