using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
    public interface ICountryViewModelBuilder
    {
        IList<CountryViewModel> GetAll(bool inactive = false);
        CountryViewModel Get(Guid id);
        void Save(CountryViewModel country);
        void SetInActive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);
        IList<CountryViewModel> Search(string searchParam, bool inactive = false);

        QueryResult<CountryViewModel> Query(QueryStandard query);

       /* IList<CountryViewModel> Querylist(QueryResult result);*/





    }
}
