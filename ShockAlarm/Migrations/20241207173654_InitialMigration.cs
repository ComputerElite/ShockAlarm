using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShockAlarm.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LastAccess = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Origin = table.Column<string>(type: "TEXT", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidUnti = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Salt = table.Column<string>(type: "TEXT", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alarms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Cron = table.Column<string>(type: "TEXT", nullable: false),
                    TimeZone = table.Column<string>(type: "TEXT", nullable: false),
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
                name: "AlarmTones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmTones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlarmTones_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OpenshockApiTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    ForOpenShockUser = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Server = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenshockApiTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenshockApiTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AlarmToneComponent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TriggerSeconds = table.Column<double>(type: "REAL", nullable: false),
                    AlarmToneId = table.Column<string>(type: "TEXT", nullable: true),
                    ControlType = table.Column<int>(type: "INTEGER", nullable: false),
                    Intensity = table.Column<byte>(type: "INTEGER", nullable: false),
                    Duration = table.Column<ushort>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlarmToneComponent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlarmToneComponent_AlarmTones_AlarmToneId",
                        column: x => x.AlarmToneId,
                        principalTable: "AlarmTones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shockers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ShockerId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApiTokenId = table.Column<string>(type: "TEXT", nullable: false),
                    AlarmId = table.Column<string>(type: "TEXT", nullable: true),
                    ToneId = table.Column<string>(type: "TEXT", nullable: true),
                    Paused = table.Column<bool>(type: "INTEGER", nullable: false),
                    LimitsId = table.Column<string>(type: "TEXT", nullable: false),
                    PermissionsId = table.Column<string>(type: "TEXT", nullable: false),
                    ControlType = table.Column<int>(type: "INTEGER", nullable: false),
                    Intensity = table.Column<byte>(type: "INTEGER", nullable: false),
                    Duration = table.Column<ushort>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shockers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shockers_AlarmTones_ToneId",
                        column: x => x.ToneId,
                        principalTable: "AlarmTones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Shockers_Alarms_AlarmId",
                        column: x => x.AlarmId,
                        principalTable: "Alarms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shockers_OpenShockShockerLimits_LimitsId",
                        column: x => x.LimitsId,
                        principalTable: "OpenShockShockerLimits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shockers_OpenShockShockerPermissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "OpenShockShockerPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AlarmToneComponent_AlarmToneId",
                table: "AlarmToneComponent",
                column: "AlarmToneId");

            migrationBuilder.CreateIndex(
                name: "IX_AlarmTones_UserId",
                table: "AlarmTones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenshockApiTokens_UserId",
                table: "OpenshockApiTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_AlarmId",
                table: "Shockers",
                column: "AlarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_ApiTokenId",
                table: "Shockers",
                column: "ApiTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_LimitsId",
                table: "Shockers",
                column: "LimitsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_PermissionsId",
                table: "Shockers",
                column: "PermissionsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shockers_ToneId",
                table: "Shockers",
                column: "ToneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlarmToneComponent");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Shockers");

            migrationBuilder.DropTable(
                name: "AlarmTones");

            migrationBuilder.DropTable(
                name: "Alarms");

            migrationBuilder.DropTable(
                name: "OpenShockShockerLimits");

            migrationBuilder.DropTable(
                name: "OpenShockShockerPermissions");

            migrationBuilder.DropTable(
                name: "OpenshockApiTokens");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
