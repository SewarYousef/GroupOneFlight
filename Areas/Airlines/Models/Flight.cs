using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using GroupOneFlight.Models;

namespace GroupOneFlight.Areas.Airlines.Models
{
    public class Flight
    {
        public int Id { get; set; }
        [Remote(
            action: "IsFlightCodeDateUnique",
            controller: "Flights",
            areaName: "Airlines",
            AdditionalFields = "Date,Id",
            ErrorMessage = "This FlightCode + Date combination already exists. Please use a different flight code or date."
        )]
        public string? FlightNumber  { get; set; }

        public string? From          { get; set; }
        public string? To            { get; set; }

        // Rahul's custom future-date validation
        [FutureDate]
        public DateTime Date         { get; set; }

        public string? DepartureTime { get; set; }
        public string? ArrivalTime   { get; set; }
        public string? CabinType     { get; set; }

        // Foreign key
        public int     AirlineId { get; set; }
        public Airline? Airline  { get; set; }
    }
}
