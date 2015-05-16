using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User
{
    public class UserDTO : MasterBaseDTO
    {
        public string Username { get; set; }
        public Guid CostCentre { get; set; }
        public string Password { get; set; }
        public int PassChange { get; set; }
        public string PIN { get; set; }
        public int UserTypeId { get; set; }
        public string Mobile { get; set; }
        public Guid GroupMasterId { get; set; }
        public string TillNumber { get; set; }
    }
}
