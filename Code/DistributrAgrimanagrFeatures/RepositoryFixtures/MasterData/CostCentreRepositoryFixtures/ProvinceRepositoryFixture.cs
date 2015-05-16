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
    public class ProvinceRepositoryFixture
    {
        private static IProvincesRepository _provinceRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _provinceRepository = _testHelper.Ioc<IProvincesRepository>();
        }

        [Test]
        public void ProvinceRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START REGION REPOSITORY UNIT TEST....");

                //Save province
                var province = _testHelper.BuildProvince();
                Trace.WriteLine(string.Format("Created province [{0}]", province.Name));
                var toSaveProvince = _provinceRepository.Save(province);
                Trace.WriteLine(string.Format("Saved province Id [{0}]", toSaveProvince));
                var savedProvince = _provinceRepository.GetById(toSaveProvince);

                AssertProvince(province, savedProvince);

                //Province listing
                var queryResult =
                    _provinceRepository.Query(new QueryStandard() { Name = province.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Province [{0}] exists in listing", province.Name));

                //Update province
                var toUpdateProvince = savedProvince;
                toUpdateProvince.Name = "province 2";

                _provinceRepository.Save(toUpdateProvince);

                var updatedProvince = _provinceRepository.GetById(toUpdateProvince.Id);
                Trace.WriteLine(string.Format("Updated province to Name  [{0}]", updatedProvince.Name));

                AssertProvince(toUpdateProvince, updatedProvince);

                //Deactivate province
                var toDeactivate = updatedProvince;
                toDeactivate._Status = EntityStatus.Inactive;

                _provinceRepository.Save(toDeactivate);

                var deactivated = _provinceRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated province  to status  [{0}]", deactivated._Status));

                //Activate province
                var toActivate = updatedProvince;
                toActivate._Status = EntityStatus.Active;

                _provinceRepository.Save(toActivate);

                var activated = _provinceRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated province to status  [{0}]", activated._Status));

                //Delete province
                var toDelete = updatedProvince;
                toDelete._Status = EntityStatus.Deleted;

                _provinceRepository.Save(toDelete);

                var deleted = _provinceRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted province to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }

        }

        private void AssertProvince(Province province, Province savedProvince)
        {
            Assert.AreEqual(province.Description, savedProvince.Description);
            Assert.AreEqual(province.Name, savedProvince.Name);
            Assert.AreEqual(province.Country, savedProvince.Country);
            Assert.AreEqual(province._Status, EntityStatus.Active);
        }
    }
}