using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Workflow;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities
{

    /// <summary>
    /// Need to store all line items from confirmation of order.
    /// Once confirmed then line items are cloned and marked as post confirmation
    /// </summary>
    public class MainOrder : BaseOrder
    {
        private MainOrder(Guid id)
            : base(id)
        {
            
            SubOrders = new List<SubOrder>();
            _paymentInf = new List<PaymentInfo>();
        }
        public void AddPayment(PaymentInfo paymentInfo)
        {
            _paymentInf.Add(paymentInfo);
        }
        public void ChangeccId(Guid ccId)
        {
            foreach (var subOrder in SubOrders)
            {
                subOrder.DocumentIssuerCostCentreApplicationId = ccId;
            }
            this.DocumentIssuerCostCentreApplicationId = ccId;
        }
        public void ChangeSaleman(DistributorSalesman salesman)
        {
            
            if (this.DocumentIssuerCostCentre is DistributorSalesman)
                this.DocumentIssuerCostCentre = salesman;
            else
                this.DocumentRecipientCostCentre = salesman;
          
            foreach (var subOrder in SubOrders)
            {
                CostCentre recepient = null;
                if (subOrder.DocumentIssuerCostCentre is DistributorSalesman)
                    subOrder.DocumentIssuerCostCentre = salesman;
                else
                    subOrder.DocumentRecipientCostCentre = salesman;
                
            }
          

        }
       
        private List<PaymentInfo> _paymentInf;
        public List<PaymentInfo> GetPayments
        {
            get { return _paymentInf; }
        }

        private void AddSubOrder(SubOrder subOrder)
        {
            SubOrders.Add(subOrder);
        }
        public bool IsEditable()
        {
           
            if (SubOrders.All(o => o.Status == DocumentStatus.Confirmed) && SubOrders.Count == 1)
                return true;
            return false;
        }
        public bool IsApprovable()
        {

            if (SubOrders.Any(o => o.Status == DocumentStatus.Confirmed))
                return true;
            return false;
        }
        private List<SubOrder> SubOrders { set; get; }
        public  OrderStatus OrderStatus { internal set; get; }
       

        public override void AddLineItem(SubOrderLineItem orderLineItem)
        {
            SubOrder order = SubOrders.Where(s => s.Status == DocumentStatus.New).First();
            order.AddLineItem(orderLineItem);
        }

        public  void AddOrderPaymentInfoLineItem(PaymentInfo paymentInfo)
        {
            SubOrder order = SubOrders.First();
            order.AddOrderPaymentInfoLineItem(paymentInfo);
        }


        public override void ApproveLineItem(SubOrderLineItem orderLineItem, decimal quantity, bool takeTheRestToLossSale)
        {
           
            SubOrder order = SubOrders.First(s => s.Status == DocumentStatus.Confirmed && s.LineItems.Any(n=> n.Id==orderLineItem.Id));
            order.ApproveLineItem(orderLineItem, quantity, takeTheRestToLossSale);
        }

        public override void ApproveLineItem(SubOrderLineItem orderLineItem)
        {
            SubOrder order = SubOrders.First(s => s.Status == DocumentStatus.Confirmed && s.LineItems.Any(n => n.Id == orderLineItem.Id));
            order.ApproveLineItem(orderLineItem);
        }
        public  void ApprovePLineItem(SubOrderLineItem orderLineItem)
        {
            SubOrder order = SubOrders.First(s =>  s.LineItems.Any(n => n.Id == orderLineItem.Id));
            order.ApproveLineItem(orderLineItem);
        }
        public override void RemoveLineItem(SubOrderLineItem orderLineItem)
        {
            SubOrder order = SubOrders.Where(s => s.Status == DocumentStatus.Confirmed).First();
            order.RemoveLineItem(orderLineItem);
        }

        public override void EditLineItem(SubOrderLineItem orderLineItem)
        {
            SubOrder order = SubOrders.Where(s => s.Status == DocumentStatus.Confirmed).First();
            order.EditLineItem(orderLineItem);
        }
        public List<LineItemSummary> ItemSummary
        {
            get
            {
              var list= SubOrders.SelectMany(s => s.LineItems).Where(s=>s.LineItemStatus!=MainOrderLineItemStatus.Removed).ToList();
                var productIds = list.Select(n => new {n.Product.Id,n.LineItemType}).Distinct();
                return productIds.Select(n => ProductSummary(list, n.Id, n.LineItemType)).ToList();
              
            }

        }
        LineItemSummary ProductSummary(List<SubOrderLineItem> list, Guid productId,MainOrderLineItemType type)
        {
            var fl = list.Where(l => l.Product.Id == productId && l.LineItemType==type).ToList();
            SubOrderLineItem lineItem = fl.First();
            var lis = new LineItemSummary { Product =lineItem.Product,LineItemType=type };
            lis.Qty = fl.Max(l => l.Qty);
            var fl1 = fl.Where(f => new[] { MainOrderLineItemStatus.Approved, MainOrderLineItemStatus.Dispatched }.Contains(f.LineItemStatus));
            lis.DispachedQuantity = fl1.Where(x => x.LineItemStatus == MainOrderLineItemStatus.Dispatched).Sum(f => f.DispatchedQuantity);
            lis.LostSaleQuantity = fl1.Sum(f => f.LostSaleQuantity);
            lis.ApprovedQuantity = fl1.Where(x => x.LineItemStatus == MainOrderLineItemStatus.Approved || x.LineItemStatus==MainOrderLineItemStatus.Dispatched).Sum(f => f.ApprovedQuantity);
            lis.ProductDiscount = fl.Max(l => l.ProductDiscount);
            if (fl1.Any())
                lis.BackOrderQuantity = fl1.Min(n => n.BackOrderQuantity);
            lis.Value = fl.First().Value;
            lis.VatValue = fl.First().LineItemVatValue;
           
            return lis;
            
        }
        public  List<SubOrderLineItem> PendingConfirmationLineItems
        {
            get
            {
              return  SubOrders.Where(o=>o.Status==DocumentStatus.New).SelectMany(s => s.LineItems).ToList();
                
            }
           
        }
        public  List<SubOrderLineItem> PendingApprovalLineItems
        {
            get
            {
             return SubOrders.Where(s=>s.Status==DocumentStatus.Confirmed).SelectMany(s => s.LineItems).ToList();
               
            }
           
        }
        public  List<SubOrderLineItem> PendingDispatchLineItems
        {
            get
            {
                return SubOrders.Where(o => o.Status == DocumentStatus.Approved ).SelectMany(s => s.LineItems).Where(s => s.LineItemStatus == MainOrderLineItemStatus.Approved).ToList();
            }
            
        }
        public List<SubOrderLineItem>  DispatchedLineItems
        {
            get
            {
                return SubOrders.SelectMany(s => s.LineItems).Where(s => s.LineItemStatus == MainOrderLineItemStatus.Dispatched).ToList();
            }
            
        }
        public List<SubOrderLineItem> LostLineItems
        {
            get
            {
                return SubOrders.Where(o => o.Status == DocumentStatus.Cancelled).SelectMany(s => s.LineItems).ToList();
            }

        }


        public decimal PaidAmount { get; set; }
        public string ExternalDocumentReference { get; set; }
        public decimal OutstandingAmount { get; set; }
       
        public override decimal TotalNet
        {
            get
            {
                return SubOrders.SelectMany(s => s.LineItems).Where(s => s.LineItemStatus != MainOrderLineItemStatus.Removed).Sum(s => (s.Qty - s.BackOrderQuantity) * s.Value);

            }
        }
        public override decimal TotalVat
        {
            get
            {
                return SubOrders.SelectMany(s => s.LineItems).Where(s => s.LineItemStatus != MainOrderLineItemStatus.Removed).Sum(s => (s.Qty - s.BackOrderQuantity) * s.LineItemVatValue);
            }
        }
        public override decimal TotalGross
        {
            get { return TotalVat+TotalNet; }
        }

        public override decimal TotalDiscount
        {
            get { return SubOrders.SelectMany(s => s.LineItems).Where(s=>s.LineItemStatus!=MainOrderLineItemStatus.Removed).Sum(s => (s.Qty - s.BackOrderQuantity)*s.ProductDiscount); }
        }

        public override void Confirm()
        { 
            OrderStatus=OrderStatus.Inprogress;
            SubOrder order = SubOrders.Where(s => s.Status == DocumentStatus.New).First();
            
            order.Confirm();
        }

        public override void Approve()
        {
            OrderStatus = OrderStatus.Inprogress;
            SubOrder order = SubOrders.Where(s => s.Status == DocumentStatus.Confirmed).First();
            order.Approve();
        }
        public  void ApproveP()
        {
            OrderStatus = OrderStatus.Inprogress;
            SubOrder order = SubOrders.First();
            order.Status = DocumentStatus.Confirmed;
            order.Approve();
        }
        public  void DispatchPendingLineItems()
        {
            OrderStatus = OrderStatus.Inprogress;
            foreach (var subOrder in SubOrders.Where(s => s.Status == DocumentStatus.Approved))
            {
                subOrder.DispatchPendingLineItems();
            }
           
        }
        public override void Reject()
        {
            OrderStatus = OrderStatus.Inprogress;
            SubOrder order = SubOrders.Where(s => s.Status == DocumentStatus.Confirmed).First();
            order.Reject();
        }

        public override void Close()
        {
            OrderStatus = OrderStatus.Inprogress;
            foreach (var order in SubOrders)
            {
                order.Close();

            }
           
        }
        public class LineItemSummary 
        {
            public LineItemSummary()
            {
            }
            public Product Product { get; set; }
            public decimal Qty { get; set; }
            public decimal Value { get; set; }
            public decimal ProductDiscount { get; set; }
            public decimal ApprovedQuantity { get; set; }
            public decimal DispachedQuantity { get; set; }
            public decimal BackOrderQuantity { get; set; }
            public decimal LostSaleQuantity { get; set; }
            public MainOrderLineItemType LineItemType { get; set; }
            public decimal TotalNet
            {
                get { return Qty*Value; }
            }
            
            public decimal VatValue { get; set; }
            public decimal TotalVat { get { return Qty*VatValue; } }
            public decimal TotalGross { get { return (TotalNet + TotalVat); } } 
        }



        
    }


    public interface IPagedDocumentList<T> : IList<T>
    {
        int TotalItemCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        int PageCount { get; }
        bool HasPrevPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }


    }

    public class PagedDocumentList<T> :List<T>, IPagedDocumentList<T>
    {
        
        public PagedDocumentList(int totalItemCount, int pageNumber,int pageSize)
        {
            TotalItemCount = totalItemCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            PageCount  = (int)Math.Ceiling(TotalItemCount / (double)PageSize);

            HasPrevPage = PageNumber > 0;
            HasNextPage = (PageNumber <= (PageCount - 1));
            IsFirstPage = (PageNumber <= 0);
            IsLastPage = (PageNumber >= (PageCount - 1));


        }
        
        public int TotalItemCount { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int PageCount { get; private set; }

        public bool HasPrevPage { get; private set; }
        public bool HasNextPage { get; private set; }
        public bool IsFirstPage { get; private set; }
        public bool IsLastPage { get; private set; }
    }

    public enum MainOrderSummaryStatus
    {
        PendingApproval ,
        PendingDispatch ,
        DispatchedOrders,
        Incomplete ,
        Delivered ,
        PartiallyPaidDeliveries,
        FullyPaidDeliveries ,
        BackOrders ,
        Rejected ,
        LostSales 

    }

}
