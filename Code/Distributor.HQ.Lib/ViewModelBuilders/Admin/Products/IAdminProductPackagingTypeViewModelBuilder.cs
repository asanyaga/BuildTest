using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IAdminProductPackagingTypeViewModelBuilder
    {
       IList<AdminProductPackagingTypeViewModel> GetAll(bool inactive = false);
       AdminProductPackagingTypeViewModel Get(Guid id);
       IList<AdminProductPackagingTypeViewModel> Search(string searchParam,bool inactive = false);
       void Save(AdminProductPackagingTypeViewModel adminProductPackagingTypeViewModel);
       void SetInActive(Guid id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);

       QueryResult<AdminProductPackagingTypeViewModel> Query(QueryStandard query);
    }
}
