using System;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Attributes;
#endif
using Distributr.Core.Domain.Master.ProductEntities;


namespace Distributr.Core.Domain.Master.CompetitorManagement
{
#if !SILVERLIGHT
   [Serializable]
#endif
#if __MOBILE__
    [Table("CompetitorProduct")]
#endif
   public class CompetitorProducts:MasterEntity
    {
       public CompetitorProducts(Guid id)
           : base(id)
       { 
       }
       public CompetitorProducts(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       
       }
       public string ProductName { get; set; }
       public string ProductDescription { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(Competitor))]
       public Guid CompetitorMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif
       public Competitor Competitor { get; set; }


    #if __MOBILE__
       [ForeignKey(typeof(ProductBrand))]
       public Guid BrandMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif       
       public ProductBrand Brand{get;set;}

    #if __MOBILE__      
       [ForeignKey(typeof(ProductPackaging))]
       public Guid PackagingMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif 
       public ProductPackaging Packaging { get; set; }
              
    #if __MOBILE__
       [ForeignKey(typeof(ProductType))]
       public Guid ProductTypeMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif 
       public ProductType ProductType { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(ProductPackagingType))]
       public Guid PackagingTypeMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif 
       public ProductPackagingType PackagingType { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(ProductFlavour))]
       public Guid FlavourMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif 
       public ProductFlavour Flavour { get; set; }
    }
}
