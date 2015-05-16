using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.Validation
{
    public class DateOfBirthValidatorAttribute  : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            var isValid = value != null && (DateTime)value < DateTime.Now;
            return isValid;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "dateofbirth"
            };
        }
    }
}
