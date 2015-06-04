using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class RouteDTO : MasterBaseDTO
    {
		public RouteDTO() {
		}

        public string Name { get; set; }
        
        public string Code { get; set; }

        public Guid RegionId { get; set; }
    }
}
