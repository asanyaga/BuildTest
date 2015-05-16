using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Resources.Util
{
   public static class MessageResourceHelper
    {
       public static Dictionary<string, string> Process(IList<string> resource)
       {
           Dictionary<string, string> _resources = new Dictionary<string, string>();
           string[] lines = resource.Where(s => !s.Trim().ToString().StartsWith("#"))
               .Where(s => s.Trim().ToString() != string.Empty)
               .Where(s => s.Contains("="))
               .ToArray();
           var data = lines.Select(line => line.Split('='))
               .Select(bits => new {Key = bits[0], value = bits[1]})
               .GroupBy(g => g.Key);
           _resources = data.ToDictionary(bits => bits.Key.Trim(), bits => bits.Last().value.Trim());
           return _resources;
       }
    }
}
