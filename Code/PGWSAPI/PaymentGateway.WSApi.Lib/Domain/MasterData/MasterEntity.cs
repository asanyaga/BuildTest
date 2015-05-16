using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.MasterData
{
   public class MasterEntity
    {
       public int Id { set; get; }
       public DateTime DateCreated { set; get; }
       public DateTime DateUpdated { set; get; }
       public bool IsActive { set; get; }
    }
}
