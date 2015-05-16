using System;
using System.Linq;
using Distributr.Core;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.Initialise;
using DistributrAgrimanagrFeatures.Helpers.SFSteps;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.MasterData.HubSetup
{
    [Binding]
    public class HubSetupSteps
    {
        private string section = "HubSetupSteps";
        [Given(@"I want to setup the client application id on a hub application")]
        public void GivenIWantToSetupTheClientApplicationIdOnAHubApplication()
        {
            TI.trace(section, "#1");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Guid appId = configService.GetClientAppId();
            TI.trace("Application Id --- ", appId.ToString());
            ClientApplication clientApplication = configService.GetClientApplications().FirstOrDefault(n => n.Id == appId);
            Assert.IsNull(clientApplication);
            //ScenarioContext.Current.Pending();
        }

       
        [When(@"i run the appid setup")]
        public void WhenIRunTheAppidSetup()
        {
            TI.trace(section, "#2");
            InitializationHelper h = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.AppIdSetup(h);
        }


        [Then(@"the configuration should have the saved client application")]
        public void ThenTheConfigurationShouldHaveTheSavedClientApplication()
        {
            TI.trace(section, "#3");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            InitializationHelper h = ObjectFactory.GetInstance<InitializationHelper>();
            Guid appId = configService.GetClientAppId();
            TI.trace("Application Id --- ", appId.ToString());
            ClientApplication clientApplication =
                configService.GetClientApplications().FirstOrDefault(n => n.Id == appId);
            h.DumpObject("ClientApplication", clientApplication);
            Assert.IsNotNull(clientApplication);
            Assert.AreEqual(VirtualCityApp.Ditributr, clientApplication.AppId);
            Assert.AreEqual(appId, clientApplication.Id);
            //ScenarioContext.Current.Pending();
        }
    }
}
