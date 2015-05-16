using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers
{
    public abstract class BaseCommandHandler
    {
        private CokeDataContext _context;

        public BaseCommandHandler(CokeDataContext context)
        {
            _context = context;
        }

        protected bool DocumentExists(Guid documentId)
        {
            return _context.tblDocument.Any(n => n.Id == documentId);
        }
        protected bool DocumentIsConfirmed(Guid documentId)
        {
            return _context.tblDocument.Any(n => n.Id == documentId && n.DocumentStatusId==(int)DocumentStatus.Confirmed);
        }

        protected tblDocument NewDocument(CreateCommand command,DocumentType documentType, Guid documentRecipientCostCentreId)
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
                command.PDCommandId,
                command.Longitude,
                command.Latitude
                );
        }

        private tblDocument _NewDocument(Guid documentId,
            DocumentType documentType,
            Guid documentIssuerCostCentreId,
            Guid documentIssuerUserId,
            Guid documentIssuerCostCentreApplicationId,
            DateTime documentDateIssued,
            DocumentStatus documentStatus,
            string documentReference,
            DateTime sendDateTime,
            Guid documentRecipientCostCentreId,
             Guid documentparentId,
            double? longitude = null,
            double? latitude = null
            )
        {
            var doc = new tblDocument
                {
                    Id = documentId,
                    DocumentIssuerCostCentreId = documentIssuerCostCentreId,
                    DocumentIssuerUserId = documentIssuerUserId,
                    DocumentRecipientCostCentre = documentRecipientCostCentreId,
                    DocumentTypeId = (int) documentType,
                    DocumentDateIssued = documentDateIssued,
                    DocumentStatusId = (int) documentStatus,
                    DocumentReference = documentReference,
                    IM_DateCreated = DateTime.Now,
                    DocumentIssuerCostCentreApplicationId = documentIssuerCostCentreApplicationId,
                    IM_IsActive = true,
                    Latitude = latitude,
                    Longitude = longitude,
                    DocumentParentId = documentparentId,
                    SendDateTime = sendDateTime.Equals(new DateTime()) ? DateTime.Now : sendDateTime
                };
            if (sendDateTime.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                doc.SendDateTime = DateTime.Now;
            }
            doc.IM_DateLastUpdated = DateTime.Now;
            
            return doc;
        }

        protected tblDocument ExistingDocument(Guid documentId)
        {
            return _context.tblDocument.FirstOrDefault(n => n.Id == documentId);
        }

        protected bool DocumentLineItemExists(Guid lineItemId)
        {
            return _context.tblLineItems.Any(n => n.id == lineItemId);
        }

        protected tblLineItems ExistingLineItem(Guid lineItemId)
        {
            return _context.tblLineItems.FirstOrDefault(n => n.id == lineItemId);
        }

        protected tblLineItems NewLineItem(Guid lineItemId,
                                           Guid documentId,
                                           Guid? productId,
                                           string description,
                                           decimal? quantity,
                                           int? lineItemSequenceNo
            )
        {
            var li = new tblLineItems
                {
                    id = lineItemId,
                    DocumentID = documentId,
                    ProductID = productId,
                    Description = description==null? "" : description,
                    Quantity = quantity,
                    LineItemSequenceNo = lineItemSequenceNo
                };
            return li;
        }

        protected void ConfirmDocument(Guid documentId)
        {
            tblDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int) DocumentStatus.Confirmed;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }
        protected void ApproveDocument(Guid documentId)
        {
            tblDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentStatus.Approved;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }
        protected void RejectDocument(Guid documentId)
        {
            tblDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentStatus.Rejected;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }

        protected void CloseDocument(Guid documentId)
        {
            tblDocument document = ExistingDocument(documentId);
            document.DocumentStatusId = (int)DocumentStatus.Closed;
            document.IM_DateLastUpdated = DateTime.Now;
            _context.SaveChanges();
        }

    }
}
