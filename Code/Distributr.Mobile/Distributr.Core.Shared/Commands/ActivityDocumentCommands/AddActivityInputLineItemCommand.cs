using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.ActivityDocumentCommands
{
    public class AddActivityInputLineItemCommand : AfterCreateCommand
    {
        public AddActivityInputLineItemCommand()
        {
            
        }

        public Guid LineItemId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string  SerialNo { get; set; }
        public DateTime? ManufacturedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
      
        public override string CommandTypeRef
        {
            get { return CommandType.AddActivityInputItem.ToString(); }
        }
    }
}
