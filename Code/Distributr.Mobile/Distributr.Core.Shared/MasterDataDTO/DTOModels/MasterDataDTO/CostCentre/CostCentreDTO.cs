using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public abstract class CostCentreDTO : MasterBaseDTO
    {
        //cost centre
        public string Name { get; set; }
        public string CostCentreCode { get; set; }
        public Guid ParentCostCentreId { get; set; }
        public int CostCentreTypeId { get; set; }
      
    }
}
