using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class InventoryAdjustmentNoteRepository :DocumentRepository, IInventoryAdjustmentNoteRepository
    {
        public InventoryAdjustmentNoteRepository(CokeDataContext ctx, ICostCentreRepository costCenterRepository, IUserRepository userRepository, IProductRepository productRepository) :
            base(ctx, costCenterRepository, userRepository,productRepository)
        {
          
        }

        public List<InventoryAdjustmentNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<InventoryAdjustmentNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            //Save(doc);
        }

        public InventoryAdjustmentNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var an = Map(tblDoc);
            return an;
        }

        private InventoryAdjustmentNote Map(tblDocument tblDoc)
        {
            var doc = PrivateConstruct<InventoryAdjustmentNote>(tblDoc.Id); //new InventoryAdjustmentNote(tblDoc.Id); 
            _Map(tblDoc, doc);
            doc.InventoryAdjustmentNoteType = tblDoc.OrderOrderTypeId.HasValue == true ? (InventoryAdjustmentNoteType)tblDoc.OrderOrderTypeId.Value : 0;
            doc._SetLineItems(tblDoc.tblLineItems.Select(n => MapIANLineItem(n)).ToList());
            return doc;
        }
        private InventoryAdjustmentNoteLineItem MapIANLineItem(tblLineItems li)
        {
            var _li = DocumentLineItemPrivateConstruct<InventoryAdjustmentNoteLineItem>(li.id);
            _li.Description = li.Description;
            _li.LineItemSequenceNo = li.LineItemSequenceNo.Value;
            _li.Product = _productRepository.GetById(li.ProductID.Value);
            _li.Qty = li.Quantity.Value;
            _li.Actual = li.IAN_Actual.Value;
            return _li;
        }
        public  List<InventoryAdjustmentNote> GetAll()
        {
            IEnumerable<tblDocument> tblDocuments = _GetAll(DocumentType.InventoryAdjustmentNote);
            return tblDocuments.ToList().Select(Map).ToList();
        }

        public List<InventoryAdjustmentNote> GetAll(DateTime startDate, DateTime endDate)
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.InventoryAdjustmentNote, startDate, endDate);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.InventoryAdjustmentNote);
        }
    }
}
