using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Distributr.HQ.Lib.ViewModels.Agrimanagr;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.SourcingContainerViewModelBuilders
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
        Dictionary<int, string> EquipmentTypes();
        Dictionary<Guid, string> CostCentres();
        Dictionary<Guid, string> Commodities();
        List<SelectListItem> GradeByCommodity(Guid id);
    }
}
