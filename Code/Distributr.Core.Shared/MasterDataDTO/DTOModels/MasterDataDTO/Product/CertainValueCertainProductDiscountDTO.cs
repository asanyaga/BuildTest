using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
   public class CertainValueCertainProductDiscountDTO:MasterBaseDTO
    {
       public decimal InitialValue { get; set; }
         [IgnoreInCsv]
       public List<CertainValueCertainProductDiscountItemDTO> CertainValueCertainProductDiscountItems { get; set; }
    }
   public class CertainValueCertainProductDiscountItemDTO : MasterBaseDTO
   {
	   
       public Guid CertainValueCertainProductDiscountId { get; set;}
       public decimal CertainValue { get; set; }
       public DateTime EffectiveDate { get; set; }
       public DateTime EndDate { get; set; }
       public int Quantity { get; set; }
       public Guid ProductMasterId { get; set; }
   }
}
