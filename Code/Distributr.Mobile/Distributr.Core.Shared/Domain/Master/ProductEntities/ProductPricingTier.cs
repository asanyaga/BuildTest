using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    //Creating a base class
#if __MOBILE__
   [Table("PricingTier")]
#endif
    public class ProductPricingTier : MasterEntity
    {
       public ProductPricingTier() : base(default(Guid)) { }

        public  ProductPricingTier (Guid id): base(id)
        {

        }
        public ProductPricingTier(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        [Required(ErrorMessage="Enter Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tier Description is required")]
        public string Description { get; set; }
    }
}
