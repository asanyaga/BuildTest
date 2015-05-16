using System;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Recollections;
using Distributr.Core.Commands.DocumentCommands.Recollections;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Recollections
{
    public class ReCollectionCommandHandler :BaseSourcingCommandHandler, IReCollectionCommandHandler
    {

        ILog _log = LogManager.GetLogger("ReCollectionCommandHandler");
        public ReCollectionCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(ReCollectionCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                DateTime date = DateTime.Now;
                tblRecollection doc = _context.tblRecollection.FirstOrDefault(s => s.Id == command.DocumentId);
                if (doc == null)
                {   doc= new tblRecollection();
                    doc.Id = command.DocumentId;
                    doc.IM_DateCreated = date;
                    doc.IsReceived = false;
                    doc.IM_Status = (int) EntityStatus.Active;
                    doc.Amount = command.Amount;
                    doc.FromCostCentreId = command.FromCostCentreId;
                    doc.CostCentreId = command.CommandGeneratedByCostCentreId;
                    doc.Description = command.Description;
                    doc.DateInserted = command.CommandCreatedDateTime;
                    _context.tblRecollection.AddObject(doc);
                   
                }
                else if (doc != null && command.ItemId!=Guid.Empty)
                {
                    var item= doc.tblRecollectionItem.FirstOrDefault(s => s.Id == command.ItemId);

                    if (item != null) item.IsComfirmed = true;
                }
                else
                {
                    tblRecollectionItem item=new tblRecollectionItem();
                    item.Id = command.CommandId;
                    item.Amount = command.PaidAmount;
                    item.DateInserted = command.CommandCreatedDateTime;
                    item.RecollectionId = doc.Id;
                    item.IM_DateCreated = date;
                    item.IM_DateLastUpdated = date;
                    item.IM_Status = (int)EntityStatus.Active;
                    item.CollectionModeId = command.RecollectionType;
                    doc.tblRecollectionItem.Add(item);
                }
                doc.IM_DateLastUpdated = date;
                _context.SaveChanges();
              
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ReCollectionCommandHandler exception ", ex);
                throw;
            }
        }
    }
}
