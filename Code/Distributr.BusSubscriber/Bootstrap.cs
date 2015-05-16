using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.BusSubscriber;
using Distributr.BusSubscriber.Service.Impl;
using Distributr.Core.Data.Repository.Transactional;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Resources.Util;
using Distributr.WSAPI.Lib.IOC;
using Distributr.WSAPI.Lib.Services;
using Distributr.WSAPI.Server.IOC;
using StructureMap;
using Distributr.Core.Data.IOC;
using EasyNetQ;
using Distributr.BusSubscriber.Service;
using StructureMap.Pipeline;

namespace Distributr.BusSubscriber
{
    public class Bootstrap
    {
        public void Init()
        {
            ObjectFactory.Initialize(x =>
                                         {
                                             x.AddRegistry<DataRegistry>();
                                             x.AddRegistry<WSAPIRegistry>();
                                             x.AddRegistry<BusRegistry>();
                                             x.AddRegistry<MongoRegistry>();
                                             x.For<IAdvancedBus>().Singleton().Use(BusBuilder.CreateMessageBus);
                                             x.For<IDistributrSubscriberService>().Use<DistributrSubriberService>();
                                             x.For<INotificationResolver>().Use<NotificationResolver>();
                                             x.For<INotificationService>().Use<NotificationService>();
                                             x.For<IDocumentHelper>().Use<DocumentHelper>();
                                             x.For<IInventoryImportService>().Use<InventoryImportService>();
                                             x.For<IInventoryTransferNoteFactory>().Use<InventoryTransferNoteFactory>();
                                             x.For<IInventoryAdjustmentNoteFactory>().Use<InventoryAdjustmentNoteFactory>();
                                             x.For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("service"));
                                             x.For<IHandleMessage>()
                                                 //.LifecycleIs(new UniquePerRequestLifecycle())
                                                 .Use<HandleMessage>();

                                         });
            
        }
        
    }
}
