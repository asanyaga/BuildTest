using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public abstract class CostCentreItem : MasterBaseItem
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Guid ParentCostCentreId { get; set; }
        [DataMember]
        public int CostCentreTypeId { get; set; }
    }
}
