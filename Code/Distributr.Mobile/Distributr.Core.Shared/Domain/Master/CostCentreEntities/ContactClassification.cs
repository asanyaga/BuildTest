using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public enum ContactClassification
    {
       None = 0,
       PrimaryContact=1,
       SecondaryContact=2
    }
}
