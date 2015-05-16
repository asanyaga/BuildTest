using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders
{
   public interface ITargetViewModelBuilder
    {
       Dictionary<Guid, string> GetProduct();
       Dictionary<Guid, string> GetDistributor();
       Dictionary<Guid, string> GetPeriod();
       IList<TargetViewModel> GetAll(bool inactive = false);
       IList<TargetViewModel> Search(string srchParam,bool inactive = false);
       TargetViewModel GetById(Guid id);
       void Save(TargetViewModel trgViewModel);
       void SetInactive(Guid id);
       void Activate(Guid id);
       void Delete(Guid id);

       QueryResult<TargetViewModel> Query(QueryStandard query);
    }
}
