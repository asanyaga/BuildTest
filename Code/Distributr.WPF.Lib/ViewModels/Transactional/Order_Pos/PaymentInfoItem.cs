using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos
{
  public  class PaymentInfoItem
    {
      public int SequenceNo { get; set; }
      public Guid Id { get; set; }
      public string PaymentTypeDisplayer { get; set; }
      public decimal Amount { get; set; }
      public decimal ConfirmedAmount { get; set; }
      public bool IsConfirmed { get; set; }
      public bool IsNotCredit { get; set; }

      public string BankInfo   { get; set; }

      public bool ShowConfirmHyperlink { get; set; }
    }
}
