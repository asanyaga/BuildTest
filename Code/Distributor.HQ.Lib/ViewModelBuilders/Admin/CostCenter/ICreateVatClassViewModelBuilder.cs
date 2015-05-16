using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
  public  interface ICreateVatClassViewModelBuilder
    {
      void Save(CreateVatClassViewModel vatClass);
    }
}
