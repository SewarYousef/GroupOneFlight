using System;

namespace GroupOneFlight.Areas.Airlines.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string? FlightNumber { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public DateTime Date { get; set; }
        public string? DepartureTime { get; set; }
        public string? ArrivalTime { get; set; }
        public string? CabinType { get; set; }

        public int AirlineId { get; set; }
        public Airline? Airline { get; set; }
    }
}