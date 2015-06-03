using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO
{
    public class InventoryDTO : MasterBaseDTO
    {
        
        public Guid WarehouseMasterID { get; set; }

        public Guid ProductMasterID { get; set; }

        public decimal Balance { get; set; }

        public decimal UnavailableBalance { get; set; }

        public decimal Value { get; set; }

    }
}
