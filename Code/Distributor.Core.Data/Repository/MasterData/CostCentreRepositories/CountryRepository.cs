using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using log4net;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class CountryRepository : RepositoryMasterBase<Country>, ICountryRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public CountryRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("Country Repository Constractor");
        }

        public Guid Save(Country entity, bool? isSync = null)
        {
            _log.Debug("Saving Country");
            //validation
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Saving invalid Country Failed");
                throw new DomainValidationException(vri, "Failed to save, invalid Country");
            }
            tblCountry cntry = _ctx.tblCountry.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (cntry == null)
            {
                cntry = new tblCountry();
                cntry.IM_Status = (int)EntityStatus.Active;// true;
                cntry.IM_DateCreated = DateTime.Now;
                cntry.IM_DateCreated = dt;
                cntry.id = entity.Id;

                _ctx.tblCountry.AddObject(cntry);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (cntry.IM_Status != (int)entityStatus)
                cntry.IM_Status = (int)entity._Status;
            cntry.Name = entity.Name;
            cntry.Code = entity.Code;
            cntry.Currency = entity.Currency;
            //cntry.IM_Status = country._Status;
            cntry.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCountry.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, cntry.id));

            return cntry.id;
        }

        public void SetInactive(Country country)
        {
            _log.Debug("Set Country inactive; Deactivate country");
            ValidationResultInfo vri = Validate(country);
            bool hasCountryDependencies = _ctx.tblProvince.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CountryId == country.Id);

            if (hasCountryDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblCountry cnty = _ctx.tblCountry.FirstOrDefault(p => p.id == country.Id);
                if (cnty != null)
                {
                    cnty.IM_Status = (int)EntityStatus.Inactive;//false;
                    cnty.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCountry.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, cnty.id));
                }
            }
        }

        public void SetActive(Country entity)
        {
            tblCountry country = _ctx.tblCountry.FirstOrDefault(p => p.id == entity.Id);
            if (country != null)
            {
                country.IM_Status = (int)EntityStatus.Active;
                country.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCountry.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, country.id));
            }
        }

        public void SetAsDeleted(Country country)
        {
            _log.Debug("Set Country as deleted; Delete country");
            ValidationResultInfo vri = Validate(country);
            bool hasCountryDependencies = _ctx.tblProvince.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CountryId == country.Id);

            if (hasCountryDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {
                tblCountry tblCountry = _ctx.tblCountry.FirstOrDefault(p => p.id == country.Id);
                if (tblCountry != null)
                {
                    tblCountry.IM_Status = (int)EntityStatus.Deleted;//false;
                    tblCountry.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCountry.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, tblCountry.id));
                }
            }
        }

      

        public Country GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.Debug("Get Country By ID");
            Country entity = (Country)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCountry.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;

        }

        protected override string _cacheKey
        {
            get { return "Country-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CountryList"; }
        }

        public override IEnumerable<Country> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all Countries");
            IList<Country> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Country>(ids.Count);
                foreach (Guid id in ids)
                {
                    Country entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCountry.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Country p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       
        public ValidationResultInfo Validate(Country itemToValidate)
        {

            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool invalidName = string.IsNullOrWhiteSpace(itemToValidate.Name);
            if (invalidName)
            {
                vri.Results.Add(new ValidationResult("Name cannot be empty"));
                return vri;
            }

            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            bool hasDuplicateCode = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => (p.Code != null ? p.Code.ToLower() : "") == itemToValidate.Code.ToLower());
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code found"));
            return vri;
        }


        public QueryResult<Country> Query(QueryStandard query)
        {
            IQueryable<tblCountry> countryquery;
            if (query.ShowInactive)
                countryquery = _ctx.tblCountry.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                countryquery = _ctx.tblCountry.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Country>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                countryquery = countryquery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                       || p.Code.ToLower().Contains(query.Name.ToLower())
                                                       ||p.Currency.ToLower().Contains(query.Name.ToLower()));
            }

            countryquery = countryquery.OrderBy(p => p.Name).ThenBy(p => p.Code);
            queryResult.Count = countryquery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                countryquery = countryquery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = countryquery.Select(Map).OfType<Country>().ToList();

            return queryResult
                ;
        }

        

        public Country Map(tblCountry country)
        {
            Country cntry = new Country(country.id)
            {
                Name = country.Name,
                Code = country.Code,
                Currency = country.Currency
            };
            cntry._SetStatus((EntityStatus)country.IM_Status);
            cntry._SetDateCreated(country.IM_DateCreated);
            cntry._SetDateLastUpdated(country.IM_DateLastUpdated);
            return cntry;

        }


    }
}
