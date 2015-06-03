using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands
{
    public class ApproveCommodityWarehouseStorageCommand : ApproveCommand
    {
        public ApproveCommodityWarehouseStorageCommand()
        {
        }

        
        public override string CommandTypeRef
        {
            get { return CommandType.ApproveCommodityWarehouseStorage.ToString(); }
        }

    }
}