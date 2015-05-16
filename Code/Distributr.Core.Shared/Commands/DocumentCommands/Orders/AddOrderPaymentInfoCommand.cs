using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
   public class AddOrderPaymentInfoCommand:DocumentCommand
    {
       public override string CommandTypeRef
       {
           get { return CommandType.OrderPaymentInfo.ToString(); }
       }
       public Guid InfoId { get; set; }
       public decimal Amount { get; set; }
       public decimal ConfirmedAmount { get; set; }
       public int PaymentModeId { get; set; }
       public string MMoneyPaymentType { get; set; }
       public string PaymentRefId { get; set; }
       public bool IsConfirmed { get; set; }
       public string NotificationId { get; set; }
       public bool IsProcessed { get; set; }
       public string Bank  { get; set; }
       public string BankBranch { get; set; }
       public DateTime DueDate { get; set; }
     

    }
}
