using System.Collections.Generic;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.ViewModels
{
    public class FlightIndexViewModel
    {
        // Main flight list
        public List<Flight> Flights { get; set; } = new();

        public List<FlightOptions> FlightOptions { get; set; } = new();

        public Dictionary<int, decimal> MinPriceByFlightId { get; set; } = new();

        // Filters (dropdowns)
        public List<string> FromCities { get; set; } = new();
        public List<string> ToCities { get; set; } = new();
        public List<string> CabinTypes { get; set; } = new();

        // Selected filters
        public string? SelectedFrom { get; set; }
        public string? SelectedTo { get; set; }
        public string? SelectedCabin { get; set; }
    }
}