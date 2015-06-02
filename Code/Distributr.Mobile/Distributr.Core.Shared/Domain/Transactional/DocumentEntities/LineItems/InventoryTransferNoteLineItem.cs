using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class InventoryTransferNoteLineItem : ProductLineItem
    {
        private InventoryTransferNoteLineItem(Guid id) : base(id)
        {

        }
    }
}
