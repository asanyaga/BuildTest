using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PaymentGateway.WSApi.Lib.Util;

namespace PaymentGateway.WSApi.Lib.Services.Payment.Impl
{
    public class MessageValidation : IMessageValidation
    {
        public bool CanDeserializeMessage<T>(string jsonMessage, out T deserializedObject)
        {
            deserializedObject = default(T);
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonMessage, new IsoDateTimeConverter());
            }
            catch
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonMessage, new SDPDateTimeConverter());
            }

            return deserializedObject != null;
        }
    }
}
