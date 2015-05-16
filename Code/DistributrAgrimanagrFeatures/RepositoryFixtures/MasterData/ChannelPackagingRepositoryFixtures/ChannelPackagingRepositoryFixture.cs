using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ChannelPackagings;
using Distributr.Core.Repository.Master.ChannelPackagings;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ChannelPackagingRepositoryFixtures
{
    class ChannelPackagingRepositoryFixture
    {
        private static IChannelPackagingRepository _channelPackagingRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _channelPackagingRepository = _testHelper.Ioc<IChannelPackagingRepository>();
        }

        [Test]
        public void ChannelPackagingRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                //Save channel packaging
                var cPack = _testHelper.BuildChannelPack();
                Trace.WriteLine(string.Format("Created channel packaging [{0}]", cPack.Id));
                var toSaveCPack = _channelPackagingRepository.Save(cPack);
                Trace.WriteLine(string.Format("Saved channel packaging Id [{0}]", toSaveCPack));
                var savedCPack = _channelPackagingRepository.GetById(toSaveCPack);

                AssertChannelPackaging(cPack, savedCPack);

                /*//Channel packaging listing
                var queryResult =
                    _channelPackagingRepository.Query(new QueryStandard() {});
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Channel packagingy [{0}] exists in listing", cPack.Id));*/

                //Update channel packaging
                var toUpdateCPack = savedCPack;
                toUpdateCPack.IsChecked = false;

                _channelPackagingRepository.Save(toUpdateCPack);

                var updatedCPack = _channelPackagingRepository.GetById(toUpdateCPack.Id);
                Trace.WriteLine(string.Format("Updated asset cartegory to Name  [{0}]", updatedCPack.Id));

                AssertChannelPackaging(toUpdateCPack, updatedCPack);

                //Deactivate channel packaging
                var toDeactivate = updatedCPack;
                toDeactivate._Status = EntityStatus.Inactive;

                _channelPackagingRepository.Save(toDeactivate);

                var deactivated = _channelPackagingRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated channel packaging  to status  [{0}]", deactivated._Status));

                //Activate channel packaging
                var toActivate = updatedCPack;
                toActivate._Status = EntityStatus.Active;

                _channelPackagingRepository.Save(toActivate);

                var activated = _channelPackagingRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated channel packaging to status  [{0}]", activated._Status));

                //Delete channel packaging
                var toDelete = updatedCPack;
                toDelete._Status = EntityStatus.Deleted;

                _channelPackagingRepository.Save(toDelete);

                var deleted = _channelPackagingRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted channel packaging to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertChannelPackaging(ChannelPackaging assetCategory, ChannelPackaging savedAsseCartegory)
        {
            Assert.AreEqual(assetCategory.OutletType.Code, savedAsseCartegory.OutletType.Code);
            Assert.AreEqual(assetCategory.Packaging.Code, savedAsseCartegory.Packaging.Code);
            Assert.AreEqual(assetCategory.IsChecked, savedAsseCartegory.IsChecked);
            Assert.AreEqual(assetCategory._Status, EntityStatus.Active);
        }
    }
}
