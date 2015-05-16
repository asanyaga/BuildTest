using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Utility
{
    public static class ValidationExtensions
    {
        public static ValidationResultInfo BasicValidation<T>(this T itemToValidate)
        {
            ValidationContext vt = new ValidationContext(itemToValidate, null, null);
            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateObject(itemToValidate, vt, results, true);

            return new ValidationResultInfo { Results = results };
        }
    }
}
