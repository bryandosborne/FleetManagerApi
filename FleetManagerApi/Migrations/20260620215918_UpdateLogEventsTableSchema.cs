using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogEventsTableSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEvent_Drivers_DriverId",
                table: "LogEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogEvent",
                table: "LogEvent");

            migrationBuilder.RenameTable(
                name: "LogEvent",
                newName: "LogEvents");

            migrationBuilder.RenameIndex(
                name: "IX_LogEvent_DriverId",
                table: "LogEvents",
                newName: "IX_LogEvents_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogEvents",
                table: "LogEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEvents_Drivers_DriverId",
                table: "LogEvents",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogEvents_Drivers_DriverId",
                table: "LogEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogEvents",
                table: "LogEvents");

            migrationBuilder.RenameTable(
                name: "LogEvents",
                newName: "LogEvent");

            migrationBuilder.RenameIndex(
                name: "IX_LogEvents_DriverId",
                table: "LogEvent",
                newName: "IX_LogEvent_DriverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogEvent",
                table: "LogEvent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogEvent_Drivers_DriverId",
                table: "LogEvent",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
