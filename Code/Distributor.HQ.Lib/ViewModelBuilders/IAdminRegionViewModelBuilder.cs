using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin;

namespace Distributr.HQ.Lib.ViewModelBuilders
    {
   public interface IAdminRegionViewModelBuilder
    {
       IList<AdminRegionViewModel> GetAll();
       AdminRegionViewModel Get(int Id);
       void save(AdminRegionViewModel adminRegionViewModel);
    }
}
