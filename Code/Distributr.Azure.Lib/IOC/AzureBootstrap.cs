using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Azure.Lib.Audit;
using Distributr.Azure.Lib.Bus;
using Distributr.Azure.Lib.CommandProcessing.Notification;
using Distributr.Azure.Lib.CommandProcessing.Routing;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.Repository.Transactional;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Resources.Util;

using Distributr.Core.Utility.Caching;
using Distributr.MongoDB.CommandRouting;
using Distributr.MongoDB.Notifications;
using Distributr.MongoDB.Repository.Impl;
using Distributr.WSAPI.Lib.Services;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Server.IOC;
using Microsoft.WindowsAzure.ServiceRuntime;
using StructureMap;
using TestAzureTables.Impl;
using log4net;

namespace Distributr.Azure.Lib.IOC
{


    public class AzureBootstrap
    {

        public void Init()
        {
            ILog _logger = LogManager.GetLogger(GetType());

            ObjectFactory.Initialize(x =>
            {
                //x.AddRegistry<DataRegistry>();


                x.AddRegistry<WSAPIRegistry>();
                //x.AddRegistry<BusRegistry>();
                //x.AddRegistry<MongoRegistry>();
                x.For<IBusSubscriber>().Use<MainBusSubscriber>();

                x.For<IBusPublisher>().Use<AzureBusPublisher>();
                x.For<IControllerBusPublisher>().Use<AzureBusPublisher>();
                //x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);
                //x.For<IDistributrSubscriberService>().Use<DistributrSubriberService>();
                //x.For<INotificationResolver>().Use<NotificationResolver>();
                //x.For<INotificationService>().Use<NotificationService>();
                //x.For<IDocumentHelper>().Use<DocumentHelper>();

                //x.For<IHandleMessage>()
                //    //.LifecycleIs(new UniquePerRequestLifecycle())
                //    .Use<HandleMessage>();

                // Data
                string cokeConnectionString = RoleEnvironment.GetConfigurationSettingValue("cokeconnectionstring");
                _logger.InfoFormat("Context Connection string --> {0}", cokeConnectionString);
                x.For<CokeDataContext>()
                 .Use<CokeDataContext>()
                 .Ctor<string>("connectionString")
                 .Is(cokeConnectionString);

                foreach (var item in DataRegistry.DefaultServiceList())
                {
                    x.For(item.Item1).Use(item.Item2);
                }
                x.For<ICacheProvider>().Use(DefaultCacheProvider.GetInstance());
                x.For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("win"));

                //azure tables
                string storageConnectionString = RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
                x.For<ICommandProcessingAuditRepository>()
                 .Use<AzureCommandProcessingAuditRepository>()
                 .Ctor<string>("storageConnectionString")
                 .Is(storageConnectionString);

                x.For<ICCAuditRepository>()
                    .Use<AzureCCAuditRepository>()
                    .Ctor<string>("storageConnectionString")
                 .Is(storageConnectionString);

                x.For<INotificationProcessingAuditRepository>()
                  .Use<AzureNotificationProcessingAuditRepository>()
                  .Ctor<string>("storageConnectionString")
               .Is(storageConnectionString);

                x.For<ICommandRoutingOnRequestRepository>()
                  .Use<AzureCommandRoutingOnRequestRepository>()
                  .Ctor<string>("storageConnectionString")
                    .Is(storageConnectionString)
                    .Ctor<string>("sqlConnectionString")
                    .Is(cokeConnectionString);


                //Mongo
                /* string mongoAuditingConnectionString = RoleEnvironment.GetConfigurationSettingValue("MongoAuditingConnectionString");
                 string mongoRoutingConnectionString = RoleEnvironment.GetConfigurationSettingValue("MongoRoutingConnectionString");
                 x.For<ICCAuditRepository>()
                  .Use<CCAuditRepository>()
                  .Ctor<string>("connectionString")
                  .Is(mongoAuditingConnectionString);

                 x.For<ICommandRoutingOnRequestRepository>()
                  .Use<CommandRoutingOnRequestMongoRepository>()
                  .Ctor<string>("connectionStringM1")
                  .Is(mongoRoutingConnectionString);

                 x.For<ICommandProcessingAuditRepository>()
                     .Use<CommandProcessingAuditRepository>()
                      .Ctor<string>("connectionString")
                                         .Is(mongoRoutingConnectionString);

                 x.For<INotificationProcessingAuditRepository>()
                  .Use<NotificationProcessingAuditRepository>()
                  .Ctor<string>("connectionStringNot")
                  .Is(mongoRoutingConnectionString);*/

                //???
                x.For<IDocumentHelper>().Use<DocumentHelper>();
            });



        }
    }
}
