using System;
using System.Collections.Generic;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
    interface ICostCentreViewModelBuilder
    {
        IList<CostCentreViewModel> GetAll(bool inactive = false);
        CostCentreViewModel GetByID(Guid id);
        void Save(CostCentreViewModel costCenter);
        void SetInactive(Guid id);
    }
}
