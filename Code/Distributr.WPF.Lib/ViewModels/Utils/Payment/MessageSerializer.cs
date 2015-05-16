using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.ViewModels.Utils.Payment
{
    public static class MessageSerializer
    {
        public static bool CanDeserializeMessage<T>(string jsonMessage, out T deserializedObject)
        {
            deserializedObject = default(T);
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonMessage, new IsoDateTimeConverter());
            }
            catch
            {

            }

            return deserializedObject != null;
        }
    }
}
