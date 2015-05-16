using System;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.AgriUserViewModelBuilders
{
    public interface IAgriUserViewModelBuilder
    {

        IList<AgriUserViewModel> GetAgriUsers(bool inactive = false);
        void Save(AgriUserViewModel agrimanagrUserViewModel);
        void SetInActive(Guid Id);
        void Activate(Guid id);
        void Delete(Guid id);
        Dictionary<int, string> uts();
        Dictionary<Guid, string> CostCentre();
        AgriUserViewModel Get(Guid Id);
        IList<AgriUserViewModel> Search(string srchParam, bool inactive = false);

        QueryResult<AgriUserViewModel> Query(QueryUsers query);
        IList<AgriUserViewModel> QueryList(QueryResult result);

    }
}
