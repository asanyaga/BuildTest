using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Workflow
{
    public interface  IDocumentFactoryParameters
    {
        CostCentre DocumentIssuerCostCentre {get;set;}
        User DocumentIssuerUser {get;set;}
        CostCentre DocumentRecipientCostCentre {get;set;}
        DocumentStatus Status {get;set;}
    }

    public interface  IDocumentLineItemFactoryParameters
    {
         string Description {get;set;}
        
    }

    public interface IProductLineItemParameters : IDocumentLineItemFactoryParameters
    {
         Product Product { get; set; }
         decimal Qty { get; set; }
        
    }

    public interface IWFManager<T> where T: Document
    {
        void SubmitChanges(T document, BasicConfig config);
    }

    public interface IAuditLogsWFManager
    {
        void AuditLogEntry(string module, string action);
    }

    public interface IDocumentWFManager<T,V> : IWFManager<T> where T : Document 
        where V : DocumentLineItem
    {
        void SaveNew(T document);
        T PendingDocumentFactory(IDocumentFactoryParameters parameters);
        V PendingDocumentLineItemFactory(T document, IDocumentLineItemFactoryParameters parameters);
        
    }
}
