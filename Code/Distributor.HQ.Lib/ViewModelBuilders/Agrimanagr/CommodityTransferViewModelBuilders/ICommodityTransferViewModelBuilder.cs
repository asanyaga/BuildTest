using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityTransferViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityTransferViewModelBuilders
{
    public interface ICommodityTransferViewModelBuilder
    {
        IList<CommodityTransferViewModel> GetAll();
        void Approve(Guid noteId, Guid storeId);
    }
}
