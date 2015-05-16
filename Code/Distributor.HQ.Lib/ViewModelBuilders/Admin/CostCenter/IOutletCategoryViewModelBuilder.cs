﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter
{
    public interface IOutletCategoryViewModelBuilder
    {
        //cn
        IList<OutletCategoryViewModel> GetAll(bool inactive = false);
        IList<OutletCategoryViewModel> Search(string searchParam, bool inactive = false);
        OutletCategoryViewModel GetByID(Guid id);
        void Save(OutletCategoryViewModel outletCategoryViewModel);
        void SetInactive(Guid id);
        void SetActive(Guid id);
        void SetAsDeleted(Guid id);

        QueryResult<OutletCategoryViewModel> Query(QueryStandard q);
    }
}
