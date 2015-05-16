using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Utility;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Impl.Repository.Transactional.AuditLog;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using Distributr.WPF.Lib.Impl.Services.Transactional.Commands.IncomingCommandHandlers;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Newtonsoft.Json;

namespace Distributr.WPF.Lib.Impl.Services.Sync
{
    public class ReceiveAndProcessPendingRemoteCommandEnvelopesService :IReceiveAndProcessPendingRemoteCommandEnvelopesService
    {
        private IIncomingCommandHandler _incomingCommandHandler;
       
        private IConfigRepository _configRepository;
        private IWebApiProxy _webApiProxy;

        public ReceiveAndProcessPendingRemoteCommandEnvelopesService(IIncomingCommandHandler incomingCommandHandler, IConfigRepository configRepository, IWebApiProxy webApiProxy)
        {
            _incomingCommandHandler = incomingCommandHandler;
            _configRepository = configRepository;
            _webApiProxy = webApiProxy;
        }

       

        public async Task<bool> ReceiveAndProcessNextEnvelopesAsync(Guid costCentreApplicationId)
        {
            List<Guid> batchIds = _configRepository.GetDeliveredCommandEnvelopeIds();
            if (batchIds == null)
            {
                batchIds = new List<Guid>();
                batchIds.Add(Guid.Empty);
            }
            EnvelopeRoutingRequest request= new EnvelopeRoutingRequest();
            request.BatchSize = 10;
            request.CostCentreApplicationId = costCentreApplicationId;
            request.DeliveredEnvelopeIds = batchIds;

            BatchDocumentCommandEnvelopeRoutingResponse response = await _webApiProxy.GetNextCommandEnvelopesAsync(request); ;

            
            if (response == null)
                return false;
            _configRepository.ClearDeliveredCommandEnvelopeIds();
            if (response.Envelopes.Count == 0)
            {

                return true;
            }
            foreach (CommandEnvelopeWrapper envelopeWrapper in response.Envelopes.OrderBy(s=>s.EnvelopeArrivedAtServerTick))
            {
                try
                {
                   _incomingCommandHandler.HandleCommandEnvelope(envelopeWrapper.Envelope);
                }
                catch (Exception ex)
                {
                    string error = ex.AllMessages();
                    
                    break;
                }
            }
            if (response.Envelopes.Count != 0)
            {
                return await ReceiveAndProcessNextEnvelopesAsync(costCentreApplicationId);
            }
            return true;
        }
    }
}