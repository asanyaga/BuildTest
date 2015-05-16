using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders
{
   public interface ICompetitorViewModelBuilder
    {
       List<CompetitorViewModel> GetAll(bool inactive=false);
       List<CompetitorViewModel> Search(string srchParam,bool inactive=false);
       CompetitorViewModel Get(Guid id);
       void Save(CompetitorViewModel cvm);
       void SetInactive(Guid id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);


       QueryResult<CompetitorViewModel> Query(QueryStandard query);
    }
}
