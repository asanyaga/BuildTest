using System;
using Distributr.Core.Domain.Master.ProductEntities;
using SQLite.Net.Attributes;

namespace Distributr.Mobile.Products
{
    public class Pricing : ProductPricing
    {
        public decimal ExFactoryRate { get; set; }
        
        [Ignore]
        public override decimal CurrentExFactory { get { return ExFactoryRate; } }
        
        public decimal SellingPrice { get; set; }

        [Ignore]
        public override decimal CurrentSellingPrice { get { return SellingPrice; } }

        public DateTime EffectiveDate { get; set;  }

        [Ignore]
        public override DateTime CurrentEffectiveDate { get { return EffectiveDate;  } }

        public Guid LineItemId { get; set; }
    }
}