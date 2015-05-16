using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using NUnit.Framework;
using Distributr.Core.Domain.Value;
using FizzWare.NBuilder;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using NSubstitute;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl;
namespace Distributr.Core.Tests.DomainFixtures.MasterDataFixtures.CostCentreFixtures
{
    [TestFixture]
    public class ICostCentreRepositoryFixtures
    {
        ICostCentreRepository _ccr;
        ICostCentreFactory _factory;

        [SetUp]
        public void setup()
        {
            _ccr = Substitute.For<ICostCentreRepository>();
            _factory = new CostCentreFactory();
        }

        [Test]
        public void a_producer_is_a_costcentre()
        {
            Guid producerWHId = Guid.NewGuid();
            _ccr.GetById(producerWHId).Returns(CreateTestProducerWH());

            CostCentre ccp = _ccr.GetById(producerWHId);
            Assert.IsNotNull(ccp);
            //Assert.AreEqual(0, ccp.Id);
            Assert.IsTrue(ccp is CostCentre);
        }

        [Test]
        public void a_producer_is_a_standardwarehouse()
        {
            Guid producerWHId = Guid.NewGuid();
            _ccr.GetById(producerWHId).Returns(CreateTestProducerWH());

            CostCentre ccp = _ccr.GetById(producerWHId);
            Assert.IsTrue(ccp is StandardWarehouse);
        }

        [Test]
        public void a_producer_has_no_parent_costcentre()
        {
            Guid producerWHId = Guid.NewGuid();
            _ccr.GetById(producerWHId).Returns(CreateTestProducerWH());

            CostCentre ccp = _ccr.GetById(producerWHId);
            Assert.IsNull(ccp.ParentCostCentre); ;
        }

        [Test]
        public void a_producer_costcentretype_is_costcentretype_producer()
        {
            Guid producerWHId = Guid.NewGuid();
            _ccr.GetById(producerWHId).Returns(CreateTestProducerWH());

            CostCentre ccp = _ccr.GetById(producerWHId);
            Assert.AreEqual(CostCentreType.Producer, ccp.CostCentreType);

        }

        [Test]
        public void a_distributor_is_a_costcentre()
        {
            Guid distributorWHId = Guid.NewGuid();
            _ccr.GetById(distributorWHId).Returns(CreateTestDistributorWH());
            CostCentre ccd = _ccr.GetById(distributorWHId);
            //Assert.AreEqual(distributorWHId, ccd.Id);
            Assert.IsTrue(ccd is CostCentre);
        }

        [Test]
        public void a_distributor_is_a_standardwarehouse()
        {
            Guid distributorWHId = Guid.NewGuid();
            _ccr.GetById(distributorWHId).Returns(CreateTestDistributorWH());
            CostCentre ccd = _ccr.GetById(distributorWHId);
            Assert.IsTrue(ccd is StandardWarehouse);
        }

        [Test]
        public void a_distributor_costcentretype_is_costcentretype_distributor()
        {
            Guid distributorWHId = Guid.NewGuid();
            _ccr.GetById(distributorWHId).Returns(CreateTestDistributorWH());
            CostCentre ccd = _ccr.GetById(distributorWHId);
            Assert.AreEqual(ccd.CostCentreType, CostCentreType.Distributor);
        }

        [Test]
        public void an_outlet_is_a_costcentre()
        {
            Guid outletId = Guid.NewGuid();
            _ccr.GetById(outletId).Returns(CreateTestOutlet());
            CostCentre o = _ccr.GetById(outletId);
            //Assert.AreEqual(outletId, o.Id);
            Assert.IsTrue(o is CostCentre);
        }

        [Test]
        public void an_outlet_is_costcentretype_outlet()
        {
            Guid outletId = Guid.NewGuid();
            _ccr.GetById(outletId).Returns(CreateTestOutlet());
            CostCentre o = _ccr.GetById(outletId);
            Assert.AreEqual(CostCentreType.Outlet, o.CostCentreType);
        }

        [Test]
        public void a_transporter_is_a_costcentre()
        {
            Guid transporterid = Guid.NewGuid();
            _ccr.GetById(transporterid).Returns(CreateTestTransporter());
            CostCentre cc = _ccr.GetById(transporterid);
            //Assert.AreEqual(transporterid, cc.Id);
            Assert.IsTrue(cc is CostCentre);
        }

        [Test]
        public void a_transporter_is_costcentretype_transporter()
        {
            Guid transporterid = Guid.NewGuid();
            _ccr.GetById(transporterid).Returns(CreateTestTransporter());
            CostCentre cc = _ccr.GetById(transporterid);
            Assert.AreEqual(CostCentreType.Transporter, cc.CostCentreType);
        }

        [Test]
        public void a_distributorsalesmanwarehouse_is_a_costcentre()
        {
            Guid id = Guid.NewGuid();
            _ccr.GetById(id).Returns(CreateTestDistributorSalesmanWarehouse());
            CostCentre cc = _ccr.GetById(id);
            //Assert.AreEqual(id, cc.Id);
            Assert.IsTrue(cc is CostCentre);
        }

        [Test]
        public void a_distributorsalesmanwarehouse_is_costcentretype_distributorsalesman()
        {
            Guid id = Guid.NewGuid();
            _ccr.GetById(id).Returns(CreateTestDistributorSalesmanWarehouse());
            CostCentre cc = _ccr.GetById(id);
            Assert.AreEqual(CostCentreType.DistributorSalesman, cc.CostCentreType);

        }

        private CostCentre CreateTestDistributorSalesmanWarehouse()
        {
            CostCentre cc = _factory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman, CreateTestDistributorWH());
            cc.Contact = CreateContact();
            return cc;
            
        }

        List<Contact> CreateContact()
        {
            return Builder<List<Contact>>.CreateNew().Build();
        }

        CostCentre CreateTestOutlet()
        {
            CostCentre cc = _factory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, CreateTestDistributorWH());
            cc.Contact = CreateContact();
            
            return cc; 
        }

        CostCentre CreateTestDistributorWH()
        {
            CostCentre cc = _factory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, CreateTestProducerWH());
            cc.Contact = CreateContact();
            return cc;
            
        }

        CostCentre CreateTestProducerWH()
        {
            CostCentre cc = _factory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            cc.Contact = CreateContact();
            return cc;
            
        }

        CostCentre CreateTestTransporter()
        {
            CostCentre cc = _factory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter, CreateTestProducerWH());
            cc.Contact = CreateContact();
            return cc;
            
        }

    }
}
