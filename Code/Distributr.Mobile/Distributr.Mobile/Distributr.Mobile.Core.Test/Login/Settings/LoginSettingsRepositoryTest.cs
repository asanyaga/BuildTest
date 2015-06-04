using Distributr.Mobile.Core.Test.Support;
using Distributr.Mobile.Login.Settings;
using NUnit.Framework;

namespace Distributr.Mobile.Core.Test.Login.Settings
{
    [TestFixture]
    [Category("Database")]
    public class LoginSettingsRepositoryTest : WithEmptyDatabaseTest
    {        
        private LoginSettingsRepository repository;

        [SetUp]
        public void Setup()
        {
            repository = new LoginSettingsRepository(Database);   
        }

        [TearDown]
        public void DeleteLoginSettings()
        {
            Database.DeleteAll<LoginSettings>();
        }

        [Test]
        public void ReturnsDefaultSettingsWhenTableIsEmpty()
        {
            Assert.NotNull(repository.GetSettings());
        }

        [Test]
        public void CanSaveAndReloadSettings()
        {
            var url = "test url";

            var settings = new LoginSettings()
            {
                ServerUrl = url
            };
            
            repository.Save(settings);
            
            Assert.AreEqual(url, repository.GetSettings().ServerUrl, "Server URL");
        }
    }
}
