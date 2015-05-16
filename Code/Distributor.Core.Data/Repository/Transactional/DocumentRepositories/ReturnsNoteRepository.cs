using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.Repository.DocumentRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Repository.Transactional.DocumentRepositories
{
    internal class ReturnsNoteRepository : DocumentRepository, IReturnsNoteRepository
    {
        public ReturnsNoteRepository(CokeDataContext ctx,
            ICostCentreRepository costCentreRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            ICacheProvider cacheProvider)
            : base(ctx, costCentreRepository, userRepository, productRepository)
        {

        }

       

        public void CancelDocument(Guid documentId)
        {
            var doc = GetById(documentId);
            _CancelDocument(doc);
            Save(doc);
        }

        public void Save(ReturnsNote documentEntity)
        {
            tblDocument docToSave = SaveDocument(documentEntity);
            ReturnsNote rn = documentEntity;
            docToSave.OrderOrderTypeId = (int)rn.ReturnsNoteType;

            foreach (ReturnsNoteLineItem lineItem in rn._lineItems)
            {
                tblLineItems ll = null;
                if (docToSave.tblLineItems.Any(p => p.id == lineItem.Id))
                    ll = docToSave.tblLineItems.First(p => p.id.ToString() == lineItem.Id.ToString());
                else
                {
                    ll = new tblLineItems();
                    ll.id = lineItem.Id;
                    docToSave.tblLineItems.Add(ll);
                }
                if (lineItem.Product != null)
                    ll.ProductID = lineItem.Product.Id;
                ll.DocumentID = documentEntity.Id;
                ll.Description = lineItem.Description;
                ll.Quantity = lineItem.Qty;
                ll.IAN_Actual = lineItem.Actual;
                ll.LineItemSequenceNo = lineItem.LineItemSequenceNo;
                ll.Value = lineItem.Value;
                ll.Receipt_PaymentTypeId = (int)lineItem.ReturnType;
                ll.OrderLineItemType = (int)lineItem.LossType;
                ll.ReturnsNoteReason = lineItem.Reason;
                ll.Other = lineItem.Other;
            }
            _ctx.SaveChanges();
        }

        public ReturnsNote GetById(Guid Id)
        {
            var tblDoc = _GetById(Id);
            if (tblDoc == null) return null;
            var cn = Map(tblDoc);
            return cn;
        }

        private ReturnsNote Map(tblDocument tblDoc)
        {
            var doc = PrivateConstruct<ReturnsNote>(tblDoc.Id);
             doc.EnableAddCommands();
            _Map(tblDoc, doc);
            doc.ReturnsNoteType = tblDoc.OrderOrderTypeId.HasValue ? (ReturnsNoteType)tblDoc.OrderOrderTypeId : ReturnsNoteType.SalesmanToDistributor;
            var lineItem = tblDoc.tblLineItems.Select(MapReturnsNoteLineItem).ToList();
            doc._SetLineItems(lineItem);
            return doc;
        }
        private ReturnsNoteLineItem MapReturnsNoteLineItem(tblLineItems li)
        {
            var lineItem = new ReturnsNoteLineItem(li.id)
            {
                Description = li.Description,
                LineItemSequenceNo = li.LineItemSequenceNo.Value,
                Qty = li.Quantity.Value,
                Value = li.Value.Value,
                Actual = li.IAN_Actual.Value,
                ReturnType = (ReturnsType)li.Receipt_PaymentTypeId.Value,
                LossType = (LossType)li.OrderLineItemType.Value,
                Reason = li.ReturnsNoteReason,
                Other = li.Other,
            };
            if (li.ProductID != null)
                lineItem.Product = _productRepository.GetById(li.ProductID.Value);

            return lineItem;
        }

        public List<ReturnsNote> GetAll()
        {
            var tblDocument = _GetAll(DocumentType.ReturnsNote);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public List<ReturnsNote> GetAll(DateTime startDate, DateTime endDate)
        {
            var tblDocument = _GetAll(DocumentType.ReturnsNote, startDate, endDate);
            return tblDocument.ToList().Select(Map).ToList();
        }

        public int GetCount()
        {
            return _GetCount(DocumentType.ReturnsNote);
        }
    }
}
