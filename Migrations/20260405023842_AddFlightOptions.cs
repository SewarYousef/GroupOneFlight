using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupOneFlight.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FlightId = table.Column<int>(type: "INTEGER", nullable: false),
                    EconomyPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    BusinessPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    FirstClassPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    AvailableSeatsEconomy = table.Column<int>(type: "INTEGER", nullable: false),
                    AvailableSeatsBusiness = table.Column<int>(type: "INTEGER", nullable: false),
                    AvailableSeatsFirstClass = table.Column<int>(type: "INTEGER", nullable: false),
                    AircraftType = table.Column<string>(type: "TEXT", nullable: true),
                    TotalCapacity = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfStops = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightOptions_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightOptions_FlightId",
                table: "FlightOptions",
                column: "FlightId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightOptions");
        }
    }
}
