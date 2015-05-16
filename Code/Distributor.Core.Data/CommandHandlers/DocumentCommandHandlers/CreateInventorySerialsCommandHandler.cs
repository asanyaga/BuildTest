using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.CommandHandler.EntityCommandHandlers.Inventory;
using Distributr.Core.Commands.EntityCommands.InventorySerials;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.InventoryRepository;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers
{
    public class CreateInventorySerialsCommandHandler : ICreateInventorySerialsCommandHandler
    {
        private IInventorySerialsRepository _inventorySerialsRepository;

        public CreateInventorySerialsCommandHandler(IInventorySerialsRepository inventorySerialsRepository)
        {
            _inventorySerialsRepository = inventorySerialsRepository;
        }

        ILog _log = LogManager.GetLogger("CreateInventorySerialsCommandHandler");
        public void Execute(CreateInventorySerialsCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());

            try
            {
                List<string> fromToList = command.CSVFromToList.Split(',').ToList();

                foreach (var item in fromToList)
                {
                    string[] fromTo = item.Split('-');
                    InventorySerials invSerials = new InventorySerials(command.EntityId)
                                                      {
                                                          CostCentreRef = new CostCentreRef {Id = command.RecipientCostCentreId},
                                                          DocumentId = command.DocumentId,
                                                          ProductRef = new ProductRef {ProductId = command.ProductId},
                                                          From = fromTo[0].Trim(),
                                                          To = fromTo[1].Trim()
                                                      };

                    _inventorySerialsRepository.AddInventorySerial(invSerials);
                }
            }
            catch(Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateInventorySerialsCommandHandler exception",ex);
                throw ;
            }
        }
    }
}
