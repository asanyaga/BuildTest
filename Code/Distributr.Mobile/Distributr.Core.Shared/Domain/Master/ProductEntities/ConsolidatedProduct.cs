using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum DomainProductType { Sale = 1, Returnable = 2, Consolidated = 3 }
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ConsolidatedProduct : Product
    {

        public ConsolidatedProduct() : base(default(Guid)) { }

        internal ConsolidatedProduct(Guid id)
            : base(id)
        {
            ProductDetails = new List<ProductDetail>();
        }
        public ConsolidatedProduct(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<ProductPricing> productPricings, List<ProductDetail> productDetails)
            : base(id, dateCreated, dateLastUpdated, isActive, productPricings)
        {
            ProductDetails = productDetails;
        }

        public List<ProductDetail> ProductDetails { get; internal set; }

#if !SILVERLIGHT
   [Serializable]
#endif
        public class ProductDetail
        {
            public Product Product { get; set; }
            public int QuantityPerConsolidatedProduct { get; set; }
            public DomainProductType ProductDetailType
            {
                get
                {
                    string stype = Product.GetType().ToString().Split('.').Last();
                    DomainProductType t = DomainProductType.Sale;
                    switch (stype)
                    {
                        case "ConsolidatedProduct":
                            t = DomainProductType.Consolidated;
                            break;
                        case "SaleProduct":
                            t = DomainProductType.Sale;
                            break;
                        case "ReturnableProduct":
                            t = DomainProductType.Returnable;
                            break;
                    }
                    return t;
                }

            }
            public string Description { get { return Product == null ? "--" : Product.Description; } }
        }


        public class FlattenedProductDetail : ProductDetail
        {
            public Guid ProductId
            {
                get { return Product.Id; }
            }
            public int Level { get; set; }
            public ConsolidatedProduct DirectParent { get; set; }
            public Guid DirectParentId { get { return DirectParent.Id; } }
            public ConsolidatedProduct HomeConsolidatedProduct { get; set; }
            public int TotalProductsForHomeConsolidatedProduct { get; set; }
        }

        /// <summary>
        /// Extracts all products in consolidated product hierarchy
        /// including quantities
        /// </summary>
        public List<FlattenedProductDetail> GetFlattenedProducts()
        {
            var fp =  _GetFlattenedProducts(this, 0,1);
           

            return fp;
        }

        private List<FlattenedProductDetail> _GetFlattenedProducts(ConsolidatedProduct homeConsolidatedProduct, int level, int noParentItems)
        {
            int _level = level + 1;
            List<FlattenedProductDetail> flattenedProducts = new List<FlattenedProductDetail>();
            foreach (var product in ProductDetails)
            {
               
                if (product.Product == null)
                    continue;

                int qty = product.QuantityPerConsolidatedProduct;
                if (product.ProductDetailType != DomainProductType.Consolidated)
                {
                    flattenedProducts.Add(new FlattenedProductDetail
                    {
                        Product = product.Product,
                        QuantityPerConsolidatedProduct = qty,
                        DirectParent = this,
                        HomeConsolidatedProduct = homeConsolidatedProduct,
                        TotalProductsForHomeConsolidatedProduct = qty * noParentItems,
                        Level = _level
                    });
                }
                else
                {
                    ConsolidatedProduct cp = product.Product as ConsolidatedProduct;
                    var fp1 = new FlattenedProductDetail
                        {
                            Product = cp,
                            QuantityPerConsolidatedProduct = qty,
                            DirectParent = this,
                            HomeConsolidatedProduct = homeConsolidatedProduct,
                            TotalProductsForHomeConsolidatedProduct = qty * noParentItems,
                            Level = _level
                        };
                    flattenedProducts.Add(fp1);

                    foreach (FlattenedProductDetail fpd in cp._GetFlattenedProducts(homeConsolidatedProduct, _level, 
                        fp1.TotalProductsForHomeConsolidatedProduct))
                    {
                        flattenedProducts.Add(fpd);
                    }
                }
            }

            return flattenedProducts;
        }

        public override decimal TotalExFactoryValue(ProductPricingTier tier)
        {
            return ProductDetails.Sum(n => n.Product.TotalExFactoryValue(tier) * n.QuantityPerConsolidatedProduct);
        }

        public override decimal ProductPrice(ProductPricingTier tier)
        {
            return ProductDetails.Sum(n => n.Product.ProductPrice(tier) * n.QuantityPerConsolidatedProduct);
        }

        //cn
        public decimal ProductVATRate(ProductPricingTier ppt, Outlet outlet = null)
        {
            decimal vatRate = 0;
            decimal totalVat = 0;
            foreach (var item in ProductDetails)
            {
                decimal itemVATRate = 0;
                decimal itemUnitVATValue = 0;
                decimal itemTotalVATValue = 0;
                if (item.Product.VATClass != null)
                {
                    itemVATRate       = GetProductVATRate(item.Product, outlet);
                    itemUnitVATValue  = item.Product.ProductPrice(ppt) * itemVATRate;
                    itemTotalVATValue = itemUnitVATValue*item.QuantityPerConsolidatedProduct;
                }
                totalVat += itemTotalVATValue;
            }

            decimal unitPrice = this.ProductPrice(ppt);
            if (unitPrice > 0)
                vatRate = (totalVat/unitPrice);

            return vatRate;
        }

        //cn:7 -- esp for purchase order
        public decimal ProductVATRate()//
        {
            decimal vatRate = 0;
            decimal totalVat = 0;
            foreach (var item in ProductDetails)
            {
                decimal itemVATRate = 0;
                decimal itemUnitVATValue = 0;
                decimal itemTotalVATValue = 0;
                if (item.Product.VATClass != null)
                {
                    itemVATRate = GetProductVATRate(item.Product);
                    itemUnitVATValue = item.Product.ExFactoryPrice * itemVATRate;
                    itemTotalVATValue = itemUnitVATValue * item.QuantityPerConsolidatedProduct;
                }
                totalVat += itemTotalVATValue;
            }

            decimal unitPrice = this.ExFactoryPrice;
            if (unitPrice > 0)
                vatRate = (totalVat/unitPrice);

            return vatRate;
        }

        decimal GetProductVATRate(Product product, Outlet outlet  = null)
        {
            if (product is ReturnableProduct)
                return 0;

            if (outlet != null)
            {
                if (outlet.VatClass != null) //cn: Outlet VAT class takes precedence 
                {
                    if (outlet.VatClass.CurrentEffectiveDate <= DateTime.Now)
                        return outlet.VatClass.CurrentRate;
                }
            }

            if (product.VATClass != null)
                return product.VATClass.CurrentRate;

            return 0;
        }

        public override bool IsAvailableForSale(ProductPricingTier tier)
        {
            bool available = false;
            available = ProductDetails.Any(n => !n.Product.IsAvailableForSale(tier));

            return available ? false : true;
        }

        public bool CanAddProductToConsolidatedProduct(Product productToAdd, ConsolidatedProduct productToAddTo)
        {
            bool isOkToAdd = true;
            foreach (var p in ProductDetails)
            {
                if (p.Product.Id == productToAdd.Id)
                    isOkToAdd = false;
                if (p.Product is ConsolidatedProduct)
                {
                    ConsolidatedProduct cp = p.Product as ConsolidatedProduct;
                    isOkToAdd = cp.CanAddProductToConsolidatedProduct(productToAdd, productToAddTo);
                }
            }
            //cn : avoid adding itself
            if (productToAdd.Id == productToAddTo.Id)
                isOkToAdd = false;
            return isOkToAdd;
        }
    }
}
