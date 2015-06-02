using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ChannelPacksDTO : MasterBaseDTO
    {
        public Guid PackagingId { get; set; }
        public Guid OutletTypeId { get; set; }
        public bool IsChecked { get; set; }
    }
}
