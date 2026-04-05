using System.Collections.Generic;
namespace GroupOneFlight.Areas.Airlines.Models
{
    public class Airline
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<Flight>? Flights { get; set; }
    }
}