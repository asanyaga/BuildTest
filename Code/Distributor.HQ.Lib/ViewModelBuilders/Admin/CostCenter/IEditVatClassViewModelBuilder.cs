using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IEditVatClassViewModelBuilder
    {
       EditVatClassViewModel Get(Guid Id);
        void Save(EditVatClassViewModel vatClass);
        void AddVatClassItem(Guid vatClassId, decimal rate, DateTime effectiveDate);
    }
}
