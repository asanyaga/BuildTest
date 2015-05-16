using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Orders
{
    public class ViewModelMessage : MessageBase
    {
        public Guid Id { get; set; }
       
      
    }
    public class ApproveStockistPurchaseOrderMessage : MessageBase
    {
        public Guid Id { get; set; }


    }
    public class OrderContinueMessage : MessageBase
    {
        public Guid Id { get; set; }
        public bool IsUnConfirmed { get; set; }

    }
    public class StockistPurchaseOrderContinueMessage : MessageBase
    {
        public Guid Id { get; set; }
        public bool IsUnConfirmed { get; set; }

    }
    public class PurchaseOrderContinueMessage : MessageBase
    {
        public Guid Id { get; set; }
        public bool IsUnConfirmed { get; set; }

    }
    public class SaleOrderContinueMessage : MessageBase
    {
        public Guid Id { get; set; }
        public bool IsUnConfirmed { get; set; }

    }

    public class DocumentDetailMessage : MessageBase
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        //Go:property stores information about source of message=>to determine back navigation after viewing
        public string MessageSourceUrl { get; set; } 
    }

   
    public class ProductPopUpItem
    {
        public Product Product { set; get; }
        public decimal Quantity { set; get; }
        public bool IsFreeOfCharge { get; set; }
        
    }
    public interface IDiscountHelper
    {
        List<MainOrderLineItem> Calculate(List<MainOrderLineItem> line, Guid id);
        List<MainOrderLineItem> CalculateForPos(List<MainOrderLineItem> line, Guid id);
        decimal CalculateSaleDiscount(decimal totalGross, Guid outletId);
        }
    public class DiscountHelper : IDiscountHelper
    {

        private List<MainOrderLineItem> discountItem;
        private IDiscountProWorkflow _discountService;
        private IProductRepository _productRepository;
        private IOutletRepository _outletRepository;
        private IInventoryRepository _inventoryRepository;
        private IConfigService _configService;

        public DiscountHelper(IDiscountProWorkflow discountService, IProductRepository productRepository, IOutletRepository outletRepository, IInventoryRepository inventoryRepository, IConfigService configService)
        {
            _discountService = discountService;
            _productRepository = productRepository;
            _outletRepository = outletRepository;
            _inventoryRepository = inventoryRepository;
            _configService = configService;
        }

        public List<MainOrderLineItem> CalculateForPos(List<MainOrderLineItem> line, Guid outletId)
        {
            var warehouseId = _configService.Load().CostCentreId;
            discountItem = new List<MainOrderLineItem>();
            CalculateCVCP(line.Sum(s => s.GrossAmount));
            CalculatePromotion(line);
            StringBuilder sb = new StringBuilder();
            foreach (var i in discountItem)
            {
                var p = _productRepository.GetById(i.ProductId) as SaleProduct;
                if (p != null)
                {
                    decimal balance = 0;
                    var inventory = _inventoryRepository.GetByProductIdAndWarehouseId(p.Id, warehouseId);
                    if (inventory != null)
                        balance = inventory.Balance-line.Where(s=>s.ProductId==p.Id).Sum(s=>s.Quantity);
                    //add returnable

                    if (balance >= i.Quantity)
                    {
                        if (p.ReturnableProduct != null)
                        {
                            var r = p.ReturnableProduct;
                            var rinventory = _inventoryRepository.GetByProductIdAndWarehouseId(r.Id, warehouseId);
                            decimal rbalance = 0;
                            if (rinventory != null)
                                rbalance = rinventory.Balance - line.Where(s => s.ProductId == r.Id).Sum(s => s.Quantity); ;
                            if (rbalance >= i.Quantity)
                            {
                                line.Add(i);
                                MainOrderLineItem item = null;
                                if (line.Any(a => a.ProductId == r.Id))
                                {
                                    item = line.First(a => a.ProductId == r.Id);
                                    item.Quantity += i.Quantity;

                                }
                                else
                                {
                                    item = new MainOrderLineItem();
                                    item.ProductId = r.Id;
                                    item.ProductName = r.Description;
                                    item.ProductType = "Retunable";
                                    item.Quantity = i.Quantity;
                                    line.Add(item);
                                }
                            }
                            else
                            {
                                //report no inventory the sale and returnable;
                                sb.AppendLine(string.Format("Product {0} of Quantity  {1}  for discount {2} cann't be fullfilled ", p.Description, i.Quantity, i.ProductType));
                            }

                        }
                        else
                        {
                            line.Add(i);
                        }
                    }
                    else
                    {
                        //report no inventory;
                        sb.AppendLine(string.Format("Product {0} of Quantity  {1}  for discount {2} cann't be fullfilled ", p.Description, i.Quantity, i.ProductType));

                    }

                }
            }
            if (sb.Length > 0)
            {
                MessageBox.Show(sb.ToString(), "POS Discount Processing");
            }
            MixiPack(line);
            Recalculate(line, outletId);
            return line.OrderBy(s => s.LineItemType).ThenByDescending(s => s.ProductType).ToList();

        }

        public List<MainOrderLineItem> Calculate(List<MainOrderLineItem> line, Guid outletId)
        {
            discountItem = new List<MainOrderLineItem>();
            CalculateCVCP(line.Sum(s => s.GrossAmount));
            CalculatePromotion(line);
            foreach (var i in discountItem)
            {
                var p = _productRepository.GetById(i.ProductId) as SaleProduct;
                if (p != null)
                {
                    i.Product = p;
                    line.Add(i);
                    //add returnable
                    if (p.ReturnableProduct != null)
                    {
                        var r = p.ReturnableProduct;
                        MainOrderLineItem item = null;
                        if (line.Any(a => a.ProductId == r.Id))
                        {
                            item = line.First(a => a.ProductId == r.Id);
                            item.Quantity += i.Quantity;

                        }
                        else
                        {
                            item = new MainOrderLineItem();
                            item.ProductId = r.Id;
                            item.ProductName = r.Description;
                            item.ProductType = "Retunable";
                            item.Quantity = i.Quantity;
                            item.Product = i.Product;
                            line.Add(item);
                        }

                    }
                }
            }
            MixiPack(line);
            Recalculate(line, outletId);
            return line.OrderBy(s => s.LineItemType).ThenByDescending(s => s.ProductType).ToList();


        }

        public decimal CalculateSaleDiscount(decimal totalGross, Guid outletId)
        {
           decimal SaleDiscount = 0;
            Outlet o = _outletRepository.GetById(outletId) as Outlet;
            if (o == null)
            {
                SaleDiscount = 0m;
            }
            else
            {

                SaleDiscount = _discountService.GetSalevalue(totalGross, outletId);
            }
            return SaleDiscount;
        }

        

        private void MixiPack(List<MainOrderLineItem> line)
        {
            List<ReturnableProduct> rlist = new List<ReturnableProduct>();
            foreach(var pid in line.Select(s=>s.ProductId))
            {
                ReturnableProduct r = _productRepository.GetById(pid) as ReturnableProduct;
                if (r != null)
                    rlist.Add(r);
            }
            var mixPackGroups = rlist
               .Where(n => n.ReturnAbleProduct != null)
               .GroupBy(n =>n.ReturnAbleProduct.Id);

            foreach (var group in mixPackGroups)
            {
                var bottlesId = group.Select(s => s.Id);
                decimal qty = line.Where(s=>bottlesId.Contains(s.ProductId)).Sum(n => n.Quantity);
                var returnablecontontiner = _productRepository.GetById(group.Key) as ReturnableProduct;
                if (returnablecontontiner != null)
                {
                    // ReturnableProduct rp =returnableProduct.ReturnAbleProduct;
                    int expected = (int)(qty / returnablecontontiner.Capacity);
                    decimal current = 0;
                    var firstOrDefault = line.FirstOrDefault(s => s.ProductId == returnablecontontiner.Id);
                    if (firstOrDefault != null)
                    {
                        current = firstOrDefault.Quantity;

                    }
                    if (expected > current)
                    {
                        if (firstOrDefault != null)
                        {
                            firstOrDefault.Quantity = expected;
                        }
                        else
                        {

                            line.Add(new MainOrderLineItem
                            {
                                ProductId = returnablecontontiner.Id,
                                ProductName=returnablecontontiner.Description,
                                ProductType = "Returnable",
                                Quantity = expected,
                            });
                        }
                    }
                }
            }
        }

        private void Recalculate(List<MainOrderLineItem> line, Guid outletId)
        {
            foreach (var mitem in line.Where(s=>s.LineItemType==MainOrderLineItemType.Sale))
            {
                if (mitem != null)
                {
                    LineItemPricingInfo info = _discountService.GetLineItemPricing(mitem.ProductId, mitem.Quantity, outletId);
                    mitem.UnitPrice = info.UnitPrice;
                    mitem.UnitVAT = info.VatValue;
                    mitem.TotalAmount =info.TotalPrice.GetTruncatedValue();
                    mitem.TotalNet = info.TotalNetPrice;
                    mitem.TotalVAT = info.TotalVatAmount;
                    mitem.GrossAmount = info.TotalPrice.GetTruncatedValue();
                    mitem.UnitDiscount = info.ProductDiscount;
                    mitem.TotalProductDiscount = info.TotalProductDiscount;
                }
            }
        }

        private void CalculateCVCP(decimal amount)
        {
                ProductAsDiscount productAsDiscounts = _discountService.GetFOCCertainValue(amount);
                if (productAsDiscounts != null)
                    AddProduct(productAsDiscounts.ProductId, productAsDiscounts.Quantity, "Sale (Certain value certain product)",DiscountType.CertainValueCertainProductDiscount);
           
        }
        private void CalculatePromotion(List<MainOrderLineItem> line)
        {
            foreach (var item in line)
            {
                List<ProductAsDiscount> productAsDiscounts = _discountService.GetFOCCertainProduct(item.ProductId, item.Quantity);
                foreach (ProductAsDiscount productAsDiscount in productAsDiscounts)
                {
                    if (productAsDiscount != null)
                        AddProduct(productAsDiscount.ProductId, productAsDiscount.Quantity, "Sale (Promotion)",DiscountType.PromotionDiscount);
                }
            }
        }

        private void AddProduct(Guid productId, decimal quantity, string description, DiscountType discountType)
        {
           
               
                if(discountItem.Any(s=>s.ProductId==productId))
                {
                    var item = discountItem.First(s => s.ProductId == productId);
                    item.Quantity+=quantity;
                }else
                {
                    Product p = _productRepository.GetById(productId);
                    MainOrderLineItem item = new MainOrderLineItem();
                    item.ProductId = p.Id;
                    item.ProductName = p.Description;
                    item.ProductType = description;
                    item.DiscountType = discountType;
                    item.Quantity =quantity;
                    item.LineItemType =MainOrderLineItemType.Discount;
                    discountItem.Add(item);
                }

           
        }
    }
}