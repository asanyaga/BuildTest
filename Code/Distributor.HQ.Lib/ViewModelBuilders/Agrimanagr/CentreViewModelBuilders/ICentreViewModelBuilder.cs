using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders
{
    public interface ICentreViewModelBuilder
    {
        CentreViewModel Setup(CentreViewModel vm);
        IList<CentreViewModel> GetAll(bool inactive = false);
        List<CentreViewModel> SearchCentres(string srchParam, bool inactive = false);
        CentreViewModel Get(Guid id);
        void Save(CentreViewModel centreviewmodel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        SelectList GetRouteList(Guid hubId = new Guid());

       // IList<CentreViewModel> Query(QueryStandard query);
        QueryResult<CentreViewModel> Query(QueryStandard query);
    }
}
