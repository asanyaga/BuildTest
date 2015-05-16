using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin;

namespace Distributr.HQ.Lib.ViewModelBuilders
{
    public interface IAdminProductViewModelBuilder
    {
        IList<AdminProductBrandViewModel> GetAll();
        AdminProductBrandViewModel Get(Guid id);
        void Save(AdminProductBrandViewModel adminProductBrandViewModel);

    }
}
