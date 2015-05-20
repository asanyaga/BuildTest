using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    //public class CreateOrderCommandHandler : BaseCommandHandler, ICreateOrderCommandHandler
    //{
    //    private CokeDataContext _cokeDataContext;
    //    ILog _log = LogManager.GetLogger("CreateOrderCommandHandler");

    //    public CreateOrderCommandHandler(CokeDataContext cokeDataContext) : base(cokeDataContext)
    //    {
    //        _cokeDataContext = cokeDataContext;
    //    }

    //    public void Execute(CreateOrderCommand command)
    //    {
    //        _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //        try
    //        {
    //            if (DocumentExists(command.DocumentId))
    //                return;

    //            tblDocument doc = NewDocument(command, DocumentType.Order, command.DocumentRecipientCostCentreId);
    //            doc.SaleDiscount = command.SaleDiscount;
    //            doc.OrderIssuedOnBehalfOfCC = command.IssuedOnBehalfOfCostCentreId;
    //            doc.OrderDateRequired = command.DateOrderRequired;
    //            doc.OrderOrderTypeId = command.OrderTypeId;
                
    //            _cokeDataContext.tblDocument.AddObject(doc);
    //            _cokeDataContext.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
    //            _log.Error("CreateOrderCommandHandler exception", ex);
    //            throw ;
    //        }
    //    }

    //}
    public class CreateMainOrderCommandHandler : BaseCommandHandler, ICreateMainOrderCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("CreateMainOrderCommandHandler");

        public CreateMainOrderCommandHandler(CokeDataContext cokeDataContext)
            : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CreateMainOrderCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.Order, command.DocumentRecipientCostCentreId);
                doc.SaleDiscount = command.SaleDiscount;
                doc.OrderIssuedOnBehalfOfCC = command.IssuedOnBehalfOfCostCentreId;
                doc.OrderDateRequired = command.DateOrderRequired;
                doc.OrderOrderTypeId = command.OrderTypeId;
                doc.OrderStatusId = command.OrderStatusId;
                doc.DocumentParentId = command.ParentId;
                doc.OrderParentId = command.ParentId;
                doc.ShipToAddress = command.ShipToAddress;
                doc.Note = command.Note;
                doc.ExtDocumentReference = command.ExtDocumentReference;
                doc.StockistId = command.StockistId;
                doc.VisitId = command.VisitId;
                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateOrderCommandHandler exception", ex);
                throw;
            }
        }

    }
}
