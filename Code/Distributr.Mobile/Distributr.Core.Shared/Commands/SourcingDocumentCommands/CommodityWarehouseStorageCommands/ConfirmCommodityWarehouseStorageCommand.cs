using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands
{
    public class ConfirmCommodityWarehouseStorageCommand : ConfirmCommand
    {
        public ConfirmCommodityWarehouseStorageCommand()
        {
        }

        
        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmCommodityWarehouseStorage.ToString(); }
        }

    }
}