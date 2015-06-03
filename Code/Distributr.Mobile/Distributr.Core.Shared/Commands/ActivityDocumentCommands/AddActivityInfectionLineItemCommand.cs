using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.ActivityDocumentCommands
{
    public class AddActivityInfectionLineItemCommand : AfterCreateCommand
    {
        public AddActivityInfectionLineItemCommand()
        {

        }

        public Guid LineItemId { get; set; }
        public Guid InfectionId { get; set; }
        public decimal Rate { get; set; }
      
        public override string CommandTypeRef
        {
            get { return CommandType.AddActivityInfectionItem.ToString(); }
        }
    }
}