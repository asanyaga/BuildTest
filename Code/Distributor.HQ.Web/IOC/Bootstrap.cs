using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Distributr.BusSubscriber.Service;
using Distributr.Core.Data.IOC;
using Distributr.HQ.Lib.IOC;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Server.IOC;
using EasyNetQ;
using Microsoft.WindowsAzure;
using StructureMap;

namespace Distributr.HQ.Web.IOC
{
    public class Bootstrap
    {
        public static void Resolver()
        {
            LegacyBootstrap();
            bool azure = ConfigurationManager.AppSettings.AllKeys.Any(n => n == "DeploymentPlatform") 
                && ConfigurationManager.AppSettings["DeploymentPlatform"].ToLower() == "azure" ;
            
                
        }

        static void LegacyBootstrap()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<DataRegistry>();
                x.AddRegistry<ViewModelBuilderRegistry>();
                x.AddRegistry<WSAPIRegistry>();
                x.AddRegistry<BusRegistry>();
                x.AddRegistry<MongoRegistry>();
                x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);
            });
            IContainer container = (IContainer)ObjectFactory.Container;
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
        }
     
    }
}