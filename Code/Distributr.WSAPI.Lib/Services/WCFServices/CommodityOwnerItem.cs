using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Distributr.WSAPI.Lib.Services.WCFServices.DataContracts;

namespace Distributr.WSAPI.Lib.Services.WCFServices
{
    public class CommodityOwnerItem : MasterBaseItem
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string IdNo { get; set; }

        [DataMember]
        public string PinNo { get; set; }

        [DataMember]
        public int GenderId { get; set; }

        [DataMember]
        public string PhysicalAddress { get; set; }
        [DataMember]
        public string PostalAddress { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string BusinessNumber { get; set; }
        [DataMember]
        public string FaxNumber { get; set; }
        [DataMember]
        public string OfficeNumber { get; set; }
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }

        [DataMember]
        public int MaritalStatasMasterId { get; set; }

        [DataMember]
        public Guid CommodityOwnerTypeMasterId { get; set; }

        [DataMember]
        public Guid CommoditySupplierMasterId { get; set; }
    }
}
