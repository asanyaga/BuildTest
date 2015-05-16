using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Impl.Services.Utility
{
   public class AutoSyncException : Exception
    {
       public AutoSyncException(string message) : base(message)
       {
       }
    }
}
