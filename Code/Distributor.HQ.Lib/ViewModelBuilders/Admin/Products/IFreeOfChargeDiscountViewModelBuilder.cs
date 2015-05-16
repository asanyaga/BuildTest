using System;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IFreeOfChargeDiscountViewModelBuilder
    {
       void Save(FreeOfChargeDiscountViewModel model, out bool isEdit);
       List<FreeOfChargeDiscountViewModel> GetAll(bool inactive = false);
       FreeOfChargeDiscountViewModel Get(Guid id);
       List<FreeOfChargeDiscountViewModel> GetByBrand(Guid brandId, bool showInActive = false);
       List<FreeOfChargeDiscountViewModel> Search(string srcParam, bool inactive = false);
       Dictionary<Guid, string> BrandList();
       Dictionary<Guid, string> ProductList();

       QueryResult<FreeOfChargeDiscountViewModel> QueryResult(QueryFOCDiscount query); 

           void SetDeleted(Guid id);
    }
  
}
