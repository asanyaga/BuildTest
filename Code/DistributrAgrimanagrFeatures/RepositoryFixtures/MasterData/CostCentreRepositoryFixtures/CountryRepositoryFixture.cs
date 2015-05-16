using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class CountryRepositoryFixture
    {
        private static ICountryRepository _countryRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _countryRepository = _testHelper.Ioc<ICountryRepository>();
        }

        [Test]
        public void CountryRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COUNTRY REPOSITORY UNIT TEST....");

                //Save country
                var country = _testHelper.BuildCountry();
                Trace.WriteLine(string.Format("Created country [{0}]", country.Name));
                var toSaveCountry = _countryRepository.Save(country);
                Trace.WriteLine(string.Format("Saved country Id [{0}]", toSaveCountry));
                var savedCountry = _countryRepository.GetById(toSaveCountry);

                AssertCountry(country, savedCountry);

                //Country listing
                var queryResult =
                    _countryRepository.Query(new QueryStandard() { Name = country.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Country [{0}] exists in listing", country.Name));

                //Update country
                var toUpdateCountry = savedCountry;
                toUpdateCountry.Name = "Country 2";

                _countryRepository.Save(toUpdateCountry);

                var updatedCountry = _countryRepository.GetById(toUpdateCountry.Id);
                Trace.WriteLine(string.Format("Updated country to Name  [{0}]", updatedCountry.Name));

                AssertCountry(toUpdateCountry, updatedCountry);

                //Deactivate country
                var toDeactivate = updatedCountry;
                toDeactivate._Status = EntityStatus.Inactive;

                _countryRepository.Save(toDeactivate);

                var deactivated = _countryRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated country  to status  [{0}]", deactivated._Status));

                //Activate country
                var toActivate = updatedCountry;
                toActivate._Status = EntityStatus.Active;

                _countryRepository.Save(toActivate);

                var activated = _countryRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated country to status  [{0}]", activated._Status));

                //Delete country
                var toDelete = updatedCountry;
                toDelete._Status = EntityStatus.Deleted;

                _countryRepository.Save(toDelete);

                var deleted = _countryRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted country to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
                }

        }

        private void AssertCountry(Country country, Country savedCountry)
        {
            Assert.AreEqual(country.Code,savedCountry.Code);
            Assert.AreEqual(country.Name,savedCountry.Name);
            Assert.AreEqual(country.Currency,savedCountry.Currency);
            Assert.AreEqual(country._Status,EntityStatus.Active);
        }
    }
}
