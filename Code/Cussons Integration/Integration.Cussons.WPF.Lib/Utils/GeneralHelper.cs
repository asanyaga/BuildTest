using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

namespace Integration.Cussons.WPF.Lib.Utils
{
   public static class GeneralHelper
    {
       public static EntityStatus GetStatus(string status)
       {
           var st = EntityStatus.New;
           if (string.IsNullOrEmpty(status))
               return st;
           switch (status)
           {
               case "0":
                   st = EntityStatus.New;
                   break;
               case "1":
                   st = EntityStatus.Active;
                   break;
               case "2":
                   st = EntityStatus.Inactive;
                   break;
               case "3":
                   st = EntityStatus.Deleted;
                   break;

           }
           return st;
       }
    }
}
