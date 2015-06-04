using System;
using System.Collections.Generic;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs
{
    public class CommodityDTO : MasterBaseDTO
    {
        public string Name { get; set; }
        public Guid CommodityTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<CommodityGradeDTO> CommodityGrades { get; set; }
        public List<Guid> DeletedCommodityGradesItem { get; set; }
    }

    public class CommodityGradeDTO : MasterBaseDTO
    {
        public Guid CommodityId { get; set; }   
        public string Name { get; set; }
        public int UsageTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
