using System;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities
{
    public class MainOrderSummary
    {
       
        public Guid OrderId { get; set; }
        public string OrderReference { get; set; }
        public string ExternalRefNo { get; set; }
        public DateTime Required { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalVat { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal SaleDiscount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DocumentStatus Status { get; set; }
        public string Salesman { get; set; }
        public Guid OutletId { get; set; }
        public Guid SalesmanId { get; set; }
        public string Outlet { get; set; }
        public DateTime DateProcessed { get; set; }
        public int RowCount { get; set; }

    }

    public class OutletMainOrderSummary
    {

        public Guid Id { get; set; }
        public string DocumentReference { get; set; }
        public string ExtDocumentReference { get; set; }
        public DateTime Required { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalVat { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal SaleDiscount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string Salesman { get; set; }
        public string Distributor { get; set; }
        public string Outlet { get; set; }
       
    }
}