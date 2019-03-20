using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Services.Attributes
{
    public class DateFormatValidatorAttribute : ValidationAttribute
    {
        public const string InvalidDateErrorMessage = "Please enter the date in MM/DD/YYYY format.";

        public DateFormatValidatorAttribute()
        {
            ErrorMessage = InvalidDateErrorMessage;
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                DateTime dateTime;
                return DateTime.TryParseExact(value.ToString(), "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out dateTime);
            }
            else
            {
                return true;
            }
        }
    }
}
