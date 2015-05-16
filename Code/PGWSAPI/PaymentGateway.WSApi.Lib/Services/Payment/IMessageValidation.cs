using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Services.Payment
{
    public interface IMessageValidation
    {
        bool CanDeserializeMessage<T>(string jsonNotification, out T deserializedObject);
    }
}
