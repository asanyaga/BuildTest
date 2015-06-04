using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
  public  class SaleValueDiscountDTO:MasterBaseDTO
    {
      public Guid TierMasterId { get; set; }
        [IgnoreInCsv]
      public List<SaleValueDiscountItemDTO> DiscountItems { get;  set; }
    }
  public class SaleValueDiscountItemDTO : MasterBaseDTO
  {
      public decimal DiscountThreshold { get; set; }
      public decimal DiscountValue { get; set; }
      public DateTime EffectiveDate { get; set; }
      public DateTime EndDate { get; set; }
      public Guid SaleValueDiscountMasterId { get; set; }
    
  }
}
