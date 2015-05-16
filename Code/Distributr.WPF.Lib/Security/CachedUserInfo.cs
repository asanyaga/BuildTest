using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.WPF.Lib.Security
{
  public class CachedUserInfo
    {
        public string TokenId { get; set; }
        public string Username { get; set; }
        public EagcContactType ContactType { get; set; }
        public string ContactId { get; set; }
       
        
    }

  public enum EagcContactType
  {
      DepositorType = 1,
      BuyerType = 2,
      VoucherAdminOrgType = 3,
      VoucherCentreClerkType = 4,
      VoucherCentreAdminType = 5,
      BankType = 6

  }
}
