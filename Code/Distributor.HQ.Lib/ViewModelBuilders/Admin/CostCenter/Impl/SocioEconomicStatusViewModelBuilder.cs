using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class SocioEconomicStatusViewModelBuilder : ISocioEconomicStatusViewModelBuilder
    {
        
        ISocioEconomicStatusRepository _socioEconomicStatusViewModel;
        public SocioEconomicStatusViewModelBuilder(ISocioEconomicStatusRepository socioEconomicStatusViewModel)
        {
            _socioEconomicStatusViewModel = socioEconomicStatusViewModel;
        }

        public IList<SocioEconomicStatusViewModel> GetAll(bool inactive = false)
        {
            return _socioEconomicStatusViewModel.GetAll(inactive).Select(n => Map(n)).ToList();
        }

        public SocioEconomicStatusViewModel GetByID(Guid id)
        {
            SocioEconomicStatus socioEconomicStatus = _socioEconomicStatusViewModel.GetById(id);
            if (socioEconomicStatus == null) return null;
               
            return Map(socioEconomicStatus);
        }

        public void Save(SocioEconomicStatusViewModel socioEconomicStatusViewModel)
        {
            SocioEconomicStatus socioEconomicStatus = new SocioEconomicStatus(socioEconomicStatusViewModel.Id)
            {
                EcoStatus = socioEconomicStatusViewModel.Status
            };
            _socioEconomicStatusViewModel.Save(socioEconomicStatus);
        }

        public void SetInActive(Guid id)
        {
            SocioEconomicStatus oc = _socioEconomicStatusViewModel.GetById(id);
            _socioEconomicStatusViewModel.SetInactive(oc);
        }

        public SocioEconomicStatusViewModel Map(SocioEconomicStatus socioEconomicStatus)
        {
            return new SocioEconomicStatusViewModel
            {
                Id       = socioEconomicStatus.Id,
                Status   = socioEconomicStatus.EcoStatus,
                isActive = socioEconomicStatus._Status == EntityStatus.Active ? true : false
            };
        }

    }
}
