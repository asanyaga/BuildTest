using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DisbursementNotes;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DisbursementNotes
{
    public class AddDisbursementNoteLineItemCommandHandler : IAddDisbursementNoteLineItemCommandHandler
    {
       
        ICostCentreRepository _costCenterRepository;
        IUserRepository _userRepository;
        IDisbursementNoteRepository _documentRepository;
        IProductRepository _productRepository;
        ILog _log = LogManager.GetLogger("AddDisbursementNoteLineItemCommandHandler");


        public AddDisbursementNoteLineItemCommandHandler(
            ICostCentreRepository costCenterRepository,
            IUserRepository userRepository,
            IDisbursementNoteRepository documentRepository,
          
            IProductRepository productRepository
            )
        {
            _costCenterRepository = costCenterRepository;
            _userRepository = userRepository;
          
            _documentRepository = documentRepository;
            _productRepository = productRepository;
        }
        public void Execute(AddDisbursementNoteLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());

            try
            {
                bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;
                if (!already_Exist)
                    return;
                DisbursementNote disbursementNote = _documentRepository.GetById(command.DocumentId) as DisbursementNote;
                DisbursementNoteLineItem dispatchNoteLineItem = new DisbursementNoteLineItem(command.CommandId);
                dispatchNoteLineItem.LineItemSequenceNo = command.LineItemSequenceNo;
                dispatchNoteLineItem.Product = _productRepository.GetById(command.ProductId);
                dispatchNoteLineItem.Qty = command.Qty;
                dispatchNoteLineItem.Value = command.Value;
                disbursementNote.AddLineItem(dispatchNoteLineItem);
                _documentRepository.Save(disbursementNote);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
               // _log.Error(ex);
                throw;
            }
        }

   
    }
}
