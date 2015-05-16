using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Workflow.FinancialWorkflow;
using log4net;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Workflow.InventoryWorkflow;
using Distributr.Core.Domain.FinancialEntities;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.ReturnsNotes
{
    public class ConfirmReturnsNoteCommandHandler :BaseCommandHandler, IConfirmReturnsNoteCommandHandler
    {
        private IFinancialsWorkflow _financialsWorkflow;
     
        private IReturnsNoteRepository _returnsNoteRepository;
        ILog _log = LogManager.GetLogger("ConfirmReturnsNoteCommandHandler");


        public ConfirmReturnsNoteCommandHandler(CokeDataContext context, IFinancialsWorkflow financialsWorkflow, IInventoryWorkflow inventoryWorkflow, IReturnsNoteRepository returnsNoteRepository) : base(context)
        {
            _financialsWorkflow = financialsWorkflow;
          
            _returnsNoteRepository = returnsNoteRepository;
        }

        public void Execute(ConfirmReturnsNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Failed to  Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                    return;
                }

                ConfirmDocument(command.DocumentId);
                var doc = _returnsNoteRepository.GetById(command.DocumentId);
                if (doc != null)
                {
                    foreach (var li in doc._lineItems)
                    {
                        if (li.ReturnType != ReturnsType.Inventory)
                        {
                            //credit recipient cc account
                            _financialsWorkflow.AccountAdjust(doc.DocumentRecipientCostCentre.Id, AccountType.CostCentre,
                                li.Value, DocumentType.ReturnsNote, doc.Id,
                                doc.DocumentDateIssued);
                            //debit issuer cash account
                            _financialsWorkflow.AccountAdjust(doc.DocumentIssuerCostCentre.Id, AccountType.Cash,
                                -li.Value,
                                DocumentType.ReturnsNote, doc.Id, doc.DocumentDateIssued);
                        }
                        else
                        {
                            //adjust the stock Distributor
                            // _inventoryWorkflow.InventoryAdjust(rn.DocumentIssuerCostCentre.Id, li.Product.Id, -(li.Qty), DocumentType.ReturnsNote
                            //                                    , rn.Id, rn.DocumentDateIssued);
                            //adjust the stock Sales Man
                            // _inventoryWorkflow.InventoryAdjust(rn.DocumentRecipientCostCentre.Id, li.Product.Id, li.Qty, DocumentType.ReturnsNote
                            //                                  , rn.Id, rn.DocumentDateIssued);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmReturnsNoteCommandHandler exception", ex);
                throw ;
            }
        }
    }
}
