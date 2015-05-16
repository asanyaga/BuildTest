using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
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

namespace DistributrAgrimanagrFeatures.DocumentFeatures.Inv
{
    [Binding]
    public class InvoiceServerSteps
    {
        private string section = "InvoiceServerSteps";
        public decimal amount = 10;
        public DiscountType DiscountType = DiscountType.ProductDiscount;
        public int LineItemSequenceNo = 1;

        [Given(@"I create an invoice \[server]")]
        public void GivenICreateAnInvoiceServer()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var config = testHelper.GetConfig();
            var user = testHelper.User();
            var settings = new ScenarioSettings()
            {
                User = user,
                HubConfig = config,
                LineItemSequenceNo = LineItemSequenceNo,
                DiscountType = DiscountType,
            };
            settings.OriginalQuantity = testHelper.Quantity(settings.DocumentId);
            settings.OriginalValue = testHelper.Value(settings.DocumentId);

            var masterDataHelper = ObjectFactory.GetInstance<MasterDataHelper>();
            var product = masterDataHelper.GetSaleProduct();
            settings.ProductId = product.Id;
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreateInvoice(settings);
            settings.DocumentId = note.Id;
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I create an invoice line item \[server]")]
        public void GivenICreateAnInvoiceLineItemServer()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var note =
                ScenarioContext.Current["note"] as Invoice;
            note.AddLineItem(testHelper.CreateLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the invoice to its respective workflow \[server]")]
        public void WhenISubmitTheInvoiceToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note =
                ScenarioContext.Current["note"] as Invoice;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
        }

        [When(@"I trigger a server sync from hub \[server]")]
        public void WhenITriggerAServerSyncFromHubServer()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [Then(@"There should be a saved invoice on the server")]
        public void ThenThereShouldBeASavedInvoiceOnTheServer()
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

        [Then(@"the sale amount in should be the same as in the hub")]
        public void ThenTheSaleAmountInShouldBeTheSameAsInTheHub()
        {
            TI.trace(section, "#6");

            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var valueLocal = testHelper.Value(settings.DocumentId);
            var qtyLocal = testHelper.Quantity(settings.DocumentId);
            var invoiceLocal = testHelper.GetById(settings.DocumentId);

            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var valueServer = testHelperServer.Value(settings);
            var qtyServer = testHelperServer.Quantity(settings);
            var invoiceServer = testHelperServer.Get(settings.DocumentId);

            Assert.AreEqual(valueLocal, valueServer);
            Assert.AreEqual(qtyLocal, qtyServer);
        }

        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
                OrderId = Guid.Empty;
                ProductId = Guid.NewGuid();
            }
            public Guid DocumentId { get; set; }

            public Guid OrderId { get; set; }

            public User User { get; set; }

            public Config HubConfig { get; set; }

            public Decimal OriginalValue { get; set; }
            public Decimal OriginalQuantity { get; set; }
            public Guid ProductId { get; set; }
            public int LineItemSequenceNo { get; set; }
            public decimal LineItemVatValue { get; set; }
            public decimal ProductDiscount { get; set; }
            public DiscountType DiscountType { get; set; }
        }

        private class ScenarioTestHelperLocal
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IInvoiceFactory _invoiceFactory;
            private IInvoiceRepository _invoiceRepository;
            private IConfirmInvoiceWorkFlowManager _confirmInvoiceWorkFlowManager;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

            public ScenarioTestHelperLocal(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IInvoiceFactory invoiceFactory, IInvoiceRepository invoiceRepository, IConfirmInvoiceWorkFlowManager confirmInvoiceWorkFlowManager,ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _invoiceFactory = invoiceFactory;
                _invoiceRepository = invoiceRepository;
                _confirmInvoiceWorkFlowManager = confirmInvoiceWorkFlowManager;
                _sendPendingEnvelopeCommandsService = sendPendingEnvelopeCommandsService;
            }

            public Config GetConfig()
            {
                return _configService.Load();
            }

            public User User()
            {
                return _userRepository.GetAll().First(m => m.Username == "Kameme");
            }

            public Invoice CreateInvoice(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var invoice = _invoiceFactory.Create(cc, config.CostCentreApplicationId, cc, localSetting.User,
                    "Invoice", Guid.Empty, localSetting.OrderId);

                return invoice;
            }

            public InvoiceLineItem CreateLineItem(ScenarioSettings localSettings)
            {
                var lineItem = _invoiceFactory.CreateLineItem(localSettings.ProductId, localSettings.OriginalQuantity,
                    localSettings.OriginalValue, "Line Item 1", localSettings.LineItemSequenceNo, localSettings.LineItemVatValue,
                    localSettings.ProductDiscount, localSettings.DiscountType);

                return lineItem;
            }

            public void SubmitToWF(Invoice invoice)
            {
                _confirmInvoiceWorkFlowManager.SubmitChanges(invoice, null);
            }

            public Invoice GetById(Guid Id)
            {
                return _invoiceRepository.GetById(Id);
            }
            

            public decimal Value(Guid documentId)
            {
                var invoice = _invoiceRepository.GetById(documentId);
                return invoice == null ? 10 : invoice.LineItems.FirstOrDefault().Value;
            }

            public decimal Quantity(Guid documentId)
            {
                var invoice = _invoiceRepository.GetById(documentId);
                return invoice == null ? 10 : invoice.LineItems.FirstOrDefault().Qty;
            }
            
            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }
        }

        private class ScenarioTestHelperServer
        {
            private IInvoiceRepository _invoiceRepository;

            public ScenarioTestHelperServer(IInvoiceRepository invoiceRepository)
            {
                _invoiceRepository = invoiceRepository;
            }

            public Invoice Get(Guid documentId)
            {
                return _invoiceRepository.GetById(documentId);
            }

            public decimal Quantity(ScenarioSettings scenarioSettings)
            {
                var invoice = _invoiceRepository.GetById(scenarioSettings.DocumentId);
                return invoice != null ? 10 : invoice.LineItems.FirstOrDefault().Qty;
            }
            
            public decimal Value(ScenarioSettings scenarioSettings)
            {
                var invoice = _invoiceRepository.GetById(scenarioSettings.DocumentId);
                return invoice != null ? 10 : invoice.LineItems.FirstOrDefault().Value;
            }
        }
    }
}