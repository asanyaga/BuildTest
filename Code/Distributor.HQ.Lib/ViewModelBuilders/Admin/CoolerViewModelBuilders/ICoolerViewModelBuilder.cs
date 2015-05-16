using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders
{
   public interface ICoolerViewModelBuilder
    {
        IList<CoolerViewModel> GetAll(bool inactive = false);
        List<CoolerViewModel> Search(string srchParam, bool inactive = false);
        CoolerViewModel Get(Guid Id);
        void Save(CoolerViewModel coolerViewModel);
        void SetInactive(Guid id);
        Dictionary<Guid, string> CoolerType();
    }
}
