using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders
{
    public interface ICommodityProducerViewModelBuilder
    {

        void SetUp(CommodityProducerViewModel vm);
        IList<CommodityProducerViewModel> GetAll(bool inactive = false);
        List<CommodityProducerViewModel> SearchCommodityProducers(string srchParam, bool inactive = false);
        CommodityProducerViewModel Get(Guid id);
        void Save(CommodityProducerViewModel assetviewmodel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        CommodityProducerViewModel AssignCentre(CommodityProducerViewModel vm);
        void UnAssignCentre(Guid centreId, Guid farmId);


        QueryResult<CommodityProducerViewModel> Query(QueryCommodityProducer q);
    }
}
