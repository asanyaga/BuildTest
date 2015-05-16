using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Validation
{
    public class DomainValidationException: Exception
    {
        public DomainValidationException(ValidationResultInfo vri, string msg)
            : base(msg)
        {
            ValidationResults = vri;
        }
        public ValidationResultInfo ValidationResults { get; set; }
    }
}
