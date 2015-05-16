using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class AppInitCCAppIdSteps : Steps
    {
        private string section = "AppInitCCAppIdSteps";

        [Given(@"I want to initialise the hub application for this cost centre by getting the cost centre application id")]
        public void GivenIWantToInitialiseTheHubApplicationForThisCostCentreByGettingTheCostCentreApplicationId()
        {
            TI.trace(section, "#1");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Config c = configService.Load();
            Assert.AreEqual(Guid.Empty, c.CostCentreApplicationId);
        }

        [When(@"I run the setup required for getting ccappid")]
        public void WhenIRunTheSetupRequiredForGettingCcappid()
        {
            TI.trace(section, "#2");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.AppIdSetup(helper);
            StepHelper.SetConfigWebserviceUrl(helper);
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                StepHelper.InitialLogin(helper);
            }
        }

        [When(@"I get cost centre application id")]
        public void WhenIGetCostCentreApplicationId()
        {
            TI.trace(section, "#3");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            Guid ccappid = Guid.Empty;
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                ccappid = StepHelper.GetCostCentreAppicationIdAndAppInit(helper).Result;
            }
            
            ScenarioContext.Current["ccappid"] = ccappid;
            TI.trace(section , string.Format("Cost centre application id {0}", ccappid));
        }

        [Then(@"i should get a valid response")]
        public void ThenIShouldGetAValidResponse()
        {
            TI.trace(section, "#4");
            Guid ccappid = (Guid) ScenarioContext.Current["ccappid"];
            Assert.IsNotNull(ccappid);
            Assert.AreNotEqual(Guid.Empty, ccappid);
        }

        [Then(@"the application should be initialized")]
        public void ThenTheApplicationShouldBeInitialized()
        {
            TI.trace(section, "#5");
            Guid ccappid = (Guid)ScenarioContext.Current["ccappid"];
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Config c = configService.Load();
            Assert.AreEqual(ccappid, c.CostCentreApplicationId);
            Assert.That(c.IsApplicationInitialized, Is.True);
            Assert.That(c.DateInitialized.Date, Is.EqualTo(DateTime.Now.Date));
            Assert.That(c.ApplicationStatus, Is.EqualTo(1));
        }

    }
}
