using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders
{
   public interface IOutletAuditViewModelBuilder
    {
        IList<OutletAuditViewModel> GetAll(bool inactive = false);
        List<OutletAuditViewModel> Search(string srchParam, bool inactive = false);
        OutletAuditViewModel Get(Guid Id);
        void Save(OutletAuditViewModel outletAuditViewModel);
        void SetInactive(Guid id);
    }
}
