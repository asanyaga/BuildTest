using System;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;

namespace Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient
{
    public class EagcServiceProxy : IEagcServiceProxy
    {
        public async Task<RequestResult<DepositorDto>> GetCommodityOwnerById(string id)
        {
            var c = Config.Init();
            IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
            var commodityOwner = await wrc.ContactClient.GetDepositorByExternalKey(id.ToUpper(), EAGCLoginDetails.TokenId);
            //   Trace.WriteLine(string.Format("WR Call DistributrserviceProxy.GetCommodityOwner WREndpoint {0} VAOKey {1} DepExt Key {2} Token {3}   ", c.WREndPoint, c.VAOKey, id, EAGCLoginDetails.TokenId));
            if (commodityOwner != null)
            {
                return commodityOwner;
            }
            return null;
        }


        public async Task<RequestResult<VCDto>> GetWarehouseByExtKey(string warehouseId)
        {
            var c = Config.Init();
            IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
            var warehouse = await wrc.VoucherCentreClient.GetByExternalKey(warehouseId.ToUpper(), EAGCLoginDetails.TokenId);
            if (warehouse != null)
            {
                return warehouse;
            }
            return null;

        }


        public async Task<RequestResult<CommodityDto>> GetCommodityByExternalKey(string commodityId)
        {
            var c = Config.Init();
            IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
            var commodity = await wrc.CommodityClient.GetByExternalKey(commodityId.ToUpper(), EAGCLoginDetails.TokenId);
            if (commodity != null)
            {
                return commodity;
            }
            return null;

        }
       public  async Task<IdResponse> CreateNewWr(WarehouseReceiptCreateNewCommand create)
        {
            var c = Config.Init();
            IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
            var newWr = await wrc.WarehouseReceiptClient.CreateNew(create, EAGCLoginDetails.TokenId,true);
            if (newWr != null)
            {
                EAGCLoginDetails.WrId = newWr.Result.Id;
                return newWr;
            }
            return null;

        }


       public async Task<IdResponse> WarehouseReceiptAddGrn(WarehouseReceiptAddGRNCommand addGrn)
        {
            var c = Config.Init();
            IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
            var newGrn = await wrc.WarehouseReceiptClient.AddGrn(addGrn, EAGCLoginDetails.TokenId,true);
            if (newGrn != null)
            {
                return newGrn;
            }
            return null;
            
        }


       public async Task<IdResponse> AddDepositor(ContactCreateDepositorCommand addDepositor)
       {
           var c = Config.Init();
           IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
           var newDepositor = await wrc.ContactClient.CreateNewDepositor(addDepositor, EAGCLoginDetails.TokenId, true);
           if (newDepositor != null)
           {
               return newDepositor;
           }
           return null;

       }


       public async Task<RequestResult<LoginTokenDto>> Login(string email, string password)
       {
           ResponseBool _response = new ResponseBool { Success = false, ErrorInfo = "" };
           var c = Config.Init();
           IWarehouseReceiptClient wrc = WarehouseReceiptClientFactory.Create(c.WREndPoint, c.VAOKey);
           try
           {
               LoginDto dto = new LoginDto(email, password);
               var logInDetals = await wrc.LoginClient.GetLoginToken(dto);
               if (logInDetals.Result.Token != null)
               {
                   return logInDetals;
               }


           }
           catch (Exception ex)
           {
              _response.ErrorInfo = "Eagc Login Failed \n" + ex.Message;
           }
           return null;
       }

    }


}
