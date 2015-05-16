using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.HQ.Lib.ViewModelBuilders;

namespace Distributr.HQ.Lib.Service.Impl
{
    public class DiscountProcessorService : IDiscountProcessorService
    {
        private IProductRepository _productRepository;
        private IProductDiscountRepository _productDiscount;
        private ICertainValueCertainProductDiscountRepository _certainValueCertainProduct;
        private IDiscountGroupRepository _discountGroupRepository;
        private IFreeOfChargeDiscountRepository _freeOfChargeDiscountRepository;
        private IProductDiscountGroupRepository _productGroupDiscountRepository;
        private IPromotionDiscountRepository _promotionDiscountRepository;
        private ISaleValueDiscountRepository _SaleValueDiscountRepository;
        private IProductPricingTierRepository _productPricingTierRepository;
        private List<DiscountBase> discountLine;
        private ICostCentreRepository _costCentreRepository;

        public DiscountProcessorService(IProductRepository productRepository, IProductDiscountRepository productDiscount, ICertainValueCertainProductDiscountRepository certainValueCertainProduct, IDiscountGroupRepository discountGroupRepository, IFreeOfChargeDiscountRepository freeOfChargeDiscountRepository, IProductDiscountGroupRepository productGroupDiscountRepository, IPromotionDiscountRepository promotionDiscountRepository, ISaleValueDiscountRepository saleValueDiscountRepository, IProductPricingTierRepository productPricingTierRepository, List<DiscountBase> discountLine, ICostCentreRepository costCentreRepository)
        {
            _productRepository = productRepository;
            _productDiscount = productDiscount;
            _certainValueCertainProduct = certainValueCertainProduct;
            _discountGroupRepository = discountGroupRepository;
            _freeOfChargeDiscountRepository = freeOfChargeDiscountRepository;
            _productGroupDiscountRepository = productGroupDiscountRepository;
            _promotionDiscountRepository = promotionDiscountRepository;
            _SaleValueDiscountRepository = saleValueDiscountRepository;
            _productPricingTierRepository = productPricingTierRepository;
            this.discountLine = discountLine;
            _costCentreRepository = costCentreRepository;
        }


        public UnitPriceDiscount GetUnitPrice(Guid productId, Guid outletId)
        {
            decimal unitPrice = 0;
            Outlet outlet = _costCentreRepository.GetById(outletId) as Outlet;
            if (outlet == null)
                throw new ArgumentException("Invalid Outlet Id");
            if (outlet.OutletProductPricingTier == null)
                throw new ArgumentException("Invalid Outlet Pricing Tier");
            ProductPricingTier tier = _productPricingTierRepository.GetById(outlet.OutletProductPricingTier.Id);
            Product p = _productRepository.GetById(productId);
            if (p == null)
                throw new ArgumentException("Invalid Product");
            UnitPriceDiscount unitPriceDiscount = new UnitPriceDiscount() { Discount = 0, UnitPrice = 0 };
            unitPrice = GetUnitPrice(p, tier);
            unitPriceDiscount.UnitPrice = unitPrice;

            if (_freeOfChargeDiscountRepository.IsProductFreeOfCharge(productId))
            {
                unitPriceDiscount.DiscountType = DiscountType.FreeOfChargeDiscount;
                unitPriceDiscount.UnitPrice = 0;
                return unitPriceDiscount;
            }
            if (p is SaleProduct)
            {
                if (outlet.DiscountGroup != null)
                {
                    ProductGroupDiscount pgd = _productGroupDiscountRepository.GetCurrentCustomerDiscount(outlet.DiscountGroup.Id, p.Id, 0);
                    if (pgd != null)
                    {
                        try
                        {
                            decimal rate = pgd.CurrentDiscount();
                            decimal discount = rate * unitPrice;
                            unitPrice = unitPrice - discount;
                            unitPriceDiscount.Discount = discount;
                            unitPriceDiscount.UnitPrice = unitPrice;
                            unitPriceDiscount.DiscountType = DiscountType.GroupDiscount;
                        }
                        catch
                        {
                        }

                    }
                }
                else
                {
                    try
                    {
                        ProductDiscount pd = _productDiscount.GetProductDiscount(productId, tier.Id);
                        decimal rate = pd.CurrentDiscountRate(0);
                        decimal discount = rate * unitPrice;
                        unitPrice = unitPrice - discount;
                        unitPriceDiscount.Discount = discount;
                        unitPriceDiscount.UnitPrice = unitPrice;
                        unitPriceDiscount.DiscountType = DiscountType.ProductDiscount;
                    }
                    catch
                    {
                    }
                }
            }

            return unitPriceDiscount;
        }

