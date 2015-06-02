namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User
{
    public class ChangePasswordDTO : MasterBaseDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
