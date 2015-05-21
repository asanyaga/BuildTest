using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributrAgrimanagrFeatures.DocumentFeatures;
using DistributrAgrimanagrFeatures.Helpers.DB;
using DistributrAgrimanagrFeatures.Helpers.IOC;
using NUnit.Framework;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures
{
    [SetUpFixture]
    public class RepositorySetup
    {
        [SetUp]
        public void BeforeRun()
        {
            SetupHelper.SetupDatabasesPullMasterdata();
            StructureMapHelper.InitialiseHub();
        }
    }
}
