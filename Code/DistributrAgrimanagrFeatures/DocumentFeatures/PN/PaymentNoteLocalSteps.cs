using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
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
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.PN
{
    [Binding]
    public class PaymentNoteLocalSteps
    {
        private string section = "PaymentNoteLocalSteps";
        public decimal amount = 10;

        [Given(@"I generate a payment note")]
        public void GivenIGenerateAPaymentNote()
        {
            TI.trace(section,"#1");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
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

        [Given(@"I add a payment line item to the payment")]
        public void GivenIAddAPaymentLineItemToThePayment()
        {
            TI.trace(section,"#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var note =
                ScenarioContext.Current["note"] as PaymentNote;
            note.AddLineItem(testHelper.CreateLineItem(settings.OriginalAmount, PaymentMode.Cash));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the payment to its workflow")]
        public void WhenISubmitThePaymentToItsWorkflow()
        {
            TI.trace(section, "#3");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note =
                ScenarioContext.Current["note"] as PaymentNote;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            testHelper.SubmitToWF(note);
        }

        [Then(@"there should be a saved payment note")]
        public void ThenThereShouldBeASavedPaymentNote()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelper.Get(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId,note.Id);
            Assert.AreEqual(note.DocumentType,DocumentType.PaymentNote);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the payment note should have a line item")]
        public void ThenThePaymentNoteShouldHaveALineItem()
        {
            TI.trace(section, "#5");
            var note =
                ScenarioContext.Current["note"] as PaymentNote;
            Assert.IsNotNull(note.LineItems);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Amount,amount);
        }

        [Then(@"there should be a corresponding payment note command envelop on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingPaymentNoteCommandEnvelopOnTheOutgoingCommandQueue()
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

        private class ScenarioTestHelper
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IPaymentNoteRepository _paymentNoteRepository;
            private IConfirmPaymentNoteWFManager _confirmPaymentNoteWfManager;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private IDocumentFactory _documentFactory;

            public ScenarioTestHelper(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IPaymentNoteRepository paymentNoteRepository, IConfirmPaymentNoteWFManager confirmPaymentNoteWfManager, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IDocumentFactory documentFactory)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _paymentNoteRepository = paymentNoteRepository;
                _confirmPaymentNoteWfManager = confirmPaymentNoteWfManager;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _documentFactory = documentFactory;
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
                note.EnableAddCommands();
                note.SendDateTime = DateTime.Now;
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

            public PaymentNote Get(Guid documentId)
            {
                return _paymentNoteRepository.GetById(documentId);
            }

            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);

            }

            public decimal Amount(Guid documentId)
            {
                var paymentNote = _paymentNoteRepository.GetById(documentId);
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