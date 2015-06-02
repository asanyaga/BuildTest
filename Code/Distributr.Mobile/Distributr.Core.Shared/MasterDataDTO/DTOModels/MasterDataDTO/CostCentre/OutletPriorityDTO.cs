using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
   public class OutletPriorityDTO : MasterBaseDTO
    {
        
        public Guid OutletMasterId { get; set; }
        public Guid RouteMasterId { get; set; }
        public int Priority { get; set; }
        public DateTime EffectiveDate { get; set; }

    }
}
