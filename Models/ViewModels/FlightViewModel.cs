using System.Collections.Generic;
using GroupOneFlight.Models.DomainModels;


namespace GroupOneFlight.Models.ViewModels
{
    public class FlightViewModel
    {
        // Core entities
        public List<Flight> Flights { get; set; } = new();
        public List<Airline> Airlines { get; set; } = new();
        public List<FlightOptions> FlightOptions { get; set; } = new();

        // Filter lists for dropdowns
        public List<string> FromCities { get; set; } = new();
        public List<string> ToCities { get; set; } = new();
        public List<string> CabinTypes { get; set; } = new();
        public List<string> AircraftTypes { get; set; } = new();

        // Current filter values
        public string? SelectedFrom { get; set; }
        public string? SelectedTo { get; set; }
        public string? SelectedCabin { get; set; }

        // Pricing information
        public Dictionary<int, decimal> MinPriceByFlightId { get; set; } = new();
    }
}