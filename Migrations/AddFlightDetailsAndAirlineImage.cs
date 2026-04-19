using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupOneFlight.Migrations
{
    public partial class AddFlightDetailsAndAirlineImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AircraftType",
                table: "Flights",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Emission",
                table: "Flights",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Flights",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Airlines",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AircraftType", table: "Flights");
            migrationBuilder.DropColumn(name: "Emission",     table: "Flights");
            migrationBuilder.DropColumn(name: "Price",        table: "Flights");
            migrationBuilder.DropColumn(name: "ImageName",    table: "Airlines");
        }
    }
}
