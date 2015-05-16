using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Master.CommodityEntity
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CommodityOwner : MasterEntity
    {
        public CommodityOwner(Guid id) : base(id)
        {
        }

        public CommodityOwner(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }

        public string Code { get; set; }

        public string Surname { get; set; }

        [Required(ErrorMessage = "FirstName is a Required Field!")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "ID Number is a Required Field!")]
        public string IdNo { get; set; }

        [Required(ErrorMessage = "PIN number is a Required Field!")]
        public string PinNo { get; set; }

        [Required(ErrorMessage = "Gender is a Required Field!")]
        public Gender Gender { get; set; }

        public string PhysicalAddress { get; set; }
        public string PostalAddress { get; set; }

        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
       
        [Required(ErrorMessage="Phone number is a Required Field")]
        public string PhoneNumber { get; set; }
        [StringLength(20, ErrorMessage = "Invalid length for business number.", MinimumLength = 6)]
        public string BusinessNumber { get; set; }
        
        public string FaxNumber { get; set; }
       
        public string OfficeNumber { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Date of Birth is a Required Field!")]
        public DateTime DateOfBirth { get; set; }

        public MaritalStatas MaritalStatus { get; set; }

        [Required(ErrorMessage = "CommodityOwnerType is a Required Field!")]
        public CommodityOwnerType CommodityOwnerType { get; set; }

        [Required(ErrorMessage = "Supplier is a Required Field!")]
        public CommoditySupplier CommoditySupplier { get; set; }

        public string FullName
        {
            get { return FirstName + " " + Surname + " " + LastName; }
            
        }
    }
}
