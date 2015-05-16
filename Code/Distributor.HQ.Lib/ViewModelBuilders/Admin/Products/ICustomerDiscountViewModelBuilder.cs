using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface ICustomerDiscountViewModelBuilder
    {
       CustomerDiscountViewModel Get(Guid id);
       void Save(CustomerDiscountViewModel customerDiscount);
       void SetInactive(Guid id);
       List<CustomerDiscountViewModel> GetAll(bool inactive=false);
       List<CustomerDiscountViewModel> Search(string srcParam, bool inactive = false);
       void AddCutomerDiscount(Guid discountId, decimal discountRate, DateTime effectiveDate);
       Dictionary<Guid, string> ProductList();
       Dictionary<Guid, string> OutletList();
    }
}
