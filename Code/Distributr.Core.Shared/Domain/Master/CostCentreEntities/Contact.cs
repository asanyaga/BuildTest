using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class Contact:MasterEntity
    {

       public Contact() : base(default(Guid)) { }
       public Contact(Guid id)
           : base(id)
       { }

       public Contact(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {

       }

       [Required(ErrorMessage = "Firstname is required")]
       public string Firstname { get; set; }
       public string Lastname { get; set; }
       public DateTime? DateOfBirth { get; set; }
       public string SpouseName { get; set; }
       public string Company { get; set; }
       public string JobTitle { get; set; }
       public string City { get; set; }
       public string HomeTown { get; set; }
       public string BusinessPhone { get; set; }
       public string Fax { get; set; }
       
       
       public string PhysicalAddress { get; set; }
       public string PostalAddress { get; set; }
       public string HomePhone { get; set; }
       public string WorkExtPhone { get; set; }
       [Required(ErrorMessage = "Mobile Number is required")]
       public string MobilePhone { get; set; }

       [RegularExpression( "^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$" , ErrorMessage = "Invalid email format." )]
       public string Email { get; set; }
       public string ChildrenNames { get; set; }
       
       public ContactClassification ContactClassification { get; set; }
    #if __MOBILE__
       [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
       public ContactType ContactType { get; set; }
       
       public Guid ContactOwnerMasterId { get; set; }
       public ContactOwnerType ContactOwnerType { get; set; }
       public MaritalStatas MStatus { get; set; }
   }
}
