using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProductRef
    {
        public Guid ProductId { get; set; }
    }
#if !SILVERLIGTH
   [Serializable]
#endif
    public class Product : MasterEntity
   {

       public Product() : this(default(Guid))
       {
           
       }

        internal Product(Guid id) : base(id)
        {
            ProductPricings = new List<ProductPricing>();
            ProductDiscounts = new List<ProductDiscount>();
        }
        public Product(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<ProductPricing> productPricings)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            ProductPricings = productPricings;
            ProductDiscounts = new List<ProductDiscount>();
        }

        [Required(ErrorMessage = "Product Description Is Required")]
        public string Description { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(ProductBrand))]
        public Guid ProductBrandMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Product brand Is Required")]
        public ProductBrand Brand { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(ProductPackaging))]
        public Guid ProductPackagingMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        //[Required(ErrorMessage = "Product Packaging Is Required")]
        public ProductPackaging Packaging { get; set; }
         //[Required(ErrorMessage = "Product Flavor Is Required")]

    #if __MOBILE__
        [ForeignKey(typeof(ProductFlavour))]
        public Guid ProductFlavourMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductFlavour Flavour { get; set; }
        [Required(ErrorMessage = "Product Packaging Type Is Required")]

    #if __MOBILE__
        [ForeignKey(typeof(ProductPackagingType))]
        public Guid ProductPackagingTypeMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductPackagingType PackagingType { get; set; }
    #if __MOBILE__
       [Column("ReturnableTypeMasterId")]
    #endif       
        [Required(ErrorMessage="Returnable Type is Required!")]
        public ReturnableType ReturnableType { get; set; }

        [Required(ErrorMessage = "Product Code Is Required")]
        public string ProductCode { get; set; }


    #if __MOBILE__
        [ForeignKey(typeof(VATClass))]
        public Guid VATClassMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public VATClass VATClass { get; set; }
        [Required(ErrorMessage = "Product ExFactoryPrice Is Required")]
        public decimal ExFactoryPrice { get; set; }

    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<ProductPricing> ProductPricings { get; set; }

    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<ProductDiscount> ProductDiscounts { get; set; }

        public virtual decimal TotalExFactoryValue(ProductPricingTier tier)
        {
                            
            var ppc = ProductPricings.Where(n => n.Tier.Id == tier.Id);
            if (ppc == null)
                throw new ArgumentException("Product is not available at this tier");
            var items = ppc.Where(n => n.CurrentEffectiveDate < DateTime.Now)
               .OrderByDescending(n => n._DateCreated)
               .ThenByDescending(n => n.CurrentEffectiveDate);

            return items.First().CurrentExFactory;
        }

        public virtual decimal ProductPrice(ProductPricingTier tier)
        {
            var ppc = ProductPricings.Where(n => n.Tier.Id == tier.Id);
            if(ppc == null)
                throw new ArgumentException("Product is not available at this tier");
            var items = ppc.Where(n => n.CurrentEffectiveDate < DateTime.Now)
               .OrderByDescending(n => n._DateCreated)
               .ThenByDescending(n => n.CurrentEffectiveDate);

            return items.First() !=null?items.First().CurrentSellingPrice:0;
        }
       
        public virtual decimal Discount(ProductPricingTier tier)
        {
            var pd = ProductDiscounts.FirstOrDefault(n => n.Tier.Id == tier.Id);
            if (pd == null) 
                return 0;
            return pd.CurrentDiscountRate(0);
        }

        public virtual bool IsAvailableForSale(ProductPricingTier tier)
        {
            return ProductPricings.Any(n => n.Tier.Id == tier.Id);
        }
    }
}
