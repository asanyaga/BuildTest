using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Distributr.Mobile.Core.Net
{
    public class HttpParams
    {
        private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

        public void Add(string key, string val)
        {
            if (parameters.ContainsKey(key))
            {
                throw new InvalidOperationException(string.Format("The key {0} already exists.", key));
            }
            parameters.Add(key, val);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            if (parameters.Count > 0)
            {
                buffer.Append("?");
            }
            foreach (var kvp in parameters)
            {
                if (buffer.Length > 1)
                {
                    buffer.Append("&");
                }
                buffer.AppendFormat("{0}={1}",
                    WebUtility.UrlEncode(kvp.Key),
                    WebUtility.UrlEncode(kvp.Value));
            }
            return buffer.ToString();
        }
    }
}