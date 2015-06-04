using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User
{
    public class UserGroupRoleDTO : MasterBaseDTO
    {
        public int UserRoleMasterId { set; get; }
        public Guid UserGroupMasterId { set; get; }
        public bool CanAccess { get; set; }
    }
}
