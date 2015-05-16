using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class SocioEconomicStatuViewModelBuilder : ISocioEconomicStatuViewModelBuilder
    {
        
        ISocioEconomicStatusRepository _socioEconomicStatusViewModel;
        public SocioEconomicStatuViewModelBuilder(ISocioEconomicStatusRepository socioEconomicStatusViewModel)
        {
            _socioEconomicStatusViewModel = socioEconomicStatusViewModel;
        }

        public IList<SocioEconomicStatusViewModel> GetAll(bool inactive = false)
        {
            return _socioEconomicStatusViewModel.GetAll().Select(n => Map(n)).ToList();
        }

        public SocioEconomicStatusViewModel GetByID(int id)
        {
            SocioEconomicStatus socioEconomicStatus = new SocioEconomicStatus(0);
            if (id > 0)
                socioEconomicStatus = _socioEconomicStatusViewModel.GetById(id);
            return Map(socioEconomicStatus);
        }

        public void Save(SocioEconomicStatusViewModel socioEconomicStatusViewModel)
        {
            SocioEconomicStatus socioEconomicStatus = new SocioEconomicStatus(socioEconomicStatusViewModel.Id)
            {
                Status = socioEconomicStatusViewModel.Status
            };
            _socioEconomicStatusViewModel.Save(socioEconomicStatus);
        }

        public void SetInActive(int id)
        {
            SocioEconomicStatus oc = _socioEconomicStatusViewModel.GetById(id);
            _socioEconomicStatusViewModel.SetInactive(oc);
        }

        public SocioEconomicStatusViewModel Map(SocioEconomicStatus socioEconomicStatus)
        {
            return new SocioEconomicStatusViewModel
            {
                 Id     = socioEconomicStatus.Id,
                 Status = socioEconomicStatus.Status
            };
        }

    }
}
