using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class SalesmanSupplierDTO : MasterBaseDTO
    {


        public Guid SupplierMasterId { get; set; }
        public Guid DistributorSalesmanMasterId { get; set; }
        public bool Assigned { get; set; }
        


    }
}
