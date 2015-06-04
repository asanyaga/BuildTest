using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.OrderSale
{
    public class OrderFilter
    {
        public OrderFilter()
        {
            ProcessingStatuses = new List<OrderStatusItem>()
            {
                new OrderStatusItem(ProcessingStatus.Created, "Created", true),
                new OrderStatusItem(ProcessingStatus.Submitted, "Submitted", true),
                new OrderStatusItem(ProcessingStatus.Approved, "Approved", true),
                new OrderStatusItem(ProcessingStatus.Deliverable, "Ready for Delivery", true),
                new OrderStatusItem(ProcessingStatus.PartiallyFulfilled, "Partially Fulfilled", true),
                new OrderStatusItem(ProcessingStatus.Confirmed, "Confirmed", true),
                new OrderStatusItem(ProcessingStatus.Rejected, "Rejected", true),
            };

            PaymentStatuses = new List<PaymentStatusItem>()
            {
                
            };
            EndDate = DateTime.Now;
            StartDate = new DateTime(EndDate.Year, EndDate.Month, 1);
        }

        public List<OrderStatusItem> ProcessingStatuses { get; set; }
        public List<PaymentStatusItem> PaymentStatuses { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<ProcessingStatus> SelectedProcessingStatuses
        {
            get
            {
                return ProcessingStatuses.Where(s => s.Selected)
                    .Select(s => s.ProcessingStatus)
                    .ToList();
            }
        }
    }

    public class PaymentStatusItem
    {
        public PaymentStatusItem(PaymentStatus paymentStatus, string displayName, bool selected)
        {
            PaymentStatus = paymentStatus;
            DisplayName = displayName;
            Selected = selected;
        }

        public PaymentStatus PaymentStatus { get; private set; }
        public string DisplayName { get; private set; }
        public bool Selected { get; set; }
    }

    public class OrderStatusItem
    {
        public OrderStatusItem(ProcessingStatus processingStatus, string displayName, bool selected)
        {
            ProcessingStatus = processingStatus;
            DisplayName = displayName;
            Selected = selected;
        }

        public ProcessingStatus ProcessingStatus { get; private set; }
        public string DisplayName { get; private set; }
        public bool Selected { get; set; }
    }
}