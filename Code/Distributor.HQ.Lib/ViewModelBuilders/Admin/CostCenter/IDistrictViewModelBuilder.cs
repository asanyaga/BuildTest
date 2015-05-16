using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using System.Web.Mvc;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IDistrictViewModelBuilder
    {
       Dictionary<Guid, string> GetCountry();
       Dictionary<Guid, string> GetProvince();
       List<DistrictViewModel> GetAll(bool inactive=false);
       List<DistrictViewModel> Search(string srchParam,bool inactive = false);
       IList<DistrictViewModel> GetByProvince(Guid provinceId, bool inactive = false);
       DistrictViewModel GetById(Guid id);
       void Save(DistrictViewModel dvm);
       void SetInactive(Guid id);
       void SetActive(Guid id);
       void SetAsDeleted(Guid id);

       QueryResult<DistrictViewModel> Query(QueryStandard q);
    }
}
