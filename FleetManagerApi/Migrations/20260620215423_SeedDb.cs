using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedTruckId",
                table: "Drivers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentLatitude",
                table: "Drivers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentLongitude",
                table: "Drivers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStatus",
                table: "Drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTruckId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "Drivers");
        }
    }
}
