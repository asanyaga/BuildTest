using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.ActivityDocumentCommands
{
    public class AddActivityServiceLineItemCommand : AfterCreateCommand
    {
        public AddActivityServiceLineItemCommand()
        {

        }

        public Guid LineItemId { get; set; }
        public Guid ServiceId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public Guid ShiftId { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.AddActivityServiceItem.ToString(); }
        }
    }
}