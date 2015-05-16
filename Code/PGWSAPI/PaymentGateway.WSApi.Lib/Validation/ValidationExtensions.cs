using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Validation
{
    public static class ValidationExtensions
    {
        public static ValidationResultInfo BasicValidation<T>(this T objToValidate)
        {
            var vt = new ValidationContext(objToValidate, null, null);
            var result = new List<ValidationResult>();
            Validator.TryValidateObject(objToValidate, vt, result, true);
            return new ValidationResultInfo { Results = result };
        }
    }
}
