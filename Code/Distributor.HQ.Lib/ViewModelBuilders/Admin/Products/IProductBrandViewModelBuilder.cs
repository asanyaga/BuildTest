using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IProductBrandViewModelBuilder
    {
       IList<ProductBrandViewModel> GetAll(bool inactive = false);      
       IList<ProductBrandViewModel> Search(string brandName, bool inactive = false);
       ProductBrandViewModel Get(Guid id);
       void Save(ProductBrandViewModel productBrandViewModel);
       void SetInactive(Guid id);
       Dictionary<Guid, string> GetSuppliers();
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);



       QueryResult<ProductBrandViewModel> Query(QueryStandard query);
    }
}
