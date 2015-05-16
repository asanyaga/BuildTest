using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.HQ.Lib.ViewModelBuilders;

namespace Distributr.HQ.Lib.Service
{
   public interface IDiscountProcessorService
    {
        decimal GetUnitPrice(Product p, ProductPricingTier tier);
        UnitPriceDiscount GetUnitPrice(Guid productId, Guid outletId);
        decimal GetVATRate(Product product, Outlet outlet);
        decimal GetVAT(Guid productId, Guid outletId);
        List<ProductAsDiscount> GetFOCCertainProduct(Guid ProductId, decimal quantity);
        decimal GetSalevalue(decimal amount, Guid outletId);
        ProductAsDiscount GetFOCCertainValue(decimal amount);
        LineItemPricingInfo GetLineItemPricing(PackagingSummary packagingSummary, Guid outletId);//cn
        List<Product> ReturnFreeOfChargeProducts(List<Product> inProducts); //cn
        bool IsProductFreeOfCharge(Guid productId);//cn

        decimal GetTotalGross(decimal amount);
        decimal GetTruncatedValue(decimal amount);
    }
   
   public class DiscountBase
   {
       public DiscountType DiscountType { set; get; }

   }
   public class ProductAsDiscount : DiscountBase
   {
       public Guid ProductId { get; set; }
       public decimal Quantity { get; set; }
   }
   public class AmountDiscount : DiscountBase
   {
       public decimal DiscountAmount { get; set; }
   }
   public class UnitPriceDiscount
   {
       public decimal Discount { get; set; }
       public decimal UnitPrice { get; set; }
       public DiscountType DiscountType { set; get; }
   }
   public struct LineItemPricingInfo
   {
       public decimal UnitPrice;
       //public decimal UnitVatRate;
       public decimal ProductDiscount;
       public decimal TotalNetPrice;
       public decimal VatValue;
       public decimal TotalVatAmount;
       public decimal TotalPrice;
       public decimal TotalProductDiscount;
   }
}
