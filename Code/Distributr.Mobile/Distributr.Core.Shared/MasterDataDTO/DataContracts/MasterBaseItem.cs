using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public abstract class MasterBaseItem
    {
        public Guid MasterId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public int StatusId { get; set; }
    }
}
