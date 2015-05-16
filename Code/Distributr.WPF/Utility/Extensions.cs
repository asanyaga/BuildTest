using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WPF.UI.Utility
{
    public static class Extensions
    {
        public static string ParseQueryString(this string url, string paramName)
        {
            string returnVal = "";
            try
            {
                string query = url.Split('?')[1];
                string[] queryItems = query.Split('&');
                foreach (var queryItem in queryItems)
                {
                    if (queryItem.Contains('='))
                    {
                        string[] qStrings = queryItem.Split('=');
                        if (qStrings[0].ToLower() == paramName.ToLower())
                            returnVal = qStrings[1];
                    }
                    else
                    {
                        if (queryItem == paramName)
                            returnVal = queryItem;
                    }
                }
            }
            catch
            {
            }
            return returnVal;
        }
    }
}
