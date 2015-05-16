using System;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders
{
    public interface ICommodityTypeViewModelBuilder
    {
        IList<CommodityTypeViewModel> GetAll(bool inactive = false);
        List<CommodityTypeViewModel> SearchCommodityTypes(string srchParam, bool inactive = false);
        CommodityTypeViewModel Get(Guid id);
        void Save(CommodityTypeViewModel commodityTypeViewmodel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<Guid, string> Region();

        QueryResult<CommodityTypeViewModel> Query(QueryStandard query);
       /* IList<CommodityTypeViewModel> QueryList(QueryResult result);*/
    }
}
