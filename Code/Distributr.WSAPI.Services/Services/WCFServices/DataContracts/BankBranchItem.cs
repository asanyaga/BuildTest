using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Distributr.Core.Domain.Master.BankEntities;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    public class BankBranchItem : MasterBaseItem
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public BankItem Bank { get; set; }
        [DataMember]
       public string Description { get; set; }
    }
}
