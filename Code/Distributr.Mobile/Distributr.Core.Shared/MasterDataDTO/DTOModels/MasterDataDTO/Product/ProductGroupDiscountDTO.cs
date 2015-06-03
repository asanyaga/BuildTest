using System;
using System.Collections.Generic;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class BatchedProductGroupDiscountDTO :MasterBaseDTO
    {
        public List<ProductGroupDiscountDTO> ProductGroupDiscounts { get;set; }
        public List<ProductGroupDiscountItemDTO> GroupDiscountItems { get; set; }
    }
    public class ProductGroupDiscountDTO:MasterBaseDTO
    {
        public Guid DiscountGroupMasterId { get; set; }
       

        //lineItem values
        public Guid ProductMasterId { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid LineItemId { get; set; }
        public decimal Quantity { get; set; }
        public bool IsByQuantity { get; set; }
        
    }

    public class ProductGroupDiscountItemDTO : MasterBaseDTO
    {
        public Guid ProductMasterId { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid DiscountGroupMasterId { get; set; }
    }

}
