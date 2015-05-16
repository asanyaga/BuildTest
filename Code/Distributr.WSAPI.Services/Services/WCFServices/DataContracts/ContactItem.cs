using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WSAPI.Lib.Services.WCFServices.DataContracts
{
    [DataContract]
    public class ContactItem : MasterBaseItem
    {
        [DataMember]
        public string Firstname { get; set; }
        [DataMember]
        public string Lastname { get; set; }
        [DataMember]
        public DateTime? DateOfBirth { get; set; }
        [DataMember]
        public string SpouseName { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string JobTitle { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string HomeTown { get; set; }
        [DataMember]
        public string BusinessPhone { get; set; }
        [DataMember]
        public string Fax { get; set; }
        [DataMember]
        public string PhysicalAddress { get; set; }
        [DataMember]
        public string PostalAddress { get; set; }
        [DataMember]
        public string HomePhone { get; set; }
        [DataMember]
        public string WorkExtPhone { get; set; }
        [DataMember]
        public string MobilePhone { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string ChildrenNames { get; set; }
        [DataMember]
        public int ContactClassification { get; set; }
        [DataMember]
        public Guid ContactTypeMasterId { get; set; }
        [DataMember]
        public Guid ContactOwnerMasterId { get; set; }
        [DataMember]
        public ContactOwnerType ContactOwnerType { get; set; }
        [DataMember]
        public Guid MaritalStatusMasterId { get; set; }
        [DataMember]
        public bool IsNew { get; set; }
    }
}
