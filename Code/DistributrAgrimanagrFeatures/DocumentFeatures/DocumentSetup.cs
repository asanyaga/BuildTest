using NUnit.Framework;

namespace DistributrAgrimanagrFeatures.DocumentFeatures
{
    /// <summary>
    /// Run once per namespace
    /// Run initial setup to prepare for Masterdata tests
    /// Clean down databases
    /// Pull down test master data
    /// </summary>
    [SetUpFixture]
    public class DocumentSetup
    {
        [SetUp]
        public void BeforeRun()
        {
            SetupHelper.SetupDatabasesPullMasterdata();
        }
    }
}
