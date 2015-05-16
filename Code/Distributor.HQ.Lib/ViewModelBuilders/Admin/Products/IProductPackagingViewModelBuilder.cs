using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IAdminProductPackagingViewModelbuilder
    {
        IList<AdminProductPackagingViewModel> GetAll(bool inactive = false);
        IList<AdminProductPackagingViewModel> Search(string searchParam,bool inactive = false);
        AdminProductPackagingViewModel Get(Guid id);
        void Save(AdminProductPackagingViewModel adminProductPackagingViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);

        AdminProductPackagingTypeViewModel GetProductPackagingSkipTake(bool inactive = false);
        QueryResult<AdminProductPackagingViewModel> Query(QueryStandard query);
    }
}
