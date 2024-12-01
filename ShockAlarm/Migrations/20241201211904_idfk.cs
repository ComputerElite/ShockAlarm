using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShockAlarm.Migrations
{
    public partial class idfk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shockers_Alarms_AlarmId",
                table: "Shockers");

            migrationBuilder.AddForeignKey(
                name: "FK_Shockers_Alarms_AlarmId",
                table: "Shockers",
                column: "AlarmId",
                principalTable: "Alarms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shockers_Alarms_AlarmId",
                table: "Shockers");

            migrationBuilder.AddForeignKey(
                name: "FK_Shockers_Alarms_AlarmId",
                table: "Shockers",
                column: "AlarmId",
                principalTable: "Alarms",
                principalColumn: "Id");
        }
    }
}
