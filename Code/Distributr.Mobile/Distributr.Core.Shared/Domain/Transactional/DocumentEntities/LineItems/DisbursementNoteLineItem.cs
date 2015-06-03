using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class DisbursementNoteLineItem : ProductLineItem
    {
        public DisbursementNoteLineItem(Guid id) : base(id) { }
    }
}
