namespace GroupOneFlight.Areas.Airlines.Models
{
    public class SearchViewModel
    {
        public List<Flight>  Flights        { get; set; } = new();
        public List<Airline> Airlines       { get; set; } = new();
        public FlightFilter  Filter         { get; set; } = new();
        public int           SelectionCount { get; set; }
        public List<string>  FromCities     { get; set; } = new();
        public List<string>  ToCities       { get; set; } = new();

        public static List<string> GetCabinTypes()    => CabinTypes.GetAll();
        public static List<string> GetAircraftTypes() => AircraftTypes.GetAll();
    }
}
