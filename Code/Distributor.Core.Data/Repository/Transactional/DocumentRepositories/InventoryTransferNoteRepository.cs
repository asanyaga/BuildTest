using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class InventoryTransferNoteRepository : DocumentRepository, IInventoryTransferNoteRepository
    {

        public InventoryTransferNoteRepository(CokeDataContext ctx, ICostCentreRepository costCenterRepository, IUserRepository userRepository, IProductRepository productRepository) :
            base(ctx, costCenterRepository, userRepository, productRepository)
        {
        }

        public List<InventoryTransferNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<InventoryTransferNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
        }

        public InventoryTransferNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var cn = Map(tblDoc);
            return cn;
        }

        private InventoryTransferNote Map(tblDocument tblDoc)
        {
            var itn = PrivateConstruct<InventoryTransferNote>(tblDoc.Id);
            _Map(tblDoc, itn);
            itn.DocumentIssueredOnBehalfCostCentre = _costCentreRepository.GetById(tblDoc.OrderIssuedOnBehalfOfCC.Value);
            itn._SetLineItems(tblDoc.tblLineItems.Select(n => MapITNLineItem(n)).ToList());
            return itn;
        }

        private InventoryTransferNoteLineItem MapITNLineItem(tblLineItems li)
        {
            var _li = DocumentLineItemPrivateConstruct<InventoryTransferNoteLineItem>(li.id);
            _li.Description = li.Description;
            _li.LineItemSequenceNo = li.LineItemSequenceNo.Value;
            _li.Product = _productRepository.GetById(li.ProductID.Value);
            _li.Qty = li.Quantity.Value;
            return _li;
        }

        public List<InventoryTransferNote> GetAll()
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.InventoryTransferNote);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }


        public List<InventoryTransferNote> GetAll(DateTime startDate, DateTime endDate)
        {
            IEnumerable<tblDocument> tblDocument = _GetAll(DocumentType.InventoryTransferNote, startDate, endDate);
            return tblDocument.ToList().Select(n => Map(n)).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.CreditNote);
        }
    }
}
