using System;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.Workflow.Impl;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WPF.Lib.Impl.Services.Transactional.Commands.MasterData
{
    public class OutgoingMasterDataRouter : IOutgoingMasterDataRouter
    {
        public OutgoingMasterDataRouter(IOutGoingMasterDataQueueItemRepository outGoingRepository)
        {
            _outGoingRepository = outGoingRepository;
        }

        private IOutGoingMasterDataQueueItemRepository _outGoingRepository;

        public void RouteMasterData(MasterBaseDTO dTO, MasterDataDTOSaveCollective type)
        {

            
            var queueItem = new OutGoingMasterDataQueueItemLocal
                                {
                                    MasterId = dTO.MasterId,
                                    Type = type,
                                    IsSent = false,
                                    DateSent = DateTime.Now,
                                    JsonDTO = JsonConvert.SerializeObject(dTO, new IsoDateTimeConverter())
                                };
            _outGoingRepository.Add(queueItem);

        }
    }
}
