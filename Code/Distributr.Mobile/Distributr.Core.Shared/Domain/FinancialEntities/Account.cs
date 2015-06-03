using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.Domain.FinancialEntities
{
    public class Account 
    {
        public Guid Id { get; set; }
        public Guid CostcentreId { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
       
    }
}
