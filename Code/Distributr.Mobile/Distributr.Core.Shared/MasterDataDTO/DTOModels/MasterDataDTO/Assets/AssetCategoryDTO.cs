using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets
{
    public class AssetCategoryDTO : MasterBaseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid AssetTypeMasterId { get; set; }
    }
}
