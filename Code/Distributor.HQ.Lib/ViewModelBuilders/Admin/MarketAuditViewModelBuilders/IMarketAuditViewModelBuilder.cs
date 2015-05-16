using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders
{
    public interface IMarketAuditViewModelBuilder
    {
        IList<MarketAuditViewModel> GetAll(bool inactive = false);
        List<MarketAuditViewModel> Search(string srchParam, bool inactive = false);
        MarketAuditViewModel Get(Guid Id);
        void Save(MarketAuditViewModel marketAuditViewModel);
        void SetInactive(Guid id);

        QueryResult<MarketAuditViewModel> Query(QueryStandard q);
    }
}
