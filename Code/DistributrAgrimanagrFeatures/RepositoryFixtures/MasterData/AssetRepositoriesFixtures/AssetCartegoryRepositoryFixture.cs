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
    public class AssetCartegoryRepositoryFixture
    {
        private static IAssetCategoryRepository _assetCategoryRepository;
        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _assetCategoryRepository = _testHelper.Ioc<IAssetCategoryRepository>();
        }

        [Test]
        public void AssetTypeRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                //Save asset cartegory
                var assetCategory = _testHelper.BuildAssetCategory();
                Trace.WriteLine(string.Format("Created asset cartegory [{0}]", assetCategory.Name));
                var toSaveAssetCartegory = _assetCategoryRepository.Save(assetCategory);
                Trace.WriteLine(string.Format("Saved asset cartegory Id [{0}]", toSaveAssetCartegory));
                var savedAsseCartegory = _assetCategoryRepository.GetById(toSaveAssetCartegory);

                AssertAssetCartegory(assetCategory, savedAsseCartegory);

                //Asset cartegory listing
                var queryResult =
                    _assetCategoryRepository.Query(new QueryStandard() { Name = assetCategory.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Asset cartegory [{0}] exists in listing", assetCategory.Name));

                //Update asset cartegory
                var toUpdateAssetCartegory = savedAsseCartegory;
                toUpdateAssetCartegory.Name = "Asset cartegory 2";
                toUpdateAssetCartegory.Description = "Asset cartegory 2";

                _assetCategoryRepository.Save(toUpdateAssetCartegory);

                var updatedAssetCartegory = _assetCategoryRepository.GetById(toUpdateAssetCartegory.Id);
                Trace.WriteLine(string.Format("Updated asset cartegory to Name  [{0}]", updatedAssetCartegory.Name));

                AssertAssetCartegory(toUpdateAssetCartegory, updatedAssetCartegory);

                //Deactivate asset cartegory
                var toDeactivate = updatedAssetCartegory;
                toDeactivate._Status = EntityStatus.Inactive;

                _assetCategoryRepository.Save(toDeactivate);

                var deactivated = _assetCategoryRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated asset cartegory  to status  [{0}]", deactivated._Status));

                //Activate asset cartegory
                var toActivate = updatedAssetCartegory;
                toActivate._Status = EntityStatus.Active;

                _assetCategoryRepository.Save(toActivate);

                var activated = _assetCategoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated asset cartegory to status  [{0}]", activated._Status));

                //Delete asset cartegory
                var toDelete = updatedAssetCartegory;
                toDelete._Status = EntityStatus.Deleted;

                _assetCategoryRepository.Save(toDelete);

                var deleted = _assetCategoryRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted asset cartegory to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertAssetCartegory(AssetCategory assetCategory, AssetCategory savedAsseCartegory)
        {
            Assert.AreEqual(assetCategory.Name,savedAsseCartegory.Name);
            Assert.AreEqual(assetCategory.Description,savedAsseCartegory.Description);
            Assert.AreEqual(assetCategory.AssetType,savedAsseCartegory.AssetType);
            Assert.AreEqual(assetCategory._Status,EntityStatus.Active);
        }
    }
}