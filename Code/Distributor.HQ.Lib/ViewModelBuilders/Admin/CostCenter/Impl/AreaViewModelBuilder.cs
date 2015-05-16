using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class AreaViewModelBuilder:IAreaViewModelBuilder
    {
       IAreaRepository _areaRepository;
       IRegionRepository _regionRepository;
       public AreaViewModelBuilder(IAreaRepository areaRepository,IRegionRepository regionRepository)
       {
           _areaRepository = areaRepository;
           _regionRepository = regionRepository;
       }
        public IList<AreaViewModel> GetAll(bool inactive = false)
        {
            var areas = _areaRepository.GetAll(inactive);
            return areas
                .Select(n => new AreaViewModel
                {
                  Name=n.Name,
                   RegionId=n.region.Id,
                  isActive = n._Status == EntityStatus.Active ? true : false,
                     Description=n.Description,
                      Id=n.Id
                }
                ).ToList();
        }

        public AreaViewModel Get(Guid Id)
        {
            Area area = _areaRepository.GetById(Id);
            if (area == null) return null;
             
            return Map(area);
        }
        AreaViewModel Map(Area area)
        { 
        return new AreaViewModel 
        {
         Id=area.Id,
         Name=area.Name,
          Description=area.Description,
         isActive = area._Status == EntityStatus.Active ? true : false,
          RegionId=area.region.Id
        };
        }
        public void Save(AreaViewModel areaViewModel)
        {
            Area area = new Area(areaViewModel.Id)
            {
                 Name=areaViewModel.Name,
                 Description=areaViewModel.Description,
                  region=_regionRepository.GetById(areaViewModel.RegionId)
            };
            _areaRepository.Save(area);
        }

        public void SetInactive(Guid Id)
        {
            Area area=_areaRepository.GetById(Id);
            _areaRepository.SetInactive(area);
        }

       public void SetActive(Guid id)
       {
           Area area = _areaRepository.GetById(id);
           _areaRepository.SetActive(area);
       }

       public Dictionary<Guid, string> Region()
        {
            return _regionRepository.GetAll()
                .Select(r => new { r.Id,r.Name}).ToList().ToDictionary(d=>d.Id,d=>d.Name);
        }


       public void SetAsDeleted(Guid id)
       {
           Area area = _areaRepository.GetById(id);
           _areaRepository.SetAsDeleted(area);
       }
    }
}
