using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class OutletTypeViewModelBuilder : IOutletTypeViewModelBuilder
    {
        IOutletTypeRepository _outletTypeRepository;
        public OutletTypeViewModelBuilder(IOutletTypeRepository outletTypeRepository)
        {
            _outletTypeRepository = outletTypeRepository;
        }
        public IList<OutletTypeViewModel> GetAll(bool inactive = false)
        {
            var outletType = _outletTypeRepository.GetAll(inactive);
            return outletType
                .Select(n => new OutletTypeViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    isActive = n._Status == EntityStatus.Active ? true : false
                    

                }
                ).ToList();
        }

        public OutletTypeViewModel GetByID(Guid id)
        {
            OutletType outletType = _outletTypeRepository.GetById(id);
            if (outletType == null) return null;
                
            return Map(outletType);
        }

        public void Save(OutletTypeViewModel outletTypeViewModel)
        {
            
            OutletType outletType = new OutletType(outletTypeViewModel.Id)
            {
                Name = outletTypeViewModel.Name,
                Code=outletTypeViewModel.code
            };
            _outletTypeRepository.Save(outletType);
        }



        public void SetInActive(Guid id)
        {
            OutletType oc = _outletTypeRepository.GetById(id);
            _outletTypeRepository.SetInactive(oc);
        }

        private OutletTypeViewModel Map(OutletType outletType)
        {
            return new OutletTypeViewModel
            {
                Id   = outletType.Id,
                Name = outletType.Name,
                isActive = outletType._Status == EntityStatus.Active ? true : false
                
            };

        }
        public void SetActive (Guid id)
        {
            OutletType ot = _outletTypeRepository.GetById(id);
            _outletTypeRepository.SetActive(ot);
        }

        public void SetAsDeleted(Guid id)
        {
            OutletType ot = _outletTypeRepository.GetById(id);
            _outletTypeRepository.SetAsDeleted(ot);
        }

        public QueryResult<OutletTypeViewModel> Query(QueryStandard q)
        {
            var queryResult = _outletTypeRepository.QueryResult(q);

            var result = new QueryResult<OutletTypeViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }


        public void SetInactive(Guid id)
        {
            OutletType outType = _outletTypeRepository.GetById(id);
            _outletTypeRepository.SetInactive(outType);
        }


        public IList<OutletTypeViewModel> Search(string searchParam, bool inactive = false)
        {
            var outletType = _outletTypeRepository.GetAll(inactive).Where(n=>(n.Name.ToLower().StartsWith(searchParam.ToLower())));
            return outletType
                .Select(n => new OutletTypeViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    isActive = n._Status == EntityStatus.Active ? true : false


                }
                ).ToList(); 
        }
    }
}
