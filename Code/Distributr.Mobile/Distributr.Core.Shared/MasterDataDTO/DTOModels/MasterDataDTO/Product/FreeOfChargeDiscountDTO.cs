using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
  public  class FreeOfChargeDiscountDTO:MasterBaseDTO
    {
      public Guid ProductRefMasterId { get; set; }
      public bool isChecked { get; set; }
      public DateTime? StartDate { get; set; }
      public DateTime? EndDate { get; set; }
    }
}
