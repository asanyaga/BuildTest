using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.ReOrderLevelViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ReOrderLevelViewModelBuilders
{
  public  interface IReOrderLevelViewModelBuilder
    {
      List<ReOrderLevelViewModel> GetAll(bool inactive=false);
      List<ReOrderLevelViewModel> Search(string srcParam,bool inactive=false);
      ReOrderLevelViewModel GetById(Guid id);
      void Save(ReOrderLevelViewModel rolvm);
      void SetInactive(Guid id);
      void Activate(Guid id);
      void Delete(Guid id);
      Dictionary<Guid, string> GetDistributor();
      Dictionary<Guid, string> GetProducts();

      QueryResult<ReOrderLevelViewModel> Query(QueryStandard query);
    }
}
