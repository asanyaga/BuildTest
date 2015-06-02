using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.MasterDataDTO
{
    public class MasterDataEnvelope
    {        
        public string masterDataName { get; set; }
        public List<MasterEntity> masterData { get; set; }

        public MasterDataEnvelope()
        {
            masterData = new List<MasterEntity>();
        }
    }

    public class TestCostCentreEnvelope {
        public Guid costCentreId { get; set; }
        public string costCentreType { get; set; }
    }
}
