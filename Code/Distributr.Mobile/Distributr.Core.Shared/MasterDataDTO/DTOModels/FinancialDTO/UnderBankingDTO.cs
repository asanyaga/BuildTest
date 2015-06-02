using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.FinancialDTO
{
#if __MOBILE__
    [Table("UnderBanking")]
#endif
  public class UnderBankingDTO : MasterBaseDTO
    {
        public Guid CostCentreId { get; set; }
        public string CostCentreName { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalReceivedAmount { get; set; }
        public string Description { get; set; }
    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [IgnoreInCsv]
        public UnderBankingItemDTO[] Items { get; set; }

    }

    public class UnderBankingItemDTO 
    {
        public decimal Amount { get; set; }
        public Guid Id { get; set; }
        public Guid UnderBankingId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
