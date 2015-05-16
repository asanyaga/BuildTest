using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class DisbursementNoteRepository : DocumentRepository, IDisbursementNoteRepository
    {
        

        public DisbursementNoteRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository, ICacheProvider cacheProvider, ICostCentreRepository costCenterRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
           
        }

        public DisbursementNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var cn = Map(tblDoc);
            return cn;
        }

        public List<DisbursementNote> GetAll()
        {
            var tblDocument = _GetAll(DocumentType.DisbursementNote); 
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public List<DisbursementNote> GetAll(DateTime startDate, DateTime endDate)
        {
            var tblDocument = _GetAll(DocumentType.DisbursementNote, startDate, endDate);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.DisbursementNote);
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }

        public  void Save(DisbursementNote documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            DisbursementNote r = documentEntity as DisbursementNote;
            foreach (DisbursementNoteLineItem rli in r.LineItems)
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


        public ValidationResultInfo Validate(DisbursementNote itemToValidate)
        {
            return _Validate(itemToValidate);
        }

        private DisbursementNote Map(tblDocument tblDoc)
        {
            var doc = new DisbursementNote(tblDoc.Id);
            _Map(tblDoc,doc);
            var lineItem = tblDoc.tblLineItems.Select(n => MapDisbursementNoteLineItems(n)).ToList();
            doc._SetLineItems(lineItem);
            return doc;
        }
        private DisbursementNoteLineItem MapDisbursementNoteLineItems(tblLineItems n)
        {
            return new DisbursementNoteLineItem(n.id)
            {

                Description = n.Description,
                Product = _productRepository.GetById(n.ProductID.Value),
                LineItemSequenceNo = n.LineItemSequenceNo.Value,
                Value = n.Value.Value,
                Qty = n.Quantity.Value,
            };
        }
    }
}
