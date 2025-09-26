using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailabilitySchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "availability_schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DayOfWeek = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_availability_schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_availability_schedules_barbershops_TenantId",
                        column: x => x.TenantId,
                        principalTable: "barbershops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$NXs.37jCIHp/18dL6XnCweRIAq/7tkXYUEsS75FEnsyWB7O4nbmJK");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$TIsfeEvfwDJTet.R7GQ6keIwXbkKe7ymdoeEohCJNbcHdDmOZHT1S");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$DWllcN0gtMYaYM1Ap5Y2WegoKonEQZ0yKIa.P4eXTZfCzaorBkOAe");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "$2a$11$IFI96Gs4/0tF9kmPGQgOAuhcQRAHWkPNxGsJUuEzMPM.p1ZZpEqFG");

            migrationBuilder.CreateIndex(
                name: "IX_availability_schedules_TenantId_DayOfWeek",
                table: "availability_schedules",
                columns: new[] { "TenantId", "DayOfWeek" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "availability_schedules");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$LR.1okQBjKbqbeam93arc.D7VdGRQtSkFx7dY7e/hdSyLfo6XV33C");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$NKgEWNv4d1ONeed8h0K.Z.VugW3MRAMXDrmDqpfUM1cGYq1dTTtp2");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$tN/UFy.UbFBjjAMB1pvt3uCy1zx6lBXO./ramGkRzkGGYaPWrMntG");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "$2a$11$3CfYjpYX.z/wm.PSYrXSDuA5.0teJ/lLZ72jhV4J1MtiJlBYhbRDW");
        }
    }
}
