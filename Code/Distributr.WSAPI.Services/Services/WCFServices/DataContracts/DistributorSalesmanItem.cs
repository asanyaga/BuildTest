using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public class DistributorSalesmanItem : CostCentreItem
    {
        [DataMember]
        public Guid RouteMasterId { get; set; }

        [DataMember]
        public string CostCentreCode { get; set; }
    }
    [DataContract]
    public class DistributorSalesmanRouteItem : MasterBaseItem
    {
        [DataMember]
        public Guid RouteMasterId { get; set; }
        [DataMember]
        public Guid CostCentreMasterId { get; set; }
    }
}
