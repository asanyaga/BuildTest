using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.Inv
{
    [Binding]
    public class InvoiceLocalSteps
    {
        private string section = "InvoiceLocalSteps";
        public decimal amount = 10;
        public DiscountType DiscountType = DiscountType.ProductDiscount;
        public int LineItemSequenceNo = 1;

        [Given(@"I create an invoice")]
        public void GivenICreateAnInvoice()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
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

        [Given(@"I create an invoice line item")]
        public void GivenICreateAnInvoiceLineItem()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var note =
                ScenarioContext.Current["note"] as Invoice;
            note.AddLineItem(testHelper.CreateLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the invoice to its respective workflow")]
        public void WhenISubmitTheInvoiceToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as Invoice;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            testHelper.SubmitToWF(note);
        }

        [Then(@"there should be a saved invoice")]
        public void ThenThereShouldBeASavedInvoice()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelper.GetById(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId, note.Id);
            Assert.AreEqual(note.DocumentType, DocumentType.Invoice);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the invoice should have a line item")]
        public void ThenTheInvoiceShouldHaveALineItem()
        {
            TI.trace(section, "#5");
            var note =
                ScenarioContext.Current["note"] as Invoice;
            Assert.IsNotNull(note.LineItems);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Value, amount);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Qty, amount);
        }

        [Then(@"there should be a corresponding invoice command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingInvoiceCommandEnvelopeOnTheOutgoingCommandQueue()
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

        private class ScenarioTestHelper
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private IInvoiceFactory _invoiceFactory;
            private IInvoiceRepository _invoiceRepository;
            private IConfirmInvoiceWorkFlowManager _confirmInvoiceWorkFlowManager;

            public ScenarioTestHelper(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IInvoiceFactory invoiceFactory, IInvoiceRepository invoiceRepository, IConfirmInvoiceWorkFlowManager confirmInvoiceWorkFlowManager)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _invoiceFactory = invoiceFactory;
                _invoiceRepository = invoiceRepository;
                _confirmInvoiceWorkFlowManager = confirmInvoiceWorkFlowManager;
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
                _confirmInvoiceWorkFlowManager.SubmitChanges(invoice,null);
            }

            public Invoice GetById(Guid Id)
            {
                return _invoiceRepository.GetById(Id);
            }
            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);
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

        }
    }
}