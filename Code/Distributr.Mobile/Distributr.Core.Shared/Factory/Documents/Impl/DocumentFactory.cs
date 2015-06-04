using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Documents;


namespace Distributr.Core.Factory.Master.Impl
{
    
    public class DocumentFactory : IDocumentFactory
    {
        public Document CreateDocument(Guid id, DocumentType documentType, CostCentre documentIssuerCC, CostCentre documentRecipientCC, User documentIssuerUser, string DocumentReference, double? longitude=null, double? latitude=null)
        {
            Document doc = null;
            switch (documentType)
            {
                case DocumentType.Order:
                    doc = new Order(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, /*null, */documentIssuerUser, DocumentReference, longitude, latitude);
                    break;
                case DocumentType.DispatchNote://
                    doc = new DispatchNote(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, /*null, */documentIssuerUser, DocumentReference, longitude, latitude);
                    break;
                
                case DocumentType.Receipt:
                    doc = new Receipt(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, documentIssuerUser, DocumentReference, longitude, latitude);
                    break;
                case DocumentType.DisbursementNote:
                    doc = new DisbursementNote(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, documentIssuerUser, DocumentReference, longitude, latitude);
                    break;
                case DocumentType.CreditNote:
                    doc = new CreditNote(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, documentIssuerUser, DocumentReference, longitude, latitude);
                    break;
                case DocumentType.ReturnsNote:
                  
                    doc = new ReturnsNote(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, documentIssuerUser, DocumentReference, longitude, latitude);
                    break;
                case DocumentType.PaymentNote:
                    doc = new PaymentNote(id);
                    doc = Map(doc, documentType, documentIssuerCC, documentRecipientCC, documentIssuerUser, DocumentReference, longitude, latitude);
                    break;


            }

            return doc;
        }

         Document Map(Document document, DocumentType documentType, CostCentre documentIssuerCostCentre,
            CostCentre documentRecipientCostCentre, User documentIssuerUser,
            string DocumentReference, double? longitude, double? latitude)
        {
            document.DocumentType = documentType;
            document.DocumentIssuerCostCentre = documentIssuerCostCentre;
            document.DocumentRecipientCostCentre = documentRecipientCostCentre;
            document.DocumentIssuerUser = documentIssuerUser;
            document.DocumentReference = DocumentReference;
            document.Status = DocumentStatus.New;
            document.DocumentDateIssued = DateTime.Now;

            return MapLocation(document, longitude, latitude);
        }

        private Document MapLocation(Document document, double? longitude, double? latitude)
        { 
            document.Longitude = longitude;
            document.Latitude = latitude;
            return document;
        }
    }

}
