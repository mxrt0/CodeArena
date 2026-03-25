using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeArena.Data.Migrations
{
    /// <inheritdoc />
    public partial class Challenge_MadeSlugRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Challenges",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Slug" },
                values: new object[] { new DateTime(2026, 3, 23, 17, 29, 43, 604, DateTimeKind.Utc).AddTicks(4380), "sum-two-numbers" });

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Slug" },
                values: new object[] { new DateTime(2026, 3, 23, 17, 29, 43, 604, DateTimeKind.Utc).AddTicks(4388), "fizzbuzz" });

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_Slug",
                table: "Challenges",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Challenges_Slug",
                table: "Challenges");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Challenges",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Slug" },
                values: new object[] { new DateTime(2026, 3, 23, 16, 39, 54, 675, DateTimeKind.Utc).AddTicks(449), null });

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Slug" },
                values: new object[] { new DateTime(2026, 3, 23, 16, 39, 54, 675, DateTimeKind.Utc).AddTicks(456), null });
        }
    }
}
