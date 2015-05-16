using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;

namespace Distributr.Core.Workflow.Impl.Discount
{
    public class DiscountProWorkflow : IDiscountProWorkflow
    {
        private IProductRepository _productService;
        private IProductDiscountRepository _productDiscount;
        private ICertainValueCertainProductDiscountRepository _certainValueCertainProduct;
        private IDiscountGroupRepository _discountGroupService;
        private IFreeOfChargeDiscountRepository _freeOfChargeDiscountService;
        private IProductDiscountGroupRepository _productGroupDiscountService;
        private IPromotionDiscountRepository _promotionDiscountService;
        private ISaleValueDiscountRepository _SaleValueDiscountService;
        private IProductPricingTierRepository _productPricingTierService;
        private List<DiscountBase> discountLine;
        private ICostCentreRepository _costCentreService;

        public DiscountProWorkflow(ICostCentreRepository costCentreService, IProductRepository productService, IProductDiscountRepository productDiscount,
            ICertainValueCertainProductDiscountRepository certainValueCertainProduct, IDiscountGroupRepository discountGroupService,
            IFreeOfChargeDiscountRepository freeOfChargeDiscountService, IProductDiscountGroupRepository productGroupDiscountService, IPromotionDiscountRepository promotionDiscountService, ISaleValueDiscountRepository saleValueDiscountService, IProductPricingTierRepository productPricingTierService)
        {
            _productService = productService;
            _productDiscount = productDiscount;
            _certainValueCertainProduct = certainValueCertainProduct;
            _discountGroupService = discountGroupService;
            _freeOfChargeDiscountService = freeOfChargeDiscountService;
            _productGroupDiscountService = productGroupDiscountService;
            _promotionDiscountService = promotionDiscountService;
            _SaleValueDiscountService = saleValueDiscountService;
            _productPricingTierService = productPricingTierService;
            _costCentreService = costCentreService;
        }


        public List<DiscountBase> GetDiscountSummary(List<OrderDiscountLineItem> productItems, Guid outletId)
        {
            discountLine = new List<DiscountBase>();
            var outlet = _costCentreService.GetById(outletId) as Outlet;
            if(outlet==null)
                throw new ArgumentException("Invalid Outlet Id");
            if (outlet.OutletProductPricingTier == null)
                throw new ArgumentException("Invalid Outlet Pricing Tier");
            ProductPricingTier tier = _productPricingTierService.GetById(outlet.OutletProductPricingTier.Id);
         
            decimal totalAmount = productItems.Sum(s => s.TotalPrice);

            #region Salevalue Discount
            SaleValueDiscount salevalueDiscount = _SaleValueDiscountService.GetCurrentDiscount(totalAmount,tier.Id);
            if (salevalueDiscount!=null)
            {
                decimal rate = salevalueDiscount.CurrentRate;
                decimal sd = rate*totalAmount;
                AmountDiscount ad = new AmountDiscount
                {
                    DiscountAmount = sd,
                    DiscountType = DiscountType.SaleValueDiscount,
                };
                AddDiscount(ad);
            }
            #endregion
            
            #region Product Discount
            foreach (OrderDiscountLineItem item in productItems)
            {
                try
                {
                    ProductDiscount pd = _productDiscount.GetProductDiscount(item.ProductId, tier.Id);
                    decimal rate = pd.CurrentDiscountRate(item.Quantity);
                    decimal issuedPd = rate * item.TotalPrice;
                    AmountDiscount ad = new AmountDiscount
                                            {
                                                DiscountAmount = issuedPd,
                                                DiscountType = DiscountType.ProductDiscount,
                                            };
                    AddDiscount(ad);
                }
                catch
                {
                }
            }
            #endregion

            #region Customer Discount
            if (outlet.DiscountGroup!=null)
            {
                foreach (OrderDiscountLineItem item in productItems)
                {
                    ProductGroupDiscount pgd = _productGroupDiscountService.GetCurrentCustomerDiscount(outlet.DiscountGroup.Id,item.ProductId,item.Quantity);
                    if (pgd != null)
                    {
                        decimal rate = pgd.CurrentDiscount();
                        decimal issuedPd = rate*item.TotalPrice;
                        if (issuedPd > 0)
                        {
                            AmountDiscount ad = new AmountDiscount
                            {
                                DiscountAmount = issuedPd,
                                DiscountType = DiscountType.GroupDiscount,
                            };
                            AddDiscount(ad);
                        }
                    }
                }

            }
            #endregion 

            #region Promotion
            foreach (OrderDiscountLineItem item in productItems)
            {
                PromotionDiscount pd = _promotionDiscountService.GetAll()
                    .Where(p => p.ProductRef.ProductId == item.ProductId).FirstOrDefault();
                if (pd != null)
                {
                    try
                    {
                        decimal rate = pd.CurrentDiscountRate;
                        decimal issuedPd = rate*item.TotalPrice;
                        if (issuedPd > 0)
                        {
                            AmountDiscount ad = new AmountDiscount
                            {
                                DiscountAmount = issuedPd,
                                DiscountType = DiscountType.PromotionDiscount,
                            };
                            AddDiscount(ad);
                        }
                        ProductRef pRef = pd.CurrentFreeOfChargeProduct;
                        decimal freequantity =  pd.CurrentFreeOfChargeQuantity;
                        if(pRef!= null && pRef.ProductId!=Guid.Empty)
                        {
                            ProductAsDiscount ad = new ProductAsDiscount
                            {
                                ProductId = pRef.ProductId,
                                DiscountType = DiscountType.PromotionDiscount,
                                Quantity = freequantity
                            };
                            AddDiscount(ad);
                        }
                       
                    }catch(Exception ex)
                    {
                    }
                }
            }
            #endregion

            #region Free of charge
            foreach (OrderDiscountLineItem item in productItems)
            {
                if(_freeOfChargeDiscountService.IsProductFreeOfCharge(item.ProductId))
                {
                    AmountDiscount ad = new AmountDiscount
                    {
                        DiscountAmount = item.TotalPrice,
                        DiscountType = DiscountType.FreeOfChargeDiscount,
                    };
                    AddDiscount(ad);
                }
            }

            #endregion

            #region Certain Value Certain Product
            CertainValueCertainProductDiscount certainvalueDiscount = _certainValueCertainProduct.GetByAmount(totalAmount);
            if (certainvalueDiscount != null)
            {
                try
                {
                    ProductRef certainProduct = certainvalueDiscount.CurrentProduct;
                    decimal certainProductQty = certainvalueDiscount.CurrentQuantity;
                    ProductAsDiscount ad = new ProductAsDiscount
                                               {
                                                   ProductId = certainProduct.ProductId,
                                                   DiscountType = DiscountType.CertainValueCertainProductDiscount,
                                                   Quantity = certainProductQty
                                               };
                    AddDiscount(ad);
                }catch(Exception ex)
                {
                    
                }

            }
            #endregion


            return discountLine;
        }

