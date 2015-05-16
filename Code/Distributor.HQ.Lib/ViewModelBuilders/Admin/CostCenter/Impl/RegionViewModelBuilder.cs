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
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class RegionViewModelBuilder : IRegionViewModelBuilder
    {
       IRegionRepository _regionRepository;
       ICountryRepository _countryRepository;
       IDistrictRepository _districtRepository;
       IProvincesRepository _provinceRepository;
       private IMasterDataUsage _masterDataUsage;
       public RegionViewModelBuilder(IRegionRepository regionRepository, ICountryRepository countryRepository,IDistrictRepository districtRepository,
       IProvincesRepository provinceRepository, IMasterDataUsage masterDataUsage)
       {
           _regionRepository = regionRepository;
           _countryRepository = countryRepository;
           _provinceRepository = provinceRepository;
           _districtRepository = districtRepository;
           _masterDataUsage = masterDataUsage;
       }

        public IList<RegionViewModel> GetAll(bool inactive = false)
        {
            var region=_regionRepository.GetAll(inactive);
            return region
                .Select(n => Map(n)).ToList();
        }

        public RegionViewModel Get(Guid id)
        {
            Region reg = _regionRepository.GetById(id);
            if (reg == null) return null;
             
            return Map(reg);
        }
        RegionViewModel Map(Region region)
        {
            return new RegionViewModel 
            {
              Name=region.Name,
               Description=region.Description,
              isActive = region._Status == EntityStatus.Active ? true : false,
                 CountryId=region.Country.Id,
                 CountryName=region.Country.Name,
                  Id=region.Id
                   //ProvinceId=region.provinceId.Id,
                   // DistrictId=region.districtId.Id
            };
        }
        public void Save(RegionViewModel region)
        {
            Region reg = new Region(region.Id)
            {
                Name = region.Name,
                 Description=region.Description,
                  Country=_countryRepository.GetById(  region.CountryId),
                   //districtId=_districtRepository.GetById(region.DistrictId),
                   // provinceId=_provinceRepository.GetById(region.ProvinceId)
                                   
            };
            _regionRepository.Save(reg);
        }

        public void SetInActive(Guid Id)
        {
            Region reg = _regionRepository.GetById(Id);
            if (_masterDataUsage.CheckAgriRegionIsUsed(reg, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate region. Region is assigned to hub(s) and/or distributr(s) and/or route. Remove dependencies first to continue");
            }
            _regionRepository.SetInactive(reg);
        }

       public void SetActive(Guid id)
       {
           Region reg = _regionRepository.GetById(id);
           _regionRepository.SetActive(reg);
       }


       public Dictionary<Guid, string> Country()
        {
            return _countryRepository.GetAll().OrderBy(n=>n.Name)
                .Select(c => new { c.Id, c.Name }).ToList().ToDictionary(d=>d.Id,d=>d.Name);
        }


        public IList<RegionViewModel> Search(string searchParam, bool inactive = false)
        {//||(n.Id.Equals(Convert.ToInt32(searchParam))) 
            var items = _regionRepository.GetAll(inactive).ToList().Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower())) || (n.Description.ToLower().StartsWith(searchParam.ToLower())) || (n.Id.ToString().StartsWith(searchParam)));
            return items.Select(n => Map(n)).ToList();
        }


       public Dictionary<Guid, string> Province()
        {
            return _provinceRepository.GetAll().OrderBy(n=>n.Name)
                .Select(c => new { c.Id, c.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> District()
        {
            return _districtRepository.GetAll().OrderBy(n=>n.DistrictName)
                 .Select(c => new { c.Id, c.DistrictName }).ToList().ToDictionary(d => d.Id, d => d.DistrictName);
        }


        public void SetAsDeleted(Guid id)
        {
            Region reg = _regionRepository.GetById(id);
            if (_masterDataUsage.CheckAgriRegionIsUsed(reg, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete region. Region is assigned to hub(s) and/or distributr(s) and/or route. Remove dependencies first to continue");
            }
            _regionRepository.SetAsDeleted(reg);
        }

       public QueryResult<RegionViewModel> Query(QueryStandard query)
       {
           var queryResults = _regionRepository.Query(query);
           var results = new QueryResult<RegionViewModel>();
           results.Data = queryResults.Data.Select(Map).ToList();
           results.Count = queryResults.Count;
           return results;
       }

       public IList<RegionViewModel> QueryList(QueryResult result)
       {
           return result.Data.OfType<Region>().ToList().Select(Map).ToList();
       }
    }
}
