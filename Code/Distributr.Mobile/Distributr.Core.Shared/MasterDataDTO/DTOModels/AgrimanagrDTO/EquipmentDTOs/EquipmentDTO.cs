using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.EquipmentDTOs
{
    public class EquipmentDTO : MasterBaseDTO
    {
        public string Code { get; set; }
        public string EquipmentNumber { get; set; }
        public string Name { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int EquipmentTypeId { get; set; }
        public string Description { get; set; }
        public Guid HubId { get; set; }
    }
}
