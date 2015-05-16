using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels.Admin.Outlets;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Outlets
{
    public interface IOutletViewModelBuilder
    {
        IEnumerable<OutletViewModel> GetAll(bool inactive = false);
        OutletViewModel GetLastCreatedOutlet(bool inactive = false);
        IList<OutletViewModel> GetByDistributor(Guid distId, bool inactive = false);
        //OutletViewModel GetByDistributor(Guid distId, bool inactive = false);
        OutletViewModel Get(Guid Id);
        IList<OutletViewModel> Search(string searchParam, bool inactive = false);
        void Save(OutletViewModel outletviewmodel);
        void SetInActive(Guid Id);
        void Activate(Guid id);
        void Delete(Guid id);
        string CheckOutletUsage(Guid outletId);
        Dictionary<Guid, string> Route();
        Dictionary<Guid, string> Route(Guid regionId);
        Dictionary<Guid, string> OutletCategory();
        Dictionary<Guid, string> OutletType();
        Dictionary<Guid, string> GetDistributor();
        Dictionary<Guid, string> GetDiscountGroup();
        Dictionary<Guid, string> GetPricingTier();
        Dictionary<Guid, string> GetVatClass();
        Dictionary<Guid, string> ASM();
        Dictionary<Guid, string> SalesRep();
        Dictionary<Guid, string> Surveyor();
        Dictionary<Guid, string> OutletUser();
        IPagedList<OutletViewModel> GetOutlet(int currentPageIndex, int defaultPageSize = 10, bool inactive = false, string searchParam = "", Guid? distId = null);
        QueryResult<OutletViewModel> Query(QueryStandard q, Guid? distId = null);
        Guid GetRegionIdForDistributor(Guid distId);
    }
}
