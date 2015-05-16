using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
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
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.DocumentFeatures.PN;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.RN
{
    [Binding]
    public class ReceiptNoteServerSteps
    {
        private string section = "PaymentNoteServerSteps";
        public decimal amount = 10;

        [Given(@"I create a receipt note \[server]")]
        public void GivenICreateAReceiptNoteServer()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
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

        [Given(@"I create a receipt line item \[server]")]
        public void GivenICreateAReceiptLineItemServer()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var note =
                ScenarioContext.Current["note"] as Receipt;
            note.AddLineItem(testHelper.CreateLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the receipt to its respective workflow \[server]")]
        public void WhenISubmitTheReceiptToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note =
                ScenarioContext.Current["note"] as Receipt;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
        }

        [When(@"I trigger a server sync \[server]")]
        public void WhenITriggerAServerSyncServer()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [Then(@"There should be a saved receipt note on the server")]
        public void ThenThereShouldBeASavedReceiptNoteOnTheServer()
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

        [Then(@"the receipt amount in should be the same as in the hub")]
        public void ThenTheReceiptAmountInShouldBeTheSameAsInTheHub()
        {
            TI.trace(section, "#6");

            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var amountLocal = testHelper.Amount(settings.DocumentId);
            var noteLocal = testHelper.GetById(settings.DocumentId);

            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var amountServer = testHelperServer.Amount(settings);
            var noteServer = testHelperServer.Get(settings.DocumentId);

            Assert.AreEqual(amountLocal, amountServer);
            Assert.AreEqual(noteLocal.LineItems.FirstOrDefault().PaymentType,noteServer.LineItems.FirstOrDefault().PaymentType);
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

        private class ScenarioTestHelperLocal
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private IReceiptRepository _receiptRepository;
            private IReceiptFactory _receiptFactory;
            private IReceiptWorkFlowManager _receiptWorkFlowManager;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

            public ScenarioTestHelperLocal(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IReceiptRepository receiptRepository, IReceiptFactory receiptFactory, IReceiptWorkFlowManager receiptWorkFlowManager, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _receiptRepository = receiptRepository;
                _receiptFactory = receiptFactory;
                _receiptWorkFlowManager = receiptWorkFlowManager;
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
                _receiptWorkFlowManager.SubmitChanges(receipt, null);
            }

            public decimal Amount(Guid documentId)
            {
                var receiptNote = _receiptRepository.GetById(documentId);
                return receiptNote == null ? 10 : receiptNote.LineItems.FirstOrDefault().Value;
            }

            public Receipt GetById(Guid documentId)
            {
                return _receiptRepository.GetById(documentId);
            }

            public PaymentMode GetPaymentMode()
            {
                return PaymentMode.Cash;
            }

            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }
        }

        private class ScenarioTestHelperServer
        {
            private IReceiptRepository _receiptRepository;

            public ScenarioTestHelperServer(IReceiptRepository receiptRepository)
            {
                _receiptRepository = receiptRepository;
            }

            public Receipt Get(Guid documentId)
            {
                return _receiptRepository.GetById(documentId);
            }

            public decimal Amount(ScenarioSettings scenarioSettings)
            {
                var receiptNote = _receiptRepository.GetById(scenarioSettings.DocumentId);
                return receiptNote == null ? 10 : receiptNote.LineItems.FirstOrDefault().Value;
            }
        }
    }
}