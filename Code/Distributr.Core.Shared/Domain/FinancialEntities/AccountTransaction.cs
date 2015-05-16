using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.Domain.FinancialEntities
{
    public class AccountTransaction //: MasterEntity
    {

        public AccountTransaction(Guid id)
            
        {
            Id = id;
        }
        
        public Guid Id {get;set;}
        public Account Account { get; set; }
        public decimal Amount { get; set; }
        public DocumentType DocumentType { get; set; }
        public Guid DocumentId { get; set; }
        public DateTime DateInserted { get; set; }
    }
}
