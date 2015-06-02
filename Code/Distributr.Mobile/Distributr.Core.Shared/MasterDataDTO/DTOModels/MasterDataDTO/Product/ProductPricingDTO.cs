using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    
    public class ProductPricingDTO : MasterBaseDTO
    {
        public Guid ProductMasterId { get; set; }
        public Guid ProductPricingTierMasterId { get; set; }
          [IgnoreInCsv]
		public List<ProductPricingItemDTO> ProductPricingItems { get; set; }

        //Items
        public decimal ExFactoryRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal SellingPrice { get; set; }
        public Guid LineItemId { get; set; }
    }

    public class ProductPricingItemDTO : MasterBaseDTO
    {
        public Guid ProductPricingMasterId { get; set; }
        public decimal ExFactoryRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
