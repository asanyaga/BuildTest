using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.HQ.Lib.DTO;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Distributr.HQ.Lib.Service
{
    public interface ICommodityTransferService
    {
        IList<StoreDTO> GetStores();
        IList<CommodityDTO> GetCommodities(Guid storeID);
        IList<GradeDTO> GetGrades(Guid commodityId, Guid storeId);
        IList<LineItemDTO> GetLineItems(Guid storeId, Guid commodityId, Guid gradeID);
        void Transfer(List<TransferLineItemDTO> lineItemList, UserViewModel userViewModel);
    }
}
