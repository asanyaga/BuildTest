using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.CSN
{
    [Binding]
    public class CommodityStorageNoteLocalSteps
    {
       /* private string section = "CommodityStorageNoteLocalSteps";
        public decimal weight = 10;

        [Given(@"I create a commodity storage note")]
        public void GivenICreateACommodityStorageNote()
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
            settings.Weight = testHelper.Weight(settings.DocumentId);

            var masterDataHelper = ObjectFactory.GetInstance<MasterDataHelper>();
            var commodity = masterDataHelper.GetCommodity();
            settings.Commodity = commodity.Id;
            var commodityGrade = masterDataHelper.GetCommodityGrade();
            settings.CommodityGrade = commodityGrade.Id;
            var containerType = masterDataHelper.GetContainerType();
            settings.ContainerType = containerType.Id;
            ScenarioContext.Current["settings"] = settings;
            var note = testHelper.CreateCommodityStorageNote(settings);
            settings.DocumentId = note.Id;
            ScenarioContext.Current["note"] = note;
        }

        [Given(@"I create a commodity storage line item")]
        public void GivenICreateACommodityStorageLineItem()
        {
            TI.trace(section, "#2");
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var note =
                ScenarioContext.Current["note"] as CommodityStorageNote;
            note.AddLineItem(testHelper.CreateCreditNoteLineItem(settings));
            note.Confirm();
            ScenarioContext.Current["note"] = note;
        }

        [When(@"I submit the commodity storage note to its respective workflow")]
        public void WhenISubmitTheCommodityStorageNoteToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            var note =
                ScenarioContext.Current["note"] as CommodityStorageNote;
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            testHelper.SubmitToWF(note);
        }

        [Then(@"there should be a saved commodity storage note")]
        public void ThenThereShouldBeASavedCommodityStorageNote()
        {
            TI.trace(section, "#4");
            var testHelper = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            var note = testHelper.GetById(settings.DocumentId);
            Assert.IsNotNull(note);
            Assert.AreEqual(settings.DocumentId, note.Id);
            Assert.AreEqual(note.DocumentType, DocumentType.CommodityStorageNote);
            ScenarioContext.Current["note"] = note;
        }

        [Then(@"the commodity storage note should have a line item")]
        public void ThenTheCommodityStorageNoteShouldHaveALineItem()
        {
            TI.trace(section, "#5");
            var note =
                ScenarioContext.Current["note"] as CommodityStorageNote;
            var settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            Assert.IsNotNull(note.LineItems);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Weight, weight);
            Assert.AreEqual(note.LineItems.FirstOrDefault().Commodity.Id,settings.Commodity);
            Assert.AreEqual(note.LineItems.FirstOrDefault().CommodityGrade.Id,settings.CommodityGrade);
            Assert.AreEqual(note.LineItems.FirstOrDefault().ContainerType.Id,settings.ContainerType);
        }

        [Then(@"there should be a corresponding commodity storage note command envelope on the outgoing command queue")]
        public void ThenThereShouldBeACorrespondingCommodityStorageNoteCommandEnvelopeOnTheOutgoingCommandQueue()
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
            }

            public Guid DocumentId { get; set; }

            public User User { get; set; }

            public Config HubConfig { get; set; }

            public Guid Commodity { get; set; }

            public Guid CommodityGrade { get; set; }
            
            public decimal Weight { get; set; }

            public Guid ContainerType { get; set; }
        }

        private class ScenarioTestHelper
        {
            private ICostCentreRepository _costCentreRepository;
            private IUserRepository _userRepository;
            private IConfigService _configService;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private ICommodityStorageNoteFactory _commodityStorageNoteFactory;
            private ICommodityStorageRepository _commodityStorageNoteRepository;
            private ICommodityStorageWFManager _commodityStorageNoteWfManager;

            public ScenarioTestHelper(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IConfigService configService, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, ICommodityStorageNoteFactory commodityStorageNoteFactory, ICommodityStorageRepository commodityStorageNoteRepository, ICommodityStorageWFManager commodityStorageNoteWfManager)
            {
                _costCentreRepository = costCentreRepository;
                _userRepository = userRepository;
                _configService = configService;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _commodityStorageNoteFactory = commodityStorageNoteFactory;
                _commodityStorageNoteRepository = commodityStorageNoteRepository;
                _commodityStorageNoteWfManager = commodityStorageNoteWfManager;
            }


            public Config GetConfig()
            {
                return _configService.Load();
            }

            public User User()
            {
                return _userRepository.GetAll().First(m => m.Username == "Kameme");
            }

            public CommodityStorageNote CreateCommodityStorageNote(ScenarioSettings localSetting)
            {
                var cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                var config = localSetting.HubConfig;
                var commodityStorageNote = _commodityStorageNoteFactory.Create(cc, config.CostCentreApplicationId, cc, localSetting.User,
                    "Commodity Storage Note", Guid.Empty, DateTime.Now,DateTime.Now);

                return commodityStorageNote;
            }

            public CommodityStorageLineItem CreateCreditNoteLineItem(ScenarioSettings localSettings)
            {
                var lineItem = _commodityStorageNoteFactory.CreateLineItem(Guid.Empty,localSettings.Commodity,localSettings.CommodityGrade,localSettings.ContainerType,"",localSettings.Weight,"");

                return lineItem;
            }

            public void SubmitToWF(CommodityStorageNote commodityStorageNote)
            {
                _commodityStorageNoteWfManager.SubmitChanges(commodityStorageNote);
            }

            public CommodityStorageNote GetById(Guid Id)
            {
                return _commodityStorageNoteRepository.GetById(Id) as CommodityStorageNote;
            }
            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);
            }

            public decimal Weight(Guid documentId)
            {
                var creditNote = _commodityStorageNoteRepository.GetById(documentId) as CommodityStorageNote;
                return creditNote == null ? 10 : creditNote.LineItems.FirstOrDefault().Weight;
            }
        }*/
    }
}