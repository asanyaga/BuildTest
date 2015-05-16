﻿using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.CN
{
    [Binding]
    public class CreditNoteLocalSteps
    {
        private string section = "CreditNoteLocalSteps";
        public decimal amount = 10;
        public int LineItemSequenceNo = 1;

        [Given(@"I create a credit note")]
        public void GivenICreateACreditNote()
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

        [Given(@"I create a credit note line item")]
        public void GivenICreateACreditNoteLineItem()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var note =
                ScenarioContext.Current["note"] as CreditNote;
            note.AddLineItem(testHelper.CreateCreditNoteLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the credit note to its respective workflow")]
        public void WhenISubmitTheCreditNoteToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as CreditNote;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            testHelper.SubmitToWF(note);
        }

        [Then(@"there should be a saved credit note")]
        public void ThenThereShouldBeASavedCreditNote()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelper.GetById(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId, note.Id);
            Assert.AreEqual(note.DocumentType, DocumentType.CreditNote);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the credit note should have a line item")]
        public void ThenTheCreditNoteShouldHaveALineItem()
        {
            TI.trace(section, "#5");
            var note =
                ScenarioContext.Current["note"] as CreditNote;
            Assert.IsNotNull(note.LineItems);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Value, amount);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Qty, amount);
        }

        [Then(@"there should be a corresponding credit note command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingCreditNoteCommandEnvelopeOnTheOutgoingCommandQueue()
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

        private class ScenarioTestHelper
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private ICreditNoteFactory _creditNoteFactory;
            private ICreditNoteRepository _creditNoteRepository;
            private IConfirmCreditNoteWFManager _confirmCreditNoteWfManager;

            public ScenarioTestHelper(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, ICreditNoteFactory creditNoteFactory, ICreditNoteRepository creditNoteRepository, IConfirmCreditNoteWFManager confirmCreditNoteWfManager)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _creditNoteFactory = creditNoteFactory;
                _creditNoteRepository = creditNoteRepository;
                _confirmCreditNoteWfManager = confirmCreditNoteWfManager;
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
            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);
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

        }
    }
}