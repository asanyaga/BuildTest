using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Utility;
using log4net;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class RegionRepository : RepositoryMasterBase<Region>, IRegionRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        ICountryRepository _countryRepository;

        public RegionRepository(CokeDataContext ctx, ICacheProvider cacheProvider,ICountryRepository countryRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _countryRepository = countryRepository;
            _log.Debug("Region Repository Constructor Bootstrap");
        }

        //protected static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Guid Save(Region entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating region");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid region");
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.region.validation.error"));
            }

            tblRegion reg = _ctx.tblRegion.FirstOrDefault(n => n.id == entity.Id);
            DateTime date = DateTime.Now;
            if (reg==null)
            {
                reg = new tblRegion {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    id= entity.Id
                };
                _ctx.tblRegion.AddObject(reg);
            }
            else
            {
              
              
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (reg.IM_Status != (int)entityStatus)
                reg.IM_Status = (int)entity._Status;
            reg.Name = entity.Name;
            reg.Description = entity.Description;
            reg.Country = entity.Country.Id;
           

            reg.IM_DateLastUpdated = date;

            _log.Debug("Saving Region");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblRegion.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, reg.id));
            _log.DebugFormat("Successfully saved item id:{0}",reg.id );
            return reg.id;
        }

        public void SetAsDeleted(Region entity)
        {
            var vri = Validate(entity);
            bool hasDependency = _ctx.tblCostCentre
                .Where(n => n.CostCentreType == (int) CostCentreType.Distributor
                            && n.IM_Status != (int) EntityStatus.Deleted
                            && n.Distributor_RegionId != null)
                .Any(n => n.Distributor_RegionId == entity.Id);

            if (hasDependency)
                throw new DomainValidationException(vri, "Cannot delete - has a distributor assigned to the region.");
            _log.Debug("Deleting region");
            tblRegion reg = _ctx.tblRegion.First(n => n.id == entity.Id);
            if(reg !=null)
            {
            reg.IM_Status = (int)EntityStatus.Deleted;
            reg.IM_DateLastUpdated = DateTime.Now;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblRegion.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, reg.id));
            }
        }

        public Region GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting Region by ID: {0}",Id);
            Region entity = (Region)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblRegion.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }
            }
            return entity; 
        }

        /*public IEnumerable<Region> GetAll(bool includeDeactivated=false)
        {
            _log.DebugFormat("Getting All Regions; include Deactivated: {0}",includeDeactivated);

            string cacheKey = string.Format(_cacheGet, "all");
            IEnumerable<Region> regions = _cacheProvider.Get(_cacheRegion, cacheKey) as IEnumerable<Region>;
            if (regions == null)
            {
                IEnumerable<Region> qry = _ctx.tblRegion.ToList().Select(n => n.Map());
                //check null
                _cacheProvider.Set(_cacheRegion, cacheKey, qry, 60);
                regions = _cacheProvider.Get(_cacheRegion, cacheKey) as IEnumerable<Region>; ;
            }
            //check for null;

            if (!includeDeactivated)
                regions = regions.Where(n => n._Status);

            return regions;
        }*/

        protected override string _cacheKey
        {
            get { return "Region-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "RegionList"; }
        }

        public override IEnumerable<Region> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All Regions; include Deactivated: {0}", includeDeactivated);
            IList<Region> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Region>(ids.Count);
                foreach (Guid id in ids)
                {
                    Region entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblRegion.ToList().Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Region p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       

        private  Region Map(tblRegion region)
        {
            Region reg = new Region(region.id)
            {
                //_Status = region.Active.Value,
                Name = region.Name,
                Description = region.Description,
                Country = _countryRepository.GetById(region.Country),
                //provinceId = _provinceRepository.GetById(region.Province),
                //districtId=_districtRepository.GetById(region.District)
            };
            reg._SetDateCreated(region.IM_DateCreated);
            reg._SetDateLastUpdated(region.IM_DateLastUpdated);
            reg._SetStatus((EntityStatus)region.IM_Status);
            return reg;
        }
        public void SetInactive(Region entity)
        {
            _log.Debug("Inactivating region");
            bool dependenciesPresent = false; //no dependencies with entities currently done
            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblRegion reg = _ctx.tblRegion.First(n => n.id == entity.Id);
                if(reg==null || reg.IM_Status==(int)EntityStatus.Inactive){//not existing or already deactivated.
                    return;
                }
                reg.IM_Status = (int)EntityStatus.Inactive;// false;
                reg.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblRegion.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, reg.id));
            }
        }

        public void SetActive(Region entity)
        {
            tblRegion reg = _ctx.tblRegion.FirstOrDefault(n => n.id == entity.Id);
            if (reg != null)
            {
            reg.IM_Status = (int) EntityStatus.Active;
            reg.IM_DateLastUpdated = DateTime.Now;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblRegion.Where(n=> n.IM_Status != (int)EntityStatus.Deleted).Select(s=>s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey,reg.id));
            }
        }

        public QueryResult<Region> Query(QueryStandard query)
        {

            IQueryable<tblRegion> regionQuery;

            if (query.ShowInactive)
                regionQuery = _ctx.tblRegion.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
            {
                regionQuery = _ctx.tblRegion.Where(p => p.IM_Status == (int) EntityStatus.Active).AsQueryable();
            }

            var queryResult = new QueryResult<Region>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                regionQuery = regionQuery.Where(
                    p =>
                        p.Name.ToLower().Contains(query.Name.ToLower()) || p.Description.ToLower().Contains(query.Name));
            }

            regionQuery = regionQuery.OrderBy(p => p.Name).ThenBy(p => p.Description);
            queryResult.Count = regionQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                regionQuery = regionQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Data = regionQuery.Select(Map).OfType<Region>().ToList();
            return queryResult;
        }

        #region IValidation<Region> Members

        public ValidationResultInfo Validate(Region itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicate = _ctx.tblRegion
                .Where(n => n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted)
                .Any(n => n.Name == itemToValidate.Name && n.Country == itemToValidate.Country.Id);
            if(hasDuplicate){
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.region.validation.dupname")));
            }
            return vri;
        }

        #endregion

       


        //public bool GetItemUpdatedSinceDateTime(DateTime dateTime)
        //{
        //    return _ctx.tblRegion.Any(n=> n.IM_DateLastUpdated > dateTime);
        //}
        //public DateTime GetLastTimeItemUpdated()
        //{
        //    DateTime date=(from region in _ctx.tblRegion select region.IM_DateLastUpdated).Max();
        //    return date;
        //} 
    }
}
