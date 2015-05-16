using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
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
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.IAN
{
    [Binding]
    public  class IANServerSteps
    {
        private string section = "IANServerSteps";
        private string productCode = "SP005";

        [Given(@"I have a product on the hub with a current stock level \[server]")]
        public void GivenIHaveAProductOnTheHubWithACurrentStockLevelServer()
        {
            TI.trace(section, "#1");
            TI.trace(section, "Choose product SP005");
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            Config c = th.GetConfig();
            Product testProduct = th.ProductByProductCode(productCode);
            User localUser = th.LocalUser();
            var settings = new ScenarioSettings { Product = testProduct, HubConfig = c, LocalUser = localUser };
            settings.OriginalInventoryLevel = th.InventoryLevel(settings);
            ScenarioContext.Current["settings"] = settings;

        }

        [Given(@"I create an IAN to adjust its stock level by ten \[server]")]
        public void GivenICreateAnIANToAdjustItsStockLevelByTenServer()
        {
            TI.trace(section, "#2");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
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

        [When(@"I submit the IAN to its respective workflow \[server]")]
        public void WhenISubmitTheIANToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            InventoryAdjustmentNote ian = ScenarioContext.Current["newian"] as InventoryAdjustmentNote;
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            th.SubmitToWF(ian);
        }

        [When(@"I trigger a server sync")]
        public void WhenITriggerAServerSync()
        {
            TI.trace(section, "#4");
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = th.SendPendingCommands().Result;
            }
        }

        [Then(@"there should be a saved IAN document on the server")]
        public void ThenThereShouldBeASavedIANDocumentOnTheServer()
        {
            TI.trace(section, "#5");
            Autofac.IContainer c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            ScenarioTestHelperServer sth = c.Resolve<ScenarioTestHelperServer>();
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            InventoryAdjustmentNote ian = sth.Get(settings.DocumentId);
            Assert.IsNotNull(ian);
            Assert.AreEqual(settings.DocumentId, ian.Id);
            //More.....

        }

        [Then(@"the product stock level on the server should be the same as the hub")]
        public void ThenTheProductStockLevelOnTheServerShouldBeTheSameAsTheHub()
        {
            TI.trace(section, "#6");
            
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelperLocal sthl = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            decimal levelLocal = sthl.InventoryLevel(settings);

            Autofac.IContainer c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            ScenarioTestHelperServer sth = c.Resolve<ScenarioTestHelperServer>();
            decimal levelServer = sth.InventoryLevel(settings);
            
            Assert.AreEqual(levelLocal, levelServer);
        }

        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
            }
            public Product Product { get; set; }
            public decimal OriginalInventoryLevel { get; set; }
            public Guid DocumentId { get; set; }
            public Config HubConfig { get; set; }
            public User LocalUser { get; set; }
        }

        

        private class ScenarioTestHelperLocal
        {
            private IConfigService _configService;
            private IProductRepository _productRepository;
            private IUserRepository _userRepository;
            private IInventoryRepository _inventoryRepository;
            private ICostCentreRepository _costCentreRepository;
            private IInventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
            private IInventoryAdjustmentNoteWfManager _inventoryAdjustmentNoteWfManager;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

            public ScenarioTestHelperLocal(IConfigService configService, IProductRepository productRepository, IUserRepository userRepository, IInventoryRepository inventoryRepository, ICostCentreRepository costCentreRepository, IInventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory, IInventoryAdjustmentNoteWfManager inventoryAdjustmentNoteWfManager, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
            {
                _configService = configService;
                _productRepository = productRepository;
                _userRepository = userRepository;
                _inventoryRepository = inventoryRepository;
                _costCentreRepository = costCentreRepository;
                _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
                _inventoryAdjustmentNoteWfManager = inventoryAdjustmentNoteWfManager;
                _sendPendingEnvelopeCommandsService = sendPendingEnvelopeCommandsService;
            }

            public Config GetConfig()
            {
                return _configService.Load();
            }

            public Product ProductByProductCode(string productCode)
            {
                return _productRepository.GetAll().First(n => n.ProductCode == productCode);
            }
            public decimal InventoryLevel(ScenarioSettings settings)
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

            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }

        }

        private class ScenarioTestHelperServer
        {
            private IInventoryRepository _inventoryRepository;
            private IInventoryAdjustmentNoteRepository _inventoryAdjustmentNoteRepository;

            public ScenarioTestHelperServer(IInventoryRepository inventoryRepository, IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteRepository)
            {
                _inventoryRepository = inventoryRepository;
                _inventoryAdjustmentNoteRepository = inventoryAdjustmentNoteRepository;
            }

            public InventoryAdjustmentNote Get(Guid documentId)
            {
                return _inventoryAdjustmentNoteRepository.GetById(documentId);
            }
            public decimal InventoryLevel(ScenarioSettings settings)
            {
                Inventory inv = _inventoryRepository.GetByProductIdAndWarehouseId(settings.Product.Id, settings.HubConfig.CostCentreId);
                return inv == null ? 0 : inv.Balance;
            }
        }

    }
}
