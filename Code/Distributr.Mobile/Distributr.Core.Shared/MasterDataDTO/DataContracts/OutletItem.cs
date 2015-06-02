using System;
using System.Collections.Generic;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class OutletItem : CostCentreItem
    {
        
        public Guid RouteMasterId { get; set; }
        
        public Guid OutletCategoryMasterId { get; set; }
        
        public Guid OutletTypeMasterId { get; set; }
        
        public Guid OutletProductPricingTierMasterId { get; set; }
        
        public Guid OutletVATClassMasterId { get; set; }
        
        public Guid OutletDiscountGroupMasterId { get; set; }
        
        public string outLetCode { get; set; }
        
        public string Latitude { get; set; }
        
        public string Longitude { get; set; }
        
        public List<ShipToAddress> ShippingAddresses { get; set; }

    }

    public class ShipToAddress : MasterBaseItem
    {
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string PostalAddress { get; set; }
        
        public string PhysicalAddress { get; set; }
        
        public decimal? Longitude { get; set; }
        
        public decimal? Latitude { get; set; }
        
        public int EntityStatus { get; set; }

    }
}
