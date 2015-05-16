using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Agrimanagr.HQ
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "GetStores",
                routeTemplate: "api/release/storelist",
                defaults: new { controller = "ApiCommodityTransfer", action = "GetStores" }
            );
            config.Routes.MapHttpRoute(
                name: "GetCommodities",
                routeTemplate: "api/release/commoditylist",
                defaults: new { controller = "ApiCommodityTransfer", action = "GetCommodities" }
            );
            config.Routes.MapHttpRoute(
                name: "GetGrades",
                routeTemplate: "api/release/gradelist",
                defaults: new { controller = "ApiCommodityTransfer", action = "GetGrades" }
            );
            config.Routes.MapHttpRoute(
                name: "GetLineItems",
                routeTemplate: "api/release/lineItemList",
                defaults: new { controller = "ApiCommodityTransfer", action = "GetLineItems" }
            );
            config.Routes.MapHttpRoute(
                name: "Transfer",
                routeTemplate: "api/release/transfer",
                defaults: new { controller = "ApiCommodityTransfer", action = "TransferCommodity" }
            );
        }
    }
}
