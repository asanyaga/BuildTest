using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Workflow
{
    public interface IDiscountProWorkflow
    {
        decimal GetUnitPrice(Product p, ProductPricingTier tier);
        UnitPriceDiscount GetUnitPrice(Guid productId, Guid outletId,decimal quantity);
        decimal GetVATRate(Product product, Outlet outlet);
        decimal GetVAT(Guid productId, Guid outletId, decimal quantity);
        List<ProductAsDiscount> GetFOCCertainProduct(Guid ProductId, decimal quantity);
        decimal GetSalevalue(decimal amount, Guid outletId);
        ProductAsDiscount GetFOCCertainValue(decimal amount);
        LineItemPricingInfo GetLineItemPricing(PackagingSummary packagingSummary, Guid outletId);//cn
        List<Product> ReturnFreeOfChargeProducts(List<Product> inProducts); //cn
        bool IsProductFreeOfCharge(Guid productId);//cn
        LineItemPricingInfo GetLineItemPricing(Guid productId, decimal quantity, Guid outletId);


        LineItemPricingInfo GetPurchaseLineItemPricing(PackagingSummary packagingSummary);

       }
    public class OrderDiscountLineItem
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal UnitPrice { get; set; }
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
    public class ProductPackagingSummary
    {
        public Product Product { set; get; }
        public decimal Quantity { set; get; }
        public bool IsOneToOne { set; get; }
        public bool IsMix { set; get; }
       
    }

    public class PackagingSummary
    {
        public Product Product { set; get; }
        public decimal Quantity { set; get; }
        public bool IsEditable { set; get; }
        public bool IsAuto { set; get; }
        public Guid ParentProductId { set; get; }
        
    }
   
}
