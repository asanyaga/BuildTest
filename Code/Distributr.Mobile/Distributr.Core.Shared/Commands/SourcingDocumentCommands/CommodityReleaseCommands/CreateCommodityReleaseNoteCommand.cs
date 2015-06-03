using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands
{
    public class CreateCommodityReleaseCommand : CreateCommand
    {
        public CreateCommodityReleaseCommand()
        {
        }

        public override string CommandTypeRef
        {
            get { return CommandType.CreateCommodityReleaseNote.ToString(); }
        }

   

     
        public Guid DocumentRecipientCostCentreId { get; set; }
        public string Note { get; set; }
        
    }
}
