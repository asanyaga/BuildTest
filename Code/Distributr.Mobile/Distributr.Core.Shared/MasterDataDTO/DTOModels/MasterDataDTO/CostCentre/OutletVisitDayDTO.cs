using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class OutletVisitDayDTO : MasterBaseDTO
    {
        public Guid OutletMasterId { get; set; }
        public int Day { get; set; }
        public DateTime EffectiveDate { get; set; }

    }
}
