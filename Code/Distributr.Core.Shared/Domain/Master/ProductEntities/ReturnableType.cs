using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public enum ReturnableType
    {
       None = 0,
       GenericReturnable=1,
       Returnable=2
    }
}
