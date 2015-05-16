using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.Services.Service.WSProxies.Impl;
using Newtonsoft.Json;
using VCVouchers.API.Dtos;
using WarehouseReceipt.Client;

namespace Distributr.WPF.Lib.Impl.Services.WSProxies
{
    public class BaseService 
    {
        protected IWarehouseReceiptClient WarehouseReceiptClient;
        
        public BaseService()
        {
            WRClient wrClient = new WRClient();
            WarehouseReceiptClient = wrClient.Client;
        }
       
    }
    public class WRClient
{

    public WRClient() 
    {
        string wrAPIEndpoint = ConfigurationManager.AppSettings["API_URI"];
        string wrAdminKey = ConfigurationManager.AppSettings["ADMIN_KEY"];
        Client = WarehouseReceiptClientFactory.Create(wrAPIEndpoint, wrAdminKey);

    }
    public IWarehouseReceiptClient Client { get; set; }

}
}
