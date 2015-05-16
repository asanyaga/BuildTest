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
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class PaymentNoteRepository : DocumentRepository, IPaymentNoteRepository
    {
        public PaymentNoteRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }

        public  void Save(PaymentNote documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            PaymentNote payement = documentEntity as PaymentNote;
            docToSave.OrderOrderTypeId = (int)payement.PaymentNoteType;
            foreach (PaymentNoteLineItem LossItem in payement.LineItems)
            {
                tblLineItems ll = null;
                if (docToSave.tblLineItems.Any(n => n.id == LossItem.Id))
                    ll = docToSave.tblLineItems.First(p => p.id == LossItem.Id);
                else
                {
                    ll = new tblLineItems();
                    ll.id = LossItem.Id;
                    ll.DocumentID = documentEntity.Id;
                    docToSave.tblLineItems.Add(ll);
                }

                ll.LineItemSequenceNo = LossItem.LineItemSequenceNo;
                ll.Value = LossItem.Amount;
                ll.LineItemSequenceNo = LossItem.LineItemSequenceNo;
                ll.OrderLineItemType = (int)LossItem.PaymentMode;
                _log.DebugFormat("DocumentRepository:ll.Reason");
            }
            _ctx.SaveChanges();
        }

        public PaymentNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var cn = Map(tblDoc);
            return cn;
        }

        private PaymentNote Map(tblDocument tblDoc)
        {
            var doc = new PaymentNote(tblDoc.Id);
            _Map(tblDoc, doc);
            doc.PaymentNoteType = (PaymentNoteType)tblDoc.OrderOrderTypeId;
            doc._SetLineItems(tblDoc.tblLineItems.Select(n => MapPayementLineItem(n)).ToList());
            return doc;
        }
        private PaymentNoteLineItem MapPayementLineItem(tblLineItems li)
        {
            return new PaymentNoteLineItem(li.id)
            {
                Description = li.Description,
                LineItemSequenceNo = li.LineItemSequenceNo.Value,
                PaymentMode = (PaymentMode)li.OrderLineItemType.Value,
                Amount = li.Value.Value,

            };
        }

        public List<PaymentNote> GetAll()
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.PaymentNote);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public List<PaymentNote> GetAll(DateTime startDate, DateTime endDate)
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.PaymentNote, startDate, endDate);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.PaymentNote);
        }
    }
}
