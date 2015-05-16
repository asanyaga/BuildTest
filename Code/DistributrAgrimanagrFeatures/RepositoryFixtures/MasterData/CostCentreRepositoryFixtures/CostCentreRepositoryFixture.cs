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
    public class CostCentreRepositoryFixture
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
        public void CostCentreRepositoryUnitTest()
        {
            using (var tra = new TransactionScope())
            {
                Trace.WriteLine("START COSTCENTRE REPOSITORY UNIT TEST....");

                #region  ***Create CostCentres
                //Create CostCentres
                Trace.WriteLine(">/ADD PRODUCER COSTCENTRE");
                Producer newProducerCostCentre = _testHelper.BuildProducerCostCentre();
                Guid newProducerCcId = _costCentreRepository.Save(newProducerCostCentre);
                var createdProducerCostCentre = _costCentreRepository.GetById(newProducerCcId);
                AssertCostCernter(createdProducerCostCentre, newProducerCostCentre);
                Trace.WriteLine(string.Format("Created ProducerCCId [{0}], -Name[{1}]", newProducerCcId, createdProducerCostCentre.Name));

                //Transporter
                Trace.WriteLine(">/ADD TRANSPORTER COSTCENTRE");
                Transporter newTranporterCostCentre = _testHelper.BuildTransporterCostCentre();
                Guid newTranporterCcId = _costCentreRepository.Save(newTranporterCostCentre);
                var createdTranporterCostCentre = _costCentreRepository.GetById(newTranporterCcId) as Transporter;
                AssertCostCernter(createdTranporterCostCentre, newTranporterCostCentre);
                if (createdTranporterCostCentre != null)
                    Trace.WriteLine(string.Format("Created TranporterCCId [{0}],-Name[{1}]", newTranporterCcId, createdTranporterCostCentre.Name));

                //Distributor
                Trace.WriteLine(">/ADD DISTRIBUTOR COSTCENTRE");
                Distributor newDistributrCostCentre = _testHelper.BuildDistributrCostCentre();
                Guid newDistributrCcId = _costCentreRepository.Save(newDistributrCostCentre);
                var createdDistributrCostCentre = _costCentreRepository.GetById(newDistributrCcId) as Distributor;
                AssertCostCernter(createdDistributrCostCentre, newDistributrCostCentre);
                if (createdDistributrCostCentre != null)
                    Trace.WriteLine(string.Format("Created DistributrCCId [{0}],-Name[{1}]", newDistributrCcId, createdDistributrCostCentre.Name));

                //Create Purchasing Clerk
                Trace.WriteLine(">/ADD PURCHASING CLERK");
                PurchasingClerk newPurchasingClerk = _testHelper.BuildPurchasingClerk(createdDistributrCostCentre.ParentCostCentre.Id, createdDistributrCostCentre.SalesRep.Group.Id);
                Guid newPurchasingClerkId = _costCentreRepository.Save(newPurchasingClerk);
                var createdPurchasingClerk = _costCentreRepository.GetById(newPurchasingClerkId) as CostCentre;
                AssertCostCernter(createdPurchasingClerk, newPurchasingClerk);
                Trace.WriteLine(string.Format("Created PurchasingClerkId [{0}], -Name[{1}]", newPurchasingClerkId, createdPurchasingClerk.Name));



                Trace.WriteLine(">/ADD STORE");
                Hub hub = _testHelper.BuildHub(createdDistributrCostCentre.Region);
                Guid hubId = _testHelper.Ioc<IHubRepository>().Save(hub);
                Store newStore = _testHelper.BuildStore(hubId);
                Guid newStoreId = _costCentreRepository.Save(newStore);
                var createdStore = _costCentreRepository.GetById(newStoreId);
                AssertCostCernter(createdStore, newStore);
                Trace.WriteLine(string.Format("Created StoreId [{0}], -Name[{1}]", newStoreId, createdStore.Name));


                #endregion

                #region    ***Update CostCentres

                //ProducerCC
                StandardWarehouse producerCCtoUpdate = _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(createdProducerCostCentre.Id, CostCentreType.Producer, null) as
                    StandardWarehouse;
                if (producerCCtoUpdate != null)
                {
                    producerCCtoUpdate.Name = "ProducerCC_updated";
                    producerCCtoUpdate.CostCentreCode = "ProdCC001_updated";
                    producerCCtoUpdate.VatRegistrationNo = "Prod_VatRegNo Updated";
                }
                Guid updatedProducerCcId = _costCentreRepository.Save(producerCCtoUpdate);
                Producer updatedProducerCC = _costCentreRepository.GetById(updatedProducerCcId) as Producer;
                Assert.IsNotNullOrEmpty(updatedProducerCcId.ToString());
                Trace.WriteLine(string.Format("Updated ProducerCCId [{0}],-Name[{1}],CCVatReg[{2}]", updatedProducerCcId, updatedProducerCC.Name, updatedProducerCC.VatRegistrationNo));


                //TransporterCC
                Transporter transporterCCtoUpdate =
                _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter,
                                                       _testHelper.Ioc<ICostCentreRepository>().GetById(createdTranporterCostCentre.ParentCostCentre.Id)) as Transporter;
                if (transporterCCtoUpdate != null)
                {
                    transporterCCtoUpdate.Name = "TransporterName_Updated";
                    transporterCCtoUpdate.CostCentreCode = "TCode002_Updated";
                    transporterCCtoUpdate.VehicleRegistrationNo = "KCC 998T";
                    transporterCCtoUpdate.DriverName = "Gitau";
                }
                Guid updatedTransporterCcId = _costCentreRepository.Save(transporterCCtoUpdate);
                Transporter updatedTransporter = _costCentreRepository.GetById(updatedTransporterCcId) as Transporter;
                Assert.IsNotNull(updatedTransporterCcId);
                Trace.WriteLine(string.Format("Updated TranporterCCId [{0}],-Name[{1}],DriverName[{2}],VehicleRegistrationNo[{3}]", updatedTransporterCcId, updatedTransporter.Name, updatedTransporter.DriverName, updatedTransporter.VehicleRegistrationNo));


                //DistributrCC
                StandardWarehouse standardWarehouse = _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(createdDistributrCostCentre.Id, CostCentreType.Distributor,
                                                       _testHelper.Ioc<ICostCentreRepository>().GetById(createdDistributrCostCentre.ParentCostCentre.Id)) as
                StandardWarehouse;
                var distributrCCtoUpdate = standardWarehouse as Distributor;
                distributrCCtoUpdate.VatRegistrationNo = "DistrCC_UpdatedVatRegNo";
                distributrCCtoUpdate.Name = "Wambui Distributr";
                distributrCCtoUpdate.Owner = "Wambui Nyaga";
                distributrCCtoUpdate.MerchantNumber = "Updated_MerchantNumber";
                distributrCCtoUpdate.PaybillNumber = "Updated_PaybillNo 0902";
                distributrCCtoUpdate.PIN = "90072";
                distributrCCtoUpdate._Status = EntityStatus.Active;
                distributrCCtoUpdate.Region = createdDistributrCostCentre.Region;
                Guid updatedDistributrCCId = _costCentreRepository.Save(distributrCCtoUpdate);
                var updatedDistributrCC = _costCentreRepository.GetById(updatedDistributrCCId) as Distributor;
                AssertCostCernter(distributrCCtoUpdate, updatedDistributrCC);
                Trace.WriteLine(string.Format("Updated DistributorCCId [{0}],-Name[{1}],Owner[{2}],PayBillNo[{3}],Region Name[{4}]", updatedDistributrCCId, updatedDistributrCC.Name, updatedDistributrCC.Owner, updatedDistributrCC.PaybillNumber, updatedDistributrCC.Region.Name));


                //Purchasing Clerk

                var purchasingClerkToUpdate = createdPurchasingClerk as PurchasingClerk;
                purchasingClerkToUpdate.Name = "Ann";
                purchasingClerkToUpdate.CostCentreCode = "pc_Code04";
                purchasingClerkToUpdate.ParentCostCentre = createdPurchasingClerk.ParentCostCentre;
                purchasingClerkToUpdate.CostCentreType = CostCentreType.PurchasingClerk;
                purchasingClerkToUpdate._Status = EntityStatus.Active;
                Guid updatePurchasingClerkId = _costCentreRepository.Save(purchasingClerkToUpdate);
                var updatedPurchasingClerk = _costCentreRepository.GetById(updatePurchasingClerkId) as PurchasingClerk;
                AssertCostCernter(purchasingClerkToUpdate, updatedPurchasingClerk);
                Trace.WriteLine(string.Format("Updated PCId [{0}],Name[{1}],Code[{2}]", updatePurchasingClerkId, updatedPurchasingClerk.Name, updatedPurchasingClerk.CostCentreCode));
               
                
                //STORECC
                CostCentre storecc = _testHelper.Ioc<ICostCentreFactory>().CreateCostCentre(createdStore.Id, CostCentreType.Store,
               _costCentreRepository.GetById(
                   createdStore.ParentCostCentre.Id));
                Store storeToUpdate = storecc as Store;
                storeToUpdate.Name = "BeansStore";
                storeToUpdate.CostCentreCode = "BeansStore001";
                Guid udatedStoreId = _costCentreRepository.Save(storeToUpdate);
                var updatedStore = _costCentreRepository.GetById(udatedStoreId);
                AssertCostCernter(updatedStore, storeToUpdate);
                Trace.WriteLine(string.Format("Updated StoreId [{0}], -Name[{1}]", udatedStoreId, updatedStore.Name));

                #endregion

                #region **COST CENTRE STATUS

                //ProducerCC Status Change
                SetInactive(updatedProducerCC, updatedProducerCcId);
                SetActive(updatedProducerCC, updatedProducerCcId);
                SetAsDeleted(updatedProducerCC, updatedProducerCcId);

                //TransporterCC Status Change
                SetInactive(updatedTransporter, updatedTransporterCcId);
                SetActive(updatedTransporter, updatedTransporterCcId);
                SetAsDeleted(updatedTransporter, updatedTransporterCcId);

                //DistributorCC Status Change
                SetInactive(updatedDistributrCC, updatedDistributrCCId);
                SetActive(updatedDistributrCC, updatedDistributrCCId);
                SetAsDeleted(updatedDistributrCC, updatedDistributrCCId);

                //Purchasing Clerk Status Change
                SetInactive(updatedPurchasingClerk, updatePurchasingClerkId);
                SetActive(updatedPurchasingClerk, updatePurchasingClerkId);
                SetAsDeleted(updatedPurchasingClerk, updatePurchasingClerkId);

                //Store Status Change
                SetInactive(updatedStore, udatedStoreId);
                SetActive(updatedStore, udatedStoreId);
                SetAsDeleted(updatedStore, udatedStoreId);

                #endregion

                _testHelper.Ioc<ICacheProvider>().Reset();
            }
        }

        private static void SetInactive(CostCentre updatedCC, Guid updatedCcId)
        {
            _costCentreRepository.SetInactive(updatedCC);
            CostCentre costCentre =
                _costCentreRepository.GetById(updatedCcId);
            Assert.AreEqual(costCentre._Status, EntityStatus.Inactive);
            Trace.WriteLine(string.Format("CostCentre[{0}] , Status[{1}]", costCentre.CostCentreType, costCentre._Status));
        }
        private static void SetActive(CostCentre updatedCC, Guid updatedCcId)
        {
            _costCentreRepository.SetActive(updatedCC);
            CostCentre costCentre = _costCentreRepository.GetById(updatedCcId);
            Assert.AreEqual(costCentre._Status, EntityStatus.Active);
            Trace.WriteLine(string.Format("CostCentre[{0}],Status[{1}]", costCentre.CostCentreType, costCentre._Status));
        }
        private static void SetAsDeleted(CostCentre updatedCC, Guid updatedCcId)
        {
            CostCentre costCentre;
            _costCentreRepository.SetAsDeleted(updatedCC);
            costCentre =
                _costCentreRepository.GetById(updatedCcId);
            Assert.AreEqual(costCentre._Status, EntityStatus.Deleted);
            Trace.WriteLine(string.Format("CostCentre[{0}],  Status[{1}]", costCentre.CostCentreType, costCentre._Status));
        }

        private void AssertCostCernter(CostCentre producerCostCentreX, CostCentre producerCostCentreY)
        {
            Assert.NotNull(producerCostCentreX.Id);
            Assert.AreEqual(producerCostCentreX.CostCentreType, producerCostCentreY.CostCentreType);
            Assert.AreEqual(producerCostCentreX.Name, producerCostCentreY.Name);
        }
    }
}
