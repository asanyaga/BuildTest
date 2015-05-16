using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders.Impl
{
   public class CompetitorViewModelBuilder:ICompetitorViewModelBuilder
    {
       ICompetitorRepository _competitorRepository;
       public CompetitorViewModelBuilder(ICompetitorRepository competitorRepository)
       {
           _competitorRepository = competitorRepository;
       }
        public List<CompetitorViewModel> GetAll(bool inactive = false)
        {
            var items=_competitorRepository.GetAll(inactive);
            return items
                .Select(n => new CompetitorViewModel
                {
                     City=n.City,
                     ContactPerson=n.ContactPerson,
                     Name=n.Name,
                      Lattitude=n.Lattitude,
                       Longitude=n.Longitude,
                     isActive = n._Status == EntityStatus.Active ? true : false,
                        PhysicalAddress=n.PhysicalAddress,
                         PostalAddress=n.PostalAddress,
                          Telephone=n.Telephone,
                           Id=n.Id

                }
                ).ToList();
        }

        public List<CompetitorViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _competitorRepository.GetAll(inactive)
                .Where(n => n.Name != null && (n.Name.ToLower().StartsWith(srchParam.ToLower())) 
                    || n.PhysicalAddress != null && (n.PhysicalAddress.ToLower().StartsWith(srchParam.ToLower()))
                    || n.PostalAddress != null && (n.PostalAddress.ToLower().StartsWith(srchParam.ToLower())) 
                    || n.City != null && (n.City.ToLower().StartsWith(srchParam.ToLower())) 
                    || n.ContactPerson != null && (n.ContactPerson.ToLower().StartsWith(srchParam.ToLower())));
            return items
                .Select(n => new CompetitorViewModel
                {
                    City = n.City,
                    ContactPerson = n.ContactPerson,
                    Name = n.Name,
                    Lattitude = n.Lattitude,
                    Longitude = n.Longitude,
                    isActive = n._Status == EntityStatus.Active ? true : false,
                    PhysicalAddress = n.PhysicalAddress,
                    PostalAddress = n.PostalAddress,
                    Telephone = n.Telephone,
                    Id = n.Id

                }
                ).ToList();
        }

        public CompetitorViewModel Get(Guid id)
        {
            Competitor compe = _competitorRepository.GetById(id);
            if (compe == null) return null;
                
            return Map(compe);
        }
        CompetitorViewModel Map(Competitor comp)
        {
            return new CompetitorViewModel 
            {
             City =comp.City,
             isActive = comp._Status == EntityStatus.Active ? true : false,
               Id=comp.Id,
                Telephone=comp.Telephone,
                 PostalAddress=comp.PostalAddress,
                  PhysicalAddress=comp.PhysicalAddress,
                   Longitude=comp.Longitude,
                    Lattitude=comp.Lattitude,
                     ContactPerson=comp.ContactPerson,
                      Name=comp.Name
            };
        }

        public void Save(CompetitorViewModel cvm)
        {
            Competitor compe = new Competitor(cvm.Id) 
            {
                Name = cvm.Name,
                 ContactPerson=cvm.ContactPerson,
                  City=cvm.City,
                   Lattitude=cvm.Lattitude,
                    Longitude=cvm.Longitude,
                     PhysicalAddress=cvm.PhysicalAddress,
                      PostalAddress=cvm.PostalAddress,
                       Telephone=cvm.Telephone,
                       

            };
            _competitorRepository.Save(compe);
        }

        public void SetInactive(Guid id)
        {
            Competitor compe = _competitorRepository.GetById(id);
            _competitorRepository.SetInactive(compe);
        }


        public void SetActive(Guid id)
        {
            Competitor compe = _competitorRepository.GetById(id);
            _competitorRepository.SetActive(compe);
        }

        public void SetAsDeleted(Guid id)
        {
            Competitor compe = _competitorRepository.GetById(id);
            _competitorRepository.SetAsDeleted(compe);
        }

       public QueryResult<CompetitorViewModel> Query(QueryStandard query)
       {
           var queryResult = _competitorRepository.Query(query);
           var result = new QueryResult<CompetitorViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;

       }
    }
}
