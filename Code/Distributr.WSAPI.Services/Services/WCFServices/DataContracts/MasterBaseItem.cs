using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public abstract class MasterBaseItem
    {
        [DataMember]
        public Guid MasterId { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public DateTime DateLastUpdated { get; set; }
        [DataMember]
        public int StatusId { get; set; }
    }
}
