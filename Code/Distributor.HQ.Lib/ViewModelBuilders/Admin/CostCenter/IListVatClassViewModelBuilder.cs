using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using System;
using System.Collections.Generic;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
  public  interface IListVatClassViewModelBuilder
    {
      IList<ListVatClassViewModel> GetAll(bool inactive = false);
      IList<ListVatClassViewModel> Search(string searchParam,bool inactive=false);
      ListVatClassViewModel Get(Guid Id);
      void Save(ListVatClassViewModel vatClass);
      void SetInactive(Guid id);
      void SetActive(Guid id);
      void SetAsDeleted(Guid id);

      QueryResult<ListVatClassViewModel> Query(QueryBase query);
    }
}
