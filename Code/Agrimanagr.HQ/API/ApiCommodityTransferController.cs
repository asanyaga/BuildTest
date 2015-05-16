using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Security;
using Distributr.HQ.Lib.DTO;
using Distributr.HQ.Lib.Service;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Newtonsoft.Json.Linq;

namespace Agrimanagr.HQ.API
{
    public class ApiCommodityTransferController : BaseApiController
    {
        public IList<StoreDTO> GetStores()
        {
            var data = Using<ICommodityTransferService>().GetStores();
            return data;
        }

        public IList<CommodityDTO> GetCommodities()
        {
            var parameters = Request.RequestUri.ParseQueryString();
            string storeID = parameters.GetKey(0);
            var data = Using<ICommodityTransferService>().GetCommodities(new Guid(storeID));
            return data;
        }

        public IList<GradeDTO> GetGrades()
        {
            var parameters = Request.RequestUri.ParseQueryString();
            string commodityID = parameters["commodityID"];
            string storeID = parameters["storeID"];
            var data = Using<ICommodityTransferService>().GetGrades(new Guid(commodityID), new Guid(storeID));
            return data;
        }

        public IList<LineItemDTO> GetLineItems()
        {
            var parameters = Request.RequestUri.ParseQueryString();
            string commodityID = parameters["commodityID"];
            string storeID = parameters["storeID"];
            string gradeID = parameters["gradeID"];
            var data = Using<ICommodityTransferService>().GetLineItems(new Guid(storeID), new Guid(commodityID), new Guid(gradeID));
            return data;
        }

        [System.Web.Http.HttpPost]
        public string TransferCommodity(ArrayList lineItems)
        {
            if (lineItems.Count > 0)
            {
                var userIdentity = User.Identity as CustomIdentity;
                if (userIdentity != null)
                {
                    var userName = userIdentity.Name;
                    var userViewModel = Using<IUserViewModelBuilder>().GetByUserName(userName);
                    List<TransferLineItemDTO> lineItemList = new List<TransferLineItemDTO>();
                    for (int i = 0; i < lineItems.Count; i++)
                    {
                        TransferLineItemDTO lineItemDto = new JavaScriptSerializer().Deserialize<TransferLineItemDTO>(lineItems[i].ToString());
                        lineItemList.Add(lineItemDto);
                    }
                    Using<ICommodityTransferService>().Transfer(lineItemList, userViewModel);
                }
            }
            
            return "Succesfull";
        }

    }
}