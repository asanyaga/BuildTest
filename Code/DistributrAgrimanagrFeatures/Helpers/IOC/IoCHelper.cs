using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Distributr.Core.ClientApp;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.Repository.Transactional;

using Distributr.Core.Data.Utility.CommandDeserialization;
using Distributr.Core.Data.Utility.Validation;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Command;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.Core.Utility.Serialization;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow.Impl;
using Distributr.DatabaseSetup;
using Distributr.MongoDB.Repository.Impl;
using Distributr.WPF.Lib.Data.ExecuteCommands;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.Data.Repository.Commands;
using Distributr.WSAPI.Lib.Services;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization.Impl;
using Distributr.WSAPI.Lib.System.Utility;
using Distributr.WSAPI.Server.IOC;
using StructureMap;
using Distributr.MongoDB.CommandRouting;
using Distributr.MongoDB.Notifications;
using Distributr.WPF.Lib.Impl.Services.Sync;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.MasterData;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Impl.Services.WSProxies;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.Services.Service.WSProxies.Impl;
using Distributr.WPF.Lib.ViewModels.Utils;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using DistributrAgrimanagrFeatures.Initialise;
using StructureMap.Diagnostics;
using IContainer = System.ComponentModel.IContainer;

namespace DistributrAgrimanagrFeatures.Helpers.IOC
{
    public class IOCHelper
    {
        /// <summary>
        /// Setup a structure map version of the hub from workflow down. To be used for testing
        /// </summary>
        /// <param name="hub_distributrsqlconnectionstring"></param>
        /// <param name="hub_routingsqlconnectionstring"></param>
        /// <param name="apiendpoint"></param>
        public static void InitialiseHubSliceWithStructurmapContainer(string hub_distributrsqlconnectionstring,
            string hub_routingsqlconnectionstring, string apiendpoint)
        {
            TI.trace("Begin structuremap setup for hub");
            ObjectFactory.Initialize(x =>
            {
                //x.AddRegistry<WPFRegistry>();
                //x.AddRegistry<RepositoryRegistry>();
                RepositoryRegistry.ManualStructuremapSetup(x, hub_distributrsqlconnectionstring, hub_routingsqlconnectionstring);
                x.AddRegistry<ServiceRegistry>();
                x.AddRegistry<CommandHandlerRegistry>();
                x.AddRegistry<WorkflowRegistry>();
                //x.AddRegistry<DataRegistry>();
                x.For<IOutgoingMasterDataRouter>().Use<OutgoingMasterDataRouter>();
                x.For<IOutGoingMasterDataQueueItemRepository>().Use<OutGoingMasterDataQueueItemRepository>();
                x.For<IOutgoingDocumentCommandRouter>().Use<OutgoingDocumentCommandRouter>();
                x.For<IResolveCommand>().Use<ResolveCommand>();
                x.For<IExecuteCommandLocally>().Use<ExecuteCommandLocally>();

                x.For<IMessageBoxWrapper>().Use<StubbedMessageBoxWrapper>();

                x.For<ISendPendingLocalCommandsService>().Use<SendPendingRemoteCommandsService>();
                x.For<ISetupApplication>().Use<SetupApplication>();
                x.For<IOtherUtilities>().Use<OtherUtilities>();
                x.For<IUpdateMasterDataService>().Use<UpdateMasterDataService>();
                x.For<IReceiveAndProcessPendingRemoteCommandsService>().Use<ReceiveAndProcessPendingRemoteCommandsService>();
                x.For<ICommandDeserialize>().Use<CommandDeserialize>();
                x.For<ICommandValidate>().Use<CommandValidate>();
                x.For<IWebApiProxy>().Use<WebApiProxy>();
                x.For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("win"));
                x.For<IDistributorServiceProxy>().Use<DistributorServiceProxy>();
                x.For<IReceiveAndProcessPendingRemoteCommandEnvelopesService>().Use<ReceiveAndProcessPendingRemoteCommandEnvelopesService>();
                x.For<ISendPendingEnvelopeCommandsService>().Use<SendPendingEnvelopeCommandsService>();
                x.For<IOutgoingCommandEnvelopeRouter>().Use<OutgoingCommandEnvelopeRouter>();
            });
            DTOToEntityMapping.SetupAutomapperMappings();
            TI.trace("Complete structuremap setup for hub");
        }


        public static Autofac.IContainer ServerContainerAutofac(Type[] typesToAdd )
        {
            DB_TestingHelper cs = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            Autofac.IContainer c = InitializeServerWithAutofacContainer(cs.Server_DistributrExmxConnection, cs.MongoConnectionString, "win", cs.MongoAuditingConnectionString).Build();
            var newBuilder = new ContainerBuilder();
            foreach (var t in typesToAdd)
                newBuilder.RegisterType(t).As(t);

            newBuilder.Update(c);
            return c;
        }


