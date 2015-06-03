namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Assets
{
    public class DistributrFileDTO : MasterBaseDTO
    {
        public string FileData { get; set; }
        public int FileTypeMasterId { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
