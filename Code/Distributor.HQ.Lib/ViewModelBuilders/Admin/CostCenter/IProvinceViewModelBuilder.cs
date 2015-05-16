using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.Contact;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
   public interface IProvinceViewModelBuilder
    {
        IList<ProvinceViewModel> GetAll(bool inactive = false);
        List<ProvinceViewModel> Search(string srchParam, bool inactive = false);
        IList<ProvinceViewModel> GetByCountry(Guid countryId, bool inactive = false);
        ProvinceViewModel Get(Guid Id);
        void Save(ProvinceViewModel provinceViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid id);
        Dictionary<Guid, string> Country();
        void SetAsDeleted(Guid id);

        QueryResult<ProvinceViewModel> Query(QueryStandard q);


    }
}
