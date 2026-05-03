
namespace GroupOneFlight.Models.ViewModels
{
    public class FlightRegulationViewModel
    {
        public string Title { get; set; } = "Flight Regulations";

        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        public string Rules { get; set; } =
            "All flights must comply with airline safety and scheduling regulations.";
    }
}