using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.WPF.Lib.Services.Service.Utility;
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
    public class InitialLoginSteps : Steps
    {
        private string section = "InitialLoginSteps";

        [Given(@"I want to establish the cost centre id of my hub")]
        public void GivenIWantToEstablishTheCostCentreIdOfMyHub()
        {
            TI.trace(section, "#1");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Config c = configService.Load();
            Assert.AreEqual(Guid.Empty, c.CostCentreId);

        }

        [Given(@"I have a valid username and password")]
        public void GivenIHaveAValidUsernameAndPassword()
        {
            TI.trace(section, "#2");
        }
        [When(@"I run prelogin setup")]
        public void WhenIRunPreloginSetup()
        {
            TI.trace(section, "#3");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.AppIdSetup(helper);
            StepHelper.SetConfigWebserviceUrl(helper);
        }

        [When(@"I login to the server")]
        public void WhenILoginToTheServer()
        {
            TI.trace(section, "#4");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            CostCentreLoginResponse r = null;
            using (var webApp = WebApp.Start<StartupServerAutofac>(StepHelper.WSUrl))
            {
                TI.trace(section, "#4_a");
                r = StepHelper.InitialLogin(helper);
            }
            ScenarioContext.Current["LoginResponse"] = r;
        }

        [Then(@"I should get a valid response")]
        public void ThenIShouldGetAValidResponse()
        {
            TI.trace(section, "#5");
            CostCentreLoginResponse r = ScenarioContext.Current["LoginResponse"] as CostCentreLoginResponse;
            Assert.IsNotNull(r.CostCentreId);
            Assert.AreNotEqual(Guid.Empty, r.CostCentreId);
            ScenarioContext.Current["CostCentreId"] = r.CostCentreId;
        }

        [Then(@"the configuration should have the saved cost centre id for the hub")]
        public void ThenTheConfigurationShouldHaveTheSavedCostCentreIdForTheHub()
        {
            TI.trace(section, "#6");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Config c = configService.Load();
            Guid costCentreId = (Guid)ScenarioContext.Current["CostCentreId"];
            Assert.AreEqual(costCentreId, c.CostCentreId);
        }

    }
}
