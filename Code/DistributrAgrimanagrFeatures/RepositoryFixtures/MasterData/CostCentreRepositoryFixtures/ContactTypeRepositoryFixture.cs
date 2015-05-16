using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using NUnit.Framework;

namespace Distributr.Core.Data._2015.Tests.RepositoryFixtures.MasterData.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class ContactTypeRepositoryFixture : RepositoryBaseFixture
    {
        private static IContactTypeRepository _contactTypeRepository;

        [SetUp]
        public void Setup()
        {
            Setup_Helper();

            _contactTypeRepository = _testHelper.Ioc<IContactTypeRepository>();
        }

        [Test]
        public void ContactTypeRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START TERRITORY REPOSITORY UNIT TEST....");

                //Save contact type
                var contactType = _testHelper.BuildContactType();
                Trace.WriteLine(string.Format("Created contact type [{0}]", contactType.Name));
                var toSaveContactType = _contactTypeRepository.Save(contactType);
                Trace.WriteLine(string.Format("Saved contact type Id [{0}]", toSaveContactType));
                var savedContactType = _contactTypeRepository.GetById(toSaveContactType);

                AssertContactType(contactType, savedContactType);

                //Contact type listing
                var queryResult =
                    _contactTypeRepository.Query(new QueryStandard() { Name = contactType.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Contact type [{0}] exists in listing", contactType.Name));

                //Update contact type
                var toUpdateContactType = savedContactType;
                toUpdateContactType.Name = "Contact type 2";

                _contactTypeRepository.Save(toUpdateContactType);

                var updatedContactType = _contactTypeRepository.GetById(toUpdateContactType.Id);
                Trace.WriteLine(string.Format("Updated contact type to Name  [{0}]", updatedContactType.Name));

                AssertContactType(toUpdateContactType, updatedContactType);

                //Deactivate contact type
                var toDeactivate = updatedContactType;
                toDeactivate._Status = EntityStatus.Inactive;

                _contactTypeRepository.Save(toDeactivate);

                var deactivated = _contactTypeRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated contact type  to status  [{0}]", deactivated._Status));

                //Activate contact type
                var toActivate = updatedContactType;
                toActivate._Status = EntityStatus.Active;

                _contactTypeRepository.Save(toActivate);

                var activated = _contactTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated contact type to status  [{0}]", activated._Status));

                //Delete contact type
                var toDelete = updatedContactType;
                toDelete._Status = EntityStatus.Deleted;

                _contactTypeRepository.Save(toDelete);

                var deleted = _contactTypeRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted contact type to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertContactType(ContactType competitor, ContactType savedCompetitor)
        {
            Assert.AreEqual(competitor.Name,savedCompetitor.Name);
            Assert.AreEqual(competitor.Code,savedCompetitor.Code);
            Assert.AreEqual(competitor.Description,savedCompetitor.Description);
            Assert.AreEqual(competitor._Status,EntityStatus.Active);
        }
    }
}