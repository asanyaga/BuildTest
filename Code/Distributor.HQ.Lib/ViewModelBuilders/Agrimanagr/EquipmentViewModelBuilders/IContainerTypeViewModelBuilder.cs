using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders
{
    public interface IContainerTypeViewModelBuilder
    {
      
        IList<ContainerTypeViewModel> GetAll(bool inactive = false);
        ContainerTypeViewModel Get(Guid id);
        ContainerTypeViewModel Setup();
        void Save(ContainerTypeViewModel containerTypeViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        Dictionary<Guid, string> Commodities();
        Dictionary<int, string> ContainerUserTypes();
        SelectList GradeByCommodity(Guid id);
        List<SelectListItem> CommodityGrade(Guid commodityId, Guid gradeId);

        QueryResult<ContainerTypeViewModel> Query(QueryStandard query);
    }
}
