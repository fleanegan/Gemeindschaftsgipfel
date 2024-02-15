using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kompetenzgipfel.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptiontoTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Topics",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Topics");
        }
    }
}
