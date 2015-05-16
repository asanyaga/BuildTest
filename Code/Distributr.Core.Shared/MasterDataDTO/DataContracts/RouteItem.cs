using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class RouteItem : MasterBaseItem
    {
        public string Name { get; set; }
        
        public string Code { get; set; }

        public Guid RegionId { get; set; }
    }
}
