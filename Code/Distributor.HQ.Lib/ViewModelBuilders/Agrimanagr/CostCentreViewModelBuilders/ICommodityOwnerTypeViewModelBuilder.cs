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
    public interface ICommodityOwnerTypeViewModelBuilder
    {
        IList<CommodityOwnerTypeViewModel> GetAll(bool inactive = false);
        List<CommodityOwnerTypeViewModel> SearchCommodityOwnerTypes(string srchParam, bool inactive = false);
        CommodityOwnerTypeViewModel Get(Guid id);
        void Save(CommodityOwnerTypeViewModel commodityOwnerTypeViewmodel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);

        QueryResult<CommodityOwnerTypeViewModel> Query(QueryStandard query);
        IList<CommodityOwnerTypeViewModel> QueryList(QueryResult result);

    }
}
