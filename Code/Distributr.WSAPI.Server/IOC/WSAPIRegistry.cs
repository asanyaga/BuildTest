using System;
using System.Collections.Generic;
using System.Configuration;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Retire;
using Distributr.Core.Data.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Data.Repository.Transactional.ThirdPartyIntegrationRepository;
using Distributr.Core.Data.Utility.CommandDeserialization;
using Distributr.Core.Data.Utility.Validation;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IIntegrationDocumentRepository;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.Mapping.impl;
using Distributr.Core.Utility.Serialization;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow;
using Distributr.MongoDB.CommandRouting;
using Distributr.MongoDB.Notifications;
using Distributr.MongoDB.Repository.Impl;
using Distributr.WSAPI.Lib.Integrations.InventoryWorkflows;
using Distributr.WSAPI.Lib.Retire;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Distributr.WSAPI.Lib.Services.Resolver;
using Distributr.WSAPI.Lib.Services.Resolver.Impl;
using Distributr.WSAPI.Lib.Services.WebService.MasterDataDTODeserialization;
using Distributr.WSAPI.Lib.System;
using Distributr.WSAPI.Lib.System.Impl;
using Distributr.WSAPI.Lib.System.Utility;
using StructureMap.Configuration.DSL;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.Bus.Impl;
using Distributr.WSAPI.Lib.Services.Routing;
using Distributr.WSAPI.Lib.Services.Routing.Implementation;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WSAPI.Lib.Services.CostCentreApplications.Impl;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization;
using Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization.Impl;
using Distributr.WSAPI.Lib.ResponseBuilders.MasterData;
using Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter;
using Distributr.WSAPI.Lib.Services.Bus.EasyNetQ;

namespace Distributr.WSAPI.Server.IOC
{
    public class WSAPIRegistry : Registry
    {
        public WSAPIRegistry()
        {
            foreach (var item in DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }

        }

        public static List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                {
                     Tuple.Create(typeof (ICommandValidate), typeof (CommandValidate)),
                    Tuple.Create(typeof (IDeserializeJson), typeof (DeserializeJson)),
                    Tuple.Create(typeof (IMasterDataToDTOMapping), typeof (MasterDataToDTOMapping)),
                    Tuple.Create(typeof (ICostCentreApplicationService), typeof (CostCentreApplicationService)),
                    Tuple.Create(typeof (ICommandDeserialize), typeof (CommandDeserialize)),
                    Tuple.Create(typeof (IPullMasterDataResponseBuilder), typeof (PullMasterDataResponseBuilder)),
                    Tuple.Create(typeof (IRunCommandOnRequestInHostedEnvironment), typeof (RunCommandOnRequestInHostedEnvironment)),
                    Tuple.Create(typeof (ICommandRoutingOnRequestResolver), typeof (CommandRoutingOnRequestResolver)),
                   
                    Tuple.Create(typeof (ICommandRouterResponseBuilder), typeof (CommandRouterOnRequestResponseBuilder)),
                    Tuple.Create(typeof (ICommandEnvelopeRouterResponseBuilder), typeof (CommandEnvelopeRouterResponseBuilder)),
                    Tuple.Create(typeof (ISubscriberSystemMessageHander), typeof (SubscriberSystemMessageHandler)),
                    Tuple.Create(typeof (ISubscriberMessageHandler), typeof (SubscriberMessageHandler)),
                    Tuple.Create(typeof (ISubscriberCommandExecutionGuard), typeof (SubscriberCommandExecutionGuard)),
                    Tuple.Create(typeof (IMasterDataDTODeserialize), typeof (MasterDataDTODeserialize)),
                    Tuple.Create(typeof (IMasterDataDTOValidation), typeof (MasterDataDTOValidation)),
                    Tuple.Create(typeof (IMasterDataEntityResolver), typeof (MasterDataEntityResolver)),
                    Tuple.Create(typeof (IHandleMasterData), typeof (HandleMasterData)),
                    Tuple.Create(typeof (IPublishMasterData), typeof (PublishMasterData)),
                    Tuple.Create(typeof (IRetireDocumentCommandHandler), typeof (RetireDocumentCommandHandler)),
                    Tuple.Create(typeof (IQAddRetryCommandHandler), typeof (QAddRetryCommandHandler)),
                    Tuple.Create(typeof (IReRouteDocumentCommandHandler), typeof (ReRouteDocumentCommandHandler)),
                    Tuple.Create(typeof (IDTOToEntityMapping), typeof (DTOToEntityMapping)),
                    Tuple.Create(typeof (IIntegrationDocumentRepository), typeof (IntegrationDocumentRepository)),
                     Tuple.Create(typeof (IOrderExportDocumentRepository), typeof (OrderExportDocumentRepository)),
                    Tuple.Create(typeof (IWsInventoryAdjustmentWorflow), typeof (WsInventoryAdjustmentWorflow)),
                     Tuple.Create(typeof (IInvoiceExportDocumentRepository), typeof (InvoiceExportDocumentRepository)),
                     Tuple.Create(typeof (IReturnInventoryExportDocumentRepository), typeof (ReturnInventoryExportDocumentRepository)),
                    Tuple.Create(typeof (IReceiptExportDocumentRepository), typeof (ReceiptExportDocumentRepository)),
                    
                };

            return serviceList;
        }
    }
}
