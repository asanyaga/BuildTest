using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IVATClassViewModelBuilder
    {
       IList<VATClassViewModel> GetAll(bool inactive = false);
       IList<VATClassViewModel>Search(string searchParam,bool inactive=false);
       VATClassViewModel Get(Guid Id);
       void Save(VATClassViewModel vatClass);
       void SetInactive(Guid id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);
    }
}
