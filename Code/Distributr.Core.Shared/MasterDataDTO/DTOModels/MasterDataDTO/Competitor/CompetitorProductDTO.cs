using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Competitor
{
    public class CompetitorProductDTO : MasterBaseDTO
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Guid CompetitorMasterId { get; set; }
        public Guid BrandMasterId { get; set; }
        public Guid PackagingMasterId { get; set; }
        public Guid ProductTypeMasterId { get; set; }
        public Guid PackagingTypeMasterId { get; set; }
        public Guid FlavourMasterId { get; set; }
    }
}
