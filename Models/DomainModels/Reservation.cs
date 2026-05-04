using System;
using System.ComponentModel.DataAnnotations;

namespace GroupOneFlight.Models.DomainModels
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string ConfirmationNumber { get; set; } = string.Empty;

        // Foreign key
        public int FlightId { get; set; }
        public Flight? Flight { get; set; }

        // Passenger info
        [Required]
        public string PassengerName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(1, 9)]
        public int PassengerCount { get; set; } = 1;

        // Cabin chosen at booking time
        public string CabinType { get; set; } = string.Empty;

        // Price snapshot at booking time
        public decimal TotalPrice { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}
