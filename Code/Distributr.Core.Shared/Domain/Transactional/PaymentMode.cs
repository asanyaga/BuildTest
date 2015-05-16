using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional
{
    public enum PaymentMode : int
    {
        Cash   = 1,
        Cheque = 2,
        MMoney = 3,
        Credit = 4,
    }
}
