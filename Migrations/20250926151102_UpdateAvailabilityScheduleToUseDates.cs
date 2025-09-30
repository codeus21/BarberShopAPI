using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAvailabilityScheduleToUseDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_availability_schedules_TenantId_DayOfWeek",
                table: "availability_schedules");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "availability_schedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduleDate",
                table: "availability_schedules",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$JnEBBjeMIASlJfX8id.17eumTHuy/w5vnUcZ/plEM/tADV6OkwvD2");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$pDgf/7EnaYktPgskNkP2FutVz1h8YkrFi/Vr7zYf/l5k6pI99Sx7a");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$Xn3tKU9Idk/6W2t7R848wu.gVze8YbY.uBIlxe3v9P1mRqRLf.nga");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "$2a$11$Lb4x4xMIJVRdW8GN2JOzmeX0x36m/lmznRCxRy/Od.SZ5UeKBeEXa");

            migrationBuilder.CreateIndex(
                name: "IX_availability_schedules_TenantId_ScheduleDate",
                table: "availability_schedules",
                columns: new[] { "TenantId", "ScheduleDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_availability_schedules_TenantId_ScheduleDate",
                table: "availability_schedules");

            migrationBuilder.DropColumn(
                name: "ScheduleDate",
                table: "availability_schedules");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "availability_schedules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

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
    }
}
