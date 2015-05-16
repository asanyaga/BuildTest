using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders
{
    public interface IAssetViewModelBuilder
    {        
        IList<AssetViewModel> GetAll(bool inactive = false);
        List<AssetViewModel> SearchAssets(string srchParam, bool inactive = false);
        AssetViewModel Get(Guid id);
        void Save(AssetViewModel assetviewmodel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<Guid, string> AssetType();
        Dictionary<Guid, string> AssetStatus();
        Dictionary<Guid, string> AssetCategory();

        QueryResult<AssetViewModel> Query(QueryStandard query);
    }
}
