using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShockAlarm.Migrations
{
    /// <inheritdoc />
    public partial class shared : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LimitsId",
                table: "Shockers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Paused",
                table: "Shockers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "OpenShockShockerLimits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    intensity = table.Column<byte>(type: "INTEGER", nullable: false),
                    duration = table.Column<ushort>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenShockShockerLimits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_LimitsId",
                table: "Shockers",
                column: "LimitsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shockers_OpenShockShockerLimits_LimitsId",
                table: "Shockers",
                column: "LimitsId",
                principalTable: "OpenShockShockerLimits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shockers_OpenShockShockerLimits_LimitsId",
                table: "Shockers");

            migrationBuilder.DropTable(
                name: "OpenShockShockerLimits");

            migrationBuilder.DropIndex(
                name: "IX_Shockers_LimitsId",
                table: "Shockers");

            migrationBuilder.DropColumn(
                name: "LimitsId",
                table: "Shockers");

            migrationBuilder.DropColumn(
                name: "Paused",
                table: "Shockers");
        }
    }
}
