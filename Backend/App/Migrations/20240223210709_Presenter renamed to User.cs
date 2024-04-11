using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gemeinschaftsgipfel.Migrations
{
    /// <inheritdoc />
    public partial class PresenterrenamedtoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "PresenterId",
                table: "Topics",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_PresenterId",
                table: "Topics",
                newName: "IX_Topics_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_UserId",
                table: "Topics",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_UserId",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Topics",
                newName: "PresenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_UserId",
                table: "Topics",
                newName: "IX_Topics_PresenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics",
                column: "PresenterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
