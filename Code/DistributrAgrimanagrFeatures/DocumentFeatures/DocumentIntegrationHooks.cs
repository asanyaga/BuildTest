using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using DistributrAgrimanagrFeatures.Helpers.TestTracing;
using TechTalk.SpecFlow;
using StructureMap = DistributrAgrimanagrFeatures.Helpers.IOC.StructureMapHelper;

namespace DistributrAgrimanagrFeatures.DocumentFeatures
{
    [Binding]
    public class DocumentIntegrationHooks
    {
        private static string section = "DocumentIntegrationHooks";
        [BeforeFeature("documenthook")]
        public static void BeforeDocumentFeature()
        {
            var s = section + "-BeforeDocumentFeature";
            TI.trace(s, "BeforeDocumentFeature");
        }

        [BeforeScenario("documenthook")]
        public static void BeforeDocumentScenario()
        {
            var s = section + "-BeforeDocumentScenario";

            TI.trace(s, "BeforeDocumentScenario #########################################");
            //STRUCTURE MAP
            StructureMapHelper.InitialiseHub();
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
