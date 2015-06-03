using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs
{
    public class ContainerTypeDTO : MasterBaseDTO
    {
        public string Name { get; set; }
        public string Make { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public decimal LoadCariage { get; set; }
        public decimal TareWeight { get; set; }
        public decimal Lenght { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal BubbleSpace { get; set; }
        public decimal Volume { get; set; }
        public decimal FreezerTemp { get; set; }
        public Guid CommodityGradeId { get; set; }
        public int ContainerUseTypeId { get; set; }
    }
}
