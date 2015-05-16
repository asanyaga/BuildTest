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
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.DocumentFeatures.Inv;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.CN
{
    [Binding]
    public class CreditNoteServerSteps
    {
        private string section = "CreditNoteServerSteps";
        public decimal amount = 10;
        public int LineItemSequenceNo = 1;

        [Given(@"I create a credit note \[server]")]
        public void GivenICreateACreditNoteServer()
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
            };
            settings.OriginalQuantity = testHelper.Quantity(settings.DocumentId);
            settings.OriginalValue = testHelper.Value(settings.DocumentId);

            var masterDataHelper = ObjectFactory.GetInstance<MasterDataHelper>();
            var product = masterDataHelper.GetSaleProduct();
            settings.ProductId = product.Id;
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreateCreditNote(settings);
            settings.DocumentId = note.Id;
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I create a credit note line item \[server]")]
        public void GivenICreateACreditNoteLineItemServer()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var note =
                ScenarioContext.Current["note"] as CreditNote;
            note.AddLineItem(testHelper.CreateCreditNoteLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the credit note to its respective workflow \[server]")]
        public void WhenISubmitTheCreditNoteToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as CreditNote;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
        }

        [When(@"I trigger a server sync \(credit note\) \[server]")]
        public void WhenITriggerAServerSyncCreditNoteServer()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [Then(@"There should be a saved credit note on the server")]
        public void ThenThereShouldBeASavedCreditNoteOnTheServer()
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

        [Then(@"the sale amount for the credit note should be the same as in the hub")]
        public void ThenTheSaleAmountForTheCreditNoteShouldBeTheSameAsInTheHub()
        {
            TI.trace(section, "#6");

            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var valueLocal = testHelper.Value(settings.DocumentId);
            var qtyLocal = testHelper.Quantity(settings.DocumentId);
            var creditNoteLocal = testHelper.GetById(settings.DocumentId);

            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var valueServer = testHelperServer.Value(settings);
            var qtyServer = testHelperServer.Quantity(settings);
            var creditNoteServer = testHelperServer.Get(settings.DocumentId);

            Assert.AreEqual(valueLocal, valueServer);
            Assert.AreEqual(qtyLocal, qtyServer);
        }

        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
                InvoiceId = Guid.Empty;
            }
            public Guid DocumentId { get; set; }

            public Guid InvoiceId { get; set; }

            public User User { get; set; }

            public Config HubConfig { get; set; }

            public Decimal OriginalValue { get; set; }
            public Decimal OriginalQuantity { get; set; }
            public Guid ProductId { get; set; }
            public int LineItemSequenceNo { get; set; }
            public decimal LineItemVatValue { get; set; }
            public decimal ProductDiscount { get; set; }
        }

        private class ScenarioTestHelperLocal
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private ICreditNoteFactory _creditNoteFactory;
            private ICreditNoteRepository _creditNoteRepository;
            private IConfirmCreditNoteWFManager _confirmCreditNoteWfManager;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

            public ScenarioTestHelperLocal(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, ICreditNoteFactory creditNoteFactory, ICreditNoteRepository creditNoteRepository, IConfirmCreditNoteWFManager confirmCreditNoteWfManager, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _creditNoteFactory = creditNoteFactory;
                _creditNoteRepository = creditNoteRepository;
                _confirmCreditNoteWfManager = confirmCreditNoteWfManager;
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

            public CreditNote CreateCreditNote(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var invoice = _creditNoteFactory.Create(cc, config.CostCentreApplicationId, cc, localSetting.User,
                    "Credit Note", Guid.Empty, localSetting.InvoiceId);

                return invoice;
            }

            public CreditNoteLineItem CreateCreditNoteLineItem(ScenarioSettings localSettings)
            {
                var lineItem = _creditNoteFactory.CreateLineItem(localSettings.ProductId, localSettings.OriginalQuantity,
                    localSettings.OriginalValue, "Line Item 1", localSettings.LineItemSequenceNo, localSettings.LineItemVatValue,
                    localSettings.ProductDiscount);

                return lineItem;
            }

            public void SubmitToWF(CreditNote creditNote)
            {
                _confirmCreditNoteWfManager.SubmitChanges(creditNote, null);
            }

            public CreditNote GetById(Guid Id)
            {
                return _creditNoteRepository.GetById(Id);
            }

            public decimal Value(Guid documentId)
            {
                var creditNote = _creditNoteRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.LineItems.FirstOrDefault().Value;
            }

            public decimal Quantity(Guid documentId)
            {
                var creditNote = _creditNoteRepository.GetById(documentId);
                return creditNote == null ? 10 : creditNote.LineItems.FirstOrDefault().Qty;
            }

            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }
        }

        private class ScenarioTestHelperServer
        {
            private ICreditNoteRepository _creditNoteRepository;

            public ScenarioTestHelperServer(ICreditNoteRepository creditNoteRepository)
            {
                _creditNoteRepository = creditNoteRepository;
            }

            public CreditNote Get(Guid documentId)
            {
                return _creditNoteRepository.GetById(documentId);
            }

            public decimal Quantity(ScenarioSettings scenarioSettings)
            {
                var creditNote = _creditNoteRepository.GetById(scenarioSettings.DocumentId);
                return creditNote != null ? 10 : creditNote.LineItems.FirstOrDefault().Qty;
            }

            public decimal Value(ScenarioSettings scenarioSettings)
            {
                var creditNote = _creditNoteRepository.GetById(scenarioSettings.DocumentId);
                return creditNote != null ? 10 : creditNote.LineItems.FirstOrDefault().Value;
            }
        }

    }
}