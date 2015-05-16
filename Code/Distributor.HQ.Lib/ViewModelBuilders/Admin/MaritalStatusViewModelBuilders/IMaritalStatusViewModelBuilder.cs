using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.MaritalStatusViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.MaritalStatusViewModelBuilders
{
   public interface IMaritalStatusViewModelBuilder
    {
       void Save(MaritalStatusViewModel mStatusVM);
       MaritalStatusViewModel GetById(Guid Id);
       List<MaritalStatusViewModel> GetAll(bool inactive = false);
       List<MaritalStatusViewModel> Search(string srcParam, bool inactive = false);
       void SetActive(Guid Id);
       void SetInactive(Guid Id);
       void SetDeleted(Guid id);
    }
}