        /// <summary>
        /// Setup an an autofac version of the server with stubbed bus and in memory server. To be used for 
        /// testing 
        /// </summary>
        /// <param name="serverSqlConnectionstring"></param>
        /// <param name="mongoConnectionString"></param>
        /// <param name="currentPlatformType"></param>
        /// <param name="mongoAuditingConnectionString"></param>
        /// <returns></returns>
        public static ContainerBuilder InitializeServerWithAutofacContainer(string serverSqlConnectionstring, string mongoConnectionString, string currentPlatformType, string mongoAuditingConnectionString)
        {
            TI.trace("Begin Server Autofac wirup");
            var builder = new ContainerBuilder();
            builder.RegisterType<CokeDataContext>()
                .AsSelf()
                .WithParameter("connectionString", serverSqlConnectionstring);
            //.SingleInstance();
            builder.RegisterType<StubbedCacheProvider>().As<ICacheProvider>();
            builder.RegisterType<StubbedMessageSourceAccessor>().As<IMessageSourceAccessor>();
            foreach (var item in DataRegistry.DefaultServiceList())
            {
                builder.RegisterType(item.Item2).As(item.Item1);
            }
            foreach (var item in SyncRegistry.DefaultServiceList())
            {
                builder.RegisterType(item.Item2).As(item.Item1);
            }
            foreach (var item in WSAPIRegistry.DefaultServiceList())
            {
                //IRunCommandOnRequestInHostedEnvironment has a direct dependency on 
                //Structuremap
                if (item.Item1 != typeof (IRunCommandOnRequestInHostedEnvironment))
                {
                    builder.RegisterType(item.Item2).As(item.Item1);
                }
                else
                {
                    builder.RegisterType<RunCommandOnRequestInHostedEnvironmentAutofac>()
                        .As<IRunCommandOnRequestInHostedEnvironment>();
                }
            }
            //bus
            builder.RegisterType<StubbedBusPublisher>().As<IBusPublisher>();
            builder.RegisterType<StubbedControllerBusPublisher>().As<IControllerBusPublisher>();
            builder.RegisterType<MainBusSubscriber>().As<IBusSubscriber>();
            //mongo
            builder.RegisterType<CCAuditRepository>().As<ICCAuditRepository>().WithParameter("connectionString", mongoAuditingConnectionString);
            builder.RegisterType<CommandRoutingOnRequestMongoRepository>().As<ICommandRoutingOnRequestRepository>().WithParameter("connectionStringM1", mongoConnectionString);
            builder.RegisterType<CommandProcessingAuditRepository>().As<ICommandProcessingAuditRepository>().WithParameter("connectionString", mongoConnectionString);
            builder.RegisterType<CommandEnvelopeRouteOnRequestCostcentreRepository>().As<ICommandEnvelopeProcessingAuditRepository>().WithParameter("connectionString", mongoConnectionString);
            builder.RegisterType<NotificationProcessingAuditRepository>().As<INotificationProcessingAuditRepository>().WithParameter("connectionStringNot", mongoConnectionString);
            builder.RegisterType<CommandEnvelopeRouteOnRequestCostcentreRepository>().As<ICommandEnvelopeRouteOnRequestCostcentreRepository>().WithParameter("connectionString", mongoConnectionString);
            //x.For<IDocumentHelper>().Use<DocumentHelper>();
            builder.RegisterType<DocumentHelper>().As<IDocumentHelper>();
                //integrations
            foreach (var item in IntegrationsRegistry.DefaultServiceList())
            {
                builder.RegisterType(item.Item2).As(item.Item1);
            }
            foreach (var item in IntegrationServiceRegistry.DefaultServiceList())
            {
                builder.RegisterType(item.Item2).As(item.Item1);
            }
            builder.RegisterType<InsertTestData>().As<IInsertTestData>();
            
            TI.trace("Complete Server Autofac wireup");
            return builder;

        }
    }
    public class StubbedMessageBoxWrapper : IMessageBoxWrapper
    {
        MessageBoxResult _result = MessageBoxResult.OK;
        public void SetResult(MessageBoxResult result)
        {
            _result = result;
        }

        public MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton messageBoxButton)
        {
            return _result;
        }
    }
    public class StubbedCacheProvider : ICacheProvider
    {
        public void Put(string key, object value)
        {
            //throw new NotImplementedException();
        }

        public object Get(string key)
        {
            return null;
        }

        public void Remove(string key)
        {
            //throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
    public class StubbedMessageSourceAccessor : IMessageSourceAccessor
    {
        public string GetText(string code)
        {
            return "StubbedText " + code;
        }

        public string GetText(string code, object[] args)
        {
            return "StubbedText " + code;
        }

        public Dictionary<string, string> GetResource()
        {
            return new Dictionary<string, string>();
        }
    }
}
