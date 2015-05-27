using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
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
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.Order
{
    [Binding]
    public class OrderLocalSteps
    {
        private string section = "OrderLocalSteps";
        public decimal amount = 10;
        public decimal LineItemVatValue = 0.16m;

        [Given(@"I create an order")]
        public void GivenICreateAnOrder()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
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

        [Given(@"I create an order line item")]
        public void GivenICreateAnOrderLineItem()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            note.AddLineItem(testHelper.CreateFOCLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the order to its respective workflow")]
        public void WhenISubmitTheOrderToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            testHelper.SubmitToWF(note);
        }

        [Then(@"there should be a saved order")]
        public void ThenThereShouldBeASavedOrder()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelper.GetById(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId, note.Id);
            Assert.AreEqual(note.DocumentType, DocumentType.Order);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the order should have a line item")]
        public void ThenTheOrderShouldHaveALineItem()
        {
            TI.trace(section, "#5");
            var note =
                ScenarioContext.Current["note"] as MainOrder;
            Assert.IsNotNull(note.ItemSummary);
            Assert.AreEqual(note.ItemSummary.FirstOrDefault().Qty, amount);
        }

        [Then(@"there should be a corresponding order command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingOrderCommandEnvelopeOnTheOutgoingCommandQueue()
        {
            TI.trace(section, "#6");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;

            var r = testHelper.GetConfirmEnvelopeEntryInOutboundQueue(settings.DocumentId);
            Assert.AreEqual(1, r.Count());
            OutGoingCommandEnvelopeQueueItemLocal item = r[0];
            Assert.AreEqual(settings.DocumentId, item.DocumentId);
            var envelope = JsonConvert.DeserializeObject<CommandEnvelope>(item.JsonEnvelope);
            Assert.AreEqual(3, envelope.CommandsList.Count());
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

        private class ScenarioTestHelper
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private IMainOrderFactory _mainOrderFactory;
            private IMainOrderRepository _mainOrderRepository;
            private IStockistPurchaseOrderWorkflow _stockistPurchaseOrderWorkflow;

            public ScenarioTestHelper(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IMainOrderFactory mainOrderFactory, IMainOrderRepository mainOrderRepository, IStockistPurchaseOrderWorkflow stockistPurchaseOrderWorkflow)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _mainOrderFactory = mainOrderFactory;
                _mainOrderRepository = mainOrderRepository;
                _stockistPurchaseOrderWorkflow = stockistPurchaseOrderWorkflow;
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
            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);
            }

            public decimal Quantity(Guid documentId)
            {
                var creditNote = _mainOrderRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.ItemSummary.FirstOrDefault().Qty;
            }

        }
    }
}