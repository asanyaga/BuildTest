using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Utility.Validation
{
    public class DateOfBirthAttribute : ValidationAttribute
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string DefaultErrorMessage = "'{0}' must be a date between {1:d} and {2:d}.";

        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(string.Format(ErrorMessage = "Please input a date value"));

            var val = (DateTime)value;

            if (val.AddYears(MinAge) > DateTime.Now)
                return new ValidationResult(string.Format(ErrorMessage="Minimum age is 18 years"));
            else
                return null;
            //return (val.AddYears(MaxAge) > DateTime.Now);
        }


    }
}
