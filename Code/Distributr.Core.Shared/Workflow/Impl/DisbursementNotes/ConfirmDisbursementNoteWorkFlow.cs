using System;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;

namespace Distributr.Core.Workflow.Impl.DisbursementNotes
{
    public class ConfirmDisbursementNoteWorkFlow : IConfirmDisbursementNoteWorkFlow
    {
        
        IOutgoingDocumentCommandRouter _commandRouter;
      
        
        private IAuditLogWFManager _auditLogWFManager;

        public ConfirmDisbursementNoteWorkFlow(
            IOutgoingDocumentCommandRouter commandRouter,
            IAuditLogWFManager auditLogWFManager)
        {
            
            _commandRouter = commandRouter;
            
            
           
            _auditLogWFManager = auditLogWFManager;
        }

        public void SubmitChanges(DisbursementNote document,BasicConfig config )
        {
            //send commands
            var coc = new CreateDisbursementNoteCommand(
                Guid.NewGuid(),                
                document.Id,              
                document.DocumentIssuerUser.Id,               
                config.CostCentreId,               
                0,               
                document.DocumentIssuerCostCentreApplicationId,               
                document.DocumentIssuerCostCentre.Id,               
                document.DocumentRecipientCostCentre.Id, 
                document.DocumentIssuerUser.Id,
                document.DocumentParentId,document.DocumentDateIssued, 
                document.DocumentReference
             );

            _commandRouter.RouteDocumentCommand(coc);
            _auditLogWFManager.AuditLogEntry("Disbursment Note", string.Format("Created Disbursement Note document: {0}", document.Id));

            foreach (var item in document.LineItems)
            {
                var ali = new AddDisbursementNoteLineItemCommand(
                    Guid.NewGuid(),
                    document.Id,
                    document.DocumentIssuerUser.Id,
                    document.DocumentIssuerCostCentre.Id,
                    0,
                    config.CostCentreApplicationId,
                    0,
                    item.Product.Id,
                    item.Qty,
                    item.Value,document.DocumentParentId);
                _commandRouter.RouteDocumentCommand(ali);
                _auditLogWFManager.AuditLogEntry("Disbursment Note", string.Format("Added Product: {1}; Quantity: {2}; Value: {3}; to Disbursement Note document: {0}", document.Id,item.Product.Description,item.Qty,item.Value));

            }

            var co = new ConfirmDisbursementNoteCommand(Guid.NewGuid(), document.Id,
                document.DocumentIssuerUser.Id,
                document.DocumentIssuerCostCentre.Id, 0,
                config.CostCentreApplicationId,document.DocumentParentId);

            _commandRouter.RouteDocumentCommand(co);
            _auditLogWFManager.AuditLogEntry("Disbursment Note", string.Format("Confirmed Disbursement Note document: {0}", document.Id));
        }


    }
}
