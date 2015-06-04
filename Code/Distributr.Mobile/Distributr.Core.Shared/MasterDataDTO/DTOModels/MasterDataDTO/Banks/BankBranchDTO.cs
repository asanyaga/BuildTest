using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Banks
{
    public class BankBranchDTO : MasterBaseDTO
    {
        public Guid BankMasterId { get; set; }
        
        public string Name { get; set; }
        
        public string Code { get; set; }
        
        
        public string Description { get; set; }
    }
}
