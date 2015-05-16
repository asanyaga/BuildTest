using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Distributr.Azure.Lib.Bus;
using Distributr.Azure.Lib.IOC;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using StructureMap;
using log4net;
using log4net.Config;

namespace Distributr.Azure.CommandWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueue _incomingCommandQueue;
        private string _queuename = "inboundqueue";
        private volatile bool onStopCalled = false;
        private volatile bool returnedFromRunMethod = false;
        private ILog _logger;
        public override void Run()
        {
            XmlConfigurator.Configure();

            _logger = LogManager.GetLogger(GetType());
            try
            {
               

                _logger.Info("Starting Worker Role");

                //wire up services
                new AzureBootstrap().Init();
                Trace.TraceInformation("Distributr.Azure.CommandWorkerRole entry point called", "Information");
                CloudQueueMessage msg = null;

                IBusSubscriber busSubscriber = ObjectFactory.GetInstance<IBusSubscriber>();
                
                bool validSettings = CheckValidSettingsAtStartup();
                while (true)
                {
                    //TODO add retry limit strategy
                    try
                    {
                        bool messageFound = false;
                        // If OnStop has been called, return to do a graceful shutdown.
                        if (onStopCalled == true)
                        {
                            Trace.TraceInformation("onStopCalled WorkerRoleB");
                            returnedFromRunMethod = true;
                            return;
                        }
                        msg = _incomingCommandQueue.GetMessage();
                        bool processed = false;

                        if (msg != null)
                        {
                            int retrycount = msg.DequeueCount;

                            BusMessage busMessage = msg.FromMessage();
                            _logger.InfoFormat("Message Id : {0} - Type : {1}  Retry count : {2}", busMessage.MessageId, busMessage.CommandType, retrycount);
                            using (IContainer nested = ObjectFactory.Container.GetNestedContainer())
                            {
                               
                                ICommandProcessingAuditRepository _processingAudit = nested.GetInstance<ICommandProcessingAuditRepository>();
                                var validDequeProcess = CheckValidDequeueProcess(msg, _processingAudit, busMessage);
                                if(!validDequeProcess)
                                    processed = true;

                                if (validDequeProcess)
                                {
                                    _processingAudit.SetCommandStatus(busMessage.MessageId, CommandProcessingStatus.SubscriberProcessBegin);
                                    IBusSubscriber subscriber = nested.GetInstance<IBusSubscriber>();
                                    subscriber.Handle(busMessage);
                                    Thread.Sleep(50);
                                    
                                    CommandProcessingAudit auditItem = _processingAudit.GetByCommandId(busMessage.MessageId);
                                    if (auditItem.Status == CommandProcessingStatus.Complete)
                                    {
                                        _logger.Info("Complete processing .. removing from queue");
                                        _incomingCommandQueue.DeleteMessage(msg);
                                        processed = true;
                                    }
                                    else
                                    {
                                        _logger.InfoFormat("Did not process message {0} {1} ..... Will be retried  ",
                                                           busMessage.MessageId, busMessage.CommandType);
                                        //default queue message invisibility is 30 seconds
                                        if (busMessage.CommandType.StartsWith("Add"))
                                        {
                                            _logger.InfoFormat(
                                                "Add message {0} - {1} requeued for 50 seconds-------------------------",
                                                busMessage.MessageId, busMessage.CommandType);
                                            _incomingCommandQueue.UpdateMessage(msg, TimeSpan.FromSeconds(50), MessageUpdateFields.Visibility);
                                        }
                                        if (busMessage.CommandType.StartsWith("Confirm"))
                                        {
                                            _logger.InfoFormat(
                                                "Confirm message {0} - {1} requeued for 100 seconds--------------------------",
                                                busMessage.MessageId, busMessage.CommandType);
                                            _incomingCommandQueue.UpdateMessage(msg, TimeSpan.FromSeconds(100), MessageUpdateFields.Visibility);
                                        }
                                    }
                                }

                            }
                            //introduce a delay for retries over 20 to reduce azure transaction count
                            if (!processed && retrycount > 20)
                            {
                                _logger.Info("s5000");
                                Thread.Sleep(5000);
                            }
                            _logger.Info("<<................................................. Command Processing Complete ......................................................................................>>");
                            _logger.Info(" ");
                        }
                        else
                        {
                            _logger.Info("s10000");
                            Thread.Sleep(10000);
                        }
                        Trace.TraceInformation("Working", "Information");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Run while Error", ex);
                        string err = ex.Message;
                        if (ex.InnerException != null)
                        {
                            err += " Inner Exception: " + ex.InnerException.Message;
                        }
                      
                        Trace.TraceError(err);
                    }


                }
            }
            catch (Exception ex)
            {
                _logger.Error("Global Error",ex);
            }  
            
        }
        /// <summary>
        /// Check can connect to database on role startup
        /// </summary>
        /// <returns></returns>
        private bool CheckValidSettingsAtStartup()
        {
            try
            {
                _logger.Info("Attempting connection to database");
              
                CokeDataContext ctx = ObjectFactory.GetInstance<CokeDataContext>();
                var costCentres = ctx.tblCostCentre.ToList();
                _logger.Info("Successful connection to database");
                
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to connect to database from role", ex);
                throw;
            }
            return true;
        }

        /// <summary>
        /// There is a slight lag on updating the CommandProcessingAudit.Status. 
        /// This is a check to ensure that a command with status Complete does not get executed twice
        /// </summary>
        private bool CheckValidDequeueProcess(CloudQueueMessage msg, ICommandProcessingAuditRepository _processingAudit,BusMessage busMessage)
        {
            bool validDequeueProcess = true;
            if (msg.DequeueCount > 1)
            {
                _logger.InfoFormat("More than one dequeue count, test message status ");
                CommandProcessingAudit auditDeque = _processingAudit.GetByCommandId(busMessage.MessageId);
                if (auditDeque.Status == CommandProcessingStatus.Complete)
                {
                    _logger.Info("Dequeue validation detected CommandProcessingStatus.Complete. Dequeueing");
                    _incomingCommandQueue.DeleteMessage(msg);
                    validDequeueProcess = false;
                }
                else
                {
                    _logger.Info("Valid dequeue process .....");
                }
            }
            return validDequeueProcess;
        }



        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            Trace.TraceInformation("Initialising storage account in worker role");
            string storageConnectionString = RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            Trace.TraceInformation("Creating queue client");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _incomingCommandQueue = queueClient.GetQueueReference(_queuename);
            _incomingCommandQueue.CreateIfNotExists();

           

            return base.OnStart();
        }

        public override void OnStop()
        {
            onStopCalled = true;
            while (returnedFromRunMethod == false)
            {
                System.Threading.Thread.Sleep(1000);
            }
            base.OnStop();
        }
    }
}
