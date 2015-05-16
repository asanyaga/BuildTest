using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.InventoryViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.InventoryViewModelBuilders
{
    public interface IInventoryViewModelBuilder
    {
        List<InventoryLevelHQViewModel> GetAll();
    }
}
