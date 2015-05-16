using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders
{
   public interface ITargetPeriodViewModelBuilder
    {
        IList<TargetPeriodViewModel> GetAll(bool inactive = false);
        List<TargetPeriodViewModel> Search(string srchParam, bool inactive = false);
        TargetPeriodViewModel Get(Guid Id);
        void Save(TargetPeriodViewModel targetPeriodViewModel);
        void SetInactive(Guid id);
        void Activate(Guid id);
        void Delete(Guid id);

       QueryResult<TargetPeriodViewModel> Query(QueryStandard query);
    }
}
