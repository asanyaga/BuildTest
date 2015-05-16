using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands
{
    public class StoreCommodityWarehouseStorageCommand : CloseCommand
    {
        public StoreCommodityWarehouseStorageCommand()
        {
        }

        
        public override string CommandTypeRef
        {
            get { return CommandType.StoreCommodityWarehouseStorage.ToString(); }
        }

        public Guid StoreId { get; set; }

    }
}