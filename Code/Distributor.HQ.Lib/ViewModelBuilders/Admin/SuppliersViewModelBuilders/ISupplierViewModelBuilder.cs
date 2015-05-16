using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.SuppliersViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.SuppliersViewModelBuilders
{
   public interface ISupplierViewModelBuilder
    {
       void Save(SupplierViewModel supplierViewModel);
       IList<SupplierViewModel> Search(string srchParam, bool inactive = false);
       SupplierViewModel GetById(Guid id);
       IList<SupplierViewModel> GetAll(bool inactive=false);
       //List<SupplierViewModel> Search(string srchParam, bool inactive = false);
       void SetInactive(Guid id);
	   void SetActive(Guid id);
       void SetDelete(Guid id);
       //SupplierViewModel GetSuppliersSkipTake(bool inactive=false);
       //SupplierViewModel Search(string brandName, bool inactive = false);

       QueryResult <SupplierViewModel> Query(QueryStandard query);
    }
}
