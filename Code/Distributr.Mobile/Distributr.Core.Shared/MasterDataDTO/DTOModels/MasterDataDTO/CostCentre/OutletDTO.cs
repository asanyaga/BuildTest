using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
   
    public class OutletDTO : CostCentreDTO
    {
        public OutletDTO()
        {
            ShippingAddresses=new List<ShipToAddressDTO>();
        }

        public Guid RouteMasterId { get; set; }
		public Guid OutletCategoryMasterId { get; set; }
        public Guid OutletTypeMasterId { get; set; }
        public Guid DiscountGroupMasterId { get; set; }
        public Guid OutletProductPricingTierMasterId { get; set; }
        public Guid VatClassMasterId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsApproved { get; set; }
        public Guid SpecialPricingTierMasterId { get; set; }
         [IgnoreInCsv]
        public List<ShipToAddressDTO> ShippingAddresses { get; set; }
    }

    public class ShipToAddressDTO : MasterBaseDTO
    {
        public Guid OutletId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PostalAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}
