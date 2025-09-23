using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarberShopAPI.Server.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsedPasswordResetTokensTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "used_password_reset_tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_used_password_reset_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_used_password_reset_tokens_admins_AdminId",
                        column: x => x.AdminId,
                        principalTable: "admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_used_password_reset_tokens_barbershops_TenantId",
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

            migrationBuilder.CreateIndex(
                name: "IX_used_password_reset_tokens_AdminId",
                table: "used_password_reset_tokens",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_used_password_reset_tokens_TenantId",
                table: "used_password_reset_tokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_used_password_reset_tokens_TokenHash",
                table: "used_password_reset_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_used_password_reset_tokens_UsedAt",
                table: "used_password_reset_tokens",
                column: "UsedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "used_password_reset_tokens");

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
                column: "PasswordHash",
                value: "$2a$11$1f2fGZFonZYlqMVjU4JDaOwE5ieYIiUnrq.hU3LNlmMP9l4qOL2EG");

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
                column: "AdminPasswordHash",
                value: "$2a$11$EdP4jFzY7y39s8jwINve6OzW31gGSCSQw6PhQHB5qcmPLU4/SespS");
        }
    }
}
