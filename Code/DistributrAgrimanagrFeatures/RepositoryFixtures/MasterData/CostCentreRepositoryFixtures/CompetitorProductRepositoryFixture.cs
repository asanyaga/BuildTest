using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using NUnit.Framework;

namespace Distributr.Core.Data._2015.Tests.RepositoryFixtures.MasterData.CostCentreRepositoryFixtures
{
    [TestFixture]
    public class CompetitorProductRepositoryFixture : RepositoryBaseFixture
    {
        private static ICompetitorProductsRepository _competitorProductRepository;

        [SetUp]
        public void Setup()
        {
            Setup_Helper();

            _competitorProductRepository = _testHelper.Ioc<ICompetitorProductsRepository>();
        }

        [Test]
        public void CompetitorRepositoryUnitTests()
        {
            using (var scope = new TransactionScope())
            {
                Trace.WriteLine("START COMPETITOR REPOSITORY UNIT TEST....");

                //Save competitor
                var competitorProduct = _testHelper.BuildCompetitorProduct();
                Trace.WriteLine(string.Format("Created competitor [{0}]", competitorProduct.ProductName));
                var toSaveCompetitorProduct = _competitorProductRepository.Save(competitorProduct);
                Trace.WriteLine(string.Format("Saved competitor Id [{0}]", toSaveCompetitorProduct));
                var savedCompetitorProduct = _competitorProductRepository.GetById(toSaveCompetitorProduct);

                AssertCompetitorProduct(competitorProduct, savedCompetitorProduct);

                //Competitor listing
                var queryResult =
                    _competitorProductRepository.Query(new QueryStandard() { Name = competitorProduct.ProductName });
                Assert.GreaterOrEqual(queryResult.Count, 1);
                Trace.WriteLine(string.Format("Competitor [{0}] exists in listing", competitorProduct.ProductName));

                //Update competitor
                var toUpdateCompetitorProduct = savedCompetitorProduct;
                toUpdateCompetitorProduct.ProductName = "Competitor Product 2";

                _competitorProductRepository.Save(toUpdateCompetitorProduct);

                var updatedCompetitorProduct = _competitorProductRepository.GetById(toUpdateCompetitorProduct.Id);
                Trace.WriteLine(string.Format("Updated competitor to Name  [{0}]", updatedCompetitorProduct.ProductName));

                AssertCompetitorProduct(toUpdateCompetitorProduct, updatedCompetitorProduct);

                //Deactivate competitor
                var toDeactivate = updatedCompetitorProduct;
                toDeactivate._Status = EntityStatus.Inactive;

                _competitorProductRepository.Save(toDeactivate);

                var deactivated = _competitorProductRepository.GetById(toDeactivate.Id);
                Assert.AreEqual(deactivated._Status, EntityStatus.Inactive);
                Trace.WriteLine(string.Format("Deactivated competitor  to status  [{0}]", deactivated._Status));

                //Activate competitor
                var toActivate = updatedCompetitorProduct;
                toActivate._Status = EntityStatus.Active;

                _competitorProductRepository.Save(toActivate);

                var activated = _competitorProductRepository.GetById(toActivate.Id);
                Assert.AreEqual(activated._Status, EntityStatus.Active);
                Trace.WriteLine(string.Format("Activated competitor to status  [{0}]", activated._Status));

                //Delete competitor
                var toDelete = updatedCompetitorProduct;
                toDelete._Status = EntityStatus.Deleted;

                _competitorProductRepository.Save(toDelete);

                var deleted = _competitorProductRepository.GetById(toActivate.Id);
                Assert.AreEqual(deleted._Status, EntityStatus.Deleted);
                Trace.WriteLine(string.Format("Deleted competitor to status  [{0}]", deleted._Status));

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private void AssertCompetitorProduct(CompetitorProducts competitor, CompetitorProducts savedCompetitor)
        {
            Assert.AreEqual(competitor.ProductName,savedCompetitor.ProductName);
            Assert.AreEqual(competitor.ProductDescription,savedCompetitor.ProductDescription);
            Assert.AreEqual(competitor._Status,EntityStatus.Active);
        }
    }
}