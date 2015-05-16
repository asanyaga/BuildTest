using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
   public class PromotionDiscountDTO:MasterBaseDTO
    {
       public Guid ProductMasterId { get; set; }
         [IgnoreInCsv]
       public List<PromotionDiscountItemDTO> PromotionDiscountItems { get; set; }
    }
   public class PromotionDiscountItemDTO : MasterBaseDTO
   {
       public Guid ProductMasterId { get; set; }
       public Guid PromotionDiscountMasterId { get; set; }
       public int FreeQuantity { get; set; }
       public int ParentQuantity { get; set; }
       public decimal DiscountRate { get; set; }
       public DateTime EffectiveDate { get; set; }
       public DateTime EndDate { get; set; }
   }
}
