using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.WPF.Lib.Impl.Model.Transactional
{
  public  class AppTempTransaction
    {
      public Guid Id { get; set; }
      public string TransactionType { get; set; }
     [Column(TypeName = "ntext")]
      public string Json { get; set; }
      public DateTime DateInserted { get; set; }
      public bool TransactionStatus { get; set; }
    }

   
}