        public decimal GetUnitPrice(Product p, ProductPricingTier tier)
        {
            decimal unitPrice;
            if (_freeOfChargeDiscountRepository.IsProductFreeOfCharge(p.Id))
            {
                return 0m;
            }
            if (p is ConsolidatedProduct)
                try
                {
                    unitPrice = ((ConsolidatedProduct)p).ProductPrice(tier);
                }
                catch
                {
                    unitPrice = 0m;
                }
            else
                try
                {
                    unitPrice = p.ProductPrice(tier);
                }
                catch
                {
                    unitPrice = 0m;
                }
            return unitPrice;
        }

        public decimal GetVAT(Guid productId, Guid outletId)
        {
            decimal vat = 0;
            decimal vatrate = 0;

            Outlet outlet = _costCentreRepository.GetById(outletId) as Outlet;
            if (outlet == null)
                throw new ArgumentException("Invalid Outlet Id");
            //if (outlet.OutletProductPricingTier == null)
            //    throw new ArgumentException("Invalid Outlet Pricing Tier");
            //ProductPricingTier tier = _productPricingTierService.GetByMasterId(outlet.OutletProductPricingTier.Id);

            Product p = _productRepository.GetById(productId);
            if (p == null)
                throw new ArgumentException("Invalid Product");

            vatrate = GetVATRate(p, outlet);

            decimal unitPrice = GetUnitPrice(productId, outletId).UnitPrice;

            vat = vatrate * unitPrice;

            return vat;
        }

        public decimal GetVATRate(Product product, Outlet outlet)
        {
            if (_freeOfChargeDiscountRepository.IsProductFreeOfCharge(product.Id))
            {
                return 0m;
            }
            if (product is ReturnableProduct)
                return 0;

            if (product is ConsolidatedProduct)
                return ((ConsolidatedProduct)product).ProductVATRate(outlet.OutletProductPricingTier, outlet);

            if (outlet.VatClass != null) //cn: Outlet VAT class takes precedence 
            {
                if (outlet.VatClass.CurrentEffectiveDate <= DateTime.Now)
                    return outlet.VatClass.CurrentRate;
            }

            if (product.VATClass != null)
                return product.VATClass.CurrentRate;
            return 0m;
        }

