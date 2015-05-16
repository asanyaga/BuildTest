using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Distributr.BusSubscriber;
using Distributr.Core.Commands.CommandPackage;
using Distributr.WSAPI.Lib.Services.Bus;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using EasyNetQ;
using EasyNetQ.Topology;
using StructureMap;
using log4net;
using System.Threading.Tasks;

namespace Distributr.BusSubscriber.Service
{
    public class DistributrSubriberService : IDistributrSubscriberService
    {
        private static readonly ILog _log = LogManager.GetLogger("Subscription Logger");

        private IAdvancedBus _bus;
        INotificationService _notificationService ;
        private IInventoryImportService _inventoryImportService;
        //private IHandleMessage _handleMessage;
        public DistributrSubriberService(IAdvancedBus advancedBus, INotificationService notificationService, IInventoryImportService inventoryImportService)
        {
            _log.Info("DistributrSubriberService constructor called");
            // _handleMessage = handleMessage;
            _bus = advancedBus;
            _notificationService = notificationService;
            _inventoryImportService = inventoryImportService;
            _log.Info("Setup complete");
        }

        public void Start()
        {
            var destinationQ = "Q" + ConfigurationManager.AppSettings["MQName"];
            _log.InfoFormat("Starting Bus Subscriber for queue {0}", destinationQ);
            var queue1 = Queue.DeclareDurable(destinationQ);
           // _bus.;
            ;
            _bus.Subscribe<EnvelopeBusMessage>(queue1,
                (message, messageReceivedInfo) =>
                {
                    var task = GetMessageHandlerTask(message.Body);
                    task.RunSynchronously();
                    return task;
                }

            );
            _inventoryImportService.Start();
            _notificationService.Start();

        }

        Task GetMessageHandlerTask(EnvelopeBusMessage message)
        {
            return new Task(() =>
            {
                using (IContainer nested = ObjectFactory.Container.GetNestedContainer())
                {
                    ICommandEnvelopeProcessingAuditRepository _processingAudit = nested.GetInstance<ICommandEnvelopeProcessingAuditRepository>();
                    try
                    {
                        var audit = _processingAudit.GetById(message.MessageId);
                        if (audit != null)
                        {
                            _processingAudit.SetStatus(message.MessageId,EnvelopeProcessingStatus.SubscriberProcessBegin, audit.LastExecutedCommand);
                            Console.WriteLine(message.MessageId.ToString());
                            _log.InfoFormat("Received messageid {0} from queue {1} ", message.MessageId.ToString(),
                                "Q" + ConfigurationManager.AppSettings["MQName"]);

                            IHandleMessage _handleMessage =
                                nested.GetInstance<IHandleMessage>();
                            //
                            _handleMessage.Handle(message);
                        }

                    }
                    catch (Exception ex)
                    {

                        _processingAudit.SetStatus(message.MessageId, EnvelopeProcessingStatus.MarkedForRetry,0);
                        _log.Error("Command Lost " + message.MessageId.ToString(), ex);

                    }
                }
            });
        }

        public void Stop()
        {
            _notificationService.Stop();
            //throw new NotImplementedException();
        }
    }
}
