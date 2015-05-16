using System;
using System.Collections.Generic;
using System.Linq;
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
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using TechTalk.SpecFlow;
using StructureMap;
using System.Threading.Tasks;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.ITN
{
    [Binding]
    public class InventoryTransferNoteToServerSteps
    {
        private string section = "InventoryTransferNoteServerSteps";
        private string productCode = "SP005";
        [Given(@"I have a product stock on the hub  that i want to issue to as a salesman \[server]")]
        public void GivenIHaveAProductStockOnTheHubThatIWantToIssueToAsASalesmanServer()
        {
            TI.trace(section, "#1");
            TI.trace(section, "Choose product SP005");
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
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

        [Given(@"I create an ITN to issue as  salesman ten unit of stock   \[server]")]
        public void GivenICreateAnITNToIssueAsSalesmanTenUnitOfStockServer()
        {
            TI.trace(section, "#2");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            InventoryTransferNote ian = th.CreateITN(settings);
            decimal quantity = 10.0m;

            InventoryTransferNoteLineItem li = th.CreateITNLineItem(settings.Product.Id, quantity);
            TI.trace(section, string.Format("DocumentId {0}", ian.Id));
            ian.AddLineItem(li);
            ian.Confirm();
            settings.DocumentId = ian.Id;
            ScenarioContext.Current["settings"] = settings;
            ScenarioContext.Current["newian"] = ian;
        }

        [When(@"I submit the ITN to its respective workflow \[server]")]
        public void WhenISubmitTheITNToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");

            InventoryTransferNote itn = ScenarioContext.Current["newian"] as InventoryTransferNote;
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            th.SubmitToWF(itn);
        }

        [When(@"I trigger a server sync send ITN to the server")]
        public void WhenITriggerAServerSyncSendITNToTheServer()
        {
            TI.trace(section, "#4");
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = th.SendPendingCommands().Result;
            }
        }

        [Then(@"there should be a saved ITN document on the server")]
        public void ThenThereShouldBeASavedITNDocumentOnTheServer()
        {
            TI.trace(section, "#5");
            Autofac.IContainer c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            ScenarioTestHelperServer sth = c.Resolve<ScenarioTestHelperServer>();
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            InventoryTransferNote ian = sth.Get(settings.DocumentId);
            Assert.IsNotNull(ian);
            Assert.AreEqual(settings.DocumentId, ian.Id);
        }

        [Then(@"the salesman product stock level on the server should be the same as the hub")]
        public void ThenTheSalesmanProductStockLevelOnTheServerShouldBeTheSameAsTheHub()
        {
            TI.trace(section, "#6");

            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelperLocal sthl = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            decimal levelLocal = sthl.SalesmanInventoryLevel(settings);

            Autofac.IContainer c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            ScenarioTestHelperServer sth = c.Resolve<ScenarioTestHelperServer>();
            decimal levelServer = sth.SalesmanInventoryLevel(settings);

            Assert.AreEqual(levelLocal, levelServer);
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
        private class ScenarioTestHelperLocal
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
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

            public ScenarioTestHelperLocal(IProductRepository productRepository, IInventoryRepository inventoryRepository, IConfigService configService, ICostCentreRepository costCentreRepository, IUserRepository userRepository, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IInventoryTransferNoteFactory inventoryTransferNoteFactory, IInventoryTransferNoteRepository inventoryTransferNoteRepository, IConfirmInventoryTransferNoteWFManager transferNoteWfManager, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
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
                _sendPendingEnvelopeCommandsService = sendPendingEnvelopeCommandsService;
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
                var note = _inventoryTransferNoteFactory.Create(cc, c.CostCentreApplicationId, localSetting.LocalUser, localSetting.Salesman, cc,
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
            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }
        }
        private class ScenarioTestHelperServer
        {
            private IInventoryRepository _inventoryRepository;
            private IInventoryTransferNoteRepository _inventoryTransferNoteRepository;

            public ScenarioTestHelperServer(IInventoryTransferNoteRepository inventoryTransferNoteRepository, IInventoryRepository inventoryRepository)
            {
                _inventoryTransferNoteRepository = inventoryTransferNoteRepository;
                _inventoryRepository = inventoryRepository;
            }


            public InventoryTransferNote Get(Guid documentId)
            {
                return _inventoryTransferNoteRepository.GetById(documentId);
            }
            public decimal SalesmanInventoryLevel(ScenarioSettings settings)
            {
                Inventory inv = _inventoryRepository.GetByProductIdAndWarehouseId(settings.Product.Id, settings.Salesman.Id);
                return inv == null ? 0 : inv.Balance;
            }
        }
    }
}
