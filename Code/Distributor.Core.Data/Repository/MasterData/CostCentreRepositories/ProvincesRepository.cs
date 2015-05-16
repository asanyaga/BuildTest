using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class ProvincesRepository : RepositoryMasterBase<Province>,IProvincesRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        ICountryRepository _countryRepository;
        public ProvincesRepository(CokeDataContext ctx,ICacheProvider cacheProvider,ICountryRepository countryRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _countryRepository = countryRepository;
        }
        public Guid Save(Province entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating Provinces");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid Provinces");
                throw new DomainValidationException(vri, "Failed to save invalid Provinces");
            }
            tblProvince provinces = _ctx.tblProvince.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (provinces == null)
            {
                provinces = new tblProvince
                                {
                                    IM_DateCreated = date,
                                    IM_Status =(int)EntityStatus.Active,// true,
                    Id= entity.Id,
                };
                _ctx.tblProvince.AddObject(provinces);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (provinces.IM_Status != (int)entityStatus)
                provinces.IM_Status = (int)entity._Status;
            provinces.Name = entity.Name;
            provinces.Description = entity.Description;
            provinces.CountryId  = entity.Country.Id;

            provinces.IM_DateLastUpdated = date;

            _log.Debug("Saving Provinces");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblProvince.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, provinces.Id));
            _log.DebugFormat("Successfully saved item id:{0}", provinces.Id);
            return provinces.Id;
        }

        public void SetInactive(Province entity)
        {
            _log.Debug("Inactivating Province");
        
            ValidationResultInfo vri = Validate(entity);
            bool hasProvinceDependencies = _ctx.tblDistrict.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.ProvinceId == entity.Id);

            if (hasProvinceDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblProvince Province = _ctx.tblProvince.FirstOrDefault(n => n.Id == entity.Id);
                if (Province != null )
                {

                    Province.IM_Status = (int)EntityStatus.Inactive;// false;
                Province.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProvince.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, Province.Id));
                
                }
            }
            
        }

        public void SetActive(Province entity)
        {
            tblProvince Province = _ctx.tblProvince.FirstOrDefault(n => n.Id == entity.Id);
            if (Province != null)
            {

                Province.IM_Status = (int)EntityStatus.Active;// false;
                Province.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblProvince.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, Province.Id));

            }
        }

        public void SetAsDeleted(Province entity)
        {
            _log.Debug("Deleting Province");

            ValidationResultInfo vri = Validate(entity);
            bool hasProvinceDependencies = _ctx.tblDistrict.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.ProvinceId == entity.Id);

            if (hasProvinceDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {
                tblProvince Province = _ctx.tblProvince.FirstOrDefault(n => n.Id == entity.Id);
                if (Province != null)
                {

                    Province.IM_Status = (int)EntityStatus.Deleted;
                    Province.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblProvince.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, Province.Id));

                }
            }
        }

        public Province GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting Province by ID: {0}", Id);

            Province entity = (Province)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblProvince.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(Province itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty){
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));}
            bool hasDuplicate = GetAll(true)
                .Where(n => n.Id != itemToValidate.Id)
                .Any(n => n.Name.ToLower() == itemToValidate.Name.ToLower() && n.Country.Name == itemToValidate.Country.Name);
            if (hasDuplicate)
            {
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            }
            return vri;
        }


        protected override string _cacheKey
        {
            get { return "Province-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProvinceList"; }
        }

        public override IEnumerable<Province> GetAll(bool includeDeactivated = false)
        {
            IList<Province> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Province>(ids.Count);
                foreach (Guid id in ids)
                {
                    Province entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblProvince.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Province p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<Province> Query(QueryStandard query)
        {
            IQueryable<tblProvince> provincequery;
            if (query.ShowInactive)
                provincequery = _ctx.tblProvince.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                provincequery = _ctx.tblProvince.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Province>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                provincequery = provincequery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                         || p.tblCountry.Name.ToLower().Contains(query.Name.ToLower()));

            }

            provincequery = provincequery.OrderBy(p => p.Name).ThenBy(p => p.tblCountry.Name);
            queryResult.Count = provincequery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                provincequery = provincequery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = provincequery.Select(Map).OfType<Province>().ToList();

            return queryResult;
        }


        public Province Map(tblProvince province)
        {
            Province retProvinces = new Province(province.Id)
            {
               
                Name = province.Name,
                Description = province.Description,
                Country  = _countryRepository.GetById(province.CountryId )  
            };
            retProvinces._SetDateCreated(province.IM_DateCreated);
            retProvinces._SetDateLastUpdated(province.IM_DateLastUpdated);
            retProvinces._SetStatus((EntityStatus)province.IM_Status);

            return retProvinces;
        }
    }
}
