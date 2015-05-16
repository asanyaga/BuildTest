using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
    public interface ISocioEconomicStatusViewModelBuilder
    {
        IList<SocioEconomicStatusViewModel> GetAll(bool inactive = false);
        SocioEconomicStatusViewModel GetByID(Guid id);
        void Save(SocioEconomicStatusViewModel socioEconomicStatusViewModel);
        void SetInActive(Guid id);
    }
}
