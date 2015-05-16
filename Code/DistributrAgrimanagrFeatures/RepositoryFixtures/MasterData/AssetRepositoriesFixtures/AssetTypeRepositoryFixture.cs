using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.AssetRepositoriesFixtures
{
    [TestFixture]
    public class AssetTypeRepositoryFixture
    {
        private static IAssetTypeRepository _assetTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _assetTypeRepository = _testHelper.Ioc<IAssetTypeRepository>();
        }

        [Test]
        public void AssetTypeRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                //Save asset type
                var assetType = _testHelper.BuildAssetType();
                Trace.WriteLine(string.Format("Created asset type [{0}]", assetType.Name));
                var toSaveAssetType = _assetTypeRepository.Save(assetType);
                Trace.WriteLine(string.Format("Saved asset type Id [{0}]", toSaveAssetType));
                var savedAssetType = _assetTypeRepository.GetById(toSaveAssetType);

                AssertAsset(assetType, savedAssetType);

                //Asset type listing
                var queryResult =
                    _assetTypeRepository.Query(new QueryStandard() { Name = assetType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Asset type [{0}] exists in listing", assetType.Name));

                //Update asset type
                var toUpdateAssetType = savedAssetType;
                toUpdateAssetType.Name = "Asset type 2";
                toUpdateAssetType.Description = "Asset type 2";

                _assetTypeRepository.Save(toUpdateAssetType);

                var updatedAssetType = _assetTypeRepository.GetById(toUpdateAssetType.Id);
                Trace.WriteLine(string.Format("Updated asset type to Name  [{0}]", updatedAssetType.Name));

                AssertAsset(toUpdateAssetType, updatedAssetType);

                //Deactivate asset type
                var toDeactivate = updatedAssetType;
                toDeactivate._Status = EntityStatus.Inactive;

                _assetTypeRepository.Save(toDeactivate);

                var deactivated = _assetTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated asset type  to status  [{0}]", deactivated._Status));

                //Activate asset type
                var toActivate = updatedAssetType;
                toActivate._Status = EntityStatus.Active;

                _assetTypeRepository.Save(toActivate);

                var activated = _assetTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated asset type to status  [{0}]", activated._Status));

                //Delete asset type
                var toDelete = updatedAssetType;
                toDelete._Status = EntityStatus.Deleted;

                _assetTypeRepository.Save(toDelete);

                var deleted = _assetTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted asset type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertAsset(AssetType assetType, AssetType savedAssetType)
        {
            Assert.AreEqual(assetType.Name, savedAssetType.Name);
            Assert.AreEqual(assetType.Description, savedAssetType.Description);
            Assert.AreEqual(assetType._Status, EntityStatus.Active);
        }
    }
}