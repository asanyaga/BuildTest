using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IReturnablesViewModelBuilder
    {
       IList<ReturnablesViewModel> GetAll(bool inactive = false);
       List<ReturnablesViewModel> Search(string srchParam, bool inactive = false);
       ReturnablesViewModel Get(Guid Id);
       void Save(ReturnablesViewModel returnablesViewModel);
       void SetInactive(Guid id);
       Dictionary<Guid, string> Shells();
    }
}
