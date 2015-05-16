using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CentreViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CentreViewModelBuilders
{
    public interface ICentreTypeViewModelBuilder
    {
        IList<CentreTypeViewModel> GetAll(bool inactive = false);
        CentreTypeViewModel Get(Guid Id);
        void Save(CentreTypeViewModel assetTypeViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetDeleted(Guid Id);

        QueryResult<CentreTypeViewModel> Query(QueryStandard q);
    }
}
