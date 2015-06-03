using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs
{
    public class CommoditySupplierDTO : CostCentreDTO
    {
        public int CommoditySupplierTypeId { get; set; }
        public DateTime JoinDate { get; set; }
        public string AccountNo { get; set; }
        public string PinNo { get; set; }
        public Guid BankId { get; set; }
        public Guid BankBranchId { get; set; }
        public string AccountName { get; set; }
    }
}
