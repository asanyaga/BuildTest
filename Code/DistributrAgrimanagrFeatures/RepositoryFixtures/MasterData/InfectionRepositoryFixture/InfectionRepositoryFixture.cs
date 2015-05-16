using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.InfectionRepositoryFixture
{
    [TestFixture]
    public class InfectionRepositoryFixture
    {
        private static IInfectionRepository _infectionRepository;

        private static TestHelper _testHelper;

        [SetUp]
        public void Setup()
        {
            _testHelper = ObjectFactory.GetInstance<TestHelper>();
            _infectionRepository = _testHelper.Ioc<IInfectionRepository>();
        }

        [Test]
        public void InfectionRepositoryOwnerUnitTest()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START INFECTION REPOSITORY UNIT TEST....");

                //Save infection
                var infection = _testHelper.BuildInfection();
                Trace.WriteLine(string.Format("Created infection [{0}]", infection.Name));
                var toSaveInfection = _infectionRepository.Save(infection);
                Trace.WriteLine(string.Format("Saved infection Id [{0}]", toSaveInfection));
                var savedInfection = _infectionRepository.GetById(toSaveInfection);

                AssertInfection(infection, savedInfection);

                //Infection listing
                /*var queryResult =
                    _infectionRepository.Query(new QueryStandard() { Name = infection.Name });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Infection [{0}] exists in listing", infection.Name));*/

                //Update infection
                var toUpdateInfection = savedInfection;
                toUpdateInfection.Name = "Infection 2";
                toUpdateInfection.Description = "Infection 2";

                _infectionRepository.Save(toUpdateInfection);

                var updatedInfection = _infectionRepository.GetById(toUpdateInfection.Id);
                Trace.WriteLine(string.Format("Updated infection to Name  [{0}]", updatedInfection.Name));

                AssertInfection(toUpdateInfection, updatedInfection);

                //Deactivate infection
                var toDeactivate = updatedInfection;
                toDeactivate._Status = EntityStatus.Inactive;

                _infectionRepository.Save(toDeactivate);

                var deactivated = _infectionRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated infection to status  [{0}]", deactivated._Status));

                //Activate infection
                var toActivate = updatedInfection;
                toActivate._Status = EntityStatus.Active;

                _infectionRepository.Save(toActivate);

                var activated = _infectionRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated infection to status  [{0}]", activated._Status));

                //Delete infection
                var toDelete = updatedInfection;
                toDelete._Status = EntityStatus.Deleted;

                _infectionRepository.Save(toDelete);

                var deleted = _infectionRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted infection to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertInfection(Infection infection, Infection savedInfection)
        {
            Assert.AreEqual(infection.Code, savedInfection.Code);
            Assert.AreEqual(infection.Name, savedInfection.Name);
            Assert.AreEqual(infection.Code, savedInfection.Code);
            Assert.AreEqual(infection.Description, savedInfection.Description);
            Assert.AreEqual(infection.InfectionType, savedInfection.InfectionType);
            Assert.AreEqual(infection._Status, savedInfection._Status);
        }
    }
}