using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO
{
    public abstract class MasterBaseDTO
    {
        public Guid MasterId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public int StatusId { get; set; }
    }
}
