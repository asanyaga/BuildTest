#if __MOBILE__
using SQLite.Net.Attributes;
#endif
namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Retire
{
    #if __MOBILE__
        [Table("RetireSetting")]
    #endif    
    public class RetireSettingDTO : MasterBaseDTO
    {
        public int RetireTypeId { get; set; }
        public int Duration { get; set; }
    }
}
