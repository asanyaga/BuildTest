using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Services.Webservice
{
    public interface INotificationValidation
    {
        bool CanDeserializeNotification<T>(string jsonNotification, out T deserializedObject);
    }
}
