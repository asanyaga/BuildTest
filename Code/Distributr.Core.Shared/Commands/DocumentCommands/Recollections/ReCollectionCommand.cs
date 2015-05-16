using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Commands.DocumentCommands.Recollections
{
   public class ReCollectionCommand:DocumentCommand
    {
       public Guid DocumentRecipientCostCentreId { get; set; }
       public Guid FromCostCentreId { get; set; }
       public Guid ItemId { get; set; }
       public decimal Amount { get; set; }
       public decimal PaidAmount { get; set; }
       public bool IsConfirm { get; set; }
       public int RecollectionType { get; set; }
       public override string CommandTypeRef
       {
           get { return CommandType.ReCollection.ToString(); }
       }
    }
}
