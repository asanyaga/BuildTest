using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class DistrictRepository : RepositoryMasterBase<District>, IDistrictRepository
    {
        CokeDataContext _ctx;
        ICountryRepository _countryRepository;
        IProvincesRepository _proviceRepository;
      //  IDistrictRepository _districtRepository;
        ICacheProvider _cacheProvider;
        public DistrictRepository(CokeDataContext ctx,
            ICountryRepository countryRepository,
            IProvincesRepository provinceRepository,
            
            ICacheProvider cacheProvider
            )
        {
            _ctx = ctx;
            _countryRepository = countryRepository;
            _proviceRepository = provinceRepository;
            
            _cacheProvider = cacheProvider;
        }
        public Guid Save(District entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating District");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Failed to validate District");
            }
            DateTime dt = DateTime.Now;
            tblDistrict tblDist = _ctx.tblDistrict.FirstOrDefault(n => n.Id == entity.Id);
            if( tblDist == null)
            {
                tblDist = new tblDistrict();
                tblDist.IM_Status = (int)EntityStatus.Active;// true;
                tblDist.IM_DateCreated = dt;
                tblDist.Id = entity.Id;
                _ctx.tblDistrict.AddObject(tblDist);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblDist.IM_Status != (int)entityStatus)
                tblDist.IM_Status = (int)entity._Status;
            tblDist.IM_DateLastUpdated = dt;
            tblDist.District = entity.DistrictName;
            tblDist.ProvinceId = _proviceRepository.GetById(entity.Province.Id).Id;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblDistrict.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblDist.Id));
            return tblDist.Id;

        }

        public void SetInactive(District entity)
        {
            _log.DebugFormat("Deactivating District");
            tblDistrict tblDist = _ctx.tblDistrict.FirstOrDefault(n=>n.Id==entity.Id);
            if (tblDist != null)
            {
                tblDist.IM_Status = (int)EntityStatus.Inactive;// false;
                tblDist.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblDistrict.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblDist.Id));
            }
        }

        public void SetActive(District entity)
        {
            tblDistrict dist = _ctx.tblDistrict.FirstOrDefault(n => n.Id == entity.Id);
            if (dist !=null)
            {
                dist.IM_Status = (int) EntityStatus.Active;
                dist.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,_ctx.tblDistrict.Where(n => n.IM_Status !=(int)EntityStatus.Deleted).Select(s=>s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, dist.Id));
            }
        }

        public void SetAsDeleted(District entity)
        {
            _log.DebugFormat("Deleting District");
            tblDistrict tblDist = _ctx.tblDistrict.FirstOrDefault(n => n.Id == entity.Id);
            if (tblDist != null)
            {
                tblDist.IM_Status = (int)EntityStatus.Deleted;
                tblDist.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblDistrict.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblDist.Id));
            }
        }

        public District GetById(Guid Id, bool includeDeactivated = false)
        {
            District entity = (District)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblDistrict.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(District itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.DistrictName.ToLower() == itemToValidate.DistrictName.ToLower());
            //////bool hasDuplicateName = _ctx.tblDistrict
            //////    .Where(d => d.Id != itemToValidate.Id && d.IM_Status != (int)EntityStatus.Deleted)
            //////    .Any(p => p.District.ToLower() == itemToValidate.DistrictName.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;
        }


        protected override string _cacheKey
        {
            get { return "District-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "DistrictList"; }
        }

        public override IEnumerable<District> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Gettting All");
            IList<District> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<District>(ids.Count);
                foreach (Guid id in ids)
                {
                    District entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblDistrict.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (District p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<District> Query(QueryStandard query)
        {
            IQueryable<tblDistrict> districtQuery;
            if (query.ShowInactive)
                districtQuery = _ctx.tblDistrict.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                districtQuery = _ctx.tblDistrict.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<District>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                districtQuery = districtQuery.Where(p => p.District.ToLower().Contains(query.Name.ToLower()));

            }

            districtQuery = districtQuery.OrderBy(p => p.District);
            queryResult.Count = districtQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                districtQuery = districtQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = districtQuery.Select(Map).ToList();

            return queryResult;
        }

        protected District Map(tblDistrict tblDist)
        {
            District dist = new District(tblDist.Id)
            {
                 DistrictName=tblDist.District,
                 
                 //Country=_countryRepository.GetById(tblDist.CountryId),
                 Province=_proviceRepository.GetById(tblDist.ProvinceId)
            };
            dist._SetDateCreated(tblDist.IM_DateCreated);
            dist._SetDateLastUpdated(tblDist.IM_DateLastUpdated);
            dist._SetStatus((EntityStatus)tblDist.IM_Status);
            return dist;
        }
    }
}
