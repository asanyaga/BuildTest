using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders
{
    public interface IAssetCategoryViewModelBuilder
    {
        IList<AssetCategoryViewModel> GetAll(bool inactive = false);
        IList<AssetCategoryViewModel> Search(string searchParam, bool inactive = false);
        IList<AssetCategoryViewModel> GetByAssetType(Guid assetTypeId, bool inactive = false);
        AssetCategoryViewModel Get(Guid id);
        void Save(AssetCategoryViewModel assetCategoryViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetDeleted(Guid id);
        Dictionary<Guid, string> AssetType();

        QueryResult<AssetCategoryViewModel> Query(QueryStandard query);
    }
}
