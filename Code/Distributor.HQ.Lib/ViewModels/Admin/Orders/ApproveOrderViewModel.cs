using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModels.Admin.Orders
{
    public class ApproveOrderViewModel
    {
        public ApproveOrderViewModel()
        {
            LineItems = new List<ApproveOrderLineItemViewModel>();
        }
        public bool CanEdit = false;
        public string DocumentId {get;set;}
        public string DocumentReference { get; set; }
        public string DocumentIssuerCostCentre { get; set; }
        public string DocumentRecipientCostCentre { get; set; }
        public string DocumentIssuerUser { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentDateIssued { get; set; }
        public string DateRequired { get; set; }

        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal OrderTotal { get; set; }

        public string Reason { get; set; }

        public List<ApproveOrderLineItemViewModel> LineItems { get; set; }

        public class ApproveOrderLineItemViewModel
        {
            public string LineItemId { get; set; }
            public string DocumentId { get; set; }
            public Guid ProductId { get; set; }
            public string ProductDesc { get; set; }
            public decimal Qty { get; set; }
            public decimal Value { get; set; }
            public decimal VatValue { get; set; }
            public decimal TotalNet { get; set; }
            public decimal TotalVat { get { return Qty*VatValue; } }
            public decimal LineTotal { get; set; }
            public string ProductType { get; set; }

        }

    }
    public class OrderLineItemViewModel
    {
        public string LineItemId { get; set; }
        public string DocumentId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductDesc { get; set; }
        public string Qty { get; set; }
        public string Value { get; set; }
        public string VatValue { get; set; }
        public string TotalNet { get; set; }
        public string TotalVat { get; set; }
        public string LineTotal { get; set; }
        public string ProductType { get; set; }

    }
}
