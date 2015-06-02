using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Distributr.Core.Utility.Serialization
{
   public abstract class JsonCreationConverter<T> : JsonConverter 

    {
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
      
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            JObject jObject = JObject.Load(reader);
            T target = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value,
                                       JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
