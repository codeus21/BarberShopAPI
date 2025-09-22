using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddHasCustomPasswordToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCustomPassword",
                table: "admins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "admins",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HasCustomPassword", "PasswordHash", "UpdatedAt" },
                values: new object[] { false, "$2a$11$NVivb4EJJk3/aMCEIM7A1u/hTu.ZgyqDpKdGiZA1AKnjHrS2eOYN2", null });

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "HasCustomPassword", "PasswordHash", "UpdatedAt" },
                values: new object[] { false, "$2a$11$LIT6BnHYlrjT/Ma5Lgy/ce7VsXgV/ER.BsAw0Rh1Yk4j3mjZtSuM6", null });

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
                column: "AdminPasswordHash",
                value: "$2a$11$wKHdyrvmorruwHj5OFdxP.5UKgJDiC7YPrU59EDaZEojtx.HO7ezy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasCustomPassword",
                table: "admins");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "admins");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$vSii9inmg7FkB4pdRrph9u9GS/9VIvAiysJ1aCrn1aMy1GXusTojy");

            migrationBuilder.UpdateData(
                table: "admins",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$dQItxJzr.kPWZj3fJm8ITOAh/qRSoEPgAsjbvC3ccGofM1yW2JAMO");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 1,
                column: "AdminPasswordHash",
                value: "$2a$11$cNXTY1r.MR2oxNC7uf40WOtd8SKtFNdclonZCqPN2n3DzkjwoncQO");

            migrationBuilder.UpdateData(
                table: "barbershops",
                keyColumn: "Id",
                keyValue: 2,
                column: "AdminPasswordHash",
                value: "$2a$11$7u6..PbuHvPIieEYCnt/dOWhDoVbOCdBC7PUoaZ50ZHi8an68hcsm");
        }
    }
}
