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

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.OutletRepositoryFixtures
{
    [TestFixture]
    public class OutletTypeRepositoryFixture
    {
        private static IOutletTypeRepository _outletTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _outletTypeRepository = _testHelper.Ioc<IOutletTypeRepository>();
        }

        [Test]
        public void OutletTypeRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START OUTLET TYPE REPOSITORY UNIT TEST....");

                //Save outlet type
                var outletType = _testHelper.BuildOutletType();
                Trace.WriteLine(string.Format("Created outlet type [{0}]", outletType.Name));
                var toSaveOutletType = _outletTypeRepository.Save(outletType);
                Trace.WriteLine(string.Format("Saved outlet type Id [{0}]", toSaveOutletType));
                var savedOutletType = _outletTypeRepository.GetById(toSaveOutletType);

                AssertOutletType(outletType, savedOutletType);

                //Outlet cartegory listing
                var queryResult =
                    _outletTypeRepository.QueryResult(new QueryStandard() { Name = outletType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Outlet type [{0}] exists in listing", outletType.Name));

                //Update outlet cartegory
                var toUpdateOutletType = savedOutletType;
                toUpdateOutletType.Name = "Outlet Cartegory 2";
                toUpdateOutletType.Code = "Outlet Cartegory 2";

                _outletTypeRepository.Save(toUpdateOutletType);

                var updatedOutletType = _outletTypeRepository.GetById(toUpdateOutletType.Id);
                Trace.WriteLine(string.Format("Updated outlet type to Name  [{0}]", updatedOutletType.Name));

                AssertOutletType(toUpdateOutletType, updatedOutletType);

                //Deactivate commodity
                var toDeactivate = updatedOutletType;
                toDeactivate._Status = EntityStatus.Inactive;

                _outletTypeRepository.Save(toDeactivate);

                var deactivated = _outletTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated outlet type  to status  [{0}]", deactivated._Status));

                //Activate commodity
                var toActivate = updatedOutletType;
                toActivate._Status = EntityStatus.Active;

                _outletTypeRepository.Save(toActivate);

                var activated = _outletTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated outlet type to status  [{0}]", activated._Status));

                //Delete commodity
                var toDelete = updatedOutletType;
                toDelete._Status = EntityStatus.Deleted;

                _outletTypeRepository.Save(toDelete);

                var deleted = _outletTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted outlet type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertOutletType(OutletType outletType, OutletType savedOutletType)
        {
            Assert.AreEqual(outletType.Code,savedOutletType.Code);
            Assert.AreEqual(outletType.Name,savedOutletType.Name);
            Assert.AreEqual(savedOutletType._Status,EntityStatus.Active);
        }
    }
}