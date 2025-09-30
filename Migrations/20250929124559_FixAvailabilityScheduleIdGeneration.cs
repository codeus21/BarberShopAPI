using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixAvailabilityScheduleIdGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$fFFx6UFMRyU0iqg6owc.6efM5PtMaq58eI8RwkmyTPX8jjhi6tw8e");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$WU1wdECFAu7Xt7KpA4pX8eVhHzgHWwMQgtFL8fagLoaqJQuG9vnPq");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$/xxN2gTOAkBlo9imUycLOeeNsdMm6QG.A1zw9DGoMsOKcEnStShfW");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "$2a$11$pv6Vd6VVgq0iK1DJ.ACZE.1xgr316jz0JvfX8QHUbHQxy.J7McylK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$/d2biCu8p8biI/h8E4YGTOZhxBVioIIrsRU3aGWtNzVgX2PutFNUC");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$TW4gGkvLqox3IO5cNAMf8Oi8S8dwIbJ6MgqQnOiXnUWzp/wmbRoeu");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$/U3VnKj6YJc4GTEdIH0KNeO6rIQzHetBovYOQ45r9iqG.B/h3TxGW");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "$2a$11$im04PtU2Wa47Hb2yHGQn7OKW4cjTBOEpfjpyqw7byNQOkamFl77Cu");
        }
    }
}
