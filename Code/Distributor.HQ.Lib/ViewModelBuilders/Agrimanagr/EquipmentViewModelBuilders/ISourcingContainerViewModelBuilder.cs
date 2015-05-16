using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.EquipmentViewModelBuilders
{
    public interface ISourcingContainerViewModelBuilder
    {
        IList<SourcingContainerViewModel> GetAll(bool inactive = false);
        List<SourcingContainerViewModel> SearchContainers(string srchParam, bool inactive = false);
        SourcingContainerViewModel Get(Guid id);
        void Save(SourcingContainerViewModel hubViewModel);
        void SetInactive(Guid Id);
        void SetActive(Guid Id);
        void SetAsDeleted(Guid Id);
        /*Dictionary<int, string> EquipmentTypes();*/
        Dictionary<Guid, string> CostCentres();
        /*Dictionary<Guid, string> Commodities();
        List<SelectListItem> GradeByCommodity(Guid id);
        List<SelectListItem> CommodityGrade(Guid commodityId, Guid gradeId);
        Dictionary<string, string> CommodityGrade(Guid commodityId,Guid gradeId);*/
        Dictionary<Guid, string> ContainerTypes();

        QueryResult<SourcingContainerViewModel> Query(QueryEquipment query);
        IList<SourcingContainerViewModel> QueryList(List<SourcingContainer> list);
    }
}
