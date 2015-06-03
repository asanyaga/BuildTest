using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public enum Gender
    {
        Male = 1, Female = 2, Unknown = 0
    }
}
