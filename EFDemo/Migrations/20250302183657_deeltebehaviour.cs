using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFDemo.Migrations
{
    /// <inheritdoc />
    public partial class deeltebehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePhotos_Movies_MovieId",
                schema: "ef",
                table: "MoviePhotos");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePhotos_Movies_MovieId",
                schema: "ef",
                table: "MoviePhotos",
                column: "MovieId",
                principalSchema: "ef",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePhotos_Movies_MovieId",
                schema: "ef",
                table: "MoviePhotos");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePhotos_Movies_MovieId",
                schema: "ef",
                table: "MoviePhotos",
                column: "MovieId",
                principalSchema: "ef",
                principalTable: "Movies",
                principalColumn: "Id");
        }
    }
}
