using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;

namespace ClientTestHarness.GenerateCommands
{
    public class GenerateIAN
    {
        public static Guid Run()
        {
            IInventoryRepository inventoryRepository = Services.Using<IInventoryRepository>();
            IConfigService configService = Services.Using<IConfigService>();
            IUserRepository userRepository = Services.Using<IUserRepository>();
            ICostCentreRepository costCentreRepository = Services.Using<ICostCentreRepository>();
            
            //fanta crate
            Guid testproductid =
                Services.Using<IProductRepository>()
                        .GetAll()
                        .First(t => t.Description.Contains("300ml Fanta  Crate"))
                        .Id;
            Product product = Services.Using<IProductRepository>().GetById(testproductid);
            
            Guid ccid = configService.Load().CostCentreId;
            Guid ccappid = configService.Load().CostCentreApplicationId;

            Inventory inventory = inventoryRepository.GetByProductIdAndWarehouseId(product.Id, ccid);
            

            User currentUser = userRepository.GetUser("kameme");
            CostCentre cc = costCentreRepository.GetById(ccid);
            IInventoryAdjustmentNoteFactory ianFactory = Services.Using<IInventoryAdjustmentNoteFactory>();
            InventoryAdjustmentNote inventoryAdjustmentNote = ianFactory.Create(cc, ccappid, cc, currentUser, "test doc",
                                                                              InventoryAdjustmentNoteType.AdjustOnly,
                                                                              Guid.Empty);
            decimal expected = inventory == null ? 0 : inventory.Balance;
            decimal actual = inventory == null ? 1 :   inventory.Balance + 1;
            InventoryAdjustmentNoteLineItem lineItem = ianFactory.CreateLineItem(actual, testproductid, expected, 0,
                                                                                 "Test adjustment");
            inventoryAdjustmentNote.AddLineItem(lineItem);

            inventoryAdjustmentNote.Confirm();
            IInventoryAdjustmentNoteWfManager wf = Services.Using<IInventoryAdjustmentNoteWfManager>();
            wf.SubmitChanges(inventoryAdjustmentNote,null);

            return inventoryAdjustmentNote.Id;
        }
    }
}
