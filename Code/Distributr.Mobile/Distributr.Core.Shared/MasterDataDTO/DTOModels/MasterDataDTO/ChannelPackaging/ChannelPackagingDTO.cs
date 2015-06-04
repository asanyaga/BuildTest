using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.ChannelPackaging
{
    public class ChannelPackagingDTO : MasterBaseDTO
    {
        public Guid ProductPackagingMasterId { get; set; }
        public Guid OutletTypeMasterId { get; set; }
        public bool IsChecked { get; set; }


    }
}
