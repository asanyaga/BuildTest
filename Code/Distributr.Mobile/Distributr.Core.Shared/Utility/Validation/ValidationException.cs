using System;

namespace Distributr.Core.Utility.Validation
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException(ValidationResultInfo validationResults, string message)
            : base(message)
        {
            ValidationResults = validationResults;
        }

        public ValidationResultInfo ValidationResults {get;set;}
    }

}
