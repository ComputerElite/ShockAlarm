using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShockAlarm.Migrations
{
    /// <inheritdoc />
    public partial class idk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shockers_ShockerId",
                table: "Shockers",
                column: "ShockerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shockers_ShockerId",
                table: "Shockers");
        }
    }
}
