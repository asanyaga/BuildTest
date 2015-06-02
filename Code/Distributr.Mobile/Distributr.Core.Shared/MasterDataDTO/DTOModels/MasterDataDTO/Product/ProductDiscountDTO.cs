using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ProductDiscountDTO : MasterBaseDTO
    {
        public ProductDiscountDTO()
        {
            DeletedProductDiscountItem= new List<Guid>();
        }
        public Guid ProductMasterId { get; set; }
        public Guid TierMasterId { get; set; }
          [IgnoreInCsv]
		public List<ProductDiscountItemDTO> DiscountItem { get; set; }
          [IgnoreInCsv]
        public List<Guid> DeletedProductDiscountItem { get; set; }
    }
    public class ProductDiscountItemDTO : MasterBaseDTO
    {
        public decimal DiscountRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsByQuantity { get; set; }
        public decimal Quantity { get; set; }
        public Guid ProductDiscountMasterId { get; set; }
    }
}
