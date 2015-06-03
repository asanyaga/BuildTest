using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class ProductLineItem : DocumentLineItem
    {
        public ProductLineItem(Guid id) : base(id)
        {

        }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
        public decimal Value { get; set; }
        public decimal ProductDiscount { get; set; }
        public DiscountType DiscountType { get; set; }
    }
}
