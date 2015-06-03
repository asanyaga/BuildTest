using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands
{
    public class GenerateReceiptCommodityWarehouseStorageCommand : AfterConfirmCommand
    {
        public GenerateReceiptCommodityWarehouseStorageCommand()
        {
        }


        public override string CommandTypeRef
        {
            get { return CommandType.GenerateReceiptCommodityWarehouseStorage.ToString(); }
        }

    }
}