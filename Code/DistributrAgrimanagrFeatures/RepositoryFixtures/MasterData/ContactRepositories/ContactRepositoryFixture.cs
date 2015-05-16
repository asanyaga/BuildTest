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

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.ContactRepositories
{
    [TestFixture]
    public class ContactRepositoryFixture
    {
        private static IContactRepository _contactRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();

            _contactRepository = _testHelper.Ioc<IContactRepository>();
        }

        [Test]
        public void ContactRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START CONTACT REPOSITORY UNIT TEST....");

                //Save contact
                var contact = _testHelper.BuildContact();
                Trace.WriteLine(string.Format("Created contact [{0}]", contact.Firstname));
                var toSaveContact = _contactRepository.Save(contact);
                Trace.WriteLine(string.Format("Saved contact Id [{0}]", toSaveContact));
                var savedContact = _contactRepository.GetById(toSaveContact);

                AssertContact(contact, savedContact);

                //Contact listing
                var queryResult =
                    _contactRepository.Query(new QueryStandard() { Name = contact.Firstname });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Contact [{0}] exists in listing", contact.Firstname));

                //Update contact
                var toUpdateContact = savedContact;
                toUpdateContact.Firstname = "First Name 2";

                _contactRepository.Save(toUpdateContact);

                var updatedContact = _contactRepository.GetById(toUpdateContact.Id);
                Trace.WriteLine(string.Format("Updated contact to Name  [{0}]", updatedContact.Firstname));

                AssertContact(toUpdateContact, updatedContact);

                //Deactivate contact
                var toDeactivate = updatedContact;
                toDeactivate._Status = EntityStatus.Inactive;

                _contactRepository.Save(toDeactivate);

                var deactivated = _contactRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated contact to status  [{0}]", deactivated._Status));

                //Activate contact
                var toActivate = updatedContact;
                toActivate._Status = EntityStatus.Active;

                _contactRepository.Save(toActivate);

                var activated = _contactRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated contact to status  [{0}]", activated._Status));

                //Delete contact
                var toDelete = updatedContact;
                toDelete._Status = EntityStatus.Deleted;

                _contactRepository.Save(toDelete);

                var deleted = _contactRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted contact to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertContact(Contact competitor, Contact savedCompetitor)
        {
            Assert.AreEqual(competitor.Firstname,savedCompetitor.Firstname);
            Assert.AreEqual(competitor.BusinessPhone,savedCompetitor.BusinessPhone);
            Assert.AreEqual(competitor.Fax,savedCompetitor.Fax);
            Assert.AreEqual(competitor.MobilePhone,savedCompetitor.MobilePhone);
            Assert.AreEqual(competitor.MStatus,savedCompetitor.MStatus);
            Assert.AreEqual(competitor.ContactType.Code,savedCompetitor.ContactType.Code);
            Assert.AreEqual(competitor.PhysicalAddress, savedCompetitor.PhysicalAddress);
            Assert.AreEqual(competitor.ContactOwnerType, savedCompetitor.ContactOwnerType);
            Assert.AreEqual(competitor._Status, EntityStatus.Active);
        }
    }
}