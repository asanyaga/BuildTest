using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
   public class ProvinceViewModelBuilder:IProvinceViewModelBuilder 
    {
       IProvincesRepository _provincesRepository;
       ICountryRepository _countryRepository;
       ICacheProvider _cacheProvider;
       public ProvinceViewModelBuilder(IProvincesRepository provincesRepository, ICountryRepository countryRepository, ICacheProvider cacheProvider)
       {
           _provincesRepository = provincesRepository;
           _countryRepository = countryRepository;
           _cacheProvider = cacheProvider;
       }
        public IList<ProvinceViewModel> GetAll(bool inactive = false)
        {
            //string cacheKey = string.Format(_cacheGet,"all");
            //var provinces = _cacheProvider.Get(_cacheRegion,cacheKey)as List<ProvinceViewModel>;
            //if(provinces==null)
            //{
            //var provincess = _provincesRepository.GetAll(inactive).Select(n => Map(n)).ToList();
            //    _cacheProvider.Set(_cacheRegion,cacheKey,provincess,60);
            //   provinces=_cacheProvider.Get(_cacheRegion,cacheKey)as List<ProvinceViewModel>;
            //}
            //if (!inactive)
            //    provinces = provinces.Where(n => n.IsActive).ToList();
            //return provinces;
            return _provincesRepository.GetAll(inactive).Select(n => Map(n)).ToList();
                
        }

        public ProvinceViewModel Get(Guid Id)
        {
            Province province = _provincesRepository.GetById(Id);
            if (province == null) return null;
               
            return Map(province);
        }

        public void Save(ProvinceViewModel provinceViewModel)
        {
            Province province = new Province(provinceViewModel.Id)
            {
                Name = provinceViewModel.Name,
                Description = provinceViewModel.Description,
               Country = _countryRepository.GetById(provinceViewModel.CountryId )
            };
            //province._SetStatus(EntityStatus.Active);
            _provincesRepository.Save(province);
        }

        public void SetInactive(Guid Id)
        {
           Province province = _provincesRepository.GetById(Id);
           _provincesRepository.SetInactive(province);
        }

       public void SetActive(Guid id)
       {
           Province province = _provincesRepository.GetById(id);
           _provincesRepository.SetActive(province);
       }

       ProvinceViewModel Map(Province province)
        {
            ProvinceViewModel provinceViewModel = new ProvinceViewModel();
            provinceViewModel. Id = province.Id;
              provinceViewModel.  Name = province.Name;
               provinceViewModel. Description = province.Description;
               if (province.Country != null)
               {
                   provinceViewModel.CountryId = _countryRepository.GetById(province.Country.Id).Id;
               }
               if (province.Country != null)
               {
                   provinceViewModel.CountryName = _countryRepository.GetById(province.Country.Id).Name;
               }
               provinceViewModel.IsActive = province._Status == EntityStatus.Active ? true : false;
               return provinceViewModel;
            //return new ProvinceViewModel
            //{
            //    Id = province.Id,
            //    Name = province.Name,
            //    Description = province.Description,
            //    CountryId=_countryRepository.GetById(province.countryId.Id ).Id ,
            //    CountryName=_countryRepository.GetById(province.countryId.Id ).Name,
            //    IsActive = province._Status,
            //};
        }
        public Dictionary<Guid, string> Country()
        {
            return _countryRepository.GetAll().OrderBy(n=>n.Name)
                .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }


        public List<ProvinceViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _provincesRepository.GetAll().Where(n => (n.Name.ToLower().StartsWith(srchParam)) || (n.Name.ToUpper().StartsWith( srchParam)));
            return items.Select(n => Map(n)).ToList();
        }


        public IList<ProvinceViewModel> GetByCountry(Guid countryId, bool inactive = false)
        {
            var provinces = _provincesRepository.GetAll(inactive).Where(n=>n.Country.Id==countryId);
            return provinces
                .Select(n => new ProvinceViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Description = n.Description,
                    CountryId = n.Country.Id,
                    IsActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }
        protected  string _cacheRegion
        {
            get { return "Province"; }
        }

        protected  string _cacheGet
        {
            get { return "Province_{0}"; }
        }



        public void SetAsDeleted(Guid id)
        {
            Province province = _provincesRepository.GetById(id);
            _provincesRepository.SetAsDeleted(province);
        }

       public QueryResult<ProvinceViewModel> Query(QueryStandard q)
       {
           var queryResult = _provincesRepository.Query(q);
           var result = new QueryResult<ProvinceViewModel>();
           result.Count = queryResult.Count;
           result.Data = queryResult.Data.Select(Map).ToList();
           return result;
       }
    }
}
