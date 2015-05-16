using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CustomerDiscount : MasterEntity
    {
        public CustomerDiscount(Guid id)
            : base(id)
        {
            CustomerDiscountItems = new List<CustomerDiscountItem>();
        }
        public CustomerDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        public CustomerDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<CustomerDiscountItem> customerDiscountItems)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            CustomerDiscountItems = customerDiscountItems;
        }

        public CostCentreRef Outlet { get; set; }
        public decimal CurrentDiscount { get { return LatestCustomerDiscountItem().DiscountRate; } }
        public DateTime CurrentEffectiveDate { get { return LatestCustomerDiscountItem().EffectiveDate; } }
        public ProductRef Product { get;set;}

        private CustomerDiscountItem LatestCustomerDiscountItem()
        {
            var items = CustomerDiscountItems.Where(n => n.EffectiveDate < DateTime.Now)
                .OrderByDescending(n => n._DateCreated)
                .ThenByDescending(n => n.EffectiveDate);
            return items.ToList().First();
        }
        public List<CustomerDiscountItem> CustomerDiscountItems { get; set; }

#if !SILVERLIGHT
   [Serializable]
#endif
        public class CustomerDiscountItem : MasterEntity
        {
            public CustomerDiscountItem(Guid id)
                : base(id) { }

            public CustomerDiscountItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive)
            { }

            public decimal DiscountRate { get; set; }
           
            public DateTime EffectiveDate { get; set; }
        }
    }
}
