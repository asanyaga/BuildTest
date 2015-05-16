using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.IAN
{
    [Binding]
    public class IANLocalSteps
    {
        private string section = "IANLocalSteps";
        private string productCode = "SP005";

        [Given(@"I have a product on the hub with a current stock level")]
        public void GivenIHaveAProductOnTheHubWithACurrentStockLevel()
        {
            TI.trace(section, "#1");
            TI.trace(section,"Choose product SP005");
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            Config c = th.GetConfig();
            Product testProduct = th.ProductByProductCode( productCode);
            User localUser = th.LocalUser();
            var settings = new ScenarioSettings { Product = testProduct, HubConfig = c, LocalUser = localUser };
            settings.OriginalInventoryLevel = th.InventoryLevel(settings);
            ScenarioContext.Current["settings"] = settings;
        }

        [Given(@"I create an IAN to adjust its stock level by ten")]
        public void GivenICreateAnIANToAdjustItsStockLevelByTen()
        {
            TI.trace(section, "#2");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            InventoryAdjustmentNote ian = th.CreateIAN(settings);
            decimal actual = settings.OriginalInventoryLevel + 10.0m;
            decimal expected = settings.OriginalInventoryLevel;
            InventoryAdjustmentNoteLineItem li = th.CreateIANLineItem(settings.Product.Id, actual, expected);
            TI.trace(section, string.Format("DocumentId {0}", ian.Id));
            ian.AddLineItem(li);
            ian.Confirm();
            settings.DocumentId = ian.Id;
            ScenarioContext.Current["settings"] = settings;
            ScenarioContext.Current["newian"] = ian;
        }

        [When(@"I submit the IAN to its respective workflow")]
        public void WhenISubmitTheIANToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            //ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            InventoryAdjustmentNote ian = ScenarioContext.Current["newian"] as InventoryAdjustmentNote;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            th.SubmitToWF(ian);
        }

        [Then(@"then the product stock level on the hub should have increased by ten")]
        public void ThenThenTheProductStockLevelOnTheHubShouldHaveIncreasedByTen()
        {
            TI.trace(section, "#4");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            decimal newLevel = th.InventoryLevel(settings);
            TI.trace(string.Format("Old level {0} , New level {1}", settings.OriginalInventoryLevel, newLevel));
            Assert.AreEqual(settings.OriginalInventoryLevel + 10.0m, newLevel);
        }

        [Then(@"there should be a saved IAN document")]
        public void ThenThereShouldBeASavedIANDocument()
        {
            TI.trace(section, "#6");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            InventoryAdjustmentNote ian = th.Get(settings.DocumentId);
            Assert.IsNotNull(ian);
            Assert.AreEqual(settings.DocumentId, ian.Id);
            Assert.AreEqual(DocumentStatus.Confirmed, ian.Status);
        }

        [Then(@"there should be a corresponding command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingCommandEnvelopeOnTheOutgoingCommandQueue()
        {
            TI.trace(section, "#6");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var r = th.GetConfirmEnvelopeEntryInOutboundQueue(settings.DocumentId);
            Assert.AreEqual(1, r.Count());
            OutGoingCommandEnvelopeQueueItemLocal item = r[0];
            Assert.AreEqual ( settings.DocumentId, item.DocumentId);
            CommandEnvelope envelope = JsonConvert.DeserializeObject<CommandEnvelope>(item.JsonEnvelope);
            Assert.AreEqual(3, envelope.CommandsList.Count());
        }

        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId  = Guid.Empty;
            }
            public Product Product { get; set; }
            public decimal OriginalInventoryLevel { get; set; }
            public Guid DocumentId { get; set; }
            public Config HubConfig { get; set; }
            public User LocalUser { get; set; }
        }

        private class ScenarioTestHelper
        {
            private IProductRepository _productRepository;
            private IInventoryRepository _inventoryRepository;
            private IConfigService _configService;
            private ICostCentreRepository _costCentreRepository;
            private IInventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
            private IInventoryAdjustmentNoteWfManager _inventoryAdjustmentNoteWfManager;
            private IUserRepository _userRepository;
            private IInventoryAdjustmentNoteRepository _inventoryAdjustmentNoteRepository;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;


            public ScenarioTestHelper(IProductRepository productRepository, IInventoryRepository inventoryRepository, IConfigService configService, ICostCentreRepository costCentreRepository, IInventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory, IInventoryAdjustmentNoteWfManager inventoryAdjustmentNoteWfManager, IUserRepository userRepository, IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteRepository, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository)
            {
                _productRepository = productRepository;
                _inventoryRepository = inventoryRepository;
                _configService = configService;
                _costCentreRepository = costCentreRepository;
                _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
                _inventoryAdjustmentNoteWfManager = inventoryAdjustmentNoteWfManager;
                _userRepository = userRepository;
                _inventoryAdjustmentNoteRepository = inventoryAdjustmentNoteRepository;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
            }

            public Config GetConfig()
            {
                return _configService.Load();
            }

            public Product ProductByProductCode(string productCode)
            {
                return _productRepository.GetAll().First(n => n.ProductCode == productCode);
            }

            public decimal InventoryLevel( ScenarioSettings settings)
            {
                Inventory inv = _inventoryRepository.GetByProductIdAndWarehouseId(settings.Product.Id, settings.HubConfig.CostCentreId);
                return inv == null ? 0 : inv.Balance;
            }

            public User LocalUser()
            {
               return _userRepository.GetAll().First(n => n.Username == "Kameme");
            }

            public InventoryAdjustmentNote CreateIAN(ScenarioSettings localSetting)
            {
                CostCentre cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                Config c = localSetting.HubConfig;
                var note = _inventoryAdjustmentNoteFactory.Create(cc, c.CostCentreApplicationId, cc, localSetting.LocalUser,
                    Guid.NewGuid().ToString().Substring(1, 10), InventoryAdjustmentNoteType.Available, Guid.Empty);
                return note;
            }

            public InventoryAdjustmentNoteLineItem CreateIANLineItem(Guid productId, decimal actual, decimal expected)
            {
                return _inventoryAdjustmentNoteFactory.CreateLineItem(actual, productId, expected, 10,
                    "Desc" + Guid.NewGuid().ToString());
            }

            public void SubmitToWF(InventoryAdjustmentNote ian)
            {
                _inventoryAdjustmentNoteWfManager.SubmitChanges(ian, null);
            }

            public InventoryAdjustmentNote Get(Guid documentId)
            {
                return _inventoryAdjustmentNoteRepository.GetById(documentId);
            }

            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);
               
            }

        }
   
    
    }
}
