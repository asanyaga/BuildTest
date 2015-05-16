﻿using System;
using System.Linq;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers
{
    public class AddCommodityPurchaseLineItemCommandHandler :BaseSourcingCommandHandler, IAddCommodityPurchaseLineItemCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("AddCommodityPurchaseLineItemCommandHandler");


        public AddCommodityPurchaseLineItemCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(AddCommodityPurchaseLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }
                tblSourcingDocument doc = ExistingDocument(command.DocumentId);
                tblSourcingLineItem lineItem = NewLineItem(command.DocumentLineItemId, command.ParentLineItemId, command.DocumentId, command.CommodityId, command.CommodityGradeId, command.ContainerTypeId, command.Weight, command.Description, command.ContainerNo);
                lineItem.NoOfContainer = command.NoOfContainers;
                lineItem.TareWeight = command.TareWeight;
                doc.tblSourcingLineItem.Add(lineItem);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddCommodityDeliveryLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
