using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManagerApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoadLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "DeliveryLocation",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "Loads");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentLatitude",
                table: "Loads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentLongitude",
                table: "Loads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryLatitude",
                table: "Loads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryLongitude",
                table: "Loads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PickupLatitude",
                table: "Loads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PickupLongitude",
                table: "Loads",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "DeliveryLatitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "DeliveryLongitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "PickupLatitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "PickupLongitude",
                table: "Loads");

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "Loads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation",
                table: "Loads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "Loads",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
