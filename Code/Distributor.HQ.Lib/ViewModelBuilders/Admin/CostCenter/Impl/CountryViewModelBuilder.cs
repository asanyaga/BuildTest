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
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter.Impl
{
    public class CountryViewModelBuilder : ICountryViewModelBuilder
    {
        ICountryRepository _countryRepository;
        ICacheProvider _cacheProvider;
        private IMasterDataUsage _masterDataUsage;

        public CountryViewModelBuilder(ICountryRepository countrRepository, ICacheProvider cacheProvider, IMasterDataUsage masterDataUsage)
        {
            _countryRepository = countrRepository;
            _cacheProvider = cacheProvider;
            _masterDataUsage = masterDataUsage;
        }
        public IList<CountryViewModel> GetAll(bool inactive = false)
        {
            return _countryRepository.GetAll(inactive).Select(n => Map(n)).ToList();

        }

        public CountryViewModel Get(Guid id)
        {
            Country coutr = _countryRepository.GetById(id);
            if (coutr == null) return null;

            return Map(coutr);
        }
        CountryViewModel Map(Country country)
        {
            return new CountryViewModel
            {
                id = country.Id,
                Name = country.Name,
                Currency = country.Currency,
                Code = country.Code,
                isActive = country._Status == EntityStatus.Active ? true : false
            };
        }

        public void Save(CountryViewModel country)
        {
            Country coutr = new Country(country.id)
            {
                Name = country.Name,
                Code = country.Code,
                Currency = country.Currency
            };
            _countryRepository.Save(coutr);

        }

        public void SetInActive(Guid id)
        {
            Country country = _countryRepository.GetById(id);
            if (_masterDataUsage.CheckCountryIsUsed(country, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate country. Country is assigned regions. Remove dependencies first to continue");
            }
            _countryRepository.SetInactive(country);
        }

        public void SetActive(Guid id)
        {
            Country ctr = _countryRepository.GetById(id);
            _countryRepository.SetActive(ctr);
        }

        public void SetAsDeleted(Guid id)
        {
            Country country = _countryRepository.GetById(id);
            if (_masterDataUsage.CheckCountryIsUsed(country, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete country. Country is assigned regions. Remove dependencies first to continue");
            }
            _countryRepository.SetAsDeleted(country);
        }

        public QueryResult<CountryViewModel> Query(QueryStandard query)
        {
            var queryResult = _countryRepository.Query(query);
            var results = new QueryResult<CountryViewModel>();
            results.Count = queryResult.Count;
            results.Data = queryResult.Data.Select(Map).ToList();

            return results;
        }

        


        public IList<CountryViewModel> Search(string searchParam, bool inactive = false)
        {
            var items = _countryRepository.GetAll(inactive)
                .Where(n => (n.Name.ToLower().StartsWith(searchParam.ToLower()))
                            || (n.Code.ToLower().StartsWith(searchParam.ToLower())))
                .ToList();
            return items.Select(n => new CountryViewModel
            {
                id = n.Id,
                isActive = n._Status == EntityStatus.Active ? true : false,
                Name = n.Name,
                Currency = n.Currency,
                Code = n.Code
            }).ToList();
        }

   

        protected string _cacheRegion
        {
            get { return "country"; }
        }

        protected string _cacheGet
        {
            get { return "country_{0}"; }
        }

       public IList<CountryViewModel> Querylist(QueryResult result)
        {
            return result.Data.OfType<Country>().ToList().Select(Map).ToList();
        }
        
    }
}
