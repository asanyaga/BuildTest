using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
   
    public abstract class CostCentreItem : MasterBaseItem
    {
        
        public string Name { get; set; }
        
        public Guid ParentCostCentreId { get; set; }
        
        public int CostCentreTypeId { get; set; }
    }
}
