using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class DistributorSalesmanDTO : StandardWarehouseDTO
    {
        public Guid RouteMasterId { get; set; }
        public int TypeId { get; set; }

    }
}
