using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO
{
  public class PaymentTrackerDTO : MasterBaseDTO
    {
        public Guid CostcentreId { get; set; }
        public int PaymentModeId { get; set; }
        public decimal Balance { get; set; }
        public decimal PendingConfirmBalance { get; set; }
    }
}
