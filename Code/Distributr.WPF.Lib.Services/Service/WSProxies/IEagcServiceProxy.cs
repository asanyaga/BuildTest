using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;

namespace Distributr.WPF.Lib.Services.Service.WSProxies
{
  public interface IEagcServiceProxy
  {
      Task<RequestResult<LoginTokenDto>> Login(string email, string password);
      Task<RequestResult<DepositorDto>> GetCommodityOwnerById(string id);
      Task<RequestResult<VCDto>> GetWarehouseByExtKey(string warehouseId);
      Task<RequestResult<CommodityDto>> GetCommodityByExternalKey(string commodityId);
      Task<IdResponse> CreateNewWr(WarehouseReceiptCreateNewCommand create);
      Task<IdResponse> WarehouseReceiptAddGrn(WarehouseReceiptAddGRNCommand addGrn);
      Task<IdResponse> AddDepositor(ContactCreateDepositorCommand addDepositor);
  }
}
