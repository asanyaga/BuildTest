using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class OutletCategoryViewModelBuilder : IOutletCategoryViewModelBuilder
    {
        //cn
        IOutletCategoryRepository _outletCategoryRepository;
        public OutletCategoryViewModelBuilder(IOutletCategoryRepository outletCategoryRepository)
        {
            _outletCategoryRepository = outletCategoryRepository;
        }

        public IList<OutletCategoryViewModel> GetAll(bool inactive = false)
        {
            var outletCategory = _outletCategoryRepository.GetAll(inactive);
            return outletCategory
                .Select(n => new OutletCategoryViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    isActive = n._Status == EntityStatus.Active ? true : false


                }
                ).ToList();
        }

        public OutletCategoryViewModel GetByID(Guid id)
        {
            OutletCategory outletCategory = _outletCategoryRepository.GetById(id);
            if (outletCategory == null) return null;
               
            return Map(outletCategory);
        }

        public void Save(OutletCategoryViewModel outletCategoryViewModel)
        {
            OutletCategory outletCat = new OutletCategory(outletCategoryViewModel.Id)
            {
                Name = outletCategoryViewModel.Name,
                 Code=outletCategoryViewModel.code
            };
            _outletCategoryRepository.Save(outletCat);
        }


        private OutletCategoryViewModel Map(OutletCategory outletCategory)
        {
            return new OutletCategoryViewModel
            {
                Id = outletCategory.Id,
                Name = outletCategory.Name,
                isActive = outletCategory._Status == EntityStatus.Active ? true : false
            };

        }


        public void SetInactive(Guid id)
        {
            OutletCategory outCategory = _outletCategoryRepository.GetById(id);
            _outletCategoryRepository.SetInactive(outCategory);
        }

        public void SetActive(Guid id)
        {
            OutletCategory outCategory = _outletCategoryRepository.GetById(id);
            _outletCategoryRepository.SetActive(outCategory);
        }

        public void SetAsDeleted(Guid id)
        {
            OutletCategory oc = _outletCategoryRepository.GetById(id);
            _outletCategoryRepository.SetAsDeleted(oc);
        }

        public QueryResult<OutletCategoryViewModel> Query(QueryStandard q)
        {
            var queryResult = _outletCategoryRepository.Query(q);

            var result = new QueryResult<OutletCategoryViewModel>();
            result.Count = queryResult.Count;
            result.Data = queryResult.Data.Select(Map).ToList();

            return result;
        }

        public IList<OutletCategoryViewModel> Search(string searchParam, bool inactive = false)
        {
            var outletCategory = _outletCategoryRepository.GetAll(inactive).Where(n=>n.Name.ToLower().StartsWith(searchParam.ToLower()));
            return outletCategory
                .Select(n => new OutletCategoryViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    isActive = n._Status == EntityStatus.Active ? true : false


                }
                ).ToList();
        }
    }
}
