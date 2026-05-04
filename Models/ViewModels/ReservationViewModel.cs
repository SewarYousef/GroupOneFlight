using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GroupOneFlight.Models.DomainModels;

namespace GroupOneFlight.Models.ViewModels
{
    public class ReservationViewModel
    {
        // Flights being booked (populated from session)
        public List<Flight> SelectedFlights { get; set; } = new();

        // Passenger info
        [Required(ErrorMessage = "Passenger name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters.")]
        public string PassengerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Range(1, 9, ErrorMessage = "Passenger count must be between 1 and 9.")]
        public int PassengerCount { get; set; } = 1;

        // Single cabin type for all flights in this booking
        [Required(ErrorMessage = "Please select a cabin type.")]
        public string CabinType { get; set; } = string.Empty;

        // Dropdown data
        public List<string> CabinTypes { get; set; } = new();

        // Computed totals (not bound from form)
        public decimal TotalPrice { get; set; }
    }
}
