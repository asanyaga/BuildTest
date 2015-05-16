using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IDiscountGroupViewModelBuilder
    {
       DiscountGroupViewModel Get(Guid id);
       void Save(DiscountGroupViewModel customerDiscount);
       void SetInactive(Guid id);
        List<DiscountGroupViewModel> GetAll(bool inactive = false);
        List<DiscountGroupViewModel> Search(string srcParam, bool inactive = false);
        QueryResult<DiscountGroupViewModel> Query(QueryStandard q);
    }
}
