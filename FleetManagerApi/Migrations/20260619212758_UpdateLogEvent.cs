using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "LogEvent",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "LogEvent",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "LogEvent");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "LogEvent");
        }
    }
}
