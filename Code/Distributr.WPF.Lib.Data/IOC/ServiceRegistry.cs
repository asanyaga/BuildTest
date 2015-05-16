using System;
using System.Collections.Generic;
using System.Configuration;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.Core.Workflow;
using Distributr.Core.Workflow.Impl.Discount;
using Distributr.WPF.Lib.Impl.Services.PaymentServices;
using Distributr.WPF.Lib.Impl.Services.PaymentServices.Impl;
using Distributr.WPF.Lib.Impl.Services.Sync;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Impl.Services.WSProxies;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Utils.Payment;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;
using StructureMap.Configuration.DSL;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class ServiceRegistry : Registry
    {
        //private string currentApp = "Distributr";
        public ServiceRegistry()
        {
            string currentApp = "Distributr";
            if (ConfigurationSettings.AppSettings["VirtualCityApp"] != null)
                currentApp = ConfigurationSettings.AppSettings["VirtualCityApp"];

            foreach (var item in DefaultServiceList(currentApp))
            {
                For(item.Item1).Use(item.Item2);
            }
        }

        public List<Tuple<Type, Type>> DefaultServiceList(string currentApp)
        {
            var serviceList = new List<Tuple<Type, Type>>();

            if (currentApp == "Distributr")
                serviceList.Add(Tuple.Create(typeof (IConfigService), typeof (DistributrConfigService)));
            else if (currentApp == "Agrimanagr")
                serviceList.Add(Tuple.Create(typeof(IConfigService), typeof(AgrimanagrConfigService)));
            serviceList.Add(Tuple.Create(typeof(ISyncService), typeof(SyncService)));
            serviceList.Add(Tuple.Create(typeof(IDocumentFactory), typeof(DocumentFactory)));
            serviceList.Add(Tuple.Create(typeof(IUpdateLocalDBService), typeof(UpdateLocalDBService)));
            serviceList.Add(Tuple.Create(typeof(IDeserializeJson), typeof(DeserializeJson)));
            serviceList.Add(Tuple.Create(typeof(IDiscountProWorkflow), typeof(DiscountProWorkflow)));
            serviceList.Add(Tuple.Create(typeof(IProductPackagingSummaryService), typeof(ProductPackagingSummaryService)));
            serviceList.Add(Tuple.Create(typeof(IPaymentService), typeof(PaymentService)));
            serviceList.Add(Tuple.Create(typeof(IWebApiProxy), typeof(WebApiProxy)));
            serviceList.Add(Tuple.Create(typeof(IDistributorServiceProxy), typeof(DistributorServiceProxy)));
            serviceList.Add(Tuple.Create(typeof(IEagcServiceProxy), typeof(EagcServiceProxy)));
            serviceList.Add(Tuple.Create(typeof(IDiscountHelper), typeof(DiscountHelper)));
            serviceList.Add(Tuple.Create(typeof(IPaymentUtils), typeof(PaymentUtils)));
            serviceList.Add(Tuple.Create(typeof(IPaymentGatewayProxy), typeof(PaymentGatewayProxy)));
             serviceList.Add(Tuple.Create(typeof(IPaymentGateWayBridge), typeof(PaymentGateWayBridge)));
             
            
            return serviceList;
        }
    }
}
