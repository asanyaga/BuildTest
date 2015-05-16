using System;
using Distributr.WPF.Lib.Services.Service.Utility;
using DistributrAgrimanagrFeatures.Helpers.Initialise;
using DistributrAgrimanagrFeatures.Helpers.SFSteps;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using NUnit.Framework;
using StructureMap;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.MasterData
{
    [Binding]
    public class ConfigWebServiceUrlSteps : Steps
    {
        
        [Given(@"I want to setup the distributr server url on a hub application")]
        public void GivenIWantToSetupTheDistributrServerUrlOnAHubApplication()
        {
            TI.trace("ConfigWebServiceUrlSteps"," #1");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Config c =  configService.Load();
            Assert.AreEqual("", c.WebServiceUrl);
        }
        [When(@"i run initial setup config webservice setup")]
        public void WhenIRunInitialSetupConfigWebserviceSetup()
        {
            TI.trace("ConfigWebServiceUrlSteps", " #2");
            InitializationHelper h = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.AppIdSetup(h);
        }

        [When(@"I set the url on the configuration")]
        public void WhenISetTheUrlOnTheConfiguration()
        {
            TI.trace("ConfigWebServiceUrlSteps"," #3");
            InitializationHelper h = ObjectFactory.GetInstance<InitializationHelper>();
            StepHelper.SetConfigWebserviceUrl(h);
        }

        [Then(@"the configuration should have the saved server url")]
        public void ThenTheConfigurationShouldHaveTheSavedServerUrl()
        {
            TI.trace("ConfigWebServiceUrlSteps"," #3");
            IConfigService configService = ObjectFactory.GetInstance<IConfigService>();
            Config c = configService.Load();
            Assert.AreEqual(StepHelper.WSUrl, c.WebServiceUrl);
            //ScenarioContext.Current.Pending();
        }
    }
}
