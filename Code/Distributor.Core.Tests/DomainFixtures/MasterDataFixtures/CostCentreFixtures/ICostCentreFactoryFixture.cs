using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Tests.DomainFixtures.MasterDataFixtures.CostCentreFixtures
{
    [TestFixture]
    public class ICostCentreFactoryFixture
    {
        ICostCentreFactory _costCentreFactory;
        [SetUp]
        public void setup()
        {
            _costCentreFactory = new CostCentreFactory();
        }

        [Test]
        public void can_create_producer_entity()
        {
            CostCentre cc = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            StandardWarehouse sw = cc as StandardWarehouse;
            Assert.IsNotNull(sw);
            Assert.IsNull(cc.ParentCostCentre);
            Assert.AreEqual(CostCentreType.Producer, sw.CostCentreType);
        }

        [Test]
        public void can_create_transporter_entity()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre transporter = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter, parent);
            Transporter t = transporter as Transporter;
            Assert.IsNotNull(t);
            Assert.AreEqual(parent.Id, t.ParentCostCentre.Id);
            Assert.AreEqual(CostCentreType.Transporter, t.CostCentreType);
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_transporter_entity_fails_if_parent_is_null()
        {
            CostCentre parent = null;
            CostCentre transporter = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter, parent);
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_transporter_entity_fails_if_parent_costcentretype_is_not_producer()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre distributor = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, parent);
            CostCentre transporter = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter, distributor);
        }

        [Test]
        public void can_create_distributor_entity()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre distributor = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, parent);
            Assert.IsNotNull(distributor);
            Assert.AreEqual(parent.Id, distributor.ParentCostCentre.Id);
            Assert.AreEqual(CostCentreType.Distributor, distributor.CostCentreType);
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_distributor_entity_fails_if_parent_is_null()
        {
            CostCentre parent = null;
            CostCentre distributor = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, parent);

        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_distributor_entity_fails_if_parent_is_not_producer()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre transporter = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Transporter, parent);
            CostCentre distributor = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, transporter);
        }

        [Test]
        public void can_create_distributorsalesmanwarehouse()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre distributor = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, parent);
            CostCentre distributorSalesmanWarehouse = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman, distributor);
            Assert.IsNotNull(distributorSalesmanWarehouse);
            Assert.AreEqual(distributor.Id, distributorSalesmanWarehouse.ParentCostCentre.Id);
            Assert.AreEqual(CostCentreType.DistributorSalesman, distributorSalesmanWarehouse.CostCentreType);
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_distributorsalesmanwarehouse_fails_if_parent_is_null()
        {
            CostCentre distributorSalesmanWarehouse = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman, null);

 
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_distributorsalesmanwarehouse_fails_if_parent_is_not_distributor()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre distributorSalesmanWarehouse = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.DistributorSalesman, parent);
        }

        [Test]
        public void can_create_outlet()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre distributor = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Distributor, parent);
            CostCentre outlet = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, distributor);
            Assert.IsNotNull(outlet);
            Assert.AreEqual(distributor.Id, outlet.ParentCostCentre.Id);
            Assert.AreEqual(CostCentreType.Outlet, outlet.CostCentreType);
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_outlet_fails_if_parent_is_null()
        {
            CostCentre outlet = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, null);
        }

        [Test]
        [ExpectedException("System.ArgumentException", UserMessage = "xxxxx")]
        public void create_outlet_fails_if_parent_is_not_distributor()
        {
            CostCentre parent = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Producer, null);
            CostCentre outlet = _costCentreFactory.CreateCostCentre(Guid.NewGuid(), CostCentreType.Outlet, parent);

        }
        
    }
}
