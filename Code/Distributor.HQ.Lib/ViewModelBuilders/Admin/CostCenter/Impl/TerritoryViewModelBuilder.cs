using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class TerritoryViewModelBuilder:ITerritoryViewModelBuilder
    {
       ITerritoryRepository _territoryRepository;
       public TerritoryViewModelBuilder(ITerritoryRepository territoryRepository)
       {
           _territoryRepository = territoryRepository;
       }
       public IList<TerritoryViewModel> GetAll(bool inactive = false)
        {
            var territory=_territoryRepository.GetAll(inactive);
            return territory
                .Select(n => new TerritoryViewModel
                {
                     Id=n.Id,
                     Name=n.Name,
                     isActive = n._Status == EntityStatus.Active ? true : false
                     
                }
                ).ToList();
        }
       public TerritoryViewModel Get(Guid Id)
        {
            Territory territory = _territoryRepository.GetById(Id);
            if (territory == null) return null;
              
            return Map(territory);
        }
       TerritoryViewModel Map(Territory territory)
        {
            return new TerritoryViewModel
            {
                 Id=territory.Id,
                 Name=territory.Name,
                 isActive = territory._Status == EntityStatus.Active ? true : false
            };
        }
       public void Save(TerritoryViewModel territory)
        {
            Territory terr = new Territory(territory.Id) 
            {
             Name=territory.Name
            };
            _territoryRepository.Save(terr);
        }
       public IList<TerritoryViewModel> Search(string searchParam, bool inactive = false)
        {
            var territory = _territoryRepository.GetAll(inactive).Where(n => n.Name.ToLower().StartsWith(searchParam.ToLower()));
            return territory
                .Select(n => new TerritoryViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    isActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }
       public void SetInactive(Guid Id)
       {
           Territory tr = _territoryRepository.GetById(Id);
           _territoryRepository.SetInactive(tr);
       }
       public void SetActive(Guid id)
       {
           Territory territory = _territoryRepository.GetById(id);
           _territoryRepository.SetActive(territory);
       }
       public void SetAsDeleted(Guid id)
       {
            Territory territory = _territoryRepository.GetById(id);
            _territoryRepository.SetAsDeleted(territory);
       }

       public QueryResult<TerritoryViewModel> Query(QueryStandard query)
       {
           var queryResult = _territoryRepository.Query(query);
           var result = new QueryResult<TerritoryViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;

       }
    }
}
