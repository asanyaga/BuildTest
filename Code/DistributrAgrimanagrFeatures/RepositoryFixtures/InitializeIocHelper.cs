using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using NUnit.Framework;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures
{
    [SetUpFixture]
    public class InitializeIocHelper
    {
        [SetUp]
        public void Setup()
        {
            DB_TestingHelper dbTestingHelper = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbTestingHelper.Hub_DistributrEdmxConnection, dbTestingHelper.Hub_RoutingConnectionString, "");
        }
    }
}
