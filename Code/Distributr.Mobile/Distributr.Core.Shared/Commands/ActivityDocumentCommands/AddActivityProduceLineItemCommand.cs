using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.ActivityDocumentCommands
{
    public class AddActivityProduceLineItemCommand : AfterCreateCommand
    {
        public AddActivityProduceLineItemCommand()
        {

        }

        public Guid LineItemId { get; set; }
        public Guid CommodityId { get; set; }
        public Guid GradeId { get; set; }
        public decimal Weight { get; set; }
        public Guid ServiceProviderId { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.AddActivityProduceItem.ToString(); }
        }
    }
}