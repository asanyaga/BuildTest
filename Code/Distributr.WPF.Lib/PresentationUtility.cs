using System;
using System.Linq;

namespace Distributr.WPF.Lib
{
    public class PresentationUtility
    {
        public static Guid ParseIdFromUri(Uri value)
        {
            var lastToken = GetLastTokenFromUri(value);

            if (lastToken == null)
            {
                throw new ArgumentException(String.Format("Could not parse Id value from uri '{0}'.", value));
            }
            else
            {
                Guid id = Guid.Empty;

                if (Guid.TryParse(lastToken, out id) == false)
                {
                    throw new ArgumentException(String.Format("Id value in uri '{0}' was not an Int32 value.", value));
                }
                else
                {
                    return id;
                }
            }
        }

        public static string GetLastTokenFromUri(Uri value)
        {
            var uriAsString = value.ToString();

            if (uriAsString.Contains("?") == false || uriAsString == "?")
            {
                return null;
            }
            else
            {
                var splitByForwardSlash = uriAsString.Split('?');

                if (splitByForwardSlash.Length == 0)
                {
                    return null;
                }
                else
                {
                    return splitByForwardSlash[splitByForwardSlash.Length - 1];
                }
            }
        }

        public static Guid ParseIdFromUrl(Uri value)
        {
            var uriAsString = value.ToString();

            if (uriAsString.Contains("?") == false || uriAsString == "?")
            {
                return Guid.Empty;
            }
            else
            {
                var splitByForwardSlash = uriAsString.Split('?');

                if (splitByForwardSlash.Length == 0)
                {
                    return Guid.Empty;
                }
                else
                {
                    string idStr = splitByForwardSlash[splitByForwardSlash.Length - 1];
                    Guid id = Guid.Empty;

                    //if (Guid.TryParse(idStr, out id) == false)
                    //{
                    //    throw new ArgumentException(String.Format("Id value in uri '{0}' was not a valid Guid value.", value));
                    //}
                    //else
                    //{
                    //    return id;
                    //}

                    try
                    {
                        Guid.TryParse(idStr, out id);
                    }
                    catch{}
                    return id;
                }
            }
        }

        public static string ParseQueryString(Uri url, string token)
        {
            string retVal = "";
            string lastToken = GetLastTokenFromUri(url);
            if (!string.IsNullOrEmpty(lastToken))
            {
                string[] keyValuePairs = lastToken.Split('&');//x=1&y=2
                if (keyValuePairs.Any())
                {
                    string keyValuePair = keyValuePairs.FirstOrDefault(n => n.Split('=').First().ToLower() == token.ToLower());
                    if (keyValuePair != null) retVal = keyValuePair.Split('=').Last().Trim();
                }
            }
            return retVal;
        }
    }
}
