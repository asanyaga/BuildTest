using System;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.ProductEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Distributor : StandardWarehouse
    {
        public Distributor() : base(default(Guid)) { }
        public Distributor(Guid id) : base (id){}
        public Distributor(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        { }

        public string Owner { get; set; }
        public string PIN { get; set; }
        public string AccountNo { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(Region))]
        public Guid RegionMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif       
        public Region Region {get;set;}


    #if __MOBILE__
       [ForeignKey(typeof(User))]
       public Guid ASMUserMasterId { get; set; }

       [OneToOne("ASMUserMasterId", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public User ASM {get;set;}


    #if __MOBILE__
       [ForeignKey(typeof(User))]
       public Guid SalesRepUserMasterId { get; set; }

       [OneToOne("SalesRepUserMasterId", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public User SalesRep { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(User))]
       public Guid SurveyorUserMasterId { get; set; }

        [OneToOne("SurveyorUserMasterId", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public User Surveyor{get;set;}


    #if __MOBILE__
       [ForeignKey(typeof(ProductPricingTier))]
        public Guid ProductPricingTierMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductPricingTier ProductPricingTier { get; set; }
        public string PaybillNumber { get; set; }
        public string MerchantNumber { get; set; }
       
    #if __MOBILE__
       public Guid ProducerId { get; set; }
    #endif  

    }
}
