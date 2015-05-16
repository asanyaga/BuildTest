using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;


namespace Distributr.Core.Workflow.InventoryWorkflow.Impl
{
    public class InventoryWorkflow : IInventoryWorkflow
    {
        IProductRepository _productRepository;
        IInventoryRepository _inventoryRepository;
        IInventoryTransactionRepository _inventoryTransactionRepository;
        ICostCentreRepository _costCentreRepository;
       // ILog _log = LogManager.GetLogger("InventoryWorkflow");
        public InventoryWorkflow(ICostCentreRepository costCentreRepository, IProductRepository productRepository, IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository)
        {
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _costCentreRepository = costCentreRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
        }


        public void InventoryAdjust(Guid costCentreId, Guid productId, decimal qty, DocumentType docType, Guid documentId, DateTime date, InventoryAdjustmentNoteType inventoryAdjustmentNoteType)
        {
           // _log.InfoFormat("Inventory Adjust costcentreid : {0} - productid : {1} - qty : {2} - doctype : {3} - docid : {4}", costCentreId, productId, qty, docType, documentId);
            try
            {
                Product p = _productRepository.GetById(productId);
                CostCentre cc = _costCentreRepository.GetById(costCentreId);
                if (!(cc is Warehouse))
                    throw new Exception("Can only have inventory in a cost centre that is a warehouse");
                //does inventory item exist for costcentre
                if (!_inventoryRepository.GetByProductId(productId).Any(n => n.Warehouse.Id == costCentreId))
                {
                    var inv = new Inventory(Guid.NewGuid())
                    {
                        Balance = 0,
                        Value = 0,
                        Product = p,
                        Warehouse = (Warehouse)cc
                    };
                    _inventoryRepository.AddInventory(inv);
                }

                Inventory inv1 = _inventoryRepository.GetByProductId(productId).First(n => n.Warehouse.Id == costCentreId);
                var it = new InventoryTransaction(Guid.NewGuid())
                {
                    DateInserted = DateTime.Now,
                    DocumentId = documentId,
                    DocumentType = docType,
                    Inventory = inv1,
                    NoItems = qty,

                };
                _inventoryTransactionRepository.Add(it);

                _inventoryRepository.AdjustInventoryBalance(inv1.Id, qty, (int)inventoryAdjustmentNoteType);

            }
            catch (Exception ex)
            {
                //_log.Error(ex);
            }
        }

    }
}
