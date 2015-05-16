using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.EntityCommands.InventorySerials
{
    public class CreateInventorySerialsCommand : DocumentCommand
    {
        public CreateInventorySerialsCommand()
        {
            
        }
        public CreateInventorySerialsCommand(Guid commandId, 
            Guid entityId,
            Guid documentId, 
            Guid commandGeneratedByUserId, 
            Guid commandGeneratedByCostCentreId, 
            int costCentreApplicationCommandSequenceId, 
            Guid commandGeneratedByCostCentreApplicationId, 
            Guid parentDocId,  
            Guid productId,
            Guid recipientCostCentreId,
            string cSVFromToList, 
            DateTime dateInventorySerialsCreated
            ) : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId, 
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, 
            parentDocId
            )
        {
            EntityId = entityId;
            ProductId = productId;
            RecipientCostCentreId = recipientCostCentreId;
            CSVFromToList = cSVFromToList;
            DateInventorySerialsCreated = dateInventorySerialsCreated;
        }

        public Guid EntityId { get; set; }

        public Guid ProductId { get; set; }

        public Guid RecipientCostCentreId { get; set; }

        public string CSVFromToList { get; set; }

        public DateTime DateInventorySerialsCreated { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateInventorySerials.ToString(); }
        }
    }
}
