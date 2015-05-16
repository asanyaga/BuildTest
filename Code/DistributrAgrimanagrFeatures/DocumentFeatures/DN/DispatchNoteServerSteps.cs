using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.CommandQueues;
using Distributr.WPF.Lib.Services.Service.Sync;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.DocumentFeatures.DN;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures
{
    [Binding]
    public class DispatchNoteServerSteps
    {
        private string section = "DNServerSteps";
        private string productCode = "SP005";


        [Given(@"I have a product on the hub  \[server]")]
        public void GivenIHaveAProductOnTheHubServer()
        {
            TI.trace(section, "#1");
            TI.trace(section, "Choose product SP005");
            DispatchNoteServerSteps.ScenarioTestHelperLocal th = ObjectFactory.GetInstance<DispatchNoteServerSteps.ScenarioTestHelperLocal>();
            Config c = th.GetConfig();
            Product testProduct = th.ProductByProductCode(productCode);
            User localUser = th.LocalUser();
            var settings = new DispatchNoteServerSteps.ScenarioSettings { Product = testProduct, HubConfig = c, LocalUser = localUser };
            ScenarioContext.Current["settings"] = settings;
        }

        [Given(@"I create an DN to dispatch (.*) products \[server]")]
        public void GivenICreateAnDNToDispatchProductsServer(int p0)
        {
            TI.trace(section, "#2");
            DispatchNoteServerSteps.ScenarioSettings settings = ScenarioContext.Current["settings"] as DispatchNoteServerSteps.ScenarioSettings;
            DispatchNoteServerSteps.ScenarioTestHelperLocal th = ObjectFactory.GetInstance<DispatchNoteServerSteps.ScenarioTestHelperLocal>();
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

        [When(@"I submit the DN to its respective workflow\[server]")]
        public void WhenISubmitTheDNToItsRespectiveWorkflowServer()
        {
            TI.trace(section, "#3");
            DispatchNote dn = ScenarioContext.Current["newdn"] as DispatchNote;
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();
            th.SubmitToWF(dn);
        }

        [When(@"I trigger a server sync for DN")]
        public void WhenITriggerAServerSyncForDN()
        {
            TI.trace(section, "#4");
            ScenarioTestHelperLocal th = ObjectFactory.GetInstance<ScenarioTestHelperLocal>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                int r = th.SendPendingCommands().Result;

            }
        }


        [Then(@"there should be a saved DN documenton the server")]
        public void ThenThereShouldBeASavedDNDocumentonTheServer()
        {
            TI.trace(section, "#5");
            DispatchNoteServerSteps.ScenarioSettings settings = ScenarioContext.Current["settings"] as DispatchNoteServerSteps.ScenarioSettings;
            DispatchNoteServerSteps.ScenarioTestHelperLocal th = ObjectFactory.GetInstance<DispatchNoteServerSteps.ScenarioTestHelperLocal>();
            DispatchNote dn = th.Get(settings.DocumentId);
            Assert.IsNotNull(dn);
            Assert.AreEqual(settings.DocumentId, dn.Id);
            Assert.AreEqual(DocumentStatus.Confirmed, dn.Status);
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

        private class ScenarioTestHelperLocal
        {
            private IProductRepository _productRepository;
            private IDispatchNoteRepository _dispatchNoteRepository;
            private IConfirmDispatchNoteWFManager _confirmDispatchNoteWfManager;
            private IConfigService _configService;
            private ICostCentreRepository _costCentreRepository;
            private IDispatchNoteFactory _dispatchNoteFactory;
            private IUserRepository _userRepository;
            private IOutgoingCommandEnvelopeQueueRepository _outgoingCommandEnvelopeQueueRepository;
            private ISendPendingEnvelopeCommandsService _sendPendingEnvelopeCommandsService;


            public ScenarioTestHelperLocal(ISendPendingEnvelopeCommandsService sendPendingEnvelopeCommandsService, IProductRepository productRepository, IDispatchNoteRepository dispatchNoteRepository, IConfigService configService, ICostCentreRepository costCentreRepository, IDispatchNoteFactory dispatchNoteFactory, IUserRepository userRepository, IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteRepository, IOutgoingCommandEnvelopeQueueRepository outgoingCommandEnvelopeQueueRepository, IConfirmDispatchNoteWFManager confirmDispatchNoteWfManager)
            {
                _productRepository = productRepository;
                _dispatchNoteRepository = dispatchNoteRepository;
                _configService = configService;
                _costCentreRepository = costCentreRepository;
                _dispatchNoteFactory = dispatchNoteFactory;
                _userRepository = userRepository;
                _outgoingCommandEnvelopeQueueRepository = outgoingCommandEnvelopeQueueRepository;
                _confirmDispatchNoteWfManager = confirmDispatchNoteWfManager;
                _sendPendingEnvelopeCommandsService = sendPendingEnvelopeCommandsService;
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

            public DispatchNote CreateDn(DispatchNoteServerSteps.ScenarioSettings localSetting)
            {
                CostCentre cc = _costCentreRepository.GetById(localSetting.HubConfig.CostCentreId);
                Config c = localSetting.HubConfig;
                var note = _dispatchNoteFactory.Create(cc, c.CostCentreApplicationId, cc, localSetting.LocalUser,
                    cc, DispatchNoteType.Delivery, Guid.NewGuid().ToString().Substring(1, 10), Guid.Empty, Guid.Empty);

                return note;
            }

            public DispatchNoteLineItem CreateDnLineItem(Guid productId)
            {
                return _dispatchNoteFactory.CreateLineItem(productId, 10, 500,
                    "Desc" + Guid.NewGuid().ToString(), 0, 0, 0, DiscountType.FreeOfChargeDiscount);
            }

            public void SubmitToWF(DispatchNote dn)
            {
                _confirmDispatchNoteWfManager.SubmitChanges(dn, null);

            }

            public DispatchNote Get(Guid documentId)
            {
                return _dispatchNoteRepository.GetById(documentId);
            }

            public List<OutGoingCommandEnvelopeQueueItemLocal> GetConfirmEnvelopeEntryInOutboundQueue(Guid documentId)
            {
                return _outgoingCommandEnvelopeQueueRepository.GetByDocumentId(documentId);

            }


            public async Task<int> SendPendingCommands()
            {
                int r = await _sendPendingEnvelopeCommandsService.SendPendingEnvelopeCommandsAsync();
                return r;
            }

        }

    }
}
