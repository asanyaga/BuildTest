using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Utility.Validation
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
