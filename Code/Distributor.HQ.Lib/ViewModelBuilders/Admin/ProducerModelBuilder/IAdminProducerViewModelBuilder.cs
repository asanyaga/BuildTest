using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.ProducerModelBuilder
{
    public interface IAdminProducerViewModelBuilder
    {
        void save(AdminProducerViewModel adminProducerViewModel);
        AdminProducerViewModel Get_Producer();
        /*QueryResult<AdminProducerViewModel> Query(QueryStandard query);*/


    }
}
