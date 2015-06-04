using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
 #if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
#if __MOBILE__
   [Table("VatClass")]
#endif
    public class VATClass : MasterEntity
    {
         public VATClass() : base(default(Guid)) { }

        public VATClass(Guid id)
            : base(id)
        {
            VATClassItems = new List<VATClassItem>();
        }
        public VATClass(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        public VATClass(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<VATClassItem> vatClassItems)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            VATClassItems = vatClassItems;
        }
        [Required(ErrorMessage = "Vat Class name is a required field")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vat class is a required field")]
        public string VatClass { get; set; }

        public decimal CurrentRate { get { return VatRate(); } }
        public DateTime CurrentEffectiveDate { get { return LatestVatClassItem().EffectiveDate; } }

    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<VATClassItem> VATClassItems { get; set; }

       private decimal VatRate()
       {
           if (LatestVatClassItem() != null)
           {
               return LatestVatClassItem().Rate;
           }
           return 0;
       }
        private VATClassItem LatestVatClassItem()
        {
            var items = VATClassItems.Where(n => n.EffectiveDate <= DateTime.Now)
                .OrderByDescending(n => n._DateCreated)
                .ThenByDescending(n => n.EffectiveDate);
            int total = (int)items.ToList().Count;
            if (total != 0)
            {
                return items.FirstOrDefault();
            }
            else
                return null;

           
        }

       public void AddVatClassItems(List<VATClassItem> items)
       {
           foreach(var item in items)
           {
               VATClassItems.Add(item);
           }
           
       }
#if !SILVERLIGHT
   [Serializable]
#endif
    #if __MOBILE__
        [Table("VatClassItems")]
    #endif
        public class VATClassItem : MasterEntity
        {
            public VATClassItem() : base(default(Guid)) { }

            public VATClassItem(Guid id)
                : base(id)
            {

            }

            public VATClassItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive)
            {

            }
            public decimal Rate { get; set; }
            public DateTime EffectiveDate { get; set; }

    #if __MOBILE__
            [ForeignKey(typeof (VATClass))]
            public Guid VatClassMasterId { get; set; }
    #endif
        }
    }
}
