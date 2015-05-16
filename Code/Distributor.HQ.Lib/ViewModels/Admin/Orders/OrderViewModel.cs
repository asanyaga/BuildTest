using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

using MvcContrib.Pagination;
using Distributr.HQ.Lib.Paging;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.HQ.Lib.ViewModels.Admin.Orders
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            orderViewModelLineItemVm = new List<OrderLineItemViewModel>();
            CurrentPage = 1;
            PageSize = 10;
            Items = new List<OrderViewModelItem>();
            
        }
        public Guid id { get; set; }
        public string isseuedOnBehalf { get; set; }
        public DocumentStatus status { get; set; }
        public decimal net { get; set; }
        public decimal vat { get; set; }
        public decimal gross { get; set; }
        public string discountName { get; set; }
        public Guid productId { get; set; }
        public string productCode { get; set; }
        public decimal quantity { get; set; }
        public string productName { get; set; }
        public string OrderReference { get; set; }
        public string outletCode { get; set; }
        public DateTime orderDate { get; set; }
        public string documentReference { get; set; }
        public int PageIndex { get; private set; }
        public string ProductType { get; set; }
        public Guid? distributor { get; set; }
        
        public IPagination<OrderViewModel> orderPagedList { get; set; }
        //Import Properties
        //public string outletCode { get; set; }
        public string salesManCode { get; set; }
        public string distributorCode { get; set; }
        public string ErrorText { get; set; }
       
        public List<OrderLineItemViewModel> orderViewModelLineItemVm { get; set; }

        public class OrderLineItemViewModel
        {
            public Guid productId { get; set; }
            public int quantity { get; set; }
            public int LineItemSequenceNo { get; set; }
            public string Description { get; set; }
            public string OrderType { get; set; }
            public string productCode { get; set; }
            public decimal Value { get; set; }
            
            public decimal LineItemVatValue { get; set; }

            public string LineItemType { get; set; }

            public decimal LineItemVatTotal
            {
                get { return quantity * LineItemVatValue; }
            }

            public decimal LineItemTotal
            {
                get
                {
                    return LineItemVatTotal + (quantity * Value);
                }
            }
        }

       public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int NoPages
        {
            get
            {
                int totalPages = (int)Math.Ceiling((double)Items.Count() / (double)PageSize);
                return totalPages;
            }
        }
        public List<OrderViewModelItem> CurrentPageItems
        {
            get
            {
                return Items;
                //return Items.Distinct().OrderByDescending(d => d.orderDate.Date).ToList();//.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            }
        }
        public List<OrderViewModelItem> Items { get; set; }
        public int CurrentPage { get; set; }
        public class OrderViewModelItem
        {
            public string isseuedOnBehalf { get; set; }
            public DocumentStatus status { get; set; }
            public decimal net { get; set; }
            public decimal vat { get; set; }
            public decimal gross { get; set; }
            public string documentReference { get; set; }
            public DateTime orderDate { get; set; }
            public Guid id { get; set; }
        }
    }
    //public class OrderlineItemVM
    //{
    //    public OrderlineItemVM(Guid id)
    //    {

    //    }
    //    public int productId { get; set; }
    //    public int quantity { get; set; }
    //    public decimal Value { get; set; }

    //    public decimal LineItemVatValue { get; set; }
    //    public OrderLineItemType LineItemType { get; set; }

    //    public decimal LineItemVatTotal
    //    {
    //        get { return quantity * LineItemVatValue; }
    //    }

    //    public decimal LineItemTotal
    //    {
    //        get
    //        {
    //            return LineItemVatTotal + (quantity * Value);
    //        }
    //    }
    //}
}
