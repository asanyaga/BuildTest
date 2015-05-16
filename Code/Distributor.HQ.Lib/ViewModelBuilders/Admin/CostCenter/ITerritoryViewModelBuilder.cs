using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface ITerritoryViewModelBuilder
    {
       IList<TerritoryViewModel> GetAll(bool inactive = false);
       IList<TerritoryViewModel> Search(string searchParam,bool inactive = false);
       TerritoryViewModel Get(Guid Id);
       void Save(TerritoryViewModel territory);
       void SetInactive(Guid Id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);

       QueryResult<TerritoryViewModel> Query(QueryStandard query);
    }
}
