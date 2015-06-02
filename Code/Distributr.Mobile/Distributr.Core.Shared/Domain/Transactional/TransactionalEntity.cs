using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;

namespace Distributr.Core.Domain.Transactional
{
    [Serializable]
    public abstract class TransactionalEntity
    {
        public TransactionalEntity(Guid id)
        {
            Id = id;
        }
        public Guid Id { get;  set; }


        
    }

    public static  class TransactionalEntityHelper
    {
        public static void  Initialize(this CommandEnvelope envelope, Document document)
        {
            envelope. Id = Guid.NewGuid();
            if (document is MainOrder ) { }
            envelope.EnvelopeGeneratedTick = DateTime.Now.Ticks;
            envelope.GeneratedByCostCentreId = document.DocumentIssuerCostCentre.Id;
            envelope.RecipientCostCentreId = document.DocumentRecipientCostCentre.Id;
            envelope.DocumentTypeId = (int)document.DocumentType;
            envelope.GeneratedByCostCentreApplicationId = document.DocumentIssuerCostCentreApplicationId;
            envelope.ParentDocumentId = document.DocumentParentId;
            envelope.DocumentId = document.Id;
            if (document is MainOrder)
            {
                var order = document as MainOrder;
                if (order.OrderType == OrderType.DistributorPOS)
                {
                    envelope.GeneratedByCostCentreId = document.DocumentIssuerCostCentre.Id;
                    envelope.RecipientCostCentreId = document.DocumentIssuerCostCentre.Id;
                }
            }
           
        }
        public static void Initialize(this CommandEnvelope envelope, SourcingDocument document)
        {
            envelope. Id = Guid.NewGuid();
            envelope.EnvelopeGeneratedTick = DateTime.Now.Ticks;
            envelope.GeneratedByCostCentreId = document.DocumentIssuerCostCentre.Id;
            envelope.RecipientCostCentreId = document.DocumentRecipientCostCentre.Id;
            envelope.DocumentTypeId = (int)document.DocumentType;
            envelope.GeneratedByCostCentreApplicationId = document.DocumentIssuerCostCentreApplicationId;
            envelope.ParentDocumentId = document.DocumentParentId;
            envelope.DocumentId = document.Id;
        }
        
        public static void Initialize(this CommandEnvelope envelope, ActivityDocument document)
        {
            envelope. Id = Guid.NewGuid();
            envelope.EnvelopeGeneratedTick = DateTime.Now.Ticks;
            envelope.GeneratedByCostCentreId = document.Hub.Id;
            envelope.RecipientCostCentreId = document.FieldClerk.Id;
            envelope.DocumentTypeId =(int)DocumentType.ActivityNote;
            envelope.GeneratedByCostCentreApplicationId = document.DocumentIssuerCostCentreApplicationId;
            envelope.ParentDocumentId = document.Id;
            envelope.DocumentId = document.Id;
        }

        
    }
}
