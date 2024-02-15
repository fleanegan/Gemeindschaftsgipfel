using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kompetenzgipfel.Migrations
{
    /// <inheritdoc />
    public partial class AddTitletoTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Topics",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Topics");
        }
    }
}
