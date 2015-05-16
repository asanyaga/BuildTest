using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction
{
   public interface IHQMailerViewModelBuilder
    {
       void Send(string source, string destination, string subject, string message);
    }
}
