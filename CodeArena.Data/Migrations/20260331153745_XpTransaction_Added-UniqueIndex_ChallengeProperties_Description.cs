using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeArena.Data.Migrations
{
    /// <inheritdoc />
    public partial class XpTransaction_AddedUniqueIndex_ChallengeProperties_Description : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_XpTransactions_UserId",
                table: "XpTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "XpTransactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ChallengeId",
                table: "XpTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "XpTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 31, 15, 37, 43, 487, DateTimeKind.Utc).AddTicks(6277));

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 31, 15, 37, 43, 487, DateTimeKind.Utc).AddTicks(6281));

            migrationBuilder.CreateIndex(
                name: "IX_XpTransactions_ChallengeId",
                table: "XpTransactions",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_XpTransactions_UserId_ChallengeId_Reason",
                table: "XpTransactions",
                columns: new[] { "UserId", "ChallengeId", "Reason" },
                unique: true,
                filter: "[ChallengeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_XpTransactions_Challenges_ChallengeId",
                table: "XpTransactions",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_XpTransactions_Challenges_ChallengeId",
                table: "XpTransactions");

            migrationBuilder.DropIndex(
                name: "IX_XpTransactions_ChallengeId",
                table: "XpTransactions");

            migrationBuilder.DropIndex(
                name: "IX_XpTransactions_UserId_ChallengeId_Reason",
                table: "XpTransactions");

            migrationBuilder.DropColumn(
                name: "ChallengeId",
                table: "XpTransactions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "XpTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "XpTransactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 14, 48, 59, 485, DateTimeKind.Utc).AddTicks(4199));

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 29, 14, 48, 59, 485, DateTimeKind.Utc).AddTicks(4205));

            migrationBuilder.CreateIndex(
                name: "IX_XpTransactions_UserId",
                table: "XpTransactions",
                column: "UserId");
        }
    }
}
