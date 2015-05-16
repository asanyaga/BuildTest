using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands;
using Distributr.Core.Commands.EntityCommands.InventorySerials;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.InventoriesDTO;

namespace Distributr.Core.Workflow.Impl.InventoryWorkFlow
{
    public class InventorySerialsWorkFlow : IInventorySerialsWorkFlow
    {
        private IOutgoingMasterDataRouter _outgoingMasterDataRouter;
        

        public InventorySerialsWorkFlow(IOutgoingMasterDataRouter outgoingMasterDataRouter)
        {
            _outgoingMasterDataRouter = outgoingMasterDataRouter;
        }

        public void Save(List<InventorySerials> inventorySerials, Document document, BasicConfig config)
        {
            //SaveToLocal(inventorySerials);
            SendCommands(inventorySerials, document,config);
        }

        private void SaveToLocal(List<InventorySerials> inventorySerials)
        {
            
        }

        private void SendCommands(List<InventorySerials> inventorySerials, Document doc, BasicConfig config)
        {
            var commands = CreateSerialsCommands(inventorySerials, doc,config);
            //send by backgroud worker
        }

        private List<ICommand> CreateSerialsCommands(List<InventorySerials> inventorySerials, Document doc,BasicConfig config)
        {
            List<ICommand> commands = new List<ICommand>();
            var inventorySerialsByProduct = inventorySerials.GroupBy(n => n.ProductRef.ProductId);
            foreach (IGrouping<Guid, InventorySerials> group in inventorySerialsByProduct)
            {
                string csvFromToSerials = "";
                foreach (InventorySerials serial in group)
                {
                    //Console.WriteLine("From: " + serial.From + " To: " + serial.To);
                    string fromTo = serial.From +"-"+serial.To;
                    csvFromToSerials += fromTo + ",";
                }

                CreateInventorySerialsCommand cmd =
                    new CreateInventorySerialsCommand(
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        doc.Id,
                        Guid.Empty, //TODO Resolve Current User Id
                        //_configService.ViewModelParameters.CurrentUserId,
                        config.CostCentreId,
                        0,
                        config.CostCentreApplicationId,
                        doc.DocumentParentId,
                        group.Key,
                        //doc.DocumentRecipientCostCentre.Id,
                        group.First().CostCentreRef.Id,
                        csvFromToSerials,
                        DateTime.Now
                        );
            }

            return commands;
        }

        public void SubmitInventorySerials(List<InventorySerials> inventorySerials)
        {
            List<InventorySerialsDTO> dtos = Map(inventorySerials);

            foreach (InventorySerialsDTO dto in dtos)
            {
                _outgoingMasterDataRouter.RouteMasterData(dto, MasterDataDTOSaveCollective.InventorySerials);
            }

        }


        List<InventorySerialsDTO> Map(List<InventorySerials> inventorySerials)
        {
            List<InventorySerialsDTO> dtos = new List<InventorySerialsDTO>();
            var inventorySerialsByProduct = inventorySerials.GroupBy(n => n.ProductRef.ProductId);
            foreach (IGrouping<Guid, InventorySerials> group in inventorySerialsByProduct)
            {
                string csvFromToSerials = "";
                foreach (InventorySerials serial in group)
                {
                    //Console.WriteLine("From: " + serial.From + " To: " + serial.To);
                    string fromTo = serial.From + "-" + serial.To;
                    csvFromToSerials += fromTo + ",";
                }
                InventorySerialsDTO dto = new InventorySerialsDTO();
                dto.MasterId = Guid.NewGuid();
                dto.CostCentreMasterId = group.First().CostCentreRef.Id;
                dto.ProductMasterId = group.Key;
                dto.DocumentId = group.First().DocumentId;
                dto.CSVFromToSerials = csvFromToSerials;
                dto.DateCreated = DateTime.Now;
                dto.DateLastUpdated = DateTime.Now;
                dto.StatusId = (int)EntityStatus.Active;

                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
