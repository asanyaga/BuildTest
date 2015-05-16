using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.ProductEntities;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Outlet : CostCentre
    {

       public Outlet() : base(default(Guid)) { }

       public  Outlet(Guid id) : base (id)
        {
            CostCentreType = CostCentreType.Outlet;
            ShipToAddresses = new List<ShipToAddress>();
        }

        public Outlet(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            CostCentreType = CostCentreType.Outlet;
            _shipToAddresses  = new List<ShipToAddress>();
        }

    #if __MOBILE__
        public bool IsApproved { get; set;}
    #endif

    #if __MOBILE__
        [ForeignKey(typeof(Route))]
        public Guid RouteMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage="Route is a required field!")]
        public Route Route { get; set; }


    #if __MOBILE__
        [ForeignKey(typeof(OutletCategory))]
        public Guid OutletCategoryMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Outlet Category is a required field!")]
        public OutletCategory OutletCategory {get; set;}


        
    #if __MOBILE__
        [ForeignKey(typeof(OutletType))]
        public Guid OutletTypeMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Outlet Type is a required field!")]
        public OutletType OutletType { get; set; }

    #if !__MOBILE__
        public OutletUser OutletUser { get; set; }
        public User UsrSurveyor { get; set; }
        public User UsrSalesRep { get; set; }
        public User UsrASM { get; set; }
        //public string outLetCode { get; set; }
        public Contact ContactPerson { get; set; }
        public Contact PhoneNumber { get; set; }
    #endif

    #if __MOBILE__
        [ForeignKey(typeof(ProductPricingTier))]
        public Guid OutletProductPricingTierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif    
        public ProductPricingTier OutletProductPricingTier { get; set; }


    #if __MOBILE__
        [ForeignKey(typeof(ProductPricingTier))]
        public Guid SpecialPricingTierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif    
        public ProductPricingTier SpecialPricingTier { get; set; }


        //public List<Contact> Contact { get; set; }


    #if __MOBILE__
        [ForeignKey(typeof(VATClass))]
        public Guid VatClassMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif    
        public VATClass VatClass { get; set; }
        
        public string Latitude { get; set; }
        
        public string Longitude { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(DiscountGroup))]
       public Guid DiscountGroupMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif    
        public DiscountGroup DiscountGroup { get; set; }

       #if __MOBILE__
       [Ignore]
       #endif
        public List<ShipToAddress> ShipToAddresses 
        {
            get { return _shipToAddresses; }
            private set { _shipToAddresses = value; }
        }
        

       private List<ShipToAddress> _shipToAddresses;

       public void AddShipToAddress (ShipToAddress address)
       {
           _shipToAddresses.Add(address);
       }
    }

#if !SILVERLIGHT
   [Serializable]
#endif
    public class ShipToAddress : MasterEntity
    {
        public ShipToAddress() : base(default(Guid)) { }
        public ShipToAddress(Guid id) : base(id)
        {
        }

        public ShipToAddress(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status)
            : base(id, dateCreated, dateLastUpdated, status)
        {
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PostalAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    #if __MOBILE__
       [ForeignKey(typeof(Outlet))]
       public Guid OutletId { get; set; }
    #endif
    }
}
