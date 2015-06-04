using System;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Factory.Documents
{
    //REFACTOR - Get rid of this contract
    //[Obsolete("Refactoring out")]
    public interface IDocumentFactory
    {
        Document CreateDocument(Guid id, DocumentType documentType, CostCentre documentIssuerCC, CostCentre documentRecipientCC, User DocumentIssuerUser, string DocumentReference, double? longitude=null, double? latitude=null);
        //Document CreateDocument(Guid id, DocumentType documentType, CostCentre documentIssuerCC, CostCentre documentRecipientCC, User DocumentIssuerUser,string DocumentReference);
    }
}
