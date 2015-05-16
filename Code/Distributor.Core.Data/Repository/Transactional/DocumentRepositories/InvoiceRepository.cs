using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class InvoiceRepository : DocumentRepository, IInvoiceRepository
    {
        public InvoiceRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
        }

        public Invoice GetInvoiceByOrderId(Guid orderId)
        {
            var tblInvoice = _ctx.tblDocument.FirstOrDefault(n => n.DocumentTypeId == (int) DocumentType.Invoice
                                                                  && n.InvoiceOrderId == orderId);
            if (tblInvoice != null)
                return Map(tblInvoice) as Invoice;
            return null;
        }

        public void SaveLineItem(InvoiceLineItem ili, Guid invoiceId)
        {
            tblLineItems lineItem = null;
            if (_ctx.tblLineItems.Any(n => n.id == ili.Id))
                lineItem = _ctx.tblLineItems.First(n => n.id == ili.Id);
            else
            {
                lineItem = new tblLineItems();
                lineItem.id = ili.Id;
                lineItem.DocumentID = invoiceId;
                _ctx.tblLineItems.AddObject(lineItem);
            }
            lineItem.ProductID = ili.Product.Id;
            lineItem.DocumentID = invoiceId;
            lineItem.Description = ili.Description;
            lineItem.Quantity = ili.Qty;
            lineItem.LineItemSequenceNo = ili.LineItemSequenceNo;
            lineItem.Value = ili.Value;
            lineItem.Vat = ili.LineItemVatValue;
            lineItem.OrderLineItemType = (int)ili.LineItemType;
            lineItem.ProductDiscount = ili.ProductDiscount;
            lineItem.DiscountLineItemTypeId = (int)ili.DiscountType;

            _ctx.SaveChanges();
        }

        public bool ChangeStatus(Guid documentId, DocumentStatus status)
        {
            return false;
            Invoice invoice = GetById(documentId);
            if (invoice == null)
                return false;
            invoice.Status = status;
            //Save(invoice);
            return true;
        }

        public InvoiceLineItem GetLineItemById(Guid lineItemId)
        {
            tblLineItems item = _ctx.tblLineItems.FirstOrDefault(n => n.id == lineItemId);
            return MapInvoiceLineItem(item);
        }

        public void RemoveLineItem(InvoiceLineItem ili)
        {
            tblLineItems li = _ctx.tblLineItems.FirstOrDefault(n => n.id == ili.Id);
            if (li != null)
            {
                _ctx.tblLineItems.DeleteObject(li);
                _ctx.SaveChanges();
            }
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            //Save(doc);
        }

       

        public Invoice GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var i = Map(tblDoc);
            return i;
        }

        private Invoice Map(tblDocument tblDoc)
        {
            var inv = PrivateConstruct<Invoice>(tblDoc.Id);
            _Map(tblDoc, inv);
            inv.OrderId = tblDoc.InvoiceOrderId.Value;
            inv.SaleDiscount = tblDoc.SaleDiscount.Value;
            inv._SetLineItems(tblDoc.tblLineItems.Select(n => MapInvoiceLineItem(n)).ToList());
            return inv;
        }
        private InvoiceLineItem MapInvoiceLineItem(tblLineItems n)
        {
            var lineItem = DocumentLineItemPrivateConstruct<InvoiceLineItem>(n.id);

            lineItem.Description = n.Description;
            lineItem.LineItemSequenceNo = n.LineItemSequenceNo.Value;
            lineItem.LineItemVatValue = n.Vat.Value;
            lineItem.Qty = n.Quantity.Value;
            lineItem.Value = n.Value.Value;
            lineItem.LineItemType = (OrderLineItemType) n.OrderLineItemType;
            lineItem.ProductDiscount = n.ProductDiscount.Value;
            lineItem.DiscountType = (DiscountType) n.DiscountLineItemTypeId;

            if (n.ProductID != null)
                lineItem.Product = _productRepository.GetById(n.ProductID.Value);

            return lineItem;
        }

        public  List<Invoice> GetAll()
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.Invoice);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public  List<Invoice> GetAll(DateTime startDate, DateTime endDate)
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.Invoice, startDate, endDate);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public  int GetCount()
        {
            return _GetCount(DocumentType.Invoice);
        }

        public List<Invoice> InvoicesPendingPayment(out List<InvoicePaymentInfo> invoicePaymentInfoList)
        {
            List<Invoice> invoicesOutstanding = new List<Invoice>();
            List<InvoicePaymentInfo> paymentInfoList = new List<InvoicePaymentInfo>();
            foreach (Invoice invoice in GetAll())
            {
                InvoicePaymentInfo invoicePaymentInfo = null;
                if (IsPendingPayment(invoice.Id, out invoicePaymentInfo, invoice))
                {
                    if (invoicePaymentInfo != null)
                        paymentInfoList.Add(invoicePaymentInfo);
                }
            }

            invoicePaymentInfoList = paymentInfoList;
            return invoicesOutstanding;
        }

        public bool IsPendingPayment(Guid invoiceId, out InvoicePaymentInfo invoicePaymentInfo, Invoice invoice = null)
        {
            bool isPending = false;
            InvoicePaymentInfo paymentInfo = null;

            invoicePaymentInfo = paymentInfo;
            return isPending;
        }

    }
}
