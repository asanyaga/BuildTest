using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.OutletVisitReasonsTypeViewModels;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.OutletVisitReasonsTypeViewModelBuilders
{
    public interface IOutletVisitReasonsTypeViewModelBuilder
    {
        IList<OutletVisitReasonsTypeViewModel> GetAll(bool inactive = false);
        OutletVisitReasonsTypeViewModel Get(Guid id);
        void Save(OutletVisitReasonsTypeViewModel outletVisitReasonsType);
        void SetInActive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        IList<OutletVisitReasonsTypeViewModel> Search(string searchParam, bool inactive = false);

        QueryResult<OutletVisitReasonsTypeViewModel> Query(QueryStandard query);

        Dictionary<int, string> OutletVisitReasonsTypeAction();
    }
}
