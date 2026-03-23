using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeArena.Data.Migrations
{
    /// <inheritdoc />
    public partial class Challenge_AddedSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Challenges",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Challenges");

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 18, 8, 30, 863, DateTimeKind.Utc).AddTicks(5181));

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 18, 8, 30, 863, DateTimeKind.Utc).AddTicks(5188));
        }
    }
}
