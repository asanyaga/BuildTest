using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.DocumentFeatures.PN;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using StructureMap.Pipeline;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.RN
{
    [Binding]
    public class ReceiptNoteLocalSteps
    {
        private string section = "ReceiptNoteLocalSteps";
        public decimal amount = 10;

        [Given(@"I create a receipt note")]
        public void GivenICreateAReceiptNote()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var config = testHelper.GetConfig();
            var user = testHelper.User();
            var settings = new ScenarioSettings()
            {
                User = user,
                HubConfig = config,
                MMomeyPaymentType = "Mpesa",
                NotificationId = "1",
                LineItemSequenceNo = 1
            };
            settings.OriginalAmount = testHelper.Amount(settings.DocumentId);
            settings.PaymentMode = testHelper.GetPaymentMode();
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreateReceiptNote(settings);
            settings.DocumentId = note.Id;
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I create a receipt line item")]
        public void GivenICreateAReceiptLineItem()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var note =
                ScenarioContext.Current["note"] as Receipt;
            note.AddLineItem(testHelper.CreateLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the receipt to its respective workflow")]
        public void WhenISubmitTheReceiptToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note =
                ScenarioContext.Current["note"] as Receipt;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            testHelper.SubmitToWF(note);
        }

        [Then(@"there should be a saved receipt note")]
        public void ThenThereShouldBeASavedReceiptNote()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelper.GetById(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId, note.Id);
            Assert.AreEqual(note.DocumentType, DocumentType.Receipt);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the receipt note should have a line item")]
        public void ThenTheReceiptNoteShouldHaveALineItem()
        {
            TI.trace(section, "#5");
            var note =
                ScenarioContext.Current["note"] as Receipt;
            Assert.IsNotNull(note.LineItems);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Value, amount);
            Assert.AreEqual(note.LineItems.FirstOrDefault().PaymentType,PaymentMode.Cash);
        }

        [Then(@"there should be a corresponding receipt note command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingReceiptNoteCommandEnvelopeOnTheOutgoingCommandQueue()
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
                InvoiceId = Guid.Empty;
                PaymentId = Guid.Empty;
            }
            public Guid DocumentId { get; set; }
            
            public Guid InvoiceId { get; set; }

            public Guid PaymentId { get; set; }

            public User User { get; set; }

            public Config HubConfig { get; set; }

            public Decimal OriginalAmount { get; set; }

            public String MMomeyPaymentType { get; set; }

            public String NotificationId { get; set; }

            public int LineItemSequenceNo { get; set; }

            public PaymentMode PaymentMode { get; set; }
        }

        private class ScenarioTestHelper
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private IReceiptRepository _receiptRepository;
            private IReceiptFactory _receiptFactory;
            private IReceiptWorkFlowManager _receiptWorkFlowManager;

            public ScenarioTestHelper(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IReceiptRepository receiptRepository, IReceiptFactory receiptFactory, IReceiptWorkFlowManager receiptWorkFlowManager)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _receiptRepository = receiptRepository;
                _receiptFactory = receiptFactory;
                _receiptWorkFlowManager = receiptWorkFlowManager;
            }

            public Config GetConfig()
            {
                return _configService.Load();
            }

            public User User()
            {
                return _userRepository.GetAll().First(m => m.Username == "Kameme");
            }

            public Receipt CreateReceiptNote(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var receiptNote = _receiptFactory.Create(cc, config.CostCentreApplicationId, cc, localSetting.User,
                    "Receipt Note", Guid.NewGuid(), localSetting.InvoiceId, localSetting.PaymentId);

                return receiptNote;
            }

            public ReceiptLineItem CreateLineItem(ScenarioSettings localSettings)
            {
                var lineItem = _receiptFactory.CreateLineItem(localSettings.OriginalAmount, localSettings.PaymentId.ToString(),
                    localSettings.MMomeyPaymentType, localSettings.NotificationId, localSettings.LineItemSequenceNo,
                    localSettings.PaymentMode, "Receipt Note Line Item", localSettings.DocumentId, true);
                return lineItem;
            }

            public void SubmitToWF(Receipt receipt)
            {
                _receiptWorkFlowManager.SubmitChanges(receipt,null);
            }

            public Receipt GetById(Guid documentId)
            {
                return _receiptRepository.GetById(documentId);
            }
            
            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);
            }

            public decimal Amount(Guid documentId)
            {
                var receiptNote = _receiptRepository.GetById(documentId);
                return receiptNote == null ? 10 : receiptNote.LineItems.FirstOrDefault().Value;
            }

            public PaymentMode GetPaymentMode()
            {
                return PaymentMode.Cash;
            }
        }
    }
}