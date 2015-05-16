using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
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

namespace DistributrAgrimanagrFeatures.DocumentFeatures.PO
{
    [Binding]
    public class PurchaseOrderServerSteps
    {
        private string section = "PurchaseOrderServerSteps";
        public decimal amount = 10;
        public decimal LineItemVatValue = 0.16m;

        [Given(@"I create a purchase order \[server]")]
        public void GivenICreateAPurchaseOrderServer()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var config = testHelper.GetConfig();
            var user = testHelper.User();
            var settings = new ScenarioSettings()
            {
                User = user,
                HubConfig = config,
                LineItemVatValue = LineItemVatValue,
            };
            settings.OriginalQuantity = testHelper.Quantity(settings.DocumentId);
            settings.OriginalValue = testHelper.Value(settings.DocumentId);

            var masterDataHelper = ObjectFactory.GetInstance<MasterDataHelper>();
            var product = masterDataHelper.GetSaleProduct();
            settings.ProductId = product.Id;
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreateMainOrder(settings);
            settings.DocumentId = note.Id;
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I create a purchase order line item \[server]")]
        public void GivenICreateAPurchaseOrderLineItemServer()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            note.AddLineItem(testHelper.CreateMainOrderLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the purchase order to its respective workflow \[server]")]
        public void WhenISubmitThePurchaseOrderToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
        }

        [When(@"I trigger a server sync from hub \(purchase order\) \[server]")]
        public void WhenITriggerAServerSyncFromHubPurchaseOrderServer()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [When(@"I approve the purchase order in the server \[server]")]
        public void WhenIApproveThePurchaseOrderInTheServerServer()
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

        [Then(@"There should be a saved purchase order on the server \[server]")]
        public void ThenThereShouldBeASavedPurchaseOrderOnTheServerServer()
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

        [Then(@"I should be able to fetch the purchase order in client application with a status of approved")]
        public void ThenIShouldBeAbleToFetchThePurchaseOrderInClientApplicationWithAStatusOfApproved()
        {
            TI.trace(section, "#8");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var mainOrderLocal = testHelper.GetById(settings.DocumentId);
            Assert.AreEqual(mainOrderLocal.PendingApprovalLineItems.Count,0);
            Assert.AreEqual(mainOrderLocal.OrderStatus,OrderStatus.Inprogress);
        }

        [Then(@"the amount of the purchase order line item should be the same as in the hub \[server]")]
        public void ThenTheAmountOfThePurchaseOrderLineItemShouldBeTheSameAsInTheHubServer()
        {
            TI.trace(section, "#6");

            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var valueLocal = testHelper.Value(settings.DocumentId);
            var qtyLocal = testHelper.Quantity(settings.DocumentId);
            var mainOrderLocal = testHelper.GetById(settings.DocumentId);

            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var valueServer = testHelperServer.Value(settings.DocumentId);
            var qtyServer = testHelperServer.Quantity(settings.DocumentId);
            var mainOrderServer = testHelperServer.Get(settings.DocumentId);

            Assert.AreEqual(valueLocal, valueServer);
            Assert.AreEqual(qtyLocal, qtyServer);
        }

        [When(@"I trigger a server sync \(purchase order\) in the client application")]
        public void WhenITriggerAServerSyncPurchaseOrderInTheClientApplication()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                bool r = testHelper.Sync(settings).Result;
            }
        }


        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
                DocumentParentId = Guid.Empty;
                OrderType = OrderType.DistributorToProducer;
            }
            public Guid DocumentId { get; set; }
            public Guid DocumentParentId { get; set; }

            public User User { get; set; }

            public Config HubConfig { get; set; }

            public Decimal OriginalValue { get; set; }
            public Decimal OriginalQuantity { get; set; }

            public string DocumentReference { get; set; }
            public string ShipToAddress { get; set; }

            public DateTime DateRequired { get; set; }

            public decimal SaleDiscount { get; set; }

            public decimal LineItemVatValue { get; set; }

            public Guid ProductId { get; set; }

            public OrderType OrderType { get; set; }
        }

        private class ScenarioTestHelperLocal
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IMainOrderFactory _mainOrderFactory;
            private IMainOrderRepository _mainOrderRepository;
            private IOrderWorkflow _orderWorkflow;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;
            private IReceiveAndProcessPendingRemoteCommandEnvelopesService _sync;

            public ScenarioTestHelperLocal(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IMainOrderFactory mainOrderFactory, IMainOrderRepository mainOrderRepository, IOrderWorkflow orderWorkflow, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService, IReceiveAndProcessPendingRemoteCommandEnvelopesService sync)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _mainOrderFactory = mainOrderFactory;
                _mainOrderRepository = mainOrderRepository;
                _orderWorkflow = orderWorkflow;
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

            public MainOrder CreateMainOrder(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var pn = _mainOrderFactory.Create(cc, config.CostCentreApplicationId, cc, localSetting.User, cc, localSetting.OrderType, localSetting.DocumentReference, localSetting.DocumentParentId, localSetting.ShipToAddress, localSetting.DateRequired, localSetting.SaleDiscount,
                    "Main Order");

                return pn;
            }

            public SubOrderLineItem CreateMainOrderLineItem(ScenarioSettings localSettings)
            {
                var lineItem = _mainOrderFactory.CreateLineItem(localSettings.ProductId, localSettings.OriginalQuantity,
                    localSettings.OriginalValue, "Line Item 1", localSettings.LineItemVatValue);

                return lineItem;
            }

            public void SubmitToWF(MainOrder mainOrder)
            {
                _orderWorkflow.Submit(mainOrder, null);
            }

            public MainOrder GetById(Guid Id)
            {
                return _mainOrderRepository.GetById(Id);
            }

            public decimal Value(Guid documentId)
            {
                var creditNote = _mainOrderRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.ItemSummary.FirstOrDefault().Value;
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
            
            public async Task<bool> Sync(ScenarioSettings settings)
            {
                var r = await _sync.ReceiveAndProcessNextEnvelopesAsync(settings.HubConfig.CostCentreApplicationId);
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

            public decimal Value(Guid documentId)
            {
                var creditNote = _mainOrderRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.ItemSummary.FirstOrDefault().Value;
            }

            public decimal Quantity(Guid documentId)
            {
                var creditNote = _mainOrderRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.ItemSummary.FirstOrDefault().Qty;
            }
        }
    }
}