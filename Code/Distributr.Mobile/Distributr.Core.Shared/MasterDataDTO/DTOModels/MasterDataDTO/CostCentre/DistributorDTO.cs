using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
   public  class DistributorDTO : StandardWarehouseDTO
    {
        public Guid ProducerId { get; set; }

        //distributor
        public string Owner { get; set; }
        public string PIN { get; set; }
        public string AccountNo { get; set; }
        public Guid RegionMasterId { get; set; }
        public Guid? ASMUserMasterId { get; set; }
        public Guid? SalesRepUserMasterId { get; set; }
        public Guid? SurveyorUserMasterId { get; set; }
        public Guid ProductPricingTierMasterId { get; set; }
        public string PaybillNumber { get; set; }
        public string MerchantNumber { get; set; }
        [IgnoreInCsv]
        public UserDTO ASMDTO { get; set; }
        [IgnoreInCsv]
        public UserDTO SalesRepDTO { get; set; }
        [IgnoreInCsv]
        public UserDTO SurveyorDTO { get; set; }
    }
}
