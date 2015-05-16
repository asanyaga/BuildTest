using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.AssetRepositoriesFixtures
{
    [TestFixture]
    public class AssetStatusRepositoryFixture
    {
        private static IAssetStatusRepository _assetStatusRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _assetStatusRepository = _testHelper.Ioc<IAssetStatusRepository>();
        }

        [Test]
        public void AssetStatusRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                //Save asset
                var assetStatus = _testHelper.BuildAssetStatus();
                Trace.WriteLine(string.Format("Created asset [{0}]", assetStatus.Name));
                var toSaveAssetStatus = _assetStatusRepository.Save(assetStatus);
                Trace.WriteLine(string.Format("Saved asset Id [{0}]", toSaveAssetStatus));
                var savedAssetStatus = _assetStatusRepository.GetById(toSaveAssetStatus);

                AssertAssetStatus(assetStatus, savedAssetStatus);

                //Asset listing
                var queryResult =
                    _assetStatusRepository.Query(new QueryStandard() { Name = assetStatus.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Outlet asset [{0}] exists in listing", assetStatus.Name));

                //Update asset
                var toUpdateAssetStatus = savedAssetStatus;
                toUpdateAssetStatus.Name = "Asset status 2";
                toUpdateAssetStatus.Description = "Asset status  2";

                _assetStatusRepository.Save(toUpdateAssetStatus);

                var updatedAssetStatus = _assetStatusRepository.GetById(toUpdateAssetStatus.Id);
                Trace.WriteLine(string.Format("Updated asset to Name  [{0}]", updatedAssetStatus.Name));

                AssertAssetStatus(toUpdateAssetStatus, updatedAssetStatus);

                //Deactivate asset
                var toDeactivate = updatedAssetStatus;
                toDeactivate._Status = EntityStatus.Inactive;

                _assetStatusRepository.Save(toDeactivate);

                var deactivated = _assetStatusRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated asset  to status  [{0}]", deactivated._Status));

                //Activate asset
                var toActivate = updatedAssetStatus;
                toActivate._Status = EntityStatus.Active;

                _assetStatusRepository.Save(toActivate);

                var activated = _assetStatusRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated asset to status  [{0}]", activated._Status));

                //Delete asset
                var toDelete = updatedAssetStatus;
                toDelete._Status = EntityStatus.Deleted;

                _assetStatusRepository.Save(toDelete);

                var deleted = _assetStatusRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted asset to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertAssetStatus(AssetStatus assetStatus, AssetStatus savedAssetStatus)
        {
            Assert.AreEqual(assetStatus.Name,savedAssetStatus.Name);
            Assert.AreEqual(assetStatus.Description,savedAssetStatus.Description);
            Assert.AreEqual(assetStatus._Status,EntityStatus.Active);
        }
    }
}
