using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Commands.CommandPackage
{
    public class CommandEnvelope
    {
        public CommandEnvelope()
        {
            CommandsList = new List<CommandEnvelopeItem>();
            OtherRecipientCostCentreList= new List<Guid>();
        }
        public Guid Id { set; get; }
        public Guid DocumentId { set; get; }
        public int DocumentTypeId { set; get; }
        public Guid GeneratedByCostCentreId { set; get; }
        public Guid RecipientCostCentreId { set; get; }
        public Guid GeneratedByCostCentreApplicationId { set; get; }
        public Guid ParentDocumentId { set; get; }
        public List<CommandEnvelopeItem> CommandsList { get; set; }
        public List<Guid> OtherRecipientCostCentreList { get; set; }
        public long EnvelopeGeneratedTick { get; set; }
        public long EnvelopeArrivedAtServerTick { get; set; }

        public bool IsSystemEnvelope { get; set; }


    }

    public class CommandEnvelopeItem
    {
        public CommandEnvelopeItem()
        {

        }
        public CommandEnvelopeItem(int order, DocumentCommand command)
        {
            Command = command;
            Order = order;
        }
        public DocumentCommand Command { get; set; }
        public int Order { get; set; }
    }

    public enum EnvelopeProcessingStatus
    {
        OnQueue = 1,
        SubscriberProcessBegin = 2,
        MarkedForRetry = 3,
        Complete = 4,
        Failed = 5,
        ManualProcessing = 6
    }
}
