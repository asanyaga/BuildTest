using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
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
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.DocumentFeatures.IAN;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.ITN
{
    [Binding]
    public class InventoryTransferNoteLocalSteps
    {
        private string section = "InventoryTransferNoteLocalSteps";
        private string productCode = "SP005";
        [Given(@"I have inventory on the hub with a current stock level")]
        public void GivenIHaveInventoryOnTheHubWithACurrentStockLevel()
        {
            TI.trace(section, "#1");
            TI.trace(section, "Choose product SP005");
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            Config c = th.GetConfig();
            Product testProduct = th.ProductByProductCode(productCode);
            User localUser = th.LocalUser();
            var salesman = th.Salesman("S001");
            var settings = new ScenarioSettings { Product = testProduct, HubConfig = c, LocalUser = localUser, Salesman = salesman };
            settings.HubOriginalQuantity = th.HubInventoryLevel(settings);
            TI.trace(section, "HubOriginalQuantity " + settings.HubOriginalQuantity);
            settings.SalesmanOriginalQuantity = th.SalesmanInventoryLevel(settings);
            TI.trace(section, "SalesmanOriginalQuantity " + settings.SalesmanOriginalQuantity);
            ScenarioContext.Current["settings"] = settings;
        }

        [Given(@"I create an ITN to adjust its level by ten")]
        public void GivenICreateAnITNToAdjustItsLevelByTen()
        {
            TI.trace(section, "#2");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            InventoryTransferNote ian = th.CreateITN(settings);
            decimal quantity =  10.0m;
          
            InventoryTransferNoteLineItem li = th.CreateITNLineItem(settings.Product.Id, quantity);
            TI.trace(section, string.Format("DocumentId {0}", ian.Id));
            ian.AddLineItem(li);
            ian.Confirm();
            settings.DocumentId = ian.Id;
            ScenarioContext.Current["settings"] = settings;
            ScenarioContext.Current["newian"] = ian;
        }

        [When(@"I submit the ITN to its respective workflow")]
        public void WhenISubmitTheITNToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            
            InventoryTransferNote itn = ScenarioContext.Current["newian"] as InventoryTransferNote;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            th.SubmitToWF(itn);
        }

        [Then(
            @"then the inventory level on the hub should have decreased by ten and increased the salesman inventory by ten"
            )]
        public void ThenThenTheInventoryLevelOnTheHubShouldHaveDecreasedByTenAndIncreasedTheSalesmanInventoryByTen()
        {
            TI.trace(section, "#4");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            decimal newHubLevel = th.HubInventoryLevel(settings);
            decimal newSalesmanLevel = th.SalesmanInventoryLevel(settings);
            TI.trace(string.Format("Hub Old level {0} ,Hub New level {1}", settings.HubOriginalQuantity, newHubLevel));
            TI.trace(string.Format("Salesman Old level {0} ,Salesman New level {1}", settings.SalesmanOriginalQuantity, newSalesmanLevel));
            Assert.AreEqual(settings.HubOriginalQuantity - 10.0m, newHubLevel);
            Assert.AreEqual(settings.SalesmanOriginalQuantity + 10.0m, newSalesmanLevel);
        }

        [Then(@"there should be a saved ITN document")]
        public void ThenThereShouldBeASavedITNDocument()
        {
            TI.trace(section, "#6");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            InventoryTransferNote itn = th.Get(settings.DocumentId);
            Assert.IsNotNull(itn);
            Assert.AreEqual(settings.DocumentId, itn.Id);
            Assert.AreEqual(DocumentStatus.Confirmed, itn.Status);
        }

        [Then(@"there should be a corresponding ITN command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingITNCommandEnvelopeOnTheOutgoingCommandQueue()
        {
            TI.trace(section, "#6");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var r = th.GetConfirmEnvelopeEntryInOutboundQueue(settings.DocumentId);
            Assert.AreEqual(1, r.Count());
            OutGoingCommandEnvelopeQueueItemLocal item = r[0];
            Assert.AreEqual(settings.DocumentId, item.DocumentId);
            CommandEnvelope envelope = JsonConvert.DeserializeObject<CommandEnvelope>(item.JsonEnvelope);
            Assert.AreEqual(3, envelope.CommandsList.Count());
            Assert.AreEqual((int)DocumentType.InventoryTransferNote, envelope.DocumentTypeId);
        }

        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
            }
            public Product Product { get; set; }
            public decimal HubOriginalQuantity { get; set; }
            public decimal SalesmanOriginalQuantity { get; set; }
            public Guid DocumentId { get; set; }
            public Config HubConfig { get; set; }
            public User LocalUser { get; set; }
            public DistributorSalesman Salesman { get; set; }
        }
        private class ScenarioTestHelper
        {
            private IProductRepository _productRepository;
            private IInventoryRepository _inventoryRepository;
            private IConfigService _configService;
            private ICostCentreRepository _costCentreRepository;
            private IInventoryTransferNoteFactory _inventoryTransferNoteFactory;
            private IConfirmInventoryTransferNoteWFManager _transferNoteWfManager;
            private IUserRepository _userRepository;
            private IInventoryTransferNoteRepository _inventoryTransferNoteRepository;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;


            public ScenarioTestHelper(IProductRepository productRepository, IInventoryRepository inventoryRepository, IConfigService configService, ICostCentreRepository costCentreRepository,  IUserRepository userRepository, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IInventoryTransferNoteFactory inventoryTransferNoteFactory, IInventoryTransferNoteRepository inventoryTransferNoteRepository, IConfirmInventoryTransferNoteWFManager transferNoteWfManager)
            {
                _productRepository = productRepository;
                _inventoryRepository = inventoryRepository;
                _configService = configService;
                _costCentreRepository = costCentreRepository;
               
                
                _userRepository = userRepository;
               
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
                _inventoryTransferNoteRepository = inventoryTransferNoteRepository;
                _transferNoteWfManager = transferNoteWfManager;
            }

            public Config GetConfig()
            {
                return _configService.Load();
            }
            public DistributorSalesman Salesman(string salesmanCode)
            {
                return _costCentreRepository.GetByCode(salesmanCode, CostCentreType.DistributorSalesman) as DistributorSalesman;
            }
            public Product ProductByProductCode(string productCode)
            {
                return _productRepository.GetAll().First(n => n.ProductCode == productCode);
            }

            public decimal HubInventoryLevel(ScenarioSettings settings)
            {
                Inventory inv = _inventoryRepository.GetByProductIdAndWarehouseId(settings.Product.Id, settings.HubConfig.CostCentreId);
                return inv == null ? 0 : inv.Balance;
            }
            public decimal SalesmanInventoryLevel(ScenarioSettings settings)
            {
                Inventory inv = _inventoryRepository.GetByProductIdAndWarehouseId(settings.Product.Id, settings.Salesman.Id);
                return inv == null ? 0 : inv.Balance;
            }
            public User LocalUser()
            {
                return _userRepository.GetAll().First(n => n.Username == "Kameme");
            }

            public InventoryTransferNote CreateITN(ScenarioSettings localSetting)
            {
                CostCentre cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                Config c = localSetting.HubConfig;
                var note = _inventoryTransferNoteFactory.Create(cc, c.CostCentreApplicationId, localSetting.LocalUser,localSetting.Salesman,cc,
                    Guid.NewGuid().ToString().Substring(1, 10));
                return note;
            }

            public InventoryTransferNoteLineItem CreateITNLineItem(Guid productId, decimal quantity)
            {
                return _inventoryTransferNoteFactory.CreateLineItem(productId, quantity, 0, 0, "Desc" + Guid.NewGuid().ToString());
            }

            public void SubmitToWF(InventoryTransferNote ian)
            {
                _transferNoteWfManager.SubmitChanges(ian, null);
            }

            public InventoryTransferNote Get(Guid documentId)
            {
                return _inventoryTransferNoteRepository.GetById(documentId);
            }

            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);

            }

        }
    }
}
