using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class ContactDTO : MasterBaseDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string BusinessPhone { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string SpouseName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string HomeTown { get; set; }
        public string HomePhone { get; set; }
        public string WorkExtPhone { get; set; }
        public string ChildrenNames { get; set; }
        public string City { get; set; }
        public Guid ContactTypeMasterId { get; set; }
        public Guid ContactOwnerMasterId { get; set; }
        public int MaritalStatusMasterId { get; set; }
        public int ContactClassificationId { get; set; }
        public int ContactOwnerType { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
