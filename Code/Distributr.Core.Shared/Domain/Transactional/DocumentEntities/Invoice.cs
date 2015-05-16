using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Domain.Transactional.DocumentEntities.Exceptions;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public class Invoice : Document
    {
       

        private Invoice(Guid id) : base (id)
        {
            _lineItems = new List<InvoiceLineItem>();
            _CanAddCommands = false;
        }

        public Guid OrderId { get; set; }

        private List<InvoiceLineItem> _lineItems;
        public List<InvoiceLineItem> LineItems
        {
            get { return _lineItems; }
        }

        public void AddLineItem(InvoiceLineItem lineItem)
        {
            _lineItems.Add(lineItem);
            _AddAddLineItemCommandToExecute(lineItem);
        }

        public void ClearLineItems()
        {
            _lineItems.Clear();
        }

        public decimal SaleDiscount { get; set; }
        public decimal TotalNet { get { return LineItems.Sum(n => n.Value * n.Qty); } }
        public decimal TotalVat { get { return LineItems.Sum(n => n.LineItemVatTotal); } }
        public decimal TotalGross { get { return (TotalNet + TotalVat)-SaleDiscount; }}//cn:

        public override void Confirm()
        {
            if (Status != DocumentStatus.New)
                throw new InvalidDocumentOperationException("Cannot confirm an invoice that is not new");
            Status = DocumentStatus.Confirmed;
            _AddCreateCommandToExecute();
            _AddConfirmCommandToExecute();
        }

        public  void Close()
        {
            if (Status != DocumentStatus.Confirmed)
                throw new InvalidDocumentOperationException("Invoice needs to be confirmed");
            Status = DocumentStatus.Closed;
        }

        protected override void _AddCreateCommandToExecute(bool isHybrid = false)
        {
            var ic = new CreateInvoiceCommand(
               Guid.NewGuid(),
               Id,
               DocumentIssuerUser.Id,
               DocumentIssuerCostCentre.Id,
               0,
               DocumentIssuerCostCentreApplicationId,
               DocumentReference,
               DocumentDateIssued,
               DocumentIssuerCostCentre.Id,
               DocumentRecipientCostCentre.Id,
               DocumentIssuerUser.Id,
               OrderId,
               SaleDiscount
               );
            ic.VersionNumber = "H-" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            _AddCommand(ic);
        }

        protected override void _AddAddLineItemCommandToExecute<T>(T lineItem, bool isHybrid = false)
        {
            var item = lineItem as InvoiceLineItem;
            var ilic = new AddInvoiceLineItemCommand(
                    item.Id,
                    Id,
                    DocumentIssuerUser.Id,
                    DocumentIssuerCostCentre.Id,
                    0,
                    DocumentIssuerCostCentreApplicationId,
                    item.LineItemSequenceNo,
                    item.Value,
                    item.Product.Id,
                    item.Qty,
                    item.LineItemVatValue,
                    item.Id,
                    DocumentParentId,
                    item.Description,
                    item.ProductDiscount,
                    (int)item.LineItemType,
                    (int)item.DiscountType
                    );
            ilic.Description = item.Description;
            _AddCommand(ilic);
        }


        protected override void _AddConfirmCommandToExecute(bool isHybrid = false)
        {
            var icc = new ConfirmInvoiceCommand(
              Guid.NewGuid(),
              Id,
              DocumentIssuerUser.Id,
              DocumentIssuerCostCentre.Id,
              0,
              DocumentIssuerCostCentreApplicationId, 
              DocumentParentId
              );
            _AddCommand(icc);
        }

        public void _SetLineItems(List<InvoiceLineItem> items)
        {
            _lineItems = items;
        }
    }
}