        public List<ProductAsDiscount> GetFOCCertainProduct(Guid ProductId, decimal quantity)
        {
            List<ProductAsDiscount> ls = new List<ProductAsDiscount>();
            PromotionDiscount pd = _promotionDiscountRepository.GetByProductId(ProductId);
            if (pd != null)
            {
                try
                {
                    //ProductRef pRef = pd.CurrentFreeProduct(quantity);
                    //decimal freequantity = pd.CurrentFreeProductQuantity(quantity);
                    //cn::
                    ;

                    PromotionDiscount.PromotionDiscountItem promotionItem = pd.AwardedPromotionDiscountItem(quantity);
                    //decimal threshold = pd.CurrentParentProductQuantity;
                    decimal threshold = promotionItem.ParentProductQuantity;
                    decimal timesToApplyThreshold = (int)(quantity / threshold);
                    ProductRef pRef = promotionItem.FreeOfChargeProduct;
                    decimal freequantity = promotionItem.FreeOfChargeQuantity;

                    if (pRef != null && pRef.ProductId != Guid.Empty)
                    {
                        ProductAsDiscount ad = new ProductAsDiscount
                        {
                            ProductId = pRef.ProductId,
                            DiscountType = DiscountType.PromotionDiscount,
                            Quantity = (freequantity * timesToApplyThreshold)
                        };
                        if (ad.Quantity > 0)
                            ls.Add(ad);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return ls;
        }


        public decimal GetSalevalue(decimal amount, Guid outletId)
        {
            Outlet outlet = _costCentreRepository.GetById(outletId) as Outlet;
            if (outlet == null)
                throw new ArgumentException("Invalid Outlet Id");
            if (outlet.OutletProductPricingTier == null)
                throw new ArgumentException("Invalid Outlet Pricing Tier");
            ProductPricingTier tier = _productPricingTierRepository.GetById(outlet.OutletProductPricingTier.Id);

            SaleValueDiscount salevalueDiscount = _SaleValueDiscountRepository.GetByAmount(amount, tier.Id);

            if (salevalueDiscount != null)
            {
                try
                {
                    decimal rate = salevalueDiscount.CurrentRate;
                    decimal sd = rate * amount;
                    return sd;
                }
                catch { }
            }
            return 0;
        }

        public ProductAsDiscount GetFOCCertainValue(decimal amount)
        {
            CertainValueCertainProductDiscount certainvalueDiscount = _certainValueCertainProduct.GetByAmount(amount);
            if (certainvalueDiscount != null)
            {
                try
                {
                    ProductRef certainProduct = certainvalueDiscount.CurrentProduct;
                    decimal certainProductQty = certainvalueDiscount.CurrentQuantity;
                    //cn::
                    decimal threshold = certainvalueDiscount.InitialValue;
                    int timesToApplyThreshold = (int)(amount / threshold);
                    ProductAsDiscount ad = new ProductAsDiscount
                    {
                        ProductId = certainProduct.ProductId,
                        DiscountType = DiscountType.CertainValueCertainProductDiscount,
                        Quantity = certainProductQty,
                    };
                    if (ad.Quantity > 0)
                        return ad;
                }
                catch (Exception ex) { }

            }
            return null;
        }

       
        public AmountDiscount GetAmountDiscount(DiscountType disType)
        {
            AmountDiscount ad;
            if (discountLine.OfType<AmountDiscount>().Any(p => p.DiscountType == disType))
            {
                ad = discountLine.OfType<AmountDiscount>().FirstOrDefault(p => p.DiscountType == disType);
            }
            else
            {
                ad = new AmountDiscount();
                ad.DiscountType = disType;
                discountLine.Add(ad);
            }
            return ad;
        }
        public ProductAsDiscount GetProductAsDiscount(DiscountType disType, Guid productId)
        {
            ProductAsDiscount pad = new ProductAsDiscount();
            if (discountLine.OfType<ProductAsDiscount>().Any(p => p.DiscountType == disType && p.ProductId == productId))
            {
                pad = discountLine.OfType<ProductAsDiscount>().FirstOrDefault(p => p.DiscountType == disType && p.ProductId == productId);
            }
            else
            {
                pad.ProductId = productId;
                pad.DiscountType = disType;
                discountLine.Add(pad);
            }
            return pad;
        }

        //cn
        public LineItemPricingInfo GetLineItemPricing(PackagingSummary packagingSummary, Guid outletId)
        {
            UnitPriceDiscount unitPriceDiscount = null;
            LineItemPricingInfo pricingInfo = new LineItemPricingInfo();
            unitPriceDiscount = GetUnitPrice(packagingSummary.Product.Id, outletId);

            decimal unitPrice = unitPriceDiscount.UnitPrice;
            decimal totalNetPrice = unitPrice * packagingSummary.Quantity;
            decimal vatValue = GetVAT(packagingSummary.Product.Id, outletId); //
            decimal totalVatAmount = vatValue * packagingSummary.Quantity;
            decimal totalPrice = totalNetPrice + totalVatAmount;

            pricingInfo.UnitPrice = unitPrice;
            pricingInfo.VatValue = vatValue;
            pricingInfo.TotalVatAmount = totalVatAmount;
            pricingInfo.TotalNetPrice = totalNetPrice;
            pricingInfo.TotalPrice = totalPrice;
            pricingInfo.ProductDiscount = unitPriceDiscount.Discount;
            pricingInfo.TotalProductDiscount = (unitPriceDiscount.Discount * packagingSummary.Quantity);

            return pricingInfo;
        }

        public List<Product> ReturnFreeOfChargeProducts(List<Product> inProducts)
        {
            List<Product> returnItems = new List<Product>();
            foreach (var prod in inProducts)
            {
                if (_freeOfChargeDiscountRepository.IsProductFreeOfCharge(prod.Id))
                {
                    returnItems.Add(prod);
                }
            }

            return returnItems;
        }

        public bool IsProductFreeOfCharge(Guid productId)
        {
            return _freeOfChargeDiscountRepository.IsProductFreeOfCharge(productId);
        }

        public decimal GetTotalGross(decimal amount)
        {
            //It rounds up anything greater than or equal to  0.04

            var decimalValues = amount - decimal.Truncate(amount);
            if (decimalValues > 0.04m)
            {
                return decimal.Truncate(amount) + 1;
            }
            return decimal.Truncate(amount);
        }

        public decimal GetTruncatedValue(decimal value)
        {
            //It truncates to two decimal places(that's why 100 is used)

            var truncatedValue = Math.Truncate(value * 100) / 100;
            return truncatedValue;
        }


    }
}
