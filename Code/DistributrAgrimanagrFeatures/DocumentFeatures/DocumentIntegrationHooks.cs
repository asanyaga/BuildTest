using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using TechTalk.SpecFlow;

namespace DistributrAgrimanagrFeatures.DocumentFeatures
{
    [Binding]
    public class DocumentIntegrationHooks
    {
        private static string section = "DocumentIntegrationHooks";
        [BeforeFeature("documenthook")]
        public static void BeforeDocumentFeature()
        {
            TI.trace(section, "BeforeDocumentFeature");
        }

        [BeforeScenario("documenthook")]
        public static void BeforeDocumentScenario()
        {
            TI.trace(section, "BeforeDocumentScenario");
            //STRUCTURE MAP
            DB_TestingHelper dbTestingHelper = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbTestingHelper.Hub_DistributrEdmxConnection, dbTestingHelper.Hub_RoutingConnectionString, "");
    
        }

        [AfterScenario("documenthook")]
        public static void AfterDocumentScenario()
        {
            TI.trace(section, "AfterDocumentScenario");

        }

        [AfterFeature("documenthook")]
        public static void AfterDocumentFeature()
        {
            TI.trace(section, "AfterDocumentFeature");

        }
    }
}
