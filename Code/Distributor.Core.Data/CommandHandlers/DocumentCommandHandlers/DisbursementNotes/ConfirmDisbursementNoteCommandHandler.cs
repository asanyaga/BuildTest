using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Workflow.InventoryWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DisbursementNotes
{
    public class ConfirmDisbursementNoteCommandHandler : IConfirmDisbursementNoteCommandHandler
    {
       IDisbursementNoteRepository _documentRepository;
       ILog _log = LogManager.GetLogger("ConfirmDisbursementNoteCommandHandler");
        private IInventoryWorkflow _inventoryWorkflow;

        public ConfirmDisbursementNoteCommandHandler(IDisbursementNoteRepository documentRepository, IInventoryWorkflow inventoryWorkflow)
        {
            _documentRepository = documentRepository;
            _inventoryWorkflow = inventoryWorkflow;
        }

        public void Execute(ConfirmDisbursementNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                
                bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;

                if (!already_Exist)
                    return;
                DisbursementNote disbursementNote = _documentRepository.GetById(command.DocumentId) as DisbursementNote;
                disbursementNote.Confirm();
                //save the order            
                _documentRepository.Save(disbursementNote);

                foreach (var lineItem in disbursementNote.LineItems)
                {
                    ////on the server debit issuer cc
                    //_inventoryWorkflow.InventoryAdjust(disbursementNote.DocumentIssuerCostCentre.Id, lineItem.Product.Id,
                    //                                   -lineItem.Qty, DocumentType.DispatchNote, disbursementNote.Id,
                    //                                   disbursementNote.DocumentDateIssued);
                    ////credit recipient cc
                    //_inventoryWorkflow.InventoryAdjust(disbursementNote.DocumentRecipientCostCentre.Id, lineItem.Product.Id,
                    //                                   lineItem.Qty, DocumentType.DispatchNote, disbursementNote.Id,
                    //                                   disbursementNote.DocumentDateIssued);
                }

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                _log.Error("ConfirmDisbursementNoteCommandHandler exception", ex);
                throw;
            }
        }

      
    }
}
