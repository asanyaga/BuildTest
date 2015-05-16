using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PaymentGateway.WSApi.Lib.Validation
{
    public static class ValidationSummary
    {
        public static void DisplayDomainValidationResult(DomainValidationException dve, ModelStateDictionary modelState)
        {
            ValidationResultInfo vri = dve.ValidationResults;
            List<ValidationResult> vrs = vri.Results;
            foreach (var item in vrs)
            {
                modelState.AddModelError("", item.ErrorMessage);
            }
        }
        public static void DisplayValidationResult(string exMessage, ModelStateDictionary modelState)
        {
            modelState.AddModelError("", exMessage);

        }



    }
}
