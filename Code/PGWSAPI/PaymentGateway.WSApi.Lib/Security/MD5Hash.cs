using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Security
{
   public static class Md5Hash
    {
       public static string GetMd5Hash(string input)
       {
          var x = new MD5CryptoServiceProvider();
           byte[] bs = Encoding.UTF8.GetBytes(input);
           bs = x.ComputeHash(bs);
           var s = new StringBuilder();
           foreach (byte b in bs)
           {
               s.Append(b.ToString("x2").ToLower());
           }
           return s.ToString();

       }

    }
}
