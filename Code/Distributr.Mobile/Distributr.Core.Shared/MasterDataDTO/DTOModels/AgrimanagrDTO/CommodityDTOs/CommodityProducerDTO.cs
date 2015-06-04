using System;
using System.Collections.Generic;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs
{
    public class CommodityProducerDTO : MasterBaseDTO
    {
        public  CommodityProducerDTO()
        {
            CenterIds= new List<Guid>();
        }
        public string Code { get; set; }
        public string Acrage { get; set; }
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string PhysicalAddress { get; set; }
        public string Description { get; set; }
        public Guid CommoditySupplierId { get; set; }
        public List<Guid> CenterIds { get; set; }
    }
}
