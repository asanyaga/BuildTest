using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CommodityOwnerRepositoryFixtures
{
    [TestFixture]
    public class CommodityOwnerRepositoryFixture
    {
        private static ICommodityOwnerRepository _commodityOwnerRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _commodityOwnerRepository = _testHelper.Ioc<ICommodityOwnerRepository>();
        }

        [Test]
        public void CommodityRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COMMODITY OWNER REPOSITORY UNIT TEST....");

                //Save commodity owner
                var commodityOwner = _testHelper.BuildCommodityOwner();
                Trace.WriteLine(string.Format("Created Commodity Owner [{0}]", commodityOwner.FirstName));
                var toSaveCommodityOwner = _commodityOwnerRepository.Save(commodityOwner);
                Trace.WriteLine(string.Format("Saved Commodity Owner Id [{0}]", toSaveCommodityOwner));
                var savedCommodityOwner = _commodityOwnerRepository.GetById(toSaveCommodityOwner);

                AssertCommodityOwner(commodityOwner, savedCommodityOwner);

                //Commodity owner listing
                /*var queryResult =
                    _commodityOwnerRepository.Query(new QueryStandard() { Name = commodityOwner.Code });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Commodity Owner [{0}] exists in listing", commodityOwner.FirstName));*/

                //Update commodity owner
                var toUpdateCommodityOwner = savedCommodityOwner;
                toUpdateCommodityOwner.FirstName = "Commodity Owner 2";
                toUpdateCommodityOwner.Surname = "Commodity Owner 2";
                toUpdateCommodityOwner.LastName = "Commodity Owner 2";
                toUpdateCommodityOwner.Description = "Commodity Owner 2";

                _commodityOwnerRepository.Save(toUpdateCommodityOwner);

                var updatedCommodityOwner = _commodityOwnerRepository.GetById(toUpdateCommodityOwner.Id);
                Trace.WriteLine(string.Format("Updated Commodity Owner Type to Name  [{0}]", updatedCommodityOwner.FirstName));

                AssertCommodityOwner(toUpdateCommodityOwner, updatedCommodityOwner);

                //Deactivate commodity owner
                var toDeactivate = updatedCommodityOwner;
                toDeactivate._Status = EntityStatus.Inactive;

                _commodityOwnerRepository.Save(toDeactivate);

                var deactivated = _commodityOwnerRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated Commodity Owner to status  [{0}]", deactivated._Status));

                //Activate commodity owner
                var toActivate = updatedCommodityOwner;
                toActivate._Status = EntityStatus.Active;

                _commodityOwnerRepository.Save(toActivate);

                var activated = _commodityOwnerRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated Commodity Owner to status  [{0}]", activated._Status));

                //Delete commodity owner
                var toDelete = updatedCommodityOwner;
                toDelete._Status = EntityStatus.Deleted;

                _commodityOwnerRepository.Save(toDelete);

                var deleted = _commodityOwnerRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Commodity Owner to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCommodityOwner(CommodityOwner commodityOwnerType, CommodityOwner savedCommodityOwnerType)
        {
            Assert.AreEqual(commodityOwnerType.Code, savedCommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.FirstName, savedCommodityOwnerType.FirstName);
            Assert.AreEqual(commodityOwnerType.Surname, savedCommodityOwnerType.Surname);
            Assert.AreEqual(commodityOwnerType.LastName, savedCommodityOwnerType.LastName);
            Assert.AreEqual(commodityOwnerType.Description, savedCommodityOwnerType.Description);
            Assert.AreEqual(commodityOwnerType.IdNo, savedCommodityOwnerType.IdNo);
            Assert.AreEqual(commodityOwnerType.PinNo, savedCommodityOwnerType.PinNo);
            Assert.AreEqual(commodityOwnerType.DateOfBirth, savedCommodityOwnerType.DateOfBirth);
            Assert.AreEqual(commodityOwnerType.MaritalStatus, savedCommodityOwnerType.MaritalStatus);
            Assert.AreEqual(commodityOwnerType.Gender, savedCommodityOwnerType.Gender);
            Assert.AreEqual(commodityOwnerType.PhysicalAddress, savedCommodityOwnerType.PhysicalAddress);
            Assert.AreEqual(commodityOwnerType.PostalAddress, savedCommodityOwnerType.PostalAddress);
            Assert.AreEqual(commodityOwnerType.Email, savedCommodityOwnerType.Email);
            Assert.AreEqual(commodityOwnerType.PhoneNumber, savedCommodityOwnerType.PhoneNumber);
            Assert.AreEqual(commodityOwnerType.BusinessNumber, savedCommodityOwnerType.BusinessNumber);
            Assert.AreEqual(commodityOwnerType.FaxNumber, savedCommodityOwnerType.FaxNumber);
            Assert.AreEqual(commodityOwnerType.OfficeNumber, savedCommodityOwnerType.OfficeNumber);
            Assert.AreEqual(commodityOwnerType.CommodityOwnerType.Code, savedCommodityOwnerType.CommodityOwnerType.Code);
            Assert.AreEqual(commodityOwnerType.CommoditySupplier.Id, savedCommodityOwnerType.CommoditySupplier.Id);
            Assert.AreEqual(commodityOwnerType._Status, savedCommodityOwnerType._Status);
        }
    }
}
