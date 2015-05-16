using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class ReceiptRepository : DocumentRepository, IReceiptRepository
    {
        public ReceiptRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository)
            : base(ctx, costCentreRepository, userRepository, productRepository)
        {
        }
        public List<Receipt> GetReceipts(Guid invoiceId = new Guid())
        {
            IQueryable<tblDocument> tblReceipts = _GetAll(DocumentType.Receipt);
            if (!invoiceId.Equals(new Guid()))
                tblReceipts = tblReceipts.Where(n => n.InvoiceOrderId == invoiceId);
            return tblReceipts.ToList().Select(n => Map(n)).ToList();
        }

        public Receipt GetByLineItemId(Guid itemId)
        {
            var tblrecipt = _ctx.tblLineItems.Where(s => s.id == itemId).Select(s => s.tblDocument).FirstOrDefault();
            if (tblrecipt == null) return null;
            
            return Map(tblrecipt) as Receipt;
        }

        public List<ReceiptLineItem> GetChildLineItemsByLineItemId(Guid parentLineItemId)
        {
            return _ctx.tblLineItems.Where(s => s.PaymentDocLineItemId == parentLineItemId)
                .Select(s => MapReceiptLineItems(s)).ToList();
        }

        public List<Receipt> GetByInvoiceId(Guid invoiceId)
        {
            var res = _GetAll(DocumentType.Receipt)
                       .Where(n => n.InvoiceOrderId == invoiceId)
                       .ToList()
                       .Select(Map)
                       .ToList();
            return res;
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }

        public void Save(Receipt documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            Receipt r = documentEntity;
            docToSave.InvoiceOrderId = r.InvoiceId;
            docToSave.PaymentDocId = r.PaymentDocId;
            foreach (ReceiptLineItem rli in r.LineItems)
            {
                tblLineItems ll = null;
                if (docToSave.tblLineItems.Any(n => n.id == rli.Id))
                    ll = docToSave.tblLineItems.First(n => n.id == rli.Id);
                else
                {
                    ll = new tblLineItems();
                    ll.id = rli.Id;
                    ll.DocumentID = documentEntity.Id;
                    docToSave.tblLineItems.Add(ll);
                }

                ll.Value = rli.Value;
                ll.PaymentDocLineItemId = rli.PaymentDocLineItemId;
                //ll.Receipt_AccountType     = (int)rli.AccountType;
                ll.Description = rli.Description;
                ll.LineItemSequenceNo = rli.LineItemSequenceNo;
                ll.Receipt_PaymentReference = rli.PaymentRefId;
                ll.Receipt_PaymentTypeId = (int)rli.PaymentType;
                ll.OrderLineItemType = (int)rli.LineItemType;
                ll.Receipt_MMoneyPaymentType = rli.MMoneyPaymentType;
                ll.NotificationId = rli.NotificationId;
            }
            _ctx.SaveChanges();
        }

        public Receipt GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var cn = Map(tblDoc);
            return cn;
        }

        private Receipt Map(tblDocument tblDoc)
        {
            var doc = new Receipt(tblDoc.Id);
            _Map(tblDoc, doc);
            if (tblDoc.InvoiceOrderId != null)
                doc.InvoiceId = tblDoc.InvoiceOrderId.Value;
            doc.PaymentDocId = tblDoc.PaymentDocId.HasValue == true ? tblDoc.PaymentDocId.Value : Guid.Empty;

            var lineItem = tblDoc.tblLineItems.Select(MapReceiptLineItems).ToList();
            doc._SetLineItems(lineItem);
            return doc;
        }
        internal ReceiptLineItem MapReceiptLineItems(tblLineItems n)
        {
            return new ReceiptLineItem(n.id)
            {
                //AccountType                 = (AccountType)n.Receipt_AccountType,
                Description = n.Description,
                PaymentDocLineItemId = n.PaymentDocLineItemId.HasValue ? n.PaymentDocLineItemId.Value : Guid.Empty,
                LineItemSequenceNo = n.LineItemSequenceNo.HasValue ? n.LineItemSequenceNo.Value : 0,
                Value = n.Value.Value,
                PaymentRefId = n.Receipt_PaymentReference,
                PaymentType = (PaymentMode)(n.Receipt_PaymentTypeId.HasValue ? n.Receipt_PaymentTypeId.Value : 1),
                //PaymentType = (PaymentMode)n.Receipt_PaymentTypeId.Value,
                LineItemType = (OrderLineItemType)n.OrderLineItemType,
                MMoneyPaymentType = n.Receipt_MMoneyPaymentType,
                NotificationId = n.NotificationId
            };
        }

        public List<Receipt> GetAll()
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.Receipt);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public List<Receipt> GetAll(DateTime startDate, DateTime endDate)
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.Receipt, startDate, endDate);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.Receipt);
        }
    }
}
