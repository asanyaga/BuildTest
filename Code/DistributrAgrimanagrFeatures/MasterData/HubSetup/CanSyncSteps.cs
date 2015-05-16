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
    public class CanSyncSteps
    {
        private string section = "CanSyncSteps";

        [Given(@"I want to sync data")]
        public void GivenIWantToSyncData()
        {
            TI.trace(section, "#1");
        }

        [Given(@"I run the setup required \[stephook one]")]
        public void GivenIRunTheSetupRequiredStephookOne()
        {
            TI.trace(section, "#2");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.AppIdSetup(helper);
            StepHelper.SetConfigWebserviceUrl(helper);
            Guid ccappid = Guid.Empty;
            using (var webApp = WebApp.Start<StartupServerAutofac>("http://localhost:9443/"))
            {
                StepHelper.InitialLogin(helper);
                ccappid = StepHelper.GetCostCentreAppicationIdAndAppInit(helper).Result;
            }
        }

        [When(@"I run Can Sync")]
        public void GivenIRunCanSync()
        {
            TI.trace(section, "#3");
            InitializationHelper helper = ObjectFactory.GetInstance<InitializationHelper>();
            helper.CanSync();
        }

        [Then(@"the client application can sync should be set to true")]
        public void ThenTheClientApplicationCanSyncShouldBeSetToTrue()
        {
            TI.trace(section, "#4");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Guid clientAppId = configService.GetClientAppId();
            ClientApplication application =
                configService.GetClientApplications().First(n => n.Id == clientAppId);
            Assert.True(application.CanSync);
        }


    }
}
