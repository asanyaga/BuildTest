using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum MaritalStatas
    {
        Single = 0, Married = 1, Unknown = 3,Default=4
    }
}
