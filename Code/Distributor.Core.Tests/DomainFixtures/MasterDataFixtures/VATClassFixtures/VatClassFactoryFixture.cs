using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Distributr.Core.Factory.Master;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Tests.DomainFixtures.MasterDataFixtures.VATClassFixtures
{
    [TestFixture]
    public class VatClassFactoryFixture
    {
        IVATClassFactory _vcFactory;
        public VatClassFactoryFixture()
        {
            _vcFactory = new VatClassFactory();
        }

        [Test]
        public void can_init_vatclass()
        {
            DateTime dt = DateTime.Now.Date;
            VATClass vc = _vcFactory.CreateVATClass("sddd", "ccc", 10, dt);

            Assert.IsNotNull(vc);
            Assert.AreEqual("sddd", vc.Name);
            Assert.AreEqual("ccc", vc.VatClass);
            Assert.AreEqual(10, vc.CurrentRate);
            Assert.AreEqual(dt, vc.CurrentEffectiveDate);
        }
    }
}
