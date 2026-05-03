using System.Collections.Generic;

namespace GroupOneFlight.Models.DomainModels
{
    public class Airline
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ImageName { get; set; }

        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}