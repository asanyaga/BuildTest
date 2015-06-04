using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master
{
    public interface IVATClassFactory
    {
        VATClass CreateVATClass(string name, string vatClass, decimal rate, DateTime effectiveDate);
    }
}
