using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
    interface ISocioEconomicStatuViewModelBuilder
    {
        IList<SocioEconomicStatusViewModel> GetAll(bool inactive = false);
        SocioEconomicStatusViewModel GetByID(int id);
        void Save(SocioEconomicStatusViewModel socioEconomicStatusViewModel);
        void SetInActive(int id);
    }
}
