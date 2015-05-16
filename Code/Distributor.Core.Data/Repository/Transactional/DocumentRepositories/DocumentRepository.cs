using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.ProductRepositories;
using log4net;
using System.Diagnostics;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.DocumentRepositories
{
    public abstract class DocumentRepository //: IDocumentRepository
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected CokeDataContext _ctx;
        protected ICostCentreRepository _costCentreRepository;
        protected IUserRepository _userRepository;
        protected IProductRepository _productRepository;

        public DocumentRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository,
            IUserRepository userRepository, IProductRepository productRepository)
        {
            _ctx = ctx;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            
        }
        protected static int Skip(int page, int pageSize)
        {
            int skip;
            if (page < 1)
                skip = 0;
            else
                skip = (page - 1) * pageSize;
            return skip;
        }
        protected tblDocument SaveDocument(Document documentEntity)
        {
            tblDocument docToSave = _ctx.tblDocument.FirstOrDefault(n => n.Id == documentEntity.Id);
            if (docToSave == null)
            {
                docToSave = new tblDocument();
                docToSave.Id = documentEntity.Id;
                docToSave.DocumentIssuerCostCentreId = documentEntity.DocumentIssuerCostCentre.Id;
                docToSave.DocumentIssuerUserId = documentEntity.DocumentIssuerUser.Id;//Problem
                docToSave.DocumentRecipientCostCentre = documentEntity.DocumentRecipientCostCentre.Id;
                docToSave.DocumentTypeId = (int)documentEntity.DocumentType;
                docToSave.DocumentDateIssued = documentEntity.DocumentDateIssued;
                docToSave.DocumentStatusId = (int)documentEntity.Status;
                docToSave.DocumentReference = documentEntity.DocumentReference;
                docToSave.IM_DateCreated = DateTime.Now;
                docToSave.DocumentIssuerCostCentreApplicationId = documentEntity.DocumentIssuerCostCentreApplicationId;
                docToSave.IM_IsActive = true;
                docToSave.Longitude = documentEntity.Longitude;
                docToSave.Latitude = documentEntity.Latitude;
                docToSave.SendDateTime = (documentEntity.SendDateTime.Equals(new DateTime()) ? DateTime.Now : documentEntity.SendDateTime);
                if (documentEntity.SendDateTime.ToString("dd/MM/yyyy") == "01/01/0001")
                {
                    docToSave.SendDateTime = DateTime.Now;
                }


                _ctx.tblDocument.AddObject(docToSave);
            }

            else
                docToSave.DocumentReference = documentEntity.DocumentReference;
            docToSave.DocumentStatusId = (int)documentEntity.Status;
            docToSave.DocumentRecipientCostCentre = documentEntity.DocumentRecipientCostCentre.Id;
            docToSave.DocumentIssuerCostCentreId = documentEntity.DocumentIssuerCostCentre.Id;
            docToSave.DocumentIssuerUserId = documentEntity.DocumentIssuerUser.Id;
            docToSave.IM_DateLastUpdated = DateTime.Now;

            return docToSave;
        }

        public tblDocument _GetById(Guid Id)
        {
            _log.DebugFormat("Getting by Id:{0}", Id);
            return _ctx.tblDocument.FirstOrDefault(n => n.Id == Id);
            //if (tbldoc == null)
            //    return null;
            //Document ord = Map(tbldoc);
            //return ord;
        }

        protected virtual ValidationResultInfo _Validate(Document itemToValidate)
        {
            return itemToValidate.BasicValidation();
        }

        protected void _Map(tblDocument tblDoc, Document doc)
        {
            doc.DocumentDateIssued = tblDoc.DocumentDateIssued;
            doc.DocumentIssuerCostCentre = _costCentreRepository.GetById(tblDoc.DocumentIssuerCostCentreId);
            doc.DocumentIssuerUser = _userRepository.GetById(tblDoc.DocumentIssuerUserId);
            doc.DocumentRecipientCostCentre = _costCentreRepository.GetById(tblDoc.DocumentRecipientCostCentre);
            doc.DocumentType = (DocumentType)tblDoc.DocumentTypeId;
            doc.Status = (DocumentStatus)tblDoc.DocumentStatusId;
            doc.DocumentReference = tblDoc.DocumentReference;
            doc.DocumentIssuerCostCentreApplicationId = tblDoc.DocumentIssuerCostCentreApplicationId ?? Guid.Empty;
            doc.Longitude = tblDoc.Longitude;
            doc.Latitude = tblDoc.Latitude;
            doc.StartDate = tblDoc.DocumentDateIssued;
            doc.EndDate = tblDoc.DocumentDateIssued;
            if (tblDoc.DocumentParentId != null)
                doc.DocumentParentId = tblDoc.DocumentParentId.Value;
        }

        [Obsolete]
        internal Document OLDMap(tblDocument tblDoc)
        {
            Document doc = null;
            DocumentType doctype = (DocumentType)tblDoc.DocumentTypeId;
            switch (doctype)
            {
                //case DocumentType.Order:
                //    doc = new Order(tblDoc.Id);
                //    break;
                //case DocumentType.InventoryAdjustmentNote:
                //    doc = new InventoryAdjustmentNote(tblDoc.Id);
                //    break;
                //case DocumentType.DispatchNote:
                //    doc = new DispatchNote(tblDoc.Id);
                //    break;
                //case DocumentType.InventoryTransferNote:
                //    doc = new InventoryTransferNote(tblDoc.Id);
                //    break;
                //case DocumentType.InventoryReceivedNote:
                    //doc = new InventoryReceivedNote(tblDoc.Id);
                    //break;
                //case DocumentType.Invoice:
                //    doc = new Invoice(tblDoc.Id);
                //    break;
                //case DocumentType.Receipt:
                //    doc = new Receipt(tblDoc.Id);
                //    break;
                case DocumentType.DisbursementNote:
                    doc = new DisbursementNote(tblDoc.Id);
                    break;
                case DocumentType.ReturnsNote:
                    doc = new ReturnsNote(tblDoc.Id);
                    break;
                //case DocumentType.PaymentNote:
                //    doc = new PaymentNote(tblDoc.Id);
                //    break;
                //case DocumentType.CreditNote:
                //    doc = new CreditNote(tblDoc.Id);
                //    break;
            }

            //if (doc != null)
            //{
            //    doc.DocumentDateIssued = tblDoc.DocumentDateIssued;
            //    doc.DocumentIssuerCostCentre = _costCentreRepository.GetById(tblDoc.DocumentIssuerCostCentreId);
            //    doc.DocumentIssuerUser = _userRepository.GetById(tblDoc.DocumentIssuerUserId);
            //    doc.DocumentRecipientCostCentre = _costCentreRepository.GetById(tblDoc.DocumentRecipientCostCentre);
            //    doc.DocumentType = (DocumentType)tblDoc.DocumentTypeId;
            //    doc.Status = (DocumentStatus)tblDoc.DocumentStatusId;
            //    doc.DocumentReference = tblDoc.DocumentReference;
            //    doc.DocumentIssuerCostCentreApplicationId = tblDoc.DocumentIssuerCostCentreApplicationId ?? Guid.Empty;
            //    doc.Longitude = tblDoc.Longitude;
            //    doc.Latitude = tblDoc.Latitude;
            //    doc.StartDate = tblDoc.DocumentDateIssued;
            //    doc.EndDate = tblDoc.DocumentDateIssued;
            //    _log.DebugFormat("Doc Ref={0}", tblDoc.DocumentReference);
            //    _log.DebugFormat("Doc DocRef:{0}", doc.DocumentReference);
            //}

            //if (doc.DocumentType == DocumentType.Order)
            //{
            //    Order o = doc as Order;
            //    o.DateRequired = tblDoc.OrderDateRequired.Value;
            //    o.IssuedOnBehalfOf = _costCentreRepository.GetById(tblDoc.OrderIssuedOnBehalfOfCC.Value);
            //    o.OrderType = (OrderType)tblDoc.OrderOrderTypeId.Value;
            //    o.SaleDiscount = tblDoc.SaleDiscount.Value;
            //    o.Note = tblDoc.Note;
            //    //tblDoc.tblLineItems.Any();
            //    o._SetLineItems(tblDoc.tblLineItems.Select(s => MapOrderLineItem(s)).ToList());

            //}


            

          

            //if (doc.DocumentType == DocumentType.InventoryTransferNote)
            //{
            //    InventoryTransferNote itn = doc as InventoryTransferNote;
            //    itn.DocumentIssueredOnBehalfCostCentre = _costCentreRepository.GetById(tblDoc.OrderIssuedOnBehalfOfCC.Value);
            //    itn._SetLineItems(tblDoc.tblLineItems.Select(n => MapITNLineItem(n)).ToList());
            //}

            //if (doc.DocumentType == DocumentType.Invoice)
            //{
            //    Invoice inv = doc as Invoice;
            //    inv.OrderId = tblDoc.InvoiceOrderId.Value;
            //    inv.SaleDiscount = tblDoc.SaleDiscount.Value;
            //    inv._SetLineItems(tblDoc.tblLineItems.Select(n => MapInvoiceLineItem(n)).ToList());
            //}

            //if (doc.DocumentType == DocumentType.Receipt)
            //{
            //    Receipt r = doc as Receipt;
            //    if (tblDoc.InvoiceOrderId != null)
            //        r.InvoiceId = tblDoc.InvoiceOrderId.Value;
            //    r.PaymentDocId = tblDoc.PaymentDocId.HasValue == true ? tblDoc.PaymentDocId.Value : Guid.Empty;

            //    var lineItem = tblDoc.tblLineItems.Select(MapReceiptLineItems).ToList();
            //    r._SetLineItems(lineItem);
            //}

            //if (doc.DocumentType == DocumentType.DisbursementNote)
            //{
            //    DisbursementNote r = doc as DisbursementNote;
            //    var lineItem = tblDoc.tblLineItems.Select(n => MapDisbursementNoteLineItems(n)).ToList();
            //    r._SetLineItems(lineItem);
            //}

            //if (doc.DocumentType == DocumentType.ReturnsNote)
            //{
            //    ReturnsNote rn = doc as ReturnsNote;

            //    rn.ReturnsNoteType = tblDoc.OrderOrderTypeId.HasValue ? (ReturnsNoteType)tblDoc.OrderOrderTypeId : ReturnsNoteType.SalesmanToDistributor;
            //    var lineItem = tblDoc.tblLineItems.Select(n => MapReturnsNoteLineItem(n)).ToList();
            //    rn._SetLineItems(lineItem);
            //}
            //if (doc.DocumentType == DocumentType.PaymentNote)
            //{
            //    PaymentNote loss = doc as PaymentNote;
            //    loss.PaymentNoteType = (PaymentNoteType)tblDoc.OrderOrderTypeId;
            //    loss._SetLineItems(tblDoc.tblLineItems.Select(n => MapPayementLineItem(n)).ToList());
            //}
            //if (doc.DocumentType == DocumentType.CreditNote)
            //{
            //    CreditNote creditNote = doc as CreditNote;
            //    creditNote.InvoiceId = tblDoc.InvoiceOrderId == null ? Guid.Empty : tblDoc.InvoiceOrderId.Value;
            //    creditNote._SetLineItems(tblDoc.tblLineItems.Select(n => MapCreditLineItem(n)).ToList());
            //}
            return doc;
        }

        private DiscountLineItem MapDiscountLineItem(tblLineItems li)
        {
            switch ((DiscountLineItem.DiscountLineItemType)li.DiscountLineItemTypeId)
            {
                case DiscountLineItem.DiscountLineItemType.Product:
                    return new DiscountLineItem(li.id)
                               {
                                   Description = li.Description,
                                   Discount_LineItemType =
                                       (DiscountLineItem.DiscountLineItemType)li.DiscountLineItemTypeId,
                                   LineItemSequenceNo = li.LineItemSequenceNo.Value,
                                   Product = li.ProductID != null ? _productRepository.GetById(li.ProductID.Value) : null,
                                   //ProductDiscount = li.
                                   Qty = li.Quantity.Value,
                                   Value = li.Value.Value,

                               };
                case DiscountLineItem.DiscountLineItemType.Value:
                    return new DiscountLineItem(li.id)
                               {
                                   Description = li.Description,
                                   Discount_LineItemType = (DiscountLineItem.DiscountLineItemType)li.DiscountLineItemTypeId,
                                   LineItemSequenceNo = li.LineItemSequenceNo.Value,
                                   //ProductDiscount = li.
                                   Value = li.Value.Value,
                               };
            }
            return null;
        }

        protected int _GetCount(DocumentType documentType)
        {
            return _ctx.tblDocument.Count(n => n.DocumentTypeId == (int) documentType);
        }

        protected void _CancelDocument(Document doc)
        {
            if (doc != null)
            {
                doc.Status = DocumentStatus.Cancelled;
            }
        }

        protected IQueryable<tblDocument> _GetAll(DocumentType documentType, DateTime startDate, DateTime endDate)
        {
            var data = _GetAll(documentType);
            endDate = endDate.Date.AddDays(1);
            data = data.Where(n => (n.DocumentDateIssued >= startDate.Date
                                                        && n.DocumentDateIssued <= endDate.Date));
            return data;
        }

        protected IQueryable<tblDocument> _GetAll(DocumentType documentType)
        {
            return _ctx.tblDocument.Where(n => n.DocumentTypeId == (int)documentType);
        }

        protected T PrivateConstruct<T>(Guid id) where T : Document
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }
        protected T DocumentLineItemPrivateConstruct<T>(Guid id) where T : ProductLineItem
        {
            ConstructorInfo ctor = typeof(T)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)[0];
            T doc = (T)ctor.Invoke(new object[] { id });
            return doc;
        }
    }
}
