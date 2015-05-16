using Distributr.Core.Resources.Util;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Data.ExecuteCommands;
using Distributr.WPF.Lib.Data.Repository.Commands;
using Distributr.WPF.Lib.Impl.Services.Sync;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.MasterData;
using Distributr.WPF.Lib.Impl.Services.Transactional.SaveAndContinue;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Impl.Services.WSProxies;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.Services.Service.WSProxies.Impl;
using Distributr.WPF.Lib.ViewModels.Transactional;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WSAPI.Lib.Services;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Data.Utility.CommandDeserialization;
using Distributr.Core.Data.Utility.Validation;
using Distributr.Core.Utility.Command;
using Distributr.Core.Utility.Serialization;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow.Impl;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class WPFRegistry : Registry
    {
        public WPFRegistry()
        {
            For<IUpdateMasterDataService>().LifecycleIs(new UniquePerRequestLifecycle()).Use<UpdateMasterDataService>();
            For<IMessageSourceAccessor>().Use(MessageSourceAccessor.GetInstance("win"));
            For<IAutoSyncService>().Singleton().Use<AutoSyncService>();
            foreach (var item in DefaultServiceList())
            {
                For(item.Item1).Use(item.Item2);
            }

        }

        public static List<Tuple<Type, Type>> DefaultServiceList()
        {
            var serviceList = new List<Tuple<Type, Type>>
                                  {
                                      
                                      
            Tuple.Create(typeof (IReceiveAndProcessPendingRemoteCommandsService), typeof ( ReceiveAndProcessPendingRemoteCommandsService)),
            Tuple.Create(typeof (IReceiveAndProcessPendingRemoteCommandEnvelopesService), typeof (ReceiveAndProcessPendingRemoteCommandEnvelopesService)),
            Tuple.Create(typeof (ISendPendingLocalCommandsService), typeof ( SendPendingRemoteCommandsService)),
            Tuple.Create(typeof (ISendPendingEnvelopeCommandsService), typeof (SendPendingEnvelopeCommandsService)),
            Tuple.Create(typeof (IOutgoingDocumentCommandRouter), typeof ( OutgoingDocumentCommandRouter)),
          
            Tuple.Create(typeof (ICommandValidate), typeof ( CommandValidate)),
            Tuple.Create(typeof (ICommandDeserialize), typeof ( CommandDeserialize)),
            Tuple.Create(typeof (IResolveCommand), typeof ( ResolveCommand)),
            Tuple.Create(typeof (IExecuteCommandLocally), typeof ( ExecuteCommandLocally)),
            Tuple.Create(typeof (IOtherUtilities), typeof ( OtherUtilities)),
            Tuple.Create(typeof (IOutgoingMasterDataRouter), typeof ( OutgoingMasterDataRouter)),
            Tuple.Create(typeof (IOutGoingMasterDataQueueItemRepository), typeof ( OutGoingMasterDataQueueItemRepository)),
            Tuple.Create(typeof (IMessageBoxWrapper), typeof (MessageBoxWrapper)),

            Tuple.Create(typeof (ISetupApplication), typeof (SetupApplication)),
            Tuple.Create(typeof (IOrderSaveAndContinueService), typeof (OrderSaveAndContinueService)),

            //TEMP FOR REFACTORING
            Tuple.Create(typeof (ITransactionalViewmodelRefactoring), typeof (TransactionalViewmodelRefactoring)),
            Tuple.Create(typeof (IOutgoingCommandEnvelopeRouter), typeof (OutgoingCommandEnvelopeRouter))
                                  };
            return serviceList;
        }
    }
}