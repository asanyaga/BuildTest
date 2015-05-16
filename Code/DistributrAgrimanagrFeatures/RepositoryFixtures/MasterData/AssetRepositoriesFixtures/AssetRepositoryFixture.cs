using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.AssetRepositoriesFixtures
{
    [TestFixture]
    public class AssetRepositoryFixture
    {
        private static IAssetRepository _assetCategoryRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _assetCategoryRepository = _testHelper.Ioc<IAssetRepository>();
        }

        [Test]
        public void AssetRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                //Save asset
                var asset = _testHelper.BuildAsset();
                Trace.WriteLine(string.Format("Created asset [{0}]", asset.Name));
                var toSaveAsset = _assetCategoryRepository.Save(asset);
                Trace.WriteLine(string.Format("Saved asset Id [{0}]", toSaveAsset));
                var savedAsset = _assetCategoryRepository.GetById(toSaveAsset);

                AssertAsset(asset, savedAsset);

                //Asset listing
                var queryResult =
                    _assetCategoryRepository.Query(new QueryStandard() { Name = asset.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Asset [{0}] exists in listing", asset.Name));

                //Update asset
                var toUpdateAsset = savedAsset;
                toUpdateAsset.Name = "Asset 2";
                toUpdateAsset.Code = "Code 2";
                toUpdateAsset.Capacity = "Two";
                toUpdateAsset.SerialNo = "987654321";
                toUpdateAsset.AssetNo = "987654321";

                _assetCategoryRepository.Save(toUpdateAsset);

                var updatedAsset = _assetCategoryRepository.GetById(toUpdateAsset.Id);
                Trace.WriteLine(string.Format("Updated asset to Name  [{0}]", updatedAsset.Name));

                AssertAsset(toUpdateAsset, updatedAsset);

                //Deactivate asset
                var toDeactivate = updatedAsset;
                toDeactivate._Status = EntityStatus.Inactive;

                _assetCategoryRepository.Save(toDeactivate);

                var deactivated = _assetCategoryRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated asset to status  [{0}]", deactivated._Status));

                //Activate asset
                var toActivate = updatedAsset;
                toActivate._Status = EntityStatus.Active;

                _assetCategoryRepository.Save(toActivate);

                var activated = _assetCategoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated asset to status  [{0}]", activated._Status));

                //Delete asset
                var toDelete = updatedAsset;
                toDelete._Status = EntityStatus.Deleted;

                _assetCategoryRepository.Save(toDelete);

                var deleted = _assetCategoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted asset to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertAsset(Asset asset, Asset savedAsset)
        {
            Assert.AreEqual(asset.Code,savedAsset.Code);
            Assert.AreEqual(asset.Name,savedAsset.Name);
            Assert.AreEqual(asset.Capacity,savedAsset.Capacity);
            Assert.AreEqual(asset.SerialNo,savedAsset.SerialNo);
            Assert.AreEqual(asset.AssetNo,savedAsset.AssetNo);
            Assert.AreEqual(asset.AssetType,savedAsset.AssetType);
            Assert.AreEqual(asset.AssetStatus,savedAsset.AssetStatus);
            Assert.AreEqual(asset.AssetCategory,savedAsset.AssetCategory);
            Assert.AreEqual(asset._Status,EntityStatus.Active);
        }
    }
}