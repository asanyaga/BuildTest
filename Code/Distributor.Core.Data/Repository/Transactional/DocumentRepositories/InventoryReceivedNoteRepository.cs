using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    public class InventoryReceivedNoteRepository : DocumentRepository, IInventoryReceivedNoteRepository
    {

        public InventoryReceivedNoteRepository(CokeDataContext ctx, ICostCentreRepository costCenterRepository, IUserRepository userRepository, IProductRepository productRepository) :
            base(ctx, costCenterRepository, userRepository, productRepository)
        {

        }

        public List<InventoryReceivedNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }

        public List<InventoryReceivedNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued)
        {
            throw new NotImplementedException();
        }



      

        public List<InventoryReceivedNote> GetAll()
        {
            var tblDocument = _GetAll(DocumentType.InventoryReceivedNote);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public List<InventoryReceivedNote> GetAll(DateTime startDate, DateTime endDate)
        {
            var tblDocument = _GetAll(DocumentType.InventoryReceivedNote, startDate, endDate);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
           // Save(doc);
        }

        public InventoryReceivedNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var rn = Map(tblDoc);
            return rn;
        }

        private InventoryReceivedNote Map(tblDocument tblDoc)
        {
            var irn = PrivateConstruct<InventoryReceivedNote>(tblDoc.Id);
            _Map(tblDoc, irn);
            irn.OrderReferences = tblDoc.IRNOrderReferences;
            irn.LoadNo = tblDoc.IRNLoadNo;
            if (tblDoc.IRNGoodsReceivedFromCC.HasValue && tblDoc.IRNGoodsReceivedFromCC.Value != Guid.Empty)
                irn.GoodsReceivedFromCostCentre = _costCentreRepository.GetById(tblDoc.IRNGoodsReceivedFromCC.Value);
            irn._SetLineItems(tblDoc.tblLineItems.Select(n => MapIRNLineItem(n)).ToList());
            return irn;
        }
        private InventoryReceivedNoteLineItem MapIRNLineItem(tblLineItems li)
        {
            var _li = DocumentLineItemPrivateConstruct<InventoryReceivedNoteLineItem>(li.id);
            _li.Description = li.Description;
            _li.LineItemSequenceNo = li.LineItemSequenceNo.Value;
            _li.Product = _productRepository.GetById(li.ProductID.Value);
            _li.Qty = li.Quantity.Value;
            _li.Value = li.Value.Value;
            _li.Expected = li.ApprovedQuantity.HasValue? li.ApprovedQuantity.Value:_li.Qty; //used to track grn editing feature
            return _li;
        }


        public int GetCount()
        {
            return _GetCount(DocumentType.InventoryReceivedNote);
        }
    }
}
