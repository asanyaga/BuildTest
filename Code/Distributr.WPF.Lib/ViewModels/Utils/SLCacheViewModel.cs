using System.Collections.Generic;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class SLCacheViewModel : DistributrViewModelBase
    {
        public SLCacheViewModel()
        {
 
        }

        public List<OrderDocument> OrdersCache { get; set; }
    }
}
