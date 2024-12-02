using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShockAlarm.Migrations
{
    /// <inheritdoc />
    public partial class shared_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PermissionsId",
                table: "Shockers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OpenShockShockerPermissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    shock = table.Column<bool>(type: "INTEGER", nullable: false),
                    vibrate = table.Column<bool>(type: "INTEGER", nullable: false),
                    sound = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenShockShockerPermissions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_PermissionsId",
                table: "Shockers",
                column: "PermissionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shockers_OpenShockShockerPermissions_PermissionsId",
                table: "Shockers",
                column: "PermissionsId",
                principalTable: "OpenShockShockerPermissions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shockers_OpenShockShockerPermissions_PermissionsId",
                table: "Shockers");

            migrationBuilder.DropTable(
                name: "OpenShockShockerPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Shockers_PermissionsId",
                table: "Shockers");

            migrationBuilder.DropColumn(
                name: "PermissionsId",
                table: "Shockers");
        }
    }
}
