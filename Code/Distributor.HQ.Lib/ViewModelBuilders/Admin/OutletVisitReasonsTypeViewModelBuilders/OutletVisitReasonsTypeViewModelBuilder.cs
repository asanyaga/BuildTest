using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.OutletVisitReasonsTypeViewModels;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.OutletVisitReasonsTypeViewModelBuilders
{
    class OutletVisitReasonsTypeViewModelBuilder : IOutletVisitReasonsTypeViewModelBuilder
    {
         IOutletVisitReasonsTypeRepository _outletVisitReasonsTypeRepository;
         ICacheProvider _cacheProvider;
         private IMasterDataUsage _masterDataUsage;

         public OutletVisitReasonsTypeViewModelBuilder(IOutletVisitReasonsTypeRepository outletVisitReasonsTypeRepository, ICacheProvider cacheProvider, IMasterDataUsage masterDataUsage)
         {
             _outletVisitReasonsTypeRepository = outletVisitReasonsTypeRepository;
             _cacheProvider = cacheProvider;
             _masterDataUsage = masterDataUsage;
         } 


        public IList<ViewModels.Admin.OutletVisitReasonsTypeViewModels.OutletVisitReasonsTypeViewModel> GetAll(bool inactive = false)
        {
            return _outletVisitReasonsTypeRepository.GetAll(inactive).Select(n => Map(n)).ToList();
        }

        public ViewModels.Admin.OutletVisitReasonsTypeViewModels.OutletVisitReasonsTypeViewModel Get(Guid id)
        {
            OutletVisitReasonsType outletVrt = _outletVisitReasonsTypeRepository.GetById(id);
            if (outletVrt == null)
                return null;
            return Map(outletVrt);
        }

        OutletVisitReasonsTypeViewModel Map(OutletVisitReasonsType outletVisitReasonsType)
        {
            return new OutletVisitReasonsTypeViewModel
            {
                id = outletVisitReasonsType.Id,
                Name = outletVisitReasonsType.Name,
                Description  = outletVisitReasonsType.Description ,
                outletVisitAction = outletVisitReasonsType.OutletVisitAction ,
                _DateCreated = outletVisitReasonsType._DateCreated,
                _DateLastUpdated  = outletVisitReasonsType ._DateLastUpdated ,

                isActive = outletVisitReasonsType._Status == EntityStatus.Active ? true : false
            };
        }

        public void Save(OutletVisitReasonsTypeViewModel outletVisitReasonsType)
        {
            
            OutletVisitReasonsType outletVrt = new OutletVisitReasonsType(outletVisitReasonsType.id)
                                                   {Name = outletVisitReasonsType.Name ,
                                                       Description = outletVisitReasonsType.Description ,
                                                       OutletVisitAction = outletVisitReasonsType.outletVisitAction 
                                                    };
            if (outletVisitReasonsType.outletVisitAction == null)
            {
                outletVisitReasonsType.outletVisitAction = OutletVisitAction.NoAction;
            }
            _outletVisitReasonsTypeRepository.Save(outletVrt);
        }

        public void SetInActive(Guid id)
        {
            OutletVisitReasonsType outletVrt = _outletVisitReasonsTypeRepository.GetById(id);
            if (_masterDataUsage.CheckOutletIsUsed(outletVrt, EntityStatus.Inactive)) 
            {
            throw new DomainValidationException(new ValidationResultInfo(),
                                                "Cannot deactivate OutletVisitReasonsType");
        }
            _outletVisitReasonsTypeRepository.SetInactive(outletVrt);
        }

        public void SetActive(Guid id)
        {
            OutletVisitReasonsType outletVrt = _outletVisitReasonsTypeRepository.GetById(id);
            _outletVisitReasonsTypeRepository.SetActive(outletVrt);
        }

        public void SetAsDeleted(Guid id)
        {
            OutletVisitReasonsType outletVrt = _outletVisitReasonsTypeRepository.GetById(id);
           /* if (_masterDataUsage.CheckOutletIsUsed(outletVrt, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                                                    "Cannot deactivate OutletVisitReasonsType");
            }*/
            _outletVisitReasonsTypeRepository.SetAsDeleted(outletVrt);
        }

        public IList<OutletVisitReasonsTypeViewModel> Search(string searchParam, bool inactive = false)
        {
            var items = _outletVisitReasonsTypeRepository.GetAll(inactive)
                 .Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower()))
                             )
                 .ToList();
            return items.Select(n => new OutletVisitReasonsTypeViewModel
            {
                id = n.Id,
                isActive = n._Status == EntityStatus.Active ? true : false,
                Name = n.Name
            }).ToList();
        }

        public QueryResult<OutletVisitReasonsTypeViewModel> Query(QueryStandard query)
        {
            var queryResult = _outletVisitReasonsTypeRepository.Query(query);
            var results = new QueryResult<OutletVisitReasonsTypeViewModel>();
            results.Count = queryResult.Count;
            results.Data = queryResult.Data.Select(Map).ToList();
            return results;
        }

        public Dictionary<int, string> OutletVisitReasonsTypeAction()
        {
            var dict = Enum.GetValues(typeof(OutletVisitAction))
                .Cast<OutletVisitAction>()
                .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }
      

    }
}
