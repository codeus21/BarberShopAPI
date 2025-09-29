using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAvailabilityScheduleNavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
