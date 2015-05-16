using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Impl.Repository.Utility;
using DistributrAgrimanagrFeatures.Helpers.Initialise;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.SFSteps;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.MasterData.HubSetup
{
    [Binding]
    public class SyncMasterData
    {
        private string section = "SyncMasterData";
        private Guid costCentreId = Guid.Empty;
        [Given(@"I want to sync master data")]
        public void GivenIWantToSyncMasterData()
        {
            TI.trace(section, "#1");
        }

        [Given(@"I run set setup required \[stephook two]")]
        public void GivenIRunSetSetupRequiredStephookTwo()
        {
            TI.trace(section, "#2");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.AppIdSetup(helper);
            StepHelper.SetConfigWebserviceUrl(helper);
            Guid ccappid = Guid.Empty;
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                CostCentreLoginResponse r = StepHelper.InitialLogin(helper);
                costCentreId = r.CostCentreId;
                ccappid = StepHelper.GetCostCentreAppicationIdAndAppInit(helper).Result;
                StepHelper.CanSync(helper);
            }
        }

        [When(@"I run sync master data on the hub")]
        public void WhenIRunSyncMasterDataOnTheHub()
        {
            TI.trace(section, "#3");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();

            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                bool masterDateSync = helper.SyncMasterData().Result;
            }
        }

        [Then(@"the hub application should have valid master data")]
        public void ThenTheHubApplicationShouldHaveValidMasterData()
        {
            TI.trace(section, "#4");

            ICostCentreRepository ccr = ObjectFactory.GetInstance<ICostCentreRepository>();
            List<CostCentre> ccs = ccr.GetAll().ToList();
            Assert.AreEqual(1, ccs.Count(n => n.Id == costCentreId) );
            //TODO more masterdata assertions
        }

    }
}
