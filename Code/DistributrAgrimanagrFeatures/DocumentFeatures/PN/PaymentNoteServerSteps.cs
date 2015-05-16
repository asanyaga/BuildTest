using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.PN
{
    [Binding]
    public class PaymentNoteServerSteps
    {
        private string section = "PaymentNoteServerSteps";
        public decimal amount = 10;

        [Given(@"I generate a payment note \[server]")]
        public void GivenIGenerateAPaymentNoteServer()
        {
            TI.trace(section, "#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var config = testHelper.GetConfig();
            var user = testHelper.User();
            var distributor = testHelper.Distributor();
            var salesman = testHelper.DistributorSalesman();
            var settings = new ScenarioSettings()
            {
                Distributor = distributor,
                DistributorSalesman = salesman,
                User = user,
                HubConfig = config
            };
            settings.OriginalAmount = testHelper.Amount(settings.DocumentId);
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreatePaymentNote(settings);
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I add a payment line item to the payment \[server]")]
        public void GivenIAddAPaymentLineItemToThePaymentServer()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var note =
                ScenarioContext.Current["note"] as PaymentNote;
            note.AddLineItem(testHelper.CreateLineItem(settings.OriginalAmount, PaymentMode.Cash));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the payment to its workflow \[server]")]
        public void WhenISubmitThePaymentToItsWorkflowServer()
        {
            TI.trace(section, "#3");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note =
                ScenarioContext.Current["note"] as PaymentNote;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            testHelper.SubmitToWF(note);
        }

        [When(@"I trigger a sync to the server")]
        public void WhenITriggerASyncToTheServer()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = testHelper.SendPendingCommands().Result;
            }
        }

        [Then(@"There should be saved payment note on the server")]
        public void ThenThereShouldBeSavedPaymentNoteOnTheServer()
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

        [Then(@"The line item amount should be the same as in the hub")]
        public void ThenTheLineItemAmountShouldBeTheSameAsInTheHub()
        {
            TI.trace(section, "#6");

            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            var amountLocal = testHelper.Amount(settings.DocumentId);

            var c = IOCHelper.ServerContainerAutofac(new[] { typeof(ScenarioTestHelperServer) });
            var testHelperServer = c.Resolve<ScenarioTestHelperServer>();
            var amountServer = testHelperServer.Amount(settings);

            Assert.AreEqual(amountLocal, amountServer);
        }

        private class ScenarioTestHelperLocal
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IPaymentNoteRepository _paymentNoteRepository;
            private IConfirmPaymentNoteWFManager _confirmPaymentNoteWfManager;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private IDocumentFactory _documentFactory;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;

            public ScenarioTestHelperLocal(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IPaymentNoteRepository paymentNoteRepository, IConfirmPaymentNoteWFManager confirmPaymentNoteWfManager, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IDocumentFactory documentFactory, ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _paymentNoteRepository = paymentNoteRepository;
                _confirmPaymentNoteWfManager = confirmPaymentNoteWfManager;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _documentFactory = documentFactory;
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

            public Distributor Distributor()
            {
                return _costCentreRepository.GetAll().OfType<Distributor>().FirstOrDefault();
            }

            public DistributorSalesman DistributorSalesman()
            {
                return _costCentreRepository.GetAll().OfType<DistributorSalesman>().FirstOrDefault();
            }

            public PaymentNote CreatePaymentNote(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var salesman = localSetting.DistributorSalesman;
                var user = localSetting.User;
                var distributor = localSetting.Distributor;

                var note =
                    _documentFactory.CreateDocument(localSetting.DocumentId, DocumentType.PaymentNote, salesman,
                        distributor, user, "Payment Note") as
                        PaymentNote;
                note.PaymentNoteType = PaymentNoteType.Availabe;
                note.DocumentDateIssued = DateTime.Now;
                note.EndDate = DateTime.Now;
                note.StartDate = DateTime.Now;
                note.SendDateTime = DateTime.Now;
                note.EnableAddCommands();
                return note;
            }

            public PaymentNoteLineItem CreateLineItem(decimal amount, PaymentMode paymentMode)
            {
                return new PaymentNoteLineItem(Guid.Empty)
                {
                    Amount = amount,
                    Description = "Payment note test",
                    PaymentMode = paymentMode,
                    LineItemSequenceNo = 0
                };
            }

            public void SubmitToWF(PaymentNote paymentNote)
            {
                _confirmPaymentNoteWfManager.SubmitChanges(paymentNote,null);
            }

            public decimal Amount(Guid documentId)
            {
                var paymentNote = _paymentNoteRepository.GetById(documentId);
                return paymentNote == null ? 10 : paymentNote.LineItems.FirstOrDefault().Amount;
            }

            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }
        }
        
        private class ScenarioTestHelperServer
        {
            private IPaymentNoteRepository _paymentNoteRepository;

            public ScenarioTestHelperServer(IPaymentNoteRepository paymentNoteRepository)
            {
                _paymentNoteRepository = paymentNoteRepository;
            }

            public PaymentNote Get(Guid documentId)
            {
                return _paymentNoteRepository.GetById(documentId);
            }

            public decimal Amount(ScenarioSettings scenarioSettings)
            {
                var paymentNote = _paymentNoteRepository.GetById(scenarioSettings.DocumentId);
                return paymentNote == null ? 10 : paymentNote.LineItems.FirstOrDefault().Amount;
            }
        }

        private class ScenarioSettings
        {
            public Guid DocumentId { get; set; }
            public User User { get; set; }

            public Distributor Distributor { get; set; }

            public Config HubConfig { get; set; }

            public decimal OriginalAmount { get; set; }

            public DistributorSalesman DistributorSalesman { get; set; }
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
            }
        }
    }
}
