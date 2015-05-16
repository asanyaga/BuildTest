using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Factory.Master.Impl
{
    public class VatClassFactory : IVATClassFactory
    {
      
        public VATClass CreateVATClass(string name, string vatClass, decimal rate, DateTime effectiveDate)
        {
            if (effectiveDate > DateTime.Now)
                throw new ArgumentException("Invalid effective date, must be in the Past");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name Cannot be Empty or Null");
            VATClass vc = new VATClass(Guid.NewGuid())
            {
                Name = name,
                VatClass = vatClass,
                VATClassItems = new List<VATClass.VATClassItem>() 
            };
            vc.VATClassItems.Add(new VATClass.VATClassItem(Guid.NewGuid()) { EffectiveDate = effectiveDate, Rate=rate });

            return vc;
        }
    }
}
