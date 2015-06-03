using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Utility
{
   public class CustomSelectListItem
    {
       public string Text { get; set; }
       public string Value { get; set; }
    }
  


   public class TranferResponse<T> 
   {
       public bool Status { get; set; }

       public string Info { get; set; }
       public TranferResponse()
       {
           Data = new List<T>();
       }

       public int RecordCount { get; set; }
       public List<T> Data;
   }
}
