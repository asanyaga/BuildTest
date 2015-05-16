using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class CostCentreViewModelBuilder : ICostCentreViewModelBuilder
    {
        //cn
        ICostCentreRepository _costCenterRepository;
        public CostCentreViewModelBuilder(ICostCentreRepository costCentreRepository)
        {
            _costCenterRepository = costCentreRepository;
        }

        public IList<CostCentreViewModel> GetAll(bool inactive = false)
        {
            return _costCenterRepository.GetAll(inactive).Select(n => Map(n)).ToList();
        }

        public CostCentreViewModel GetByID(Guid id)
        {
            CostCentre cc = _costCenterRepository.GetById(id);
            if (cc != null) return null;
            return Map(cc);
        }

        public void Save(CostCentreViewModel costCenter)
        {
            CostCentre cc = new Outlet(costCenter.Id)
            {
                CostCentreType = costCenter.CostCentreType,
                
            };
            _costCenterRepository.Save(cc);
        }

        public void SetInActive(Guid id)
        {
            CostCentre oc = _costCenterRepository.GetById(id);
            _costCenterRepository.SetInactive(oc);
        }

        public CostCentreViewModel Map(CostCentre costCentre)
        {
            return new CostCentreViewModel
            {
                CostCentreType   = costCentre.CostCentreType,
               
                ParentCostCentre = costCentre.ParentCostCentre,
                Contact          = costCentre.Contact
            };
        }


        public void SetInactive(Guid id)
        {
            CostCentre ctr = _costCenterRepository.GetById(id);
            _costCenterRepository.SetInactive(ctr);
        }
    }
}
