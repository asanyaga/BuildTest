using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders
{
    public interface ICommoditySupplierViewModelBuilder
    {
        IList<CommoditySupplierListingViewModel> GetAll(bool inactive = false);
        List<CommoditySupplierListingViewModel> SearchCommoditySuppliers(string srchParam, bool inactive = false);
        CommoditySupplierListingViewModel Get(Guid id);
        CommoditySupplierDTO GetDto(Guid id);
        void Save(CommoditySupplierDTO hubViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<int, string> CommoditySupplierType();
        Dictionary<Guid, string> ParentCostCentre();
        Dictionary<Guid, string> GetBanks();
        List<BankBranch> GetBankBranches(Guid bankId);
        void SetUp(CommoditySupplierViewModel vm);
        SelectList GetUnAssignedCentres(CommoditySupplierViewModel vm);
        void SetUpNew(CommoditySupplierViewModel model);

        QueryResult<CommoditySupplierListingViewModel> Query(QueryCommoditySupplier q);

        CommoditySupplierListingViewModel Edit(Guid id);

    }
}
