using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.SuppliersEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.UserEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class User : MasterEntity
    {
        public User() : base(default(Guid)) { }

        public User(Guid id) : base(id)
        {
            UserRoles = new List<string>();
        }
        public User(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            UserRoles = new List<string>();
        }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Cost Center is required")]
        public Guid CostCentre { get; set; }

        //public CostCentre CostcentreRef { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public string PIN { get; set; }

        public int PassChange { get; set; }
        public string TillNumber { get; set; }

        [Required(ErrorMessage = "User type is required")]
        public UserType UserType { get; set; }
        public List<string> UserRoles { get; internal set; }

        [Required(ErrorMessage = "User mobile phone number is required")]
        public string Mobile { get; set; }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public UserGroup Group { get; set; }

        public string Code { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public Supplier Supplier { set; get; }
       
    }
}
