using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders
{
    public interface ICommodityOwnerViewModelBuilder
    {
        IList<CommodityOwnerViewModel> GetAll(bool inactive = false);
        List<CommodityOwnerViewModel> SearchCommodityOwners(string srchParam, bool inactive = false);
        CommodityOwnerViewModel Get(Guid id);
        void Save(CommodityOwnerViewModel hubViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<int, string> Gender();
        Dictionary<int, string> MaritalStatus();
        Dictionary<Guid, string> CommodityOwnerType();
        Dictionary<Guid, string> CommoditySupplier();

        CommodityOwnerViewModel GetByQuery(QueryCommodityOwner q);

        QueryResult<CommodityOwnerViewModel> Query(QueryCommodityOwner q);
    }
}
