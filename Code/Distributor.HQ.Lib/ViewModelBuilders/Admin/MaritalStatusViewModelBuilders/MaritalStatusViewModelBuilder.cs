using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.MaritalStatusViewModels;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.MaritalStatusViewModelBuilders
{
   public class MaritalStatusViewModelBuilder:IMaritalStatusViewModelBuilder
    {
      
       public MaritalStatusViewModelBuilder()//IMaritalStatusRepository maritalStatusRepository)
       {
           //_maritalStatusRepository = maritalStatusRepository;
       }
        public void Save(MaritalStatusViewModel mStatusVM)
        {
            MaritalStatus mStatus = new MaritalStatus(mStatusVM.Id)
            {
                MStatus=mStatusVM.Status,
                Description=mStatusVM.Description,
                Code=mStatusVM.Code,
            };
           // _maritalStatusRepository.Save(mStatus);
        }

        public MaritalStatusViewModel GetById(Guid Id)
        {
            //MaritalStatus   mStatus = _maritalStatusRepository.GetById(Id);
            //if (mStatus == null) return null;
               
           // return Map(mStatus);
            return null;
        }

        public List<MaritalStatusViewModel> GetAll(bool inactive = false)
        {
            return null;// _maritalStatusRepository.GetAll(inactive).Select(m => Map(m)).ToList();
        }

        public void SetInactive(Guid Id)
        {
           // MaritalStatus mStatus = _maritalStatusRepository.GetById(Id);
            //_maritalStatusRepository.SetInactive(mStatus);
        }

        public void SetDeleted(Guid Id)
        {
            //MaritalStatus mStatus = _maritalStatusRepository.GetById(Id);
            //_maritalStatusRepository.SetAsDeleted(mStatus);
        }

        MaritalStatusViewModel Map(MaritalStatus maritalStatus)
        {
            return new MaritalStatusViewModel
            {
            Code=maritalStatus.Code,
            Id=maritalStatus.Id,
            isActive = maritalStatus._Status == EntityStatus.Active ? true : false,
            Status=maritalStatus.MStatus,
             Description=maritalStatus.Description
            };
        }

        public void SetActive(Guid Id)
        {
            //MaritalStatus maritalStatus = _maritalStatusRepository.GetById(Id);
            //_maritalStatusRepository.SetActive(maritalStatus);
        }

       public List<MaritalStatusViewModel> Search(string srcParam, bool inactive = false)
        {
            return null; //return _maritalStatusRepository.GetAll(inactive).Where(m=>(m.Code.ToLower().StartsWith(srcParam.ToLower()))||(m.MStatus.ToLower().StartsWith(srcParam.ToLower()))||(m.Description.ToLower().StartsWith(srcParam.ToLower()))).Select(m => Map(m)).ToList();
        }
    }
}
