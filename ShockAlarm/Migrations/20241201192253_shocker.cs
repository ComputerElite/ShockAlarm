using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShockAlarm.Migrations
{
    public partial class shocker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alarms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Cron = table.Column<string>(type: "TEXT", nullable: false),
                    NextTrigger = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisableAfterFirstTrigger = table.Column<bool>(type: "INTEGER", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alarms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alarms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shockers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ShockerId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ControlType = table.Column<int>(type: "INTEGER", nullable: false),
                    Intensity = table.Column<byte>(type: "INTEGER", nullable: false),
                    Duration = table.Column<ushort>(type: "INTEGER", nullable: false),
                    ApiTokenId = table.Column<string>(type: "TEXT", nullable: false),
                    AlarmId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shockers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shockers_Alarms_AlarmId",
                        column: x => x.AlarmId,
                        principalTable: "Alarms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shockers_OpenshockApiTokens_ApiTokenId",
                        column: x => x.ApiTokenId,
                        principalTable: "OpenshockApiTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alarms_UserId",
                table: "Alarms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_AlarmId",
                table: "Shockers",
                column: "AlarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_ApiTokenId",
                table: "Shockers",
                column: "ApiTokenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shockers");

            migrationBuilder.DropTable(
                name: "Alarms");
        }
    }
}
