using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFDemo.Migrations
{
    /// <inheritdoc />
    public partial class discriminator_added2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoTypeEntity",
                schema: "ef",
                table: "VideoTypeEntity");

            migrationBuilder.RenameTable(
                name: "VideoTypeEntity",
                schema: "ef",
                newName: "VideoTypes",
                newSchema: "ef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoTypes",
                schema: "ef",
                table: "VideoTypes",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoTypes",
                schema: "ef",
                table: "VideoTypes");

            migrationBuilder.RenameTable(
                name: "VideoTypes",
                schema: "ef",
                newName: "VideoTypeEntity",
                newSchema: "ef");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoTypeEntity",
                schema: "ef",
                table: "VideoTypeEntity",
                column: "Id");
        }
    }
}
