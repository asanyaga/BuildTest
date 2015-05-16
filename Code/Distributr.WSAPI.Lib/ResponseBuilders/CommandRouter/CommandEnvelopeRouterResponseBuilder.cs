using System;
using System.Collections.Generic;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.WSAPI.Lib.Services.CostCentreApplications;
using Distributr.WSAPI.Lib.Services.Routing;
using Newtonsoft.Json;

/* ----  May2015_Notes -----------
 Core part of envelope routing
 */

namespace Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter
{
    public class CommandEnvelopeRouterResponseBuilder : ICommandEnvelopeRouterResponseBuilder
    {
        ICommandEnvelopeRouteOnRequestCostcentreRepository _envelopeRouteOnRequestCostcentreRepository;
        ICostCentreApplicationService _costCentreApplicationService;

        public CommandEnvelopeRouterResponseBuilder(ICommandEnvelopeRouteOnRequestCostcentreRepository envelopeRouteOnRequestCostcentreRepository, ICostCentreApplicationService costCentreApplicationService)
        {
            _envelopeRouteOnRequestCostcentreRepository = envelopeRouteOnRequestCostcentreRepository;
            _costCentreApplicationService = costCentreApplicationService;
        }

     

        public BatchDocumentCommandEnvelopeRoutingResponse GetNextCommandEnvelopes(Guid costCentreApplicationId, Guid costCentreId,
            List<Guid> lastDeliveredEnvelopeIds, int batchSize)
        {
            //if (lastDeliveredEnvelopeIds != null && lastDeliveredEnvelopeIds.Count > 0)
            //    _envelopeRouteOnRequestCostcentreRepository.MarkEnvelopesAsDelivered(lastDeliveredEnvelopeIds, costCentreApplicationId, costCentreId);

            List<CommandEnvelope> envelopes = _envelopeRouteOnRequestCostcentreRepository.GetUnDeliveredEnvelopesByDestinationCostCentreApplicationId
                (costCentreApplicationId, costCentreId, batchSize, true);
            var responce = new BatchDocumentCommandEnvelopeRoutingResponse();
            if (envelopes == null || envelopes.Count == 0)
            {
                responce.Envelopes = new List<CommandEnvelopeWrapper>();
                responce.ErrorInfo = "No Pending Download";
            }
            else
            {
                foreach (var envelope in envelopes)
                {   var wrapper = new CommandEnvelopeWrapper
                    {
                       
                        DocumentType = ((DocumentType)envelope.DocumentTypeId).ToString(),
                        EnvelopeArrivedAtServerTick=envelope.EnvelopeArrivedAtServerTick,
                        Envelope =envelope
                    };

                    responce.Envelopes.Add(wrapper);
                   
                }
             
                responce.ErrorInfo = "Success";
             

            }
            return responce;
        }

        public BatchDocumentCommandEnvelopeRoutingResponse GetNextInventoryCommandEnvelopes(Guid costCentreApplicationId,
            Guid costCentreId, List<Guid> lastDeliveredEnvelopeIds, int batchSize)
        {
            //Mark as received on client
            if (lastDeliveredEnvelopeIds != null && lastDeliveredEnvelopeIds.Count > 0)
                _envelopeRouteOnRequestCostcentreRepository.MarkEnvelopesAsDelivered(lastDeliveredEnvelopeIds, costCentreApplicationId, costCentreId);

            List<CommandEnvelope> envelopes = _envelopeRouteOnRequestCostcentreRepository.GetUnDeliveredInventoryEnvelopesByDestinationCostCentreApplicationId
                (costCentreApplicationId, costCentreId, batchSize, true);
            var responce = new BatchDocumentCommandEnvelopeRoutingResponse();
            if (envelopes == null || envelopes.Count == 0)
            {
                responce.Envelopes = new List<CommandEnvelopeWrapper>();
                responce.ErrorInfo = "No Pending Download";
            }
            else
            {
                foreach (var envelope in envelopes)
                {
                    var wrapper = new CommandEnvelopeWrapper
                    {

                        DocumentType = ((DocumentType)envelope.DocumentTypeId).ToString(),
                        EnvelopeArrivedAtServerTick = envelope.EnvelopeArrivedAtServerTick,
                        Envelope = envelope
                    };

                    responce.Envelopes.Add(wrapper);

                }

                responce.ErrorInfo = "Success";


            }
            return responce;
        }
    }
}