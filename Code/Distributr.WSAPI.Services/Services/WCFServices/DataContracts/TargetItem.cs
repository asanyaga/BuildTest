using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public class CostCentreTargetItem : MasterBaseItem
    {
        [DataMember]
        public Guid CostCentreMasterId { get; set; }

        [DataMember]
        public Guid TargetPeriodMasterId { get; set; }

        [DataMember]
        public decimal TargetValue { get; set; }

        [DataMember]
        public bool IsQuantityTarget { get; set; }
    }
}
