using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kompetenzgipfel.Migrations
{
    /// <inheritdoc />
    public partial class SupportTaskadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportTasks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 10000, nullable: false),
                    Duration = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RequiredSupporters = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportPromises",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SupportTaskId = table.Column<string>(type: "TEXT", nullable: false),
                    SupporterId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportPromises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportPromises_AspNetUsers_SupporterId",
                        column: x => x.SupporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportPromises_SupportTasks_SupportTaskId",
                        column: x => x.SupportTaskId,
                        principalTable: "SupportTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportPromises_SupporterId",
                table: "SupportPromises",
                column: "SupporterId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportPromises_SupportTaskId",
                table: "SupportPromises",
                column: "SupportTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportPromises");

            migrationBuilder.DropTable(
                name: "SupportTasks");
        }
    }
}
