using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders
{
    public interface IAssetStatusViewModelBuilder
    {
        void Save(AssetStatusViewModel assetStatusVM);
        AssetStatusViewModel GetById(Guid Id);
        List<AssetStatusViewModel> GetAll(bool inactive = false);
        List<AssetStatusViewModel> Search(string srcParam, bool inactive = false);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetDeleted(Guid id);

        QueryResult<AssetStatusViewModel> Query(QueryStandard q);
    }
}
