namespace GroupOneFlight.Areas.Airlines.Models
{
    public class FlightOptions
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        // Pricing
        public decimal EconomyPrice { get; set; }
        public decimal BusinessPrice { get; set; }
        public decimal FirstClassPrice { get; set; }

        // Availability / Inventory
        public int AvailableSeatsEconomy { get; set; }
        public int AvailableSeatsBusiness { get; set; }
        public int AvailableSeatsFirstClass { get; set; }

        // Aircraft Information
        public string? AircraftType { get; set; }
        public int TotalCapacity { get; set; }

        // Additional Info
        public int NumberOfStops { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
