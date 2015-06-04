using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs
{
    public class CommodityOwnerDTO : MasterBaseDTO
    {
        public string Code { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNo { get; set; }
        public string PinNo { get; set; }
        public int GenderId { get; set; }
        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
        public string FaxNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string Description { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int MaritalStatusId { get; set; }
        public Guid CommodityOwnerTypeId { get; set; }
        public Guid CommoditySupplierId { get; set; }
    }
}
