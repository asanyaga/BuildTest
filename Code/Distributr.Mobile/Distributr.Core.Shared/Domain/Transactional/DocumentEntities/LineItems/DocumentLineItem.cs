using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public abstract class DocumentLineItem : TransactionalEntity
    {
        public DocumentLineItem(Guid id) : base(id)
        {

        }

        public string Description { get; set; }
        public int LineItemSequenceNo { get; set; }
        internal bool IsNew { get; set; }   
    }

    
}
