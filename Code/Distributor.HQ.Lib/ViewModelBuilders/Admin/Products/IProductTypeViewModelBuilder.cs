using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IProductTypeViewModelBuilder
    {
        IList<AdminProductTypeViewModel> GetAll(bool inactive = false);
        AdminProductTypeViewModel Get(Guid id);
        void Save(AdminProductTypeViewModel adminProductTypeViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);

        QueryResult<AdminProductTypeViewModel> Query(QueryStandard query);
    }
}
