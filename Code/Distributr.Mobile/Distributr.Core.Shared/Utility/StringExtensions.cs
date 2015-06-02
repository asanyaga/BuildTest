using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Utility
{
   public static class StringExtensions
    {
       public static string RemoveWhitespace(this string input)
       {
           return new string(input.ToCharArray()
               .Where(c => !Char.IsWhiteSpace(c))
               .ToArray());
       }
    }
}
