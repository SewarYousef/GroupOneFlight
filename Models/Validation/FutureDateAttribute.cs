using System;
using System.ComponentModel.DataAnnotations;

namespace GroupOneFlight.Models.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                var today = DateTime.Today;
                var max = today.AddYears(3);

                if (date > today && date <= max)
                    return true;

                ErrorMessage = $"Date must be between {today:yyyy-MM-dd} and {max:yyyy-MM-dd}.";
                return false;
            }

            return false;
        }
    }
}