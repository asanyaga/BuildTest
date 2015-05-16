using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public class RouteItem : MasterBaseItem
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public Guid DistributorId { get; set; }
    }
}
