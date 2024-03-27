using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kompetenzgipfel.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreatortoPresenterYAGNI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_CreatorId",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Topics",
                newName: "PresenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_CreatorId",
                table: "Topics",
                newName: "IX_Topics_PresenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics",
                column: "PresenterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "PresenterId",
                table: "Topics",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_PresenterId",
                table: "Topics",
                newName: "IX_Topics_CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_CreatorId",
                table: "Topics",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
