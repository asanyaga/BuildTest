using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PaymentGateway.WSApi.Lib.Services.Webservice.Impl
{
    public class NotificationValidation : INotificationValidation
    {
        public bool CanDeserializeNotification<T>(string jsonNotification, out T deserializedObject)
        {
            deserializedObject = default(T);
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonNotification, new IsoDateTimeConverter());

            }
            catch(Exception ex)
            {
                //TODO Log
            }
            return deserializedObject != null;
        }
    }
}
