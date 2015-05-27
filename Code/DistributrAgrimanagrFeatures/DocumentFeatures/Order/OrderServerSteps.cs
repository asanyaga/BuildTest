using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.Order
{
    [Binding]
    public class OrderServerSteps
    {
        private string section = "StockistPurchaseOrderServerSteps";
        public decimal amount = 10;

        [Given(@"I create an order \[server]")]
        public void GivenICreateAnOrderServer()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var config = testHelper.GetConfig();
            var user = testHelper.User();
            var settings = new ScenarioSettings()
            {
                User = user,
                HubConfig = config,
            };
            settings.OriginalQuantity = testHelper.Quantity(settings.DocumentId);
            settings.DistributorSalesman = testHelper.DistributorSalesman();
            settings.Outlet = testHelper.Outlet();

            var masterDataHelper = ObjectFactory.GetInstance<MasterDataHelper>();
            var product = masterDataHelper.GetSaleProduct();
            settings.ProductId = product.Id;
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreateMainOrder(settings);
            settings.DocumentId = note.Id;
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I create an order line item \[server]")]
        public void GivenICreateAnOrderLineItemServer()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            note.AddLineItem(testHelper.CreateFOCLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the order to its respective workflow \[server]")]
        public void WhenISubmitTheOrderToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
        }

        [When(@"I trigger a server sync from hub \(order\) \[server]")]
        public void WhenITriggerAServerSyncFromHubOrderServer()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [When(@"I approve the order in the client application \[server]")]
        public void WhenIApproveTheOrderInTheClientApplicationServer()
        {
            TI.trace(section, "#7");
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            note.Approve();
            note.ApproveLineItem(note.PendingApprovalLineItems.First());
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I trigger a server sync \(order\) in the client application")]
        public void WhenITriggerAServerSyncOrderInTheClientApplication()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [Then(@"There should be a saved order on the server \[server]")]
        public void ThenThereShouldBeASavedOrderOnTheServerServer()
        {
            TI.trace(section, "#5");
            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelperServer.Get(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId, note.Id);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the amount of the order line item should be the same as in the hub \[server]")]
        public void ThenTheAmountOfTheOrderLineItemShouldBeTheSameAsInTheHubServer()
        {
            TI.trace(section, "#6");

            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var qtyLocal = testHelper.Quantity(settings.DocumentId);
            var mainOrderLocal = testHelper.GetById(settings.DocumentId);

            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var qtyServer = testHelperServer.Quantity(settings.DocumentId);
            var mainOrderServer = testHelperServer.Get(settings.DocumentId);

            Assert.AreEqual(qtyLocal, qtyServer);
        }

        [Then(@"I should be able to fetch the order in server with a status of approved")]
        public void ThenIShouldBeAbleToFetchTheOrderInServerWithAStatusOfApproved()
        {
            TI.trace(section, "#8");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var mainOrderLocal = testHelper.GetById(settings.DocumentId);
            Assert.AreEqual(mainOrderLocal.PendingApprovalLineItems.Count, 0);
            Assert.AreEqual(mainOrderLocal.OrderStatus, OrderStatus.Inprogress);
        }

        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
                DocumentParentId = Guid.Empty;
                OrderType = OrderType.OutletToDistributor;
                DiscountType = DiscountType.FreeOfChargeDiscount;
            }
            public Guid DocumentId { get; set; }
            public Guid DocumentParentId { get; set; }

            public User User { get; set; }

            public Config HubConfig { get; set; }

            public Decimal OriginalQuantity { get; set; }

            public string DocumentReference { get; set; }
            public string ShipToAddress { get; set; }

            public DateTime DateRequired { get; set; }

            public decimal SaleDiscount { get; set; }

            public Guid ProductId { get; set; }

            public DistributorSalesman DistributorSalesman { get; set; }

            public Outlet Outlet { get; set; }

            public OrderType OrderType { get; set; }

            public DiscountType DiscountType { get; set; }
        }

        private class ScenarioTestHelperLocal
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IMainOrderFactory _mainOrderFactory;
            private IMainOrderRepository _mainOrderRepository;
            private IStockistPurchaseOrderWorkflow _stockistPurchaseOrderWorkflow;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;
            private IReceiveAndProcessPendingRemoteCommandEnvelopesService _sync;

            public ScenarioTestHelperLocal(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IMainOrderFactory mainOrderFactory, IMainOrderRepository mainOrderRepository, IStockistPurchaseOrderWorkflow stockistPurchaseOrderWorkflow, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService, IReceiveAndProcessPendingRemoteCommandEnvelopesService sync)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _mainOrderFactory = mainOrderFactory;
                _mainOrderRepository = mainOrderRepository;
                _stockistPurchaseOrderWorkflow = stockistPurchaseOrderWorkflow;
                _sendPendingEnvelopeCommandsService = sendPendingEnvelopeCommandsService;
                _sync = sync;
            }

            public Config GetConfig()
            {
                return _configService.Load();
            }

            public User User()
            {
                return _userRepository.GetAll().First(m => m.Username == "Kameme");
            }

            public DistributorSalesman DistributorSalesman()
            {
                return _costCentreRepository.GetAll().OfType<DistributorSalesman>().FirstOrDefault();
            }

            public Outlet Outlet()
            {
                return _costCentreRepository.GetAll().OfType<Outlet>().FirstOrDefault();
            }

            public MainOrder CreateMainOrder(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var pn = _mainOrderFactory.Create(cc, config.CostCentreApplicationId, localSetting.DistributorSalesman, localSetting.User, localSetting.Outlet, localSetting.OrderType, localSetting.DocumentReference, localSetting.DocumentParentId, localSetting.ShipToAddress, localSetting.DateRequired, localSetting.SaleDiscount,
                    "Main Order");

                return pn;
            }

            public SubOrderLineItem CreateFOCLineItem(ScenarioSettings localSettings)
            {
                var lineItem = _mainOrderFactory.CreateFOCLineItem(localSettings.ProductId, localSettings.OriginalQuantity,
                    "Line Item 1", localSettings.DiscountType);

                return lineItem;
            }

            public void SubmitToWF(MainOrder mainOrder)
            {
                _stockistPurchaseOrderWorkflow.Submit(mainOrder, null);
            }

            public MainOrder GetById(Guid Id)
            {
                return _mainOrderRepository.GetById(Id);
            }


            public decimal Quantity(Guid documentId)
            {
                var creditNote = _mainOrderRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.ItemSummary.FirstOrDefault().Qty;
            }

            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }
        }

        private class ScenarioTestHelperServer
        {
            private IMainOrderRepository _mainOrderRepository;

            public ScenarioTestHelperServer(IMainOrderRepository mainOrderRepository)
            {
                _mainOrderRepository = mainOrderRepository;
            }

            public MainOrder Get(Guid Id)
            {
                return _mainOrderRepository.GetById(Id);
            }

            public decimal Quantity(Guid documentId)
            {
                var creditNote = _mainOrderRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.ItemSummary.FirstOrDefault().Qty;
            }
        }
    }
}