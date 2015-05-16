using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.DocumentFeatures.IAN;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Newtonsoft.Json;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures.DN
{
    [Binding]
    public class DispatchNoteLocalSteps
    {
        private string section = "DNLocalSteps";
        private string productCode = "SP005";

        [Given(@"I have a product on the hub")]
        public void GivenIHaveAProductOnTheHub()
        {
           
            TI.trace(section, "#1");
            TI.trace(section, "Choose product SP005");
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            Config c = th.GetConfig();
            Product testProduct = th.ProductByProductCode(productCode);
            User localUser = th.LocalUser();
            var settings = new ScenarioSettings { Product = testProduct, HubConfig = c, LocalUser = localUser };
            ScenarioContext.Current["settings"] = settings;

        }

        [Given(@"I create an DN to dispatch (.*) products")]
        public void GivenICreateAnDNToDispatchProducts(int p0)
        {
            
            TI.trace(section, "#2");
            DispatchNoteLocalSteps.ScenarioSettings settings = ScenarioContext.Current["settings"] as DispatchNoteLocalSteps.ScenarioSettings;
            DispatchNoteLocalSteps.ScenarioTestHelper th = ObjectFactory.GetInstance<DispatchNoteLocalSteps.ScenarioTestHelper>();
            DispatchNote dn = th.CreateDn(settings);
            TI.trace(section, string.Format("DocumentId.**** {0}", dn.Id));
            Product testProduct = th.ProductByProductCode(productCode);
            DispatchNoteLineItem li = th.CreateDnLineItem(testProduct.Id);
            TI.trace(section, string.Format("DocumentId {0}", dn.Id));
            dn.AddLineItem(li);
            dn.Confirm();
            settings.DocumentId = dn.Id;
            ScenarioContext.Current["settings"] = settings;
            ScenarioContext.Current["newdn"] = dn;

        }

        [When(@"I submit the DN to its respective workflow")]
        public void WhenISubmitTheDNToItsRespectiveWorkflow()
        {
            TI.trace(section, "#3");
            DispatchNote dn = ScenarioContext.Current["newdn"] as DispatchNote;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            th.SubmitToWF(dn);
        }

       /* [Then(@"then the product  on the hub should have been dispatched")]
        public void ThenThenTheProductOnTheHubShouldHaveBeenDispatched()
        {
            ScenarioContext.Current.Pending();
        }*/

        [Then(@"there should be a saved DN document")]
        public void ThenThereShouldBeASavedDNDocument()
        {
            TI.trace(section, "#4");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            DispatchNote dn = th.Get(settings.DocumentId);
            Assert.IsNotNull(dn);
            Assert.AreEqual(settings.DocumentId, dn.Id);
            Assert.AreEqual(DocumentStatus.Confirmed, dn.Status);
        }

    
       
        [Then(@"there should be a corresponding command envelope on the outgoing command queue of the dn")]
        public void ThenThereShouldBeACorrespondingCommandEnvelopeOnTheOutgoingCommandQueueOfTheDn()
        {
            TI.trace(section, "#5");
            ScenarioSettings settings = ScenarioContext.Current["settings"] as ScenarioSettings;
            ScenarioTestHelper th = ObjectFactory.GetInstance<ScenarioTestHelper>();
            var r = th.GetConfirmEnvelopeEntryInOutboundQueue(settings.DocumentId);
            Assert.AreEqual(1, r.Count());
            OutGoingCommandEnvelopeQueueItemLocal item = r[0];
            Assert.AreEqual(settings.DocumentId, item.DocumentId);
            CommandEnvelope envelope = JsonConvert.DeserializeObject<CommandEnvelope>(item.JsonEnvelope);
            Assert.AreEqual(3, envelope.CommandsList.Count());
        }






        private class ScenarioSettings
        {
            public ScenarioSettings()
            {
                DocumentId = Guid.Empty;
            }
            public Product Product { get; set; }
            public Guid DocumentId { get; set; }
            public Config HubConfig { get; set; }
            public User LocalUser { get; set; }
        }

        private class ScenarioTestHelper
        {
            private IProductRepository _productRepository;
            private IDispatchNoteRepository _dispatchNoteRepository;
            private IConfirmDispatchNoteWFManager _confirmDispatchNoteWfManager;
            private IConfigService _configService;
            private ICostCentreRepository _costCentreRepository;
            private IDispatchNoteFactory _dispatchNoteFactory;
            private IUserRepository _userRepository;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;


            public ScenarioTestHelper( IProductRepository productRepository, IDispatchNoteRepository dispatchNoteRepository, IConfigService configService, ICostCentreRepository costCentreRepository, IDispatchNoteFactory dispatchNoteFactory, IUserRepository userRepository, IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteRepository, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IConfirmDispatchNoteWFManager confirmDispatchNoteWfManager)
            {
                _productRepository = productRepository;
                _dispatchNoteRepository = dispatchNoteRepository;
                _configService = configService;
                _costCentreRepository = costCentreRepository;
                _dispatchNoteFactory = dispatchNoteFactory;
                _userRepository = userRepository;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _confirmDispatchNoteWfManager = confirmDispatchNoteWfManager;
            }

            public Config GetConfig()
            {
                return _configService.Load();
               
            }

            public Product ProductByProductCode(string productCode)
            {
                return _productRepository.GetAll().First(n => n.ProductCode == productCode);
            }


            public User LocalUser()
            {
                return _userRepository.GetAll().First(n => n.Username == "Kameme");
            }

            public DispatchNote CreateDn(ScenarioSettings localSetting)
             {
                 CostCentre cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                 Config c = localSetting.HubConfig;
                 var note = _dispatchNoteFactory.Create(cc, c.CostCentreApplicationId, cc, localSetting.LocalUser,
                     cc, DispatchNoteType.Delivery, Guid.NewGuid().ToString().Substring(1, 10), Guid.Empty, Guid.Empty);
               /* var note = _documentFactory.CreateDocument(Guid.NewGuid(), DocumentType.DispatchNote, cc,
                    cc, localSetting.LocalUser, Guid.NewGuid().ToString());*/
                
                return note;
             }

            public DispatchNoteLineItem CreateDnLineItem(Guid productId)
             {
                 return _dispatchNoteFactory.CreateLineItem( productId, 10,500,
                     "Desc" + Guid.NewGuid().ToString(),0,0,0,DiscountType.FreeOfChargeDiscount);
             }

            public void SubmitToWF(DispatchNote dn)
             {
                 _confirmDispatchNoteWfManager.SubmitChanges(dn,null);
               
             }

            public DispatchNote Get(Guid documentId)
            {
                return _dispatchNoteRepository.GetById(documentId);
            }

            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);

            }

          
        }


    }
}
