using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.LossesRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class LossRepository : DocumentRepository,ILossRepository
    {
        

        public LossRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository, ICacheProvider cacheProvider, ICostCentreRepository costCenterRepository) : base(ctx, costCentreRepository, userRepository, productRepository)
        {
          
        }

        public List<PaymentNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<PaymentNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }


        public void Save(PaymentNote documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            _ctx.SaveChanges();
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
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
            var pn = new PaymentNote(tblDoc.Id);
            _Map(tblDoc, pn);
            pn.PaymentNoteType = (PaymentNoteType)tblDoc.OrderOrderTypeId;
            pn._SetLineItems(tblDoc.tblLineItems.Select(n => MapPayementLineItem(n)).ToList());
            return pn;
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
