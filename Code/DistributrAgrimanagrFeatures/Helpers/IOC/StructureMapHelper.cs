using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributrAgrimanagrFeatures.Helpers.DB;

namespace DistributrAgrimanagrFeatures.Helpers.IOC
{
    public static class StructureMapHelper
    {
        public static void InitialiseHub()
        {
            DB_TestingHelper dbTestingHelper = DefaultDbTestingHelper.GetDefaultDbTestingHelper();
            IOCHelper.InitialiseHubSliceWithStructurmapContainer(dbTestingHelper.Hub_DistributrEdmxConnection, dbTestingHelper.Hub_RoutingConnectionString, "");
        }
    }
}
