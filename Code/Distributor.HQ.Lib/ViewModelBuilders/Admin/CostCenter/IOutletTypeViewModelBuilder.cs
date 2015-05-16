using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
    public interface IOutletTypeViewModelBuilder
    {
        IList<OutletTypeViewModel> GetAll(bool inactive = false);
        IList<OutletTypeViewModel> Search(string searchParam, bool inactive = false);
        OutletTypeViewModel GetByID(Guid id);
        void Save(OutletTypeViewModel outletTypeViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);

        QueryResult<OutletTypeViewModel> Query(QueryStandard q);
    }
}
