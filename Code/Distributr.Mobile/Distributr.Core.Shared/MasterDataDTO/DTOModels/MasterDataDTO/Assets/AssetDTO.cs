using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets
{
    public class AssetDTO : MasterBaseDTO
    {
        public Guid AssetTypeMasterId { get; set; }
        public Guid AssetCategoryMasterId { get; set; }
        public Guid AssetStatusMasterId { get; set; }
        public string Code { get; set; }
            
        public string Name { get; set; }
       
        public string Capacity { get; set; }
        
        public string SerialNo { get; set; }
    
        public string AssetNo { get; set; }
    }
}