        public UnitPriceDiscount GetUnitPrice(Guid productId, Guid outletId,decimal quantity)
        {
            UnitPriceDiscount unitPriceDiscount = new UnitPriceDiscount() { Discount = 0, UnitPrice = 0 };
            try
            {
                decimal unitPrice = 0;
                ProductPricingTier tier = null;
                Product p = null;
                Outlet outlet = _costCentreService.GetById(outletId) as Outlet;
                if (outlet == null)
                    throw new ArgumentException("Invalid Outlet Id");

                if (outlet.SpecialPricingTier != null)
                {
                    tier = _productPricingTierService.GetById(outlet.SpecialPricingTier.Id);
                    p = _productService.GetById(productId);
                    if (p == null)
                        throw new ArgumentException("Invalid Product");

                    unitPrice = GetUnitPrice(p, tier);
                    unitPriceDiscount.UnitPrice = unitPrice;

                }
                if (outlet.OutletProductPricingTier != null && unitPrice==0)
                {
                    tier = _productPricingTierService.GetById(outlet.OutletProductPricingTier.Id);
                    p = _productService.GetById(productId);
                    if (p == null)
                        throw new ArgumentException("Invalid Product");

                    unitPrice = GetUnitPrice(p, tier);
                    unitPriceDiscount.UnitPrice = unitPrice;
                   
                }
                else if (outlet.OutletProductPricingTier==null)
                {
                    throw new Exception("Invalid Outlet Pricing Tier");
                }

                if (_freeOfChargeDiscountService.IsProductFreeOfCharge(productId))
                {
                    unitPriceDiscount.DiscountType = DiscountType.FreeOfChargeDiscount;
                    unitPriceDiscount.UnitPrice = 0;
                    return unitPriceDiscount;
                }
                if (p is SaleProduct)
                {
                    if (outlet.DiscountGroup != null)
                    {
                        ProductGroupDiscount pgd = _productGroupDiscountService.GetCurrentCustomerDiscount(outlet.DiscountGroup.Id, p.Id, quantity);
                        if (pgd != null)
                        {

                            decimal rate = pgd.CurrentDiscount();
                            decimal discount = rate*unitPrice;
                            unitPrice = unitPrice - discount;


                            unitPriceDiscount.Discount = discount;
                            unitPriceDiscount.UnitPrice = unitPrice;
                            unitPriceDiscount.DiscountType = DiscountType.GroupDiscount;

                        }
                    }
                    else
                    {
                       
                            ProductDiscount pd = _productDiscount.GetProductDiscount(productId, tier.Id);
                            decimal rate =pd!=null? pd.CurrentDiscountRate(quantity):0;
                            decimal discount = rate * unitPrice;
                            unitPrice = unitPrice - discount;
                            unitPriceDiscount.Discount = discount;
                            unitPriceDiscount.UnitPrice = unitPrice;
                            unitPriceDiscount.DiscountType = DiscountType.ProductDiscount;
                        
                    }
                }

                return unitPriceDiscount;
                
            }
            catch(Exception e)
            {
                throw e;
            }
          
        }

