using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using GroupOneFlight.Models.DomainModels;
using GroupOneFlight.Models.Validation;

namespace GroupOneFlight.Models.DomainModels
{
    public class Flight
    {
        public int Id { get; set; }

        [Remote(
            action: "IsFlightCodeDateUnique",
            controller: "Flights",
            areaName: "Airlines",
            AdditionalFields = "Date,Id",
            ErrorMessage = "This FlightCode + Date combination already exists."
        )]
        public string? FlightCode { get; set; }

        public string? From { get; set; }
        public string? To { get; set; }

        [FutureDate]
        public DateTime Date { get; set; }

        public string? DepartureTime { get; set; }
        public string? ArrivalTime { get; set; }
        public string? CabinType { get; set; }
        public string? AircraftType { get; set; }

        public decimal Emission { get; set; }
        public decimal Price { get; set; }

        public int AirlineId { get; set; }
        public Airline? Airline { get; set; }
    }
}