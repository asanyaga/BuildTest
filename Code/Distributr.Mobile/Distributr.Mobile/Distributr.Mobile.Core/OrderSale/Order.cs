using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Products;
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public enum ProcessingStatus
    {
        Created, Submitted, Approved, Deliverable, PartiallyFulfilled, Confirmed, Rejected
    }

    [Table("OrderOrSale")]
    public class Order : MasterEntity
    {
        public Order() : base(default(Guid))
        {
        }

        public Order(Guid guid, Outlet outlet) : base(guid)
        {
            LineItems = new List<ProductLineItem>();
            Payments = new List<Payment>();
            Outlet = outlet;
            ShipToAddress = string.Empty;
            _DateCreated = DateTime.Now;
            InvoiceId = Guid.NewGuid();
        }

        [ForeignKey(typeof(Outlet))]
        public Guid OutletMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Outlet Outlet { get; set; }

        public virtual OrderType OrderType { get; set; }
        public string OrderReference { get; set; }
        public string ShipToAddress { get; set; }
        public string Note { get; set; }
        public decimal SaleDiscount { get; set;}
        public Guid InvoiceId { get; set; }

        //Values are raised to nearest full shlling
        public decimal TotalValueIncludingVat
        {
            get { return Math.Ceiling(AllItems.Sum(item => item.TotalLineItemValueInculudingVat)); }
        }

        public decimal BalanceOutstanding
        {
            get { return TotalValueIncludingVat - TotalPayments; }
        }

        public bool IsFullyPaid
        {
            get { return BalanceOutstanding == 0; }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<ProductLineItem> LineItems { get; set; }

        [Ignore]
        public List<BaseProductLineItem> AllInvoiceItems
        {
            get
            {
                var result = new List<BaseProductLineItem>();
                LineItems.ForEach(l =>
                {
                    result.Add(l);
                    if (l.ContainerReturnable != null && l.ContainerReturnable.SaleQuantity > 0) result.Add(l.ContainerReturnable);
                    if (l.ItemReturnable != null && l.ItemReturnable.SaleQuantity > 0) result.Add(l.ItemReturnable);
                });
                return result;
            }
        }

        [Ignore]
        public List<BaseProductLineItem> AllItems
        {
            get
            {
                var result = new List<BaseProductLineItem>();
                LineItems.ForEach(l =>
                {
                    result.Add(l);
                    if (l.ContainerReturnable != null) result.Add(l.ContainerReturnable);
                    if (l.ItemReturnable != null) result.Add(l.ItemReturnable);
                });
                return result;
            }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Payment> Payments { get; set; }

        public decimal TotalPayments
        {
            get { return Payments.Sum(payment => payment.Amount); }
        }

        public decimal TotalValue
        {
            get { return AllItems.Sum(item => item.Value); }
        }

        public decimal TotalVatValue
        {
            get { return AllItems.Sum(item => item.VatValue); }
        }

        public string InvoiceReference { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; }

        public bool ContainsProduct(Guid productMasterId)
        {
            return LineItems.Find(l => l.ProductMasterId == productMasterId) != null;
        }

        //For Order, we always include returnables which the Hub needs as part of Envelope
        public void AddOrUpdateOrderLineItem(ProductWrapper productWrapper)
        {
            var product = productWrapper.SaleProduct;
            var totalQuantity = productWrapper.EachQuantity + (productWrapper.CaseQuantity * product.ContainerCapacity);
            var lineItem = LineItems.Find(item => item.ProductMasterId == product.Id);
            if (lineItem != null)
            {
                LineItems.Remove(lineItem);                
            }

            if (totalQuantity < 1) return;

            var price = product.ProductPrice(Outlet.OutletProductPricingTier);
            var vatRate = product.VATClass.CurrentRate;

            lineItem = AddLineItem(Guid.NewGuid(), product, productWrapper.MaxQuantity, totalQuantity, productWrapper.EachQuantity, productWrapper.CaseQuantity, price, vatRate);
            lineItem.Available = productWrapper.MaxQuantity;

            if (product.ReturnableProduct != null && totalQuantity > 0)
            {
                var itemReturnablePrice = product.ReturnableProduct.ProductPrice(Outlet.OutletProductPricingTier);

                lineItem.ItemReturnable = CreateReturnableLineItem(product.ReturnableProduct,
                    product.ReturnableProduct.Id, 
                    itemReturnablePrice,
                    totalQuantity, 
                    productWrapper.MaxEachReturnableQuantity);
            }
                
            if (product.ReturnableContainer != null && productWrapper.CaseQuantity > 0)
            {
                var containerPrice = product.ReturnableContainer.ProductPrice(Outlet.OutletProductPricingTier);
                
                lineItem.ContainerReturnable = CreateReturnableLineItem(product.ReturnableContainer,
                    product.ReturnableContainer.Id, 
                    containerPrice, 
                    productWrapper.CaseQuantity, 
                    productWrapper.MaxCaseReturnableQuantity);
            }
                
        }
       
        public void AddOrUpdateSaleLineItem(ProductWrapper productWrapper)
        {
            var product = productWrapper.SaleProduct;
            var quantity = productWrapper.EachQuantity + (productWrapper.CaseQuantity * product.ContainerCapacity);
            var lineItem = LineItems.Find(item => item.ProductMasterId == product.Id);
            if (lineItem != null)
            {
                if (quantity == 0)
                {
                    LineItems.Remove(lineItem);
                    return;
                }

                lineItem.Quantity = quantity;
                lineItem.EachQuantity = productWrapper.EachQuantity;
                lineItem.CaseQuantity = productWrapper.CaseQuantity;
                lineItem.Available = productWrapper.MaxQuantity;
            }
            else
            {
                if (quantity == 0) return;
                var price = product.ProductPrice(Outlet.OutletProductPricingTier);
                var vatRate = product.VATClass.CurrentRate;

                lineItem = AddLineItem(Guid.NewGuid(), product, productWrapper.MaxQuantity, quantity, productWrapper.EachQuantity, productWrapper.CaseQuantity, price, vatRate);
            }

            AddOrUpdateReturnables(productWrapper, product, lineItem);
        }

        private void AddOrUpdateReturnables(ProductWrapper productWrapper, SaleProduct product, ProductLineItem lineItem)
        {
            if (productWrapper.EachReturnableQuantity > 0 && product.ReturnableProduct != null)
            {
                var itemReturnablePrice = product.ReturnableProduct.ProductPrice(Outlet.OutletProductPricingTier);

                if (lineItem.ItemReturnable == null)
                {
                    lineItem.ItemReturnable = CreateReturnableLineItem(product.ReturnableProduct,
                        product.ReturnableProduct.Id,
                        itemReturnablePrice,
                        productWrapper.EachReturnableQuantity,
                        productWrapper.MaxEachReturnableQuantity);
                    lineItem.ItemReturnable.SaleQuantity = productWrapper.EachReturnableQuantity;
                    lineItem.ItemReturnable.ApprovedQuantity = productWrapper.MaxEachReturnableQuantity;
                }
                else
                {
                    lineItem.ItemReturnable.Product = product.ReturnableProduct;
                    lineItem.ItemReturnable.Price = itemReturnablePrice;
                    lineItem.ItemReturnable.SaleQuantity = productWrapper.EachReturnableQuantity;
                }
            }
            else
            {
                if (lineItem.ItemReturnable != null)
                {
                    lineItem.ItemReturnable.SaleQuantity = 0;
                }
            }

            if (productWrapper.CaseReturnableQuantity > 0 && product.ReturnableContainer != null)
            {
                var containerPrice = product.ReturnableContainer.ProductPrice(Outlet.OutletProductPricingTier);
                if (lineItem.ContainerReturnable == null)
                {
                    lineItem.ContainerReturnable = CreateReturnableLineItem(product.ReturnableContainer,
                        product.ReturnableContainer.Id,
                        containerPrice,
                        productWrapper.CaseReturnableQuantity,
                        productWrapper.MaxCaseReturnableQuantity);

                    lineItem.ContainerReturnable.ApprovedQuantity = productWrapper.MaxCaseReturnableQuantity;
                    lineItem.ContainerReturnable.SaleQuantity = productWrapper.CaseReturnableQuantity;
                }
                else
                {
                    lineItem.ContainerReturnable.Product = product.ReturnableContainer;
                    lineItem.ContainerReturnable.Price = containerPrice;
                    lineItem.ContainerReturnable.SaleQuantity = productWrapper.CaseReturnableQuantity;
                }
            }
            else
            {
                if (lineItem.ContainerReturnable != null)
                {
                    lineItem.ContainerReturnable.SaleQuantity = 0;
                }
            }            
        }
       
        public ReturnableProductLineItem CreateReturnableLineItem(ReturnableProduct product, Guid productMasterId, decimal price, decimal quantity, decimal availableQuantity)
        {
            return new ReturnableProductLineItem (Id)
            {
                Product = product,
                Quantity = quantity,
                Available = availableQuantity,
                Price = price,
                LineItemStatus = LineItemStatus.New,
                ProductMasterId = productMasterId
            };
        }

        public ProductLineItem AddLineItem(Guid lineItemId, SaleProduct product, decimal available, decimal quantity, decimal eachQuantity, decimal caseQuantity, decimal price, decimal vatRate)
        {
            var lineItem = new ProductLineItem(lineItemId, Id)
            {
                Product = product,
                Quantity = quantity,
                EachQuantity = eachQuantity,
                CaseQuantity = caseQuantity,
                Available = available,
                Price = price,
                VatRate = vatRate,
                ProductMasterId = product.Id,
                LineItemStatus = LineItemStatus.New
            };

            LineItems.Add(lineItem);
            return lineItem;
        }

        public void AddCashPayment(string reference, decimal amount)
        {
            var payment = new Payment(Id)
            {
                PaymentMode = PaymentMode.Cash, 
                PaymentReference = reference, 
                PaymentStatus = PaymentStatus.New,
                Amount = amount
            };

            Payments.Add(payment);
        }

        public void AddChequePayment(string chequeNumber, decimal amount, Bank bank, BankBranch bankBranch, DateTime dueDate)
        {            
            var payment = new Payment(Id)
            {
                PaymentMode = PaymentMode.Cheque,
                PaymentReference = string.Format("{0} - {1} - {2}", chequeNumber, bank.Code, bankBranch.Code),
                Amount = amount,
                Bank = bank.Code, 
                BankBranch = bankBranch.Code,
                PaymentStatus = PaymentStatus.New,
                DueDate = dueDate
            };

            Payments.Add(payment);
        }

        public List<Payment> NewPayments
        {
            get { return Payments.Where(p => p.PaymentStatus == PaymentStatus.New).ToList(); }
        }

        public IEnumerable<Payment> CashPayments()
        {
            return Payments.Where(p => p.PaymentMode == PaymentMode.Cash);
        }

        public void ConfirmAllLineItems()
        {
            AllItems.ForEach(l =>
            {
                l.LineItemStatus = LineItemStatus.Confirmed;
                l.ApprovedQuantity = 0;
                l.ConfirmedQuantity = l.Quantity;
            });
        }

        public List<BaseProductLineItem> ApprovedLineItems
        {
            get
            {
                return AllItems.Where(l => l.LineItemStatus == LineItemStatus.Approved).ToList();
            }
        }

        public void ApproveNewLineItems()
        {
            AllItems.Where(l => l.LineItemStatus == LineItemStatus.New)
            .ToList()
            .ForEach(l =>
            {
                l.ApprovedQuantity = l.Quantity;
                l.LineItemStatus = LineItemStatus.Approved;
            });
        }

        public void ConfirmApprovedLineItems()
        {
            ApprovedLineItems.ForEach(l =>
            {
                l.LineItemStatus = LineItemStatus.Confirmed;
                l.ConfirmedQuantity += l.ApprovedQuantity;
                l.ApprovedQuantity = 0;
            });
        }

        public void ConfirmNewPayments()
        {
            NewPayments.ForEach(p => p.PaymentStatus = PaymentStatus.Confirmed);
        }

        public bool IsClosable()
        {
            return IsFullyPaid && HasNoBackorderItems;
        }

        public bool HasNoBackorderItems
        {
            get 
            {
                return LineItems.All(item => item.Quantity == item.ApprovedQuantity + item.ConfirmedQuantity);
            }            
        }

        public IEnumerable<Payment> ConfirmedPayments
        {
            get { return Payments.Where(p => p.PaymentStatus == PaymentStatus.Confirmed); }
        }

        public ProductWrapper ItemAsProductWrapper(Guid productId)
        {
            var lineItem = LineItems.First(l => l.ProductMasterId == productId);
            var tier = Outlet.OutletProductPricingTier;

            return new ProductWrapper()
            {
                MasterId = productId,
                SaleProduct = lineItem.Product,
                Description = lineItem.Product.Description,
                EachQuantity = lineItem.EachQuantity,
                MaxEachQuantity = lineItem.Available,
                CaseQuantity = lineItem.CaseQuantity,
                MaxCaseQuantity = Math.Floor(lineItem.Available / lineItem.Product.ContainerCapacity),
                Price = lineItem.Price,
                EachReturnableQuantity = lineItem.ItemReturnable == null ? 0 : lineItem.ItemReturnable.SaleQuantity,
                CaseReturnableQuantity = lineItem.ContainerReturnable == null ? 0 : lineItem.ContainerReturnable.SaleQuantity,
                EachReturnablePrice = lineItem.Product.ReturnableProduct.ProductPrice(tier),
                CaseReturnablePrice = lineItem.Product.ReturnableContainer.ProductPrice(tier),
            };
        }
    }    
}

