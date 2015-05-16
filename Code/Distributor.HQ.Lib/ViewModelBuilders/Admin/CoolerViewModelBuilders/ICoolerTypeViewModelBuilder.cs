using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders
{
   public interface ICoolerTypeViewModelBuilder
    {
        IList<CoolerTypeViewModel> GetAll(bool inactive = false);
        List<CoolerTypeViewModel> Search(string srchParam, bool inactive = false);
        CoolerTypeViewModel Get(Guid Id);
        void Save(CoolerTypeViewModel coolerTypeViewModel);
        void SetInactive(Guid id);
    }
}
