using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities
{
    public class ServiceProviderDTO : MasterBaseDTO
    {
        public string Code { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }
        public string AccountName { get; set; }
        public string IdNo { get; set; }
        public string PinNo { get; set; }
        public int GenderId { get; set; }
        public Guid BankId { get; set; }
        public Guid BankBranchId { get; set; }
        public string Description { get; set; }

       
        public string MobileNumber { get; set; }
    }
}
