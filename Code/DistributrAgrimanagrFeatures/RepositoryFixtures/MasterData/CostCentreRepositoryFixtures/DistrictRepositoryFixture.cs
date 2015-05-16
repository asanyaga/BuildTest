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
    public class DistrictRepositoryFixture
    {
        private static IDistrictRepository _districtRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _districtRepository = _testHelper.Ioc<IDistrictRepository>();
        }

        [Test]
        public void DistrictRepositoryUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START DISTRICT REPOSITORY UNIT TEST....");

                //Save district
                var district = _testHelper.BuildDistrict();
                Trace.WriteLine(string.Format("Created district [{0}]", district.DistrictName));
                var toSaveDistrict = _districtRepository.Save(district);
                Trace.WriteLine(string.Format("Saved district Id [{0}]", toSaveDistrict));
                var savedDistrict = _districtRepository.GetById(toSaveDistrict);

                AssertDistrict(district, savedDistrict);

                //District listing
                var queryResult =
                    _districtRepository.Query(new QueryStandard() { Name = district.DistrictName });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("District [{0}] exists in listing", district.DistrictName));

                //Update district
                var toUpdateDistrict = savedDistrict;
                toUpdateDistrict.DistrictName = "District 2";

                _districtRepository.Save(toUpdateDistrict);

                var updatedDistrict = _districtRepository.GetById(toUpdateDistrict.Id);
                Trace.WriteLine(string.Format("Updated district to Name  [{0}]", updatedDistrict.DistrictName));

                AssertDistrict(toUpdateDistrict, updatedDistrict);

                //Deactivate district
                var toDeactivate = updatedDistrict;
                toDeactivate._Status = EntityStatus.Inactive;

                _districtRepository.Save(toDeactivate);

                var deactivated = _districtRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated district  to status  [{0}]", deactivated._Status));

                //Activate district
                var toActivate = updatedDistrict;
                toActivate._Status = EntityStatus.Active;

                _districtRepository.Save(toActivate);

                var activated = _districtRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated district to status  [{0}]", activated._Status));

                //Delete district
                var toDelete = updatedDistrict;
                toDelete._Status = EntityStatus.Deleted;

                _districtRepository.Save(toDelete);

                var deleted = _districtRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted district to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }

        }

        private void AssertDistrict(District province, District savedProvince)
        {
            Assert.AreEqual(province.DistrictName, savedProvince.DistrictName);
            Assert.AreEqual(province.Province, savedProvince.Province);
            Assert.AreEqual(province._Status, EntityStatus.Active);
        }
    }
}