using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.RouteViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.RouteViewBuilder
{
    public interface IAdminRouteViewModelBuilder
    {

        IList<AdminRouteViewModel> GetAll(bool inactive = false);
        List<AdminRouteViewModel> GetByDistributor(Guid distId);
        List<AdminRouteViewModel> GetDefaultRoute();
        AdminRouteViewModel Get(Guid id);
        void Save(AdminRouteViewModel adminRouteViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        Dictionary<Guid, string> Distributor();
        Dictionary<Guid, string> Regions();
        Dictionary<Guid, string> Hubs();
        QueryResult<AdminRouteViewModel> Query(QueryStandard query);

      /*  IList<AdminRouteViewModel> Querylist(QueryResult result);
*/
    }
}
