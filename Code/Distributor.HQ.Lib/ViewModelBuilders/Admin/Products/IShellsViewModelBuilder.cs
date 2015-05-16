using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.Product;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Products
{
   public interface IShellsViewModelBuilder
    {
       IList<ShellsViewModel> GetAll(bool inactive = false);
       List<ShellsViewModel> Search(string srchParam, bool inactive = false);
       ShellsViewModel Get(Guid Id);
       void Save(ShellsViewModel shellsViewModel);
       void SetInactive(Guid id);
    }
}
