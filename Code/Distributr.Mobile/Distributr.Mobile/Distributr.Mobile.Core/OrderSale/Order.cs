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
            ReturnableLineItems = new List<ReturnableLineItem>();
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

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<ReturnableLineItem> ReturnableLineItems { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<ProductLineItem> LineItems { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public List<Payment> Payments { get; set; }

        public string InvoiceReference { get; set; }
        public ProcessingStatus ProcessingStatus { get; set; }

        public virtual OrderType OrderType { get; set; }
        public string OrderReference { get; set; }
        public string ShipToAddress { get; set; }
        public string Note { get; set; }
        public decimal SaleDiscount { get; set;}
        public Guid InvoiceId { get; set; }


        //Values are raised to nearest full shlling
        public decimal TotalValueIncludingVat
        {
            get { return Math.Ceiling(AllInvoiceItems.Sum(item => item.TotalLineItemValueInculudingVat)); }
        }

        public decimal BalanceOutstanding
        {
            get { return TotalValueIncludingVat - TotalPayments; }
        }

        public bool IsFullyPaid
        {
            get { return BalanceOutstanding == 0; }
        }

        [Ignore]
        public List<BaseProductLineItem> AllInvoiceItems
        {
            get
            {
                var result = new List<BaseProductLineItem>();
                result.AddRange(LineItems);
                result.AddRange(ReturnableLineItems.Where(l => l.LineItemStatus == LineItemStatus.Approved && l.SaleQuantity > 0));
                return result;
            }
        }

        [Ignore]
        public List<BaseProductLineItem> AllItems
        {
            get
            {
                var result = new List<BaseProductLineItem>();
                result.AddRange(LineItems);
                result.AddRange(ReturnableLineItems);
                return result;
            }
        }

        [Ignore]
        public List<BaseProductLineItem> ApprovedLineItems
        {
            get 
            {
                return AllItems.Where(l => l.LineItemStatus == LineItemStatus.Approved).ToList();
            }            
        }

        public decimal TotalPayments
        {
            get { return Payments.Sum(payment => payment.Amount); }
        }

        public decimal TotalValue
        {
            get { return AllInvoiceItems.Sum(item => item.Value); }
        }

        public decimal TotalVatValue
        {
            get { return AllInvoiceItems.Sum(item => item.VatValue); }
        }

        //This method is called when constructing the order from commands as part of a DB rebuild or when the Hub modified the order
        public ProductLineItem AddItem(Guid lineItemId, SaleProduct product, Guid saleProductId, decimal quantity, decimal price, decimal vatRate)
        {
            var item = new ProductLineItem(lineItemId, Id)
            {
                Quantity = quantity,
                Product = product,
                ProductMasterId = saleProductId,
                Price = price,
                VatRate = vatRate
            };

            LineItems.Add(item);

            return item;
        }

        //This method is used when the user adds an item via the UI
        public ProductLineItem AddItem(SaleProduct product, decimal quantity)
        {
            var item = AddItem(Guid.NewGuid(), product, product.Id, quantity, PriceFor(product), VatRateFor(product));

            if (product.ReturnableProduct != null)
            {
                AddReturnableItem(Guid.NewGuid(), product.ReturnableProduct, product.ReturnableProduct.Id, quantity, PriceFor(product.ReturnableProduct));
            }

            var caseQuantity = CaseQuantityFor(product, quantity);

            if (product.ReturnableContainer != null && caseQuantity > 0)
            {
                AddReturnableItem(Guid.NewGuid(), product.ReturnableContainer, product.ReturnableContainer.Id, caseQuantity, PriceFor(product.ReturnableContainer));
            }

            return item;
        }

        public void RemoveItem(Guid lineItemId, bool removeReturnables = true)
        {
            var item = LineItems.Find(i => i.Id == lineItemId);

            LineItems.Remove(item);

            if (removeReturnables) RemoveReturnables(item);
        }

        public ReturnableLineItem AddReturnableItem(Guid lineItemId, ReturnableProduct product, Guid returnableProductId, decimal quantity, decimal price)
        {
            var item = new ReturnableLineItem(lineItemId, Id)
            {
                Quantity = quantity,
                Product = product,
                ProductMasterId = returnableProductId,
                Price = price
            };

            ReturnableLineItems.Add(item);
            return item;
        }

        protected void RemoveReturnables(ProductLineItem item)
        {
            var eachReturnable = ReturnableLineItems
                .First(i => i.ProductMasterId == item.Product.ReturnableProductMasterId && i.Quantity == item.Quantity);

            ReturnableLineItems.Remove(eachReturnable);

            var caseQuantity = CaseQuantityFor(item.Product, item.Quantity);
            if (caseQuantity > 0)
            {
                var caseReturnable = ReturnableLineItems
                    .First(i => i.ProductMasterId == item.Product.ReturnableContainerMasterId && i.Quantity == caseQuantity);
                ReturnableLineItems.Remove(caseReturnable);
            }
        }

        public decimal CaseQuantityFor(SaleProduct product, decimal eachQuantity)
        {
            return Math.Floor(eachQuantity / product.ContainerCapacity);
        }

        protected decimal VatRateFor(Product product)
        {
            return product.VATClass.CurrentRate;
        }

        protected decimal PriceFor(Product product)
        {
            return product.ProductPrice(Outlet.OutletProductPricingTier);
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
                return LineItems.All(item => item.Quantity == item.SaleQuantity);
            }            
        }

        public IEnumerable<Payment> ConfirmedPayments
        {
            get { return Payments.Where(p => p.PaymentStatus == PaymentStatus.Confirmed); }
        }

        public ProductLineItem FindLineItem(Guid productId)
        {
            return LineItems.Find(l => l.ProductMasterId == productId);
        }
    }    
}

