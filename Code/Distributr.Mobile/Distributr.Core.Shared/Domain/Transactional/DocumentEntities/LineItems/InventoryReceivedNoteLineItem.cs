using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class InventoryReceivedNoteLineItem : ProductLineItem
    {
        private InventoryReceivedNoteLineItem(Guid id) : base(id)
        {
           
        }

        public decimal Expected { get; set; }
    }
}
