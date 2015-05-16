using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class DispatchNoteRepository : DocumentRepository,IDispatchNoteRepository
    {
        public DispatchNoteRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository, ICacheProvider cacheProvider, ICostCentreRepository costCenterRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
          
        }

        public List<DispatchNote> GetByIssuerCostCentre(Guid issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<DispatchNote> GetByRecipientCostCentre(Guid recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<DispatchNote> GetByOrderId(Guid orderId)
        {
            var tblDoc = _GetAll(DocumentType.DispatchNote).Where(n => n.InvoiceOrderId == orderId);
            return tblDoc.ToList().Select(Map).ToList();
        }

        public List<DispatchNote> GetAll(DateTime startDate, DateTime endDate)
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.DispatchNote, startDate, endDate);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.DispatchNote);
        }

        public  void Save(DispatchNote documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            DispatchNote DN = documentEntity ;
            docToSave.OrderOrderTypeId = (int)DN.DispatchType;
            docToSave.InvoiceOrderId = DN.OrderId;

            foreach (DispatchNoteLineItem DNitem in DN.LineItems)
            {
                tblLineItems ll = null;
                if (docToSave.tblLineItems.Any(n => n.id == DNitem.Id))
                    ll = docToSave.tblLineItems.First(p => p.id == DNitem.Id);
                else
                {
                    ll = new tblLineItems();
                    ll.id = DNitem.Id;
                    ll.DocumentID = documentEntity.Id;
                    docToSave.tblLineItems.Add(ll);
                }
                ll.ProductID = DNitem.Product.Id;
                ll.LineItemSequenceNo = DNitem.LineItemSequenceNo;
                ll.Value = DNitem.Value;
                ll.Description = DNitem.Description;
                ll.Quantity = DNitem.Qty;
                ll.LineItemSequenceNo = DNitem.LineItemSequenceNo;
                ll.OrderLineItemType = (int)DNitem.LineItemType;
                ll.DiscountLineItemTypeId = (int)DNitem.DiscountType;
                ll.ProductDiscount = DNitem.ProductDiscount;
                ll.Vat = DNitem.LineItemVatValue;
                ll.Description = DNitem.Description;
            }
            _ctx.SaveChanges();
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }

        public DispatchNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var dn = Map(tblDoc);
            return dn;
        }

        private DispatchNote Map(tblDocument tblDoc)
        {
            var dn = new DispatchNote(tblDoc.Id);
            _Map(tblDoc, dn);
            if (tblDoc.OrderOrderTypeId != null)
                dn.DispatchType = (DispatchNoteType)tblDoc.OrderOrderTypeId;
            if (tblDoc.InvoiceOrderId != null)
                dn.OrderId = tblDoc.InvoiceOrderId.Value;
            dn._SetLineItems(tblDoc.tblLineItems.Select(n => MapDNLineItem(n)).ToList());
            return dn;
        }
        private DispatchNoteLineItem MapDNLineItem(tblLineItems li)
        {
            return new DispatchNoteLineItem(li.id)
            {
                Description = li.Description,
                LineItemSequenceNo = li.LineItemSequenceNo.Value,
                Product = _productRepository.GetById(li.ProductID.Value),
                Qty = li.Quantity.Value,
                Value = li.Value.Value,

                LineItemType = (OrderLineItemType)li.OrderLineItemType,
                ProductDiscount = li.ProductDiscount.Value,
                LineItemVatValue = li.Vat.Value,
                DiscountType = (DiscountType)li.DiscountLineItemTypeId
            };
        }
        public  List<DispatchNote> GetAll()
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.DispatchNote);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }
    }
}
