using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kompetenzgipfel.Migrations
{
    /// <inheritdoc />
    public partial class Presentermandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics");

            migrationBuilder.AlterColumn<string>(
                name: "PresenterId",
                table: "Topics",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics",
                column: "PresenterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics");

            migrationBuilder.AlterColumn<string>(
                name: "PresenterId",
                table: "Topics",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_AspNetUsers_PresenterId",
                table: "Topics",
                column: "PresenterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
