using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO
{
    public class InventorySerialsDTO : MasterBaseDTO
    {
        //public string From { get; set; }
        //public string To { get; set; }
        //public Guid ProductMasterId { get; set; }
        //public Guid CostCentreMasterId { get; set; }
        //public Guid DocumentId { get; set; }

        public string CSVFromToSerials { get; set; }

        public Guid ProductMasterId { get; set; }

        public Guid CostCentreMasterId { get; set; }

        public Guid DocumentId { get; set; }
    }
}
