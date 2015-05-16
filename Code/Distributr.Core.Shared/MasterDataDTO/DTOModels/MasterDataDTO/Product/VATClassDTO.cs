using System;
using System.Collections.Generic;
using Distributr.Core.Utility;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class VATClassDTO : MasterBaseDTO
    {
        public string Name { get; set; }

        public string VatClass { get; set; }
          [IgnoreInCsv]
        public List<VatClassItemDTO> VatClassItems { get; set; }
    }

    public class VatClassItemDTO : MasterBaseDTO
    {
        public Guid VatClassMasterId { get; set; }

        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
