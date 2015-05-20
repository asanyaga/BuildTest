using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class CreditNoteRepository : DocumentRepository, ICreditNoteRepository
    {
        public CreditNoteRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
        }
        public List<CreditNote> GetCreditNotesByInvoiceId(Guid invoiceId = new Guid())
        {
            IEnumerable<tblDocument> tblCreditNotes = null;
            if (invoiceId.Equals(new Guid()))
                tblCreditNotes = _ctx.tblDocument.Where(n => n.DocumentTypeId == (int)DocumentType.CreditNote);
            else
                tblCreditNotes = _ctx.tblDocument.Where(n => n.DocumentTypeId == (int)DocumentType.CreditNote
                                                          && n.InvoiceOrderId == invoiceId);

            return tblCreditNotes.Select(Map).Select(n => n as CreditNote).ToList();
        }
        /// <summary>
        /// Used only for cancelling document
        /// </summary>
        /// <param name="documentEntity"></param>
        private void Save(CreditNote documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            CreditNote r = documentEntity as CreditNote;
            docToSave.InvoiceOrderId = r.InvoiceId;
            foreach (CreditNoteLineItem rli in r.LineItems)
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
                ll.ProductID = rli.Product.Id;
                ll.Value = rli.Value;
                ll.Quantity = rli.Qty;
                ll.Description = rli.Description;
                ll.LineItemSequenceNo = rli.LineItemSequenceNo;
            }
            _ctx.SaveChanges();
        }

        public List<CreditNote> GetAll()
        {
            var tblDocument = _GetAll(DocumentType.CreditNote); 
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }


        public CreditNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var cn = Map(tblDoc);
            return cn;
        }

        private CreditNote Map(tblDocument tblDoc)
        {
            var cn = new CreditNote(tblDoc.Id);
            _Map(tblDoc, cn);
            cn.InvoiceId = tblDoc.InvoiceOrderId == null ? Guid.Empty : tblDoc.InvoiceOrderId.Value;
            cn._SetLineItems(tblDoc.tblLineItems.Select(n => MapCreditLineItem(n)).ToList());
            return cn;
        }

        private CreditNoteLineItem MapCreditLineItem(tblLineItems n)
        {
            return new CreditNoteLineItem(n.id)
            {

                Description = n.Description,
                LineItemSequenceNo = n.LineItemSequenceNo.Value,
                Value = n.Value.Value,
                Product = _productRepository.GetById(n.ProductID.Value),
                Qty = n.Quantity.Value

            };
        }

        public List<CreditNote> GetAll(DateTime startDate, DateTime endDate)
        {
            var tblDocument = _GetAll(DocumentType.CreditNote, startDate, endDate);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.CreditNote);
        }


        public ValidationResultInfo Validate(CreditNote itemToValidate)
        {
            return _Validate(itemToValidate);
        }



        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }



       
    }
}
