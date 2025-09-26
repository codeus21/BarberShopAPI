using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddUsedPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "admins",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "HasCustomPassword",
                table: "admins",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "admins",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$benDf35qRWXNw0osak1bT.l0YJzO8HvMF97sPDan1awa2M6ZrhYx6");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "amazedave15@gmail.com", "$2a$11$1f2fGZFonZYlqMVjU4JDaOwE5ieYIiUnrq.hU3LNlmMP9l4qOL2EG" });

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$lAJCNG2qgXjW11SmKtSczendtw7M/RcHTq8chwceU9.GNX00I6sTi");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AdminEmail", "AdminPasswordHash" },
                values: new object[] { "amazedave15@gmail.com", "$2a$11$EdP4jFzY7y39s8jwINve6OzW31gGSCSQw6PhQHB5qcmPLU4/SespS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "admins",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "HasCustomPassword",
                table: "admins",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

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

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$NVivb4EJJk3/aMCEIM7A1u/hTu.ZgyqDpKdGiZA1AKnjHrS2eOYN2");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "admin@elitecuts.com", "$2a$11$LIT6BnHYlrjT/Ma5Lgy/ce7VsXgV/ER.BsAw0Rh1Yk4j3mjZtSuM6" });

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$uM8X0K08yGYFx2Bg8EbkyOK9Si5irGsj0cyRMFMExWGoRWrNN.lka");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AdminEmail", "AdminPasswordHash" },
                values: new object[] { "admin@elitecuts.com", "$2a$11$wKHdyrvmorruwHj5OFdxP.5UKgJDiC7YPrU59EDaZEojtx.HO7ezy" });
        }
    }
}
