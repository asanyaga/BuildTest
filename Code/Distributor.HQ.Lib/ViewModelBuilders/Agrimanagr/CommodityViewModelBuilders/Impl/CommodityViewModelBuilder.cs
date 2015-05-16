using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders.Impl
{
    public class CommodityViewModelBuilder : ICommodityViewModelBuilder
    {
        ICommodityRepository _commodityRepository;
        ICommodityTypeRepository _commodityTypeRepository;
        private IMasterDataUsage _masterDataUsage;

        public CommodityViewModelBuilder(ICommodityRepository commodityRepository, ICommodityTypeRepository commodityTypeRepository, IMasterDataUsage masterDataUsage)
        {
            _commodityRepository = commodityRepository;
            _commodityTypeRepository = commodityTypeRepository;
            _masterDataUsage = masterDataUsage;
        }

        public IList<CommodityViewModel> Search(string searchParam, bool inactive = false)
        {
            List<Commodity> searchResult = null;
            if (searchParam == "")
            {
                searchResult = _commodityRepository.GetAll(inactive).ToList();
            }
            else
            {
                searchResult = _commodityRepository.GetAll(inactive).Where(s => s.Name.ToLower().StartsWith(searchParam.ToLower())).ToList();
            }
            return searchResult.Select(n => new CommodityViewModel
            {
                CommodityTypeId = n.CommodityType.Id,
                Active = n._Status == EntityStatus.Active ? true : false,
                Id = n.Id,
                Name = n.Name,
                Code = n.Code,
                Description = n.Description,
                CommodityTypeName = n.CommodityType.Name
            }).ToList();
        }

        public IList<CommodityGradeViewModel> Search(Guid commodityId, string searchParam, bool inactive = false)
        {
            List<CommodityGrade> grades = null;

            if (searchParam == "")
            {
                grades = _commodityRepository.GetAllGradeByCommodityId(commodityId, inactive).ToList();
            }
            else
            {
                grades = _commodityRepository.GetAllGradeByCommodityId(commodityId, inactive).Where(s => s.Name.ToLower().StartsWith(searchParam.ToLower())).ToList();
            }
            return grades.Select(s => new CommodityGradeViewModel
            {
                Active = s._Status == EntityStatus.Active ? true : false,
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                UsageTypeId = s.UsageTypeId,
                CommodityId = commodityId

            }).ToList();

        }

        public CommodityViewModel Get(Guid id)
        {
            return Map(_commodityRepository.GetById(id));
        }

        public void Save(CommodityViewModel commodityViewModel)
        {
            Commodity Commodity = new Commodity(commodityViewModel.Id);
            Commodity.Name = commodityViewModel.Name;
            Commodity.Code = commodityViewModel.Code;
            Commodity.Description = commodityViewModel.Description;
            Commodity.CommodityType = _commodityTypeRepository.GetById(commodityViewModel.CommodityTypeId);
            _commodityRepository.Save(Commodity);

        }

        public void SetInactive(Guid id)
        {
            Commodity pp = _commodityRepository.GetById(id);
            if (_masterDataUsage.CheckCommodityIsUsed(pp, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity. Commodity is assigned grades and/or is used to in transactions. Remove dependencies first to continue");
            }
            _commodityRepository.SetInactive(pp);
        }

        public void SetAsDeleted(Guid id)
        {
            Commodity pp = _commodityRepository.GetById(id);
            if (_masterDataUsage.CheckCommodityIsUsed(pp, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete commodity. Commodity is assigned grades and/or is used to in transactions. Remove dependencies first to continue");
            }
            _commodityRepository.SetAsDeleted(pp);
        }

        public void SetActive(Guid id)
        {
            Commodity pp = _commodityRepository.GetById(id);
            _commodityRepository.SetActive(pp);
        }

        public Dictionary<Guid, string> CommodityTypeList()
        {
            return _commodityTypeRepository.GetAll()
               .Select(s => new { s.Id, s.Name })

               .OrderBy(s => s.Name)
               .ToDictionary(d => d.Id, d => d.Name);
        }

        private CommodityViewModel Map(Commodity commodity)
        {

            CommodityViewModel viewModel = new CommodityViewModel();
            {

                viewModel.Id = commodity.Id;
                if (commodity.CommodityType.Id != null)
                {
                    viewModel.CommodityTypeId = commodity.CommodityType.Id;

                }
                if (_commodityTypeRepository.GetById(commodity.CommodityType.Id).Name != null)
                {
                    viewModel.CommodityTypeName = _commodityTypeRepository.GetById(commodity.CommodityType.Id).Name;
                }
                viewModel.CommodityTypeId = commodity.CommodityType.Id;
                viewModel.Name = commodity.Name;
                viewModel.Code = commodity.Code;
                viewModel.Description = commodity.Description;

                viewModel.Active = commodity._Status == EntityStatus.Active ? true : false;
            };
            return viewModel;
        }

        public void AddCommodityGrades(Guid commodityId, Guid gradeId, string commodityGradeName, int usageTypeId, string commodityGradeCode, string commodityGradeDescription)
        {
            _commodityRepository.AddCommodityGrade(commodityId, gradeId, commodityGradeName, usageTypeId, commodityGradeCode, commodityGradeDescription);

        }

        public void SetGradeInactive(Guid commodityId, Guid gradeId)
        {
            if (_masterDataUsage.CheckCommodityGradeIsUsed(gradeId, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity grade. Grade is used in transactions. Remove dependencies first to continue");
            }
            _commodityRepository.SetGradeInactive(commodityId, gradeId);

        }

        public void SetGradeActive(Guid commodityId, Guid gradeId)
        {
            _commodityRepository.SetGradeActive(commodityId, gradeId);

        }

        public void SetGradeAsDeleted(Guid commodityId, Guid gradeId)
        {
            if (_masterDataUsage.CheckCommodityGradeIsUsed(gradeId, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete commodity grade. Grade is used in transactions. Remove dependencies first to continue");
            }
            _commodityRepository.SetGradeAsDeleted(commodityId, gradeId);

        }

        public QueryResult<CommodityViewModel> Query(QueryStandard query)
        {
            var queryResult = _commodityRepository.Query(query);

            var result = new QueryResult<CommodityViewModel>();
            result.Data = queryResult.Data.OfType<Commodity>().Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<CommodityViewModel> QueryList(List<Commodity> list)
        {
            return list.Select(Map).ToList();
        }

        public QueryResult QueryGrade(QueryGrade query)
        {
            throw new NotImplementedException();
        }

        public IList<CommodityGradeViewModel> QueryGradeList(List<CommodityGrade> list)
        {
            throw new NotImplementedException();
        }

        //public QueryResult QueryGrade(QueryGrade query)
        //{
        //    return _commodityRepository
        //}

        //public IList<CommodityGradeViewModel> QueryGradeList(List<CommodityGrade> list)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
