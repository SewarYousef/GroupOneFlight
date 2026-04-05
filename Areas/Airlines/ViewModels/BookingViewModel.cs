using System.Collections.Generic;
using GroupOneFlight.Areas.Airlines.Models;

namespace GroupOneFlight.Areas.Airlines.ViewModels
{
    public class BookingViewModel
    {
        // Selected flights for booking
        public List<Flight> SelectedFlights { get; set; } = new();
        public List<FlightOptions> FlightOptions { get; set; } = new();
        public List<Airline> Airlines { get; set; } = new();

        // Booking details
        public Dictionary<int, string> SelectedCabinByFlightId { get; set; } = new();
        public Dictionary<int, int> PassengerCountByFlightId { get; set; } = new();

        // Total summary
        public decimal TotalPrice { get; set; }
        public int TotalPassengers { get; set; }

        // Lists for dropdowns
        public List<string> CabinTypes { get; set; } = new();
    }
}
