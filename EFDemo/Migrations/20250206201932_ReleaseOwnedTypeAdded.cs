using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFDemo.Migrations
{
    /// <inheritdoc />
    public partial class ReleaseOwnedTypeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Release_Amount",
                schema: "ef",
                table: "Movies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Release_Date",
                schema: "ef",
                table: "Movies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Release_Amount",
                schema: "ef",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Release_Date",
                schema: "ef",
                table: "Movies");
        }
    }
}
