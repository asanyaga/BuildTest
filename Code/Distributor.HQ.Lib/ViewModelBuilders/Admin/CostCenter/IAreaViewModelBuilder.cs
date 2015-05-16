using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IAreaViewModelBuilder
    {
       IList<AreaViewModel> GetAll(bool inactive = false);
       AreaViewModel Get(Guid Id);
       void Save(AreaViewModel areaViewModel);
       void SetInactive(Guid Id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);
       Dictionary<Guid, string> Region();
    }
}
