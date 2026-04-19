namespace GroupOneFlight.Areas.Airlines.Models
{
    public class FlightFilter
    {
        public string?   From          { get; set; }
        public string?   To            { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string?   CabinType     { get; set; }
    }
}
