using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
    public interface IProductFlavoursViewModelBuilder
    {
        IList<ProductFlavoursViewModel> GetAll(bool inactive=false);
        IList<ProductFlavoursViewModel> Search(string searchParam,bool inactive=false);
        ProductFlavoursViewModel Get(Guid id);
        void Save(ProductFlavoursViewModel productflavoursviewmodel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        Dictionary<Guid, string> GetBrands();
        List<ProductFlavoursViewModel> GetByBrand(Guid brandId, bool inactive = false);
        ProductFlavoursViewModel GetFlavourSkipTake(bool inactive = false);




        QueryResult<ProductFlavoursViewModel> Query(QueryStandard query);
    }
}
