using System;
using System.ComponentModel.DataAnnotations;

namespace GroupOneFlight.Models
{
    /// <summary>
    /// Custom validation attribute for flight dates.
    /// Ensures date is in the future and within 3 years from today.
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                DateTime today = DateTime.Today;
                DateTime maxDate = today.AddYears(3);

                // Check if date > today AND date <= today + 3 years
                if (date > today && date <= maxDate)
                {
                    return true;
                }

                ErrorMessage = $"Date must be after today and within 3 years (between {today:yyyy-MM-dd} and {maxDate:yyyy-MM-dd}).";
                return false;
            }

            return false;
        }
    }
}
