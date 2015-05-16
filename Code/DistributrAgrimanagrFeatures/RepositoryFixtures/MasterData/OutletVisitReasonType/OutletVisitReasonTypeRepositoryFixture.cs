using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.OutletVisitReasonType
{
    [TestFixture]
    public class OutletVisitReasonTypeRepositoryFixture
    {
        private static IOutletVisitReasonsTypeRepository _outletVisitReasonsTypeRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _outletVisitReasonsTypeRepository = _testHelper.Ioc<IOutletVisitReasonsTypeRepository>();
        }

        [Test]
        public void OutletVisitReasonsTypeUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START OUTLET VISIT REASON TYPE REPOSITORY UNIT TEST....");

                //Save outlet visit reason
                var outletVisitReasonsType = _testHelper.BuildOutletVisitReasonsType();
                Trace.WriteLine(string.Format("Created Outlet Visit Reason Type [{0}]", outletVisitReasonsType.Name));
                var toSaveOutletVisitReasonsType = _outletVisitReasonsTypeRepository.Save(outletVisitReasonsType);
                Trace.WriteLine(string.Format("Saved Outlet Visit Reason Type Id [{0}]", toSaveOutletVisitReasonsType));
                var savedOutletVisitReasonsType = _outletVisitReasonsTypeRepository.GetById(toSaveOutletVisitReasonsType);

                AssertOutletVisitReasonsType(outletVisitReasonsType, savedOutletVisitReasonsType);

                //Outlet visit reason listing
                var queryResult =
                    _outletVisitReasonsTypeRepository.Query(new QueryStandard() { Name = outletVisitReasonsType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Outlet Visit Reason Type [{0}] exists in listing", outletVisitReasonsType.Name));

                //Update outlet visit reason
                var toUpdateOutletVisitReasonsType = outletVisitReasonsType;
                toUpdateOutletVisitReasonsType.Name = "Outlet Visit Reason Type 2";
                toUpdateOutletVisitReasonsType.Description = "Outlet Visit Reason Type 2";

                _outletVisitReasonsTypeRepository.Save(toUpdateOutletVisitReasonsType);

                var updatedOutletVisitReasonsType = _outletVisitReasonsTypeRepository.GetById(toUpdateOutletVisitReasonsType.Id);
                Trace.WriteLine(string.Format("Updated Outlet Visit Reason Type to Name  [{0}]", updatedOutletVisitReasonsType.Name));

                AssertOutletVisitReasonsType(toUpdateOutletVisitReasonsType, updatedOutletVisitReasonsType);

                //Deactivate outlet visit reason
                var toDeactivate = updatedOutletVisitReasonsType;
                toDeactivate._Status = EntityStatus.Inactive;

                _outletVisitReasonsTypeRepository.Save(toDeactivate);

                var deactivated = _outletVisitReasonsTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated Outlet Visit Reason Type to status  [{0}]", deactivated._Status));

                //Activate outlet visit reason
                var toActivate = updatedOutletVisitReasonsType;
                toActivate._Status = EntityStatus.Active;

                _outletVisitReasonsTypeRepository.Save(toActivate);

                var activated = _outletVisitReasonsTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated Outlet Visit Reason Type to status  [{0}]", activated._Status));

                //Delete outlet visit reason
                var toDelete = updatedOutletVisitReasonsType;
                toDelete._Status = EntityStatus.Deleted;

                _outletVisitReasonsTypeRepository.Save(toDelete);

                var deleted = _outletVisitReasonsTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted Outlet Visit Reason Type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertOutletVisitReasonsType(OutletVisitReasonsType toSaveOutletVisitReasonsType, OutletVisitReasonsType savedOutletVisitReasonsType)
        {
            Assert.AreEqual(toSaveOutletVisitReasonsType.Name,savedOutletVisitReasonsType.Name);
            Assert.AreEqual(toSaveOutletVisitReasonsType.Description,savedOutletVisitReasonsType.Description);
            Assert.AreEqual(toSaveOutletVisitReasonsType.OutletVisitAction,savedOutletVisitReasonsType.OutletVisitAction);
            Assert.AreEqual(toSaveOutletVisitReasonsType._Status,EntityStatus.Active);
        }
    }
}
