using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;
using SQLite.Net.Attributes;

namespace Distributr.Mobile.Login
{
    public class User : UserDTO
    {
        public string CostCentreApplicationId { get; set; }

        [Ignore]
        public bool IsNewUser { get; set; }
    }
}