using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Distributr.Core.Domain.Master.OutletEntities;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl.OutletFactory;

namespace Distributor.Core.Tests.DomainFixtures.MasterDataFixtures.OutletFixtures
{
    [TestFixture]
    public class OutletCategoryFactoryFixture
    {
        IOutletCategoryFactory _OutletCategoryFactory;
        public OutletCategoryFactoryFixture()
        {
            _OutletCategoryFactory = new OutletCategoryFactory();
        }

        [Test]
        public void can_init_outletcategory()
        {
            string name = "Test Outlet Category @: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss");
            OutletCategory outletcategory = _OutletCategoryFactory.CreateOutletCategory(name);
            Assert.IsNotNull(outletcategory);
            Assert.AreEqual(name, outletcategory.Name);
        }
    }
}
