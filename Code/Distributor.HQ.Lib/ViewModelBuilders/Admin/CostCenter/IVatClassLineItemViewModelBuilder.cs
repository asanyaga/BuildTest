using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IVatClassLineItemViewModelBuilder
    {
       IList<VatClassLineItemViewModel> GetByVatClass(Guid vatClassId, bool inactive = false);
       VatClassLineItemViewModel Get(Guid id);
       //void Save(VatClassLineItemViewModel vatLineItemViewModel);
       void SetInactive(Guid id);
       void AddVatClassItem(Guid vatClassId, decimal rate, DateTime effectiveDate);
    }
}
