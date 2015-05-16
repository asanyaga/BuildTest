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
    public interface IStoreViewModelBuilder
    {
        IList<StoreViewModel> GetAll(bool inactive = false);
        List<StoreViewModel> SearchStores(string srchParam, bool inactive = false);
        StoreViewModel Get(Guid id);
        void Save(StoreViewModel storeViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<Guid, string> Contact();
        Dictionary<Guid, string> ParentCostCentre();
        Dictionary<int, string> CostCentreTypes();

        QueryResult<StoreViewModel> Query(QueryStandard query);
    }
}
