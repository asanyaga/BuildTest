using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Validation;

namespace PaymentGateway.WSApi.Lib.Repository.MasterData
{
    public interface IValidation<T> where T : class
    {
        ValidationResultInfo Validate(T objToValidate);
    }
}
