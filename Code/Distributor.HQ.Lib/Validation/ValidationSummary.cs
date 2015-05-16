using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.Validation
{
   public static class ValidationSummary
    {
       public static void DomainValidationErrors(DomainValidationException ve,ModelStateDictionary modelState)
       {

           modelState.AddModelError("", ve.Message);
           ValidationResultInfo vri = ve.ValidationResults;
           List<ValidationResult> vrs = vri.Results;
           foreach (ValidationResult vr in vrs)
           {
             
               modelState.AddModelError("", vr.ErrorMessage);
           }
          
       }


       public static void DomainValidationErrors(String dve, ModelStateDictionary modelState)
       {
           modelState.AddModelError("", dve);
       }
    }
}
