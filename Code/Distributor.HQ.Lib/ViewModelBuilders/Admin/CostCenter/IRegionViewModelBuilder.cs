using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IRegionViewModelBuilder
    {
       IList<RegionViewModel> GetAll(bool inactive = false);
       void Save(RegionViewModel region);
       void SetInActive(Guid Id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);
       RegionViewModel Get(Guid id);
       Dictionary<Guid, string> Country();
       Dictionary<Guid, string> Province();
       Dictionary<Guid, string> District();
       
       IList<RegionViewModel> Search(string searchParam, bool inactive = false);
       QueryResult<RegionViewModel > Query(QueryStandard query);
       /*IList<RegionViewModel> QueryList(QueryResult result);*/
    }
}
