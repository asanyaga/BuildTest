using System;
using System.Linq;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers
{
    public abstract class BaseSourcingCommandHandler
    {
        protected CokeDataContext _context;

        public BaseSourcingCommandHandler(CokeDataContext context)
        {
            _context = context;
        }

        protected bool DocumentExists(Guid documentId)
        {
            return _context.tblSourcingDocument.Any(n => n.Id == documentId);
        }


        protected tblSourcingDocument NewDocument(CreateCommand command, DocumentType documentType, Guid documentRecipientCostCentreId)
        {
            return _NewDocument(
                command.DocumentId,
                documentType,
                command.DocumentIssuerCostCentreId,
                command.DocIssuerUserId,
                command.CommandGeneratedByCostCentreApplicationId,
                command.DocumentDateIssued,
                DocumentStatus.New,
                command.DocumentReference,
                command.SendDateTime,
                documentRecipientCostCentreId,
                command.PDCommandId
                );
        }

        private tblSourcingDocument _NewDocument(Guid documentId,
            DocumentType documentType,
            Guid documentIssuerCostCentreId,
            Guid documentIssuerUserId,
            Guid documentIssuerCostCentreApplicationId,
            DateTime documentDateIssued,
            DocumentStatus documentStatus,
            string documentReference,
            DateTime sendDateTime,
            Guid documentRecipientCostCentreId,
             Guid documentparentId

           
            )
        {
            var doc = new tblSourcingDocument
                {
                    Id = documentId,
                    DocumentIssuerCostCentreId = documentIssuerCostCentreId,
                    DocumentIssuerUserId = documentIssuerUserId,
                    DocumentRecipientCostCentreId = documentRecipientCostCentreId,
                    DocumentTypeId = (int) documentType,
                    DocumentDate = documentDateIssued,
                    DocumentStatusId = (int) documentStatus,
                    DocumentReference = documentReference,
                    IM_DateCreated = DateTime.Now,
                    DocumentIssuerCostCentreApplicationId = documentIssuerCostCentreApplicationId,
                    DateIssued = DateTime.Now,
                    DocumentParentId = documentparentId,
                    DateSent = sendDateTime.Equals(new DateTime()) ? DateTime.Now : sendDateTime,
                  
                };
            if (sendDateTime.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                doc.DateSent = DateTime.Now;
            }
            doc.IM_DateLastUpdated = DateTime.Now;
            
            return doc;
        }

        protected tblSourcingDocument ExistingDocument(Guid documentId)
        {
            return _context.tblSourcingDocument.FirstOrDefault(n => n.Id == documentId);
        }

        protected bool DocumentLineItemExists(Guid lineItemId)
        {
            return _context.tblSourcingLineItem.Any(n => n.Id == lineItemId);
        }

        protected tblSourcingLineItem ExistingLineItem(Guid lineItemId)
        {
            return _context.tblSourcingLineItem.FirstOrDefault(n => n.Id == lineItemId);
        }

        protected tblSourcingLineItem NewLineItem(Guid lineItemId,Guid parentId,
                                           Guid documentId,
                                           Guid conmmodityId,
                                           Guid gradeId,
                                            Guid containerId,
                                           decimal? weight,
                                           string description,
                                           string containerNo
                                          
            )
        {
            var li = new tblSourcingLineItem
                {
                    Id = lineItemId,
                    DocumentId = documentId,
                    CommodityId = conmmodityId,
                    ContainerId =containerId,
                    GradeId = gradeId,
                    Weight = weight,
                    NoOfContainer = 0,
                    TareWeight = 0,
                    Note  = description,
                    ParentId = parentId,
                    Description = description==null? "" : description,
                    ContainerNo = containerNo,
                    LineItemStatusId = (int)SourcingLineItemStatus.New
                };
            return li;
        }

        protected void ConfirmDocument(Guid documentId)
        {
            tblSourcingDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentSourcingStatus.Confirmed;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }

        protected void CloseDocument(Guid documentId)
        {
            tblSourcingDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentSourcingStatus.Closed;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }

        protected void ReceiveDocument(Guid documentId)
        {
            tblSourcingDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentSourcingStatus.Received;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }

        protected void ApproveDocument(Guid documentId)
        {
            tblSourcingDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentSourcingStatus.Approved; document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }
        
    }
}
