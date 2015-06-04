using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class InventoryAdjustmentNoteLineItem : ProductLineItem
    {
        private InventoryAdjustmentNoteLineItem(Guid id)
            : base(id)
        {

        }

        public decimal Actual { get; set; }

    }
}
