using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PaymentGateway.WSApi.Lib.Util
{
    public class SDPDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string date;
            if (value is DateTime)
            {
                date = DateTime.Now.ToString("yyyyMMddhhmmss");
            }
            else
            {
                throw new Exception("Expected date object value.");
            }
            writer.WriteValue(date);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new Exception(
                    String.Format("Unexpected token parsing date. Expected Integer, got {0}.",
                    reader.TokenType));
            }

            string[] formats = new string[] { "yyyyMMddHHmmss", "yyyyMMddhhmmss" };
            DateTime myDate = DateTime.ParseExact(reader.Value.ToString().Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None);

            return myDate;
        }
    }
}
