using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Import.Entities;
using log4net;

namespace Distributr.Integrations.Imports.Impl
{
    public class CountryImporterService:BaseImporterService,ICountryImporterService
    {

        private ICountryRepository _countryRepository;
        private readonly CokeDataContext _context;

        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CountryImporterService(ICountryRepository countryRepository, CokeDataContext context)
        {
            _countryRepository = countryRepository;
            _context = context;
        }


        public ImportResponse Save(List<CountryImport> imports)
        {
            List<Country> countries = imports.Select(Map).ToList();

            List<ValidationResultInfo> validationResults = countries.Select(Validate).ToList();

            if(validationResults.Any(p=>!p.IsValid))
            {
                var invalidEntities=validationResults.Where(p => !p.IsValid).ToList();
                foreach (var invalidEntity in invalidEntities)
                {
                    var result=invalidEntity.Results;
                }
                return new ImportResponse() { Status = false, Info ="Countries Error "+ValidationResultsInfo(invalidEntities) };
               
            }
            List<Country> changedCountries = HasChanged(countries);

            foreach (var changedCountry in changedCountries)
            {
                _countryRepository.Save(changedCountry);
            }
            return new ImportResponse() {Status = true, Info = changedCountries.Count+" Countries Successfully Imported"};
        }

        public ImportResponse Delete(List<string> deletedCodes)
        {

            foreach (var deletedCode in deletedCodes)
            {
                try
                {
                    var countryId = _context.tblCountry.Where(p => p.Code == deletedCode).Select(p => p.id).FirstOrDefault();

                    var country = _countryRepository.GetById(countryId);
                    if (country != null)
                    {
                        _countryRepository.SetAsDeleted(country);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Country Delete Error" + ex.ToString());
                }

            }
            return new ImportResponse() { Info = "Country Deleted Succesfully", Status = true };
        }


        private List<Country> HasChanged(List<Country> countries)
        {
            var changedCountries = new List<Country>();
            foreach (var country in countries)
            {
                var entity = _countryRepository.GetById(country.Id);
                if(entity==null)
                {
                    changedCountries.Add(country);
                    continue;
                }
                bool hasChanged = entity.Name.ToLower() != country.Name.ToLower() || entity.Code.ToLower() != country.Code.ToLower();
                
                if(hasChanged)
                {
                    changedCountries.Add(country);
                }
            }
            return changedCountries;
        }

        protected ValidationResultInfo Validate(Country country)
        {
            return _countryRepository.Validate(country);
        }

        protected Country Map(CountryImport countryImport)
        {
            var exists = Queryable.FirstOrDefault(_context.tblCountry, p => p.Code == countryImport.Code);
            Guid id = exists != null ? exists.id : Guid.NewGuid();
            
            var country = new Country(id);
            country.Name = countryImport.Name;
            country.Code = countryImport.Code;
            country.Currency = "";

            return country;

        }
    }
}
