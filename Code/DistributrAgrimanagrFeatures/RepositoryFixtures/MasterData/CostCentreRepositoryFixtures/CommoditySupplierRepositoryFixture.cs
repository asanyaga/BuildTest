using System;
using System.Diagnostics;
using System.Transactions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using DistributrAgrimanagrFeatures.Helpers.MasterData;
using NUnit.Framework;
using StructureMap;

namespace DistributrAgrimanagrFeatures.RepositoryFixtures.CostCentreRepositoryFixtures
{
   [TestFixture]
   public class CommoditySupplierRepositoryFixture
    {
       private static ICostCentreRepository _costCentreRepository;
       private static TestHelper _testHelper;

       [SetUp]
       public void Setup()
       {
           _testHelper = ObjectFactory.GetInstance<TestHelper>();
           _costCentreRepository = _testHelper.Ioc<ICostCentreRepository>();
       }

       [Test]
       public void CommoditySupplierRepositoryUnitTest()
       {
           using (var tra = new TransactionScope())
           {
               Trace.WriteLine(">/ADD COMMODITY SUPPLIER COSTCENTRE");

               CostCentre newCommoditySupplier = _testHelper.BuildCommoditySupplier();
               Guid newCommoditySupplierCcId = _costCentreRepository.Save(newCommoditySupplier);
               var createdCommoditySupplier = _costCentreRepository.GetById(newCommoditySupplierCcId);
               AssertCostCernter(createdCommoditySupplier, newCommoditySupplier);
               if (createdCommoditySupplier != null)
                   Trace.WriteLine(string.Format("Created CommoditySupplierCCId [{0}],-Name[{1}]",
                       newCommoditySupplierCcId, createdCommoditySupplier.Name));


               //Update
               CommoditySupplier CommoditySupplierToUpdate = _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(createdCommoditySupplier.Id,
                   CostCentreType.CommoditySupplier,
                   _testHelper.Ioc<ICostCentreRepository>().GetById(createdCommoditySupplier.ParentCostCentre.Id)) as CommoditySupplier;
               CommoditySupplierToUpdate.Name = "UpdateCommoditySupplierCC";
               CommoditySupplierToUpdate.CostCentreCode = "UdatedCode_2001";
               CommoditySupplierToUpdate.CommoditySupplierType = CommoditySupplierType.Individual;
               CommoditySupplierToUpdate.AccountName = "UpdatedAccountName";
               CommoditySupplierToUpdate.AccountNo = "UpdatedAccountName";
               CommoditySupplierToUpdate.JoinDate = DateTime.Now;
               Guid CommoditySupplierToUpdateId = _costCentreRepository.Save(CommoditySupplierToUpdate);
               var updatedCommoditySupplier = _costCentreRepository.GetById(CommoditySupplierToUpdateId);
               AssertCostCernter(CommoditySupplierToUpdate, updatedCommoditySupplier);
               Trace.WriteLine(string.Format("Updated CommoditySupplierCCId [{0}],-Name[{1}]", CommoditySupplierToUpdateId, updatedCommoditySupplier.Name));


               //CommoditySupplierCC Status Change
               _costCentreRepository.SetInactive(updatedCommoditySupplier);
               CostCentre CommoditySupplierStatus =
                   _costCentreRepository.GetById(CommoditySupplierToUpdateId);
               Assert.AreEqual(CommoditySupplierStatus._Status, EntityStatus.Inactive);
               Trace.WriteLine(string.Format("CommoditySupplierCC  Status[{0}]", CommoditySupplierStatus._Status));


               _costCentreRepository.SetActive(updatedCommoditySupplier);
               CommoditySupplierStatus =
                  _costCentreRepository.GetById(CommoditySupplierToUpdateId);
               Assert.AreEqual(CommoditySupplierStatus._Status, EntityStatus.Active);
               Trace.WriteLine(string.Format("CommoditySupplierCC  Status[{0}]", CommoditySupplierStatus._Status));

               _costCentreRepository.SetAsDeleted(updatedCommoditySupplier);
               CommoditySupplierStatus =
                  _costCentreRepository.GetById(CommoditySupplierToUpdateId);
               Assert.AreEqual(CommoditySupplierStatus._Status, EntityStatus.Deleted);
               Trace.WriteLine(string.Format("CommoditySupplierCC  Status[{0}]", CommoditySupplierStatus._Status));

               _testHelper.Ioc<ICacheProvider>().Reset();
           }

       }

       private void AssertCostCernter(CostCentre producerCostCentreX, CostCentre producerCostCentreY)
       {
           Assert.NotNull(producerCostCentreX.Id);
           Assert.AreEqual(producerCostCentreX.CostCentreType, producerCostCentreY.CostCentreType);
           Assert.AreEqual(producerCostCentreX.Name, producerCostCentreY.Name);
       }
    }
}
