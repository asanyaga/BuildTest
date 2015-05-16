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
    public interface IHubViewModelBuilder
    {
        IList<HubViewModel> GetAll(bool inactive = false);
        List<HubViewModel> SearchHubs(string srchParam, bool inactive = false);
        HubViewModel Get(Guid id);
        void Save(HubViewModel hubViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<Guid, string> Region();
        Dictionary<Guid, string> Contact();
        Dictionary<Guid, string> ParentCostCentre();


        QueryResult<HubViewModel> Query(QueryStandard query);
   /*     IList<HubViewModel> Querylist(QueryResult result );*/
    }
}