        public decimal GetUnitPrice(Product p, ProductPricingTier tier)
        {
            decimal unitPrice;
            if (_freeOfChargeDiscountService.IsProductFreeOfCharge(p.Id))
            {
                return 0m;
            }
            if (p is ConsolidatedProduct)
                try
                {
                    unitPrice = ((ConsolidatedProduct) p).ProductPrice(tier);
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

        public decimal GetVAT(Guid productId, Guid outletId,decimal quantity)
        {
            decimal vat = 0;
            decimal vatrate = 0;
            
            Outlet outlet = _costCentreService.GetById(outletId) as Outlet;
            if (outlet == null)
                throw new ArgumentException("Invalid Outlet Id");
            //if (outlet.OutletProductPricingTier == null)
            //    throw new ArgumentException("Invalid Outlet Pricing Tier");
            //ProductPricingTier tier = _productPricingTierService.GetByMasterId(outlet.OutletProductPricingTier.Id);

            Product p = _productService.GetById(productId);
            if (p == null)
                throw new ArgumentException("Invalid Product");

            vatrate = GetVATRate(p, outlet);

            decimal unitPrice = GetUnitPrice(productId, outletId, quantity).UnitPrice;

            vat = vatrate*unitPrice;

            return vat;
        }

        public decimal GetVATRate(Product product, Outlet outlet)
        {
            if (_freeOfChargeDiscountService.IsProductFreeOfCharge(product.Id))
            {
                return 0m;
            }
            if (product is ReturnableProduct)
                return 0;

            if (product is ConsolidatedProduct)
                return ((ConsolidatedProduct) product).ProductVATRate(outlet.OutletProductPricingTier, outlet);

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
            var ls = new List<ProductAsDiscount>();
            PromotionDiscount pd = _promotionDiscountService.GetCurrentDiscount(ProductId);
            if (pd != null)
            {
               

                    PromotionDiscount.PromotionDiscountItem promotionItem = pd.AwardedPromotionDiscountItem(quantity);
                    if (promotionItem != null)
                    {
                        //decimal threshold = pd.CurrentParentProductQuantity;
                        decimal threshold = promotionItem.ParentProductQuantity;
                        decimal timesToApplyThreshold = (int) (quantity/threshold);
                        ProductRef pRef = promotionItem.FreeOfChargeProduct;
                        decimal freequantity = promotionItem.FreeOfChargeQuantity;

                        if (pRef != null && pRef.ProductId != Guid.Empty)
                        {
                            ProductAsDiscount ad = new ProductAsDiscount
                                                       {
                                                           ProductId = pRef.ProductId,
                                                           DiscountType = DiscountType.PromotionDiscount,
                                                           Quantity = (freequantity*timesToApplyThreshold)
                                                       };
                            if (ad.Quantity > 0)
                                ls.Add(ad);
                        }
                    }

            }
            return ls;
        }

        public decimal GetSalevalue(decimal amount, Guid outletId)
        {
            Outlet outlet = _costCentreService.GetById(outletId) as Outlet;
            if (outlet == null)
                throw new ArgumentException("Invalid Outlet Id");
            if (outlet.OutletProductPricingTier == null)
                throw new ArgumentException("Invalid Outlet Pricing Tier");
            ProductPricingTier tier = _productPricingTierService.GetById(outlet.OutletProductPricingTier.Id);
         
            SaleValueDiscount salevalueDiscount = _SaleValueDiscountService.GetCurrentDiscount(amount, tier.Id);
            if (salevalueDiscount != null)
            {
                try
                {
                    decimal rate = salevalueDiscount.CurrentRate;
                    decimal sd = rate*amount;
                    return sd;
                }catch{}
            }
            return 0;
        }

        public ProductAsDiscount GetFOCCertainValue(decimal amount)
        {
            CertainValueCertainProductDiscount certainvalueDiscount = _certainValueCertainProduct.GetByAmount(amount);
            if (certainvalueDiscount != null)
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
            return null;
        }

        private void AddDiscount(DiscountBase discount)
        {
            if (discount is AmountDiscount)
            {
                AmountDiscount a = discount as AmountDiscount;
                AmountDiscount aad = GetAmountDiscount(discount.DiscountType);
                aad.DiscountAmount += a.DiscountAmount;
            }
            if (discount is ProductAsDiscount)
            {
                ProductAsDiscount a = discount as ProductAsDiscount;
                ProductAsDiscount pad = GetProductAsDiscount(discount.DiscountType,a.ProductId);
                pad.Quantity += a.Quantity;
            }
        }
        public AmountDiscount GetAmountDiscount(DiscountType disType)
        {
            AmountDiscount ad;
            if(discountLine.OfType<AmountDiscount>().Any(p=>p.DiscountType==disType))
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
            }else
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
            unitPriceDiscount = GetUnitPrice(packagingSummary.Product.Id, outletId,packagingSummary.Quantity);

            decimal unitPrice = unitPriceDiscount.UnitPrice;
            decimal totalNetPrice = unitPrice * packagingSummary.Quantity;
            decimal vatValue = GetVAT(packagingSummary.Product.Id, outletId, packagingSummary.Quantity); //
            decimal totalVatAmount = vatValue * packagingSummary.Quantity;
            decimal totalPrice = (totalNetPrice + totalVatAmount).GetTruncatedValue();

            pricingInfo.UnitPrice = unitPrice;
            pricingInfo.VatValue = vatValue;
            pricingInfo.TotalVatAmount = totalVatAmount;
            pricingInfo.TotalNetPrice = totalNetPrice;
            pricingInfo.TotalPrice = totalPrice;
            pricingInfo.ProductDiscount = unitPriceDiscount.Discount;
            pricingInfo.TotalProductDiscount = (unitPriceDiscount.Discount*packagingSummary.Quantity);

            return pricingInfo;
        }
        public LineItemPricingInfo GetLineItemPricing(Guid productId,decimal quantity, Guid outletId)
        {
            UnitPriceDiscount unitPriceDiscount = null;
            LineItemPricingInfo pricingInfo = new LineItemPricingInfo();
            unitPriceDiscount = GetUnitPrice(productId, outletId,quantity);

            decimal unitPrice = unitPriceDiscount.UnitPrice;
            decimal totalNetPrice = unitPrice * quantity;
            decimal vatValue = GetVAT(productId, outletId, quantity); //
            decimal totalVatAmount = vatValue *quantity;
            decimal totalPrice = totalNetPrice + totalVatAmount;

            pricingInfo.UnitPrice = unitPrice;
            pricingInfo.VatValue = vatValue;
            pricingInfo.TotalVatAmount = totalVatAmount;
            pricingInfo.TotalNetPrice = totalNetPrice;
            pricingInfo.TotalPrice = totalPrice;
            pricingInfo.ProductDiscount = unitPriceDiscount.Discount;
            pricingInfo.TotalProductDiscount = (unitPriceDiscount.Discount * quantity);

            return pricingInfo;
        }

        public LineItemPricingInfo GetPurchaseLineItemPricing(PackagingSummary packagingSummary)
        {
            LineItemPricingInfo pricingInfo = new LineItemPricingInfo();
            decimal UnitPrice = 0;
            decimal vatValue = 0;
            if (packagingSummary.Product is ConsolidatedProduct)
                try
                {
                    UnitPrice = packagingSummary.Product.ExFactoryPrice;
                }
                catch
                {
                    UnitPrice = 0m;
                }
            else
                try
                {
                    UnitPrice = packagingSummary.Product.ExFactoryPrice;
                }
                catch
                {
                    UnitPrice = 0m;
                }
            //if (packagingSummary.Product.VATClass != null && packagingSummary.Product is SaleProduct)
            //    vatValue = packagingSummary.Product.VATClass.CurrentRate;
            //else
            //    vatValue = 0;
            vatValue = 0;

            decimal totalNetPrice = UnitPrice * packagingSummary.Quantity;
            decimal totalVatAmount = vatValue * packagingSummary.Quantity;
            decimal totalPrice = totalNetPrice + totalVatAmount;

            pricingInfo.UnitPrice = UnitPrice;
            pricingInfo.VatValue = vatValue;
            pricingInfo.TotalVatAmount = totalVatAmount;
            pricingInfo.TotalNetPrice = totalNetPrice;
            pricingInfo.TotalPrice = totalPrice;
            pricingInfo.ProductDiscount =0;
            pricingInfo.TotalProductDiscount = 0;

            return pricingInfo;
        }

        

        public List<Product> ReturnFreeOfChargeProducts(List<Product> inProducts)
        {
            List<Product> returnItems = new List<Product>();
            foreach (var prod in inProducts)
            {
                if (_freeOfChargeDiscountService.IsProductFreeOfCharge(prod.Id))
                {
                    returnItems.Add(prod);
                }
            }

            return returnItems;
        }

        public bool IsProductFreeOfCharge(Guid productId)
        {
            return _freeOfChargeDiscountService.IsProductFreeOfCharge(productId);
        }

       
    }
}
