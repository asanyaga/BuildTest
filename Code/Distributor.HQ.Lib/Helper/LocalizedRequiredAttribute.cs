using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Distributr.Core.Resources.Util;
using StructureMap;

namespace Distributr.HQ.Lib.Helper
{
    public class LocalizedRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        private string _displayName;

        public LocalizedRequiredAttribute()
        {
            ErrorMessageResourceName = "Validation_Required";

        }

        protected override ValidationResult IsValid
        (object value, ValidationContext validationContext)
        {
            _displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            string msg = ErrorMessage;

            IMessageSourceAccessor msa = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            if (msa != null)
                msg = msa.GetText(ErrorMessage);
            return string.Format(msg, _displayName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            string msg = ErrorMessage;
           
            IMessageSourceAccessor msa = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            if (msa != null)
                msg = msa.GetText(ErrorMessage);
            yield return new ModelClientValidationRule
                             {
                                 ErrorMessage = msg,
                                 ValidationType = "required"
                             };
        }
    }
}
