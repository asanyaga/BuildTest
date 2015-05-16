using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders
{
    public interface IAssetTypeViewModelBuilder
    {
        IList<AssetTypeViewModel> GetAll(bool inactive =  false);
        List<AssetTypeViewModel> Search(string srchParam, bool inactive = false);
        AssetTypeViewModel Get(Guid Id);
        void Save(AssetTypeViewModel assetTypeViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetDeleted(Guid Id);

        QueryResult<AssetTypeViewModel> Query(QueryStandard query);
    }
}
