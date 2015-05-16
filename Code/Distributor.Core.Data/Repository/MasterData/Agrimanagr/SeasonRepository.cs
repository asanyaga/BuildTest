using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.Agrimanagr
{
    public class SeasonRepository:RepositoryMasterBase<Season>,ISeasonRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private ICommodityProducerRepository _commodityProducerRepository;


        public SeasonRepository(CokeDataContext ctx, ICacheProvider cacheProvider, ICommodityProducerRepository commodityProducerRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _commodityProducerRepository = commodityProducerRepository;
        }

        public Guid Save(Season entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("Season.validation.error"));
                throw new DomainValidationException(vri, "");
            }
            DateTime dt = DateTime.Now;

            tblSeason season = _ctx.tblSeason.FirstOrDefault(n => n.id == entity.Id);
            if (season == null)
            {
                season = new tblSeason();
                season.id = entity.Id;
                season.IM_Status = (int)EntityStatus.Active;
                season.IM_DateCreated = dt;
                _ctx.tblSeason.AddObject(season);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (season.IM_Status != (int)entityStatus)
                season.IM_Status = (int)entity._Status;
            season.Code = entity.Code;
            season.Name = entity.Name;
            season.Description = entity.Description;
            if(entity.CommodityProducer!=null)
                season.CommodityProducerId = entity.CommodityProducer.Id;
            season.StartDate = entity.StartDate;
            season.EndDate = entity.EndDate;
            season.IM_DateLastUpdated = dt;
            
           

            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblSeason.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, season.id));
            return season.id; 
        }

        public void SetInactive(Season entity)
        {
            var season = _ctx.tblSeason.FirstOrDefault(n => n.id == entity.Id);
            if (season != null)
            {
                season.IM_Status = (int)EntityStatus.Inactive;
                season.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSeason.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, season.id));
            }
        }

        public void SetActive(Season entity)
        {
            var season = _ctx.tblSeason.FirstOrDefault(n => n.id == entity.Id);
            if (season != null)
            {
                season.IM_Status = (int) EntityStatus.Active;
                season.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                   _ctx.tblSeason.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select(s => s.id)
                                       .ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, season.id));
            }
        }

        public void SetAsDeleted(Season entity)
        {
            var season = _ctx.tblSeason.FirstOrDefault(n => n.id == entity.Id);
            if (season != null)
            {
                season.IM_Status = (int)EntityStatus.Deleted;
                season.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSeason.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, season.id));
            }
        }

        public Season GetById(Guid Id, bool includeDeactivated = false)
        {
            Season entity = (Season)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblSeason.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Season> GetAll(bool includeDeactivated = false)
        {
            IList<Season> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Season>(ids.Count);
                foreach (Guid id in ids)
                {
                    Season entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSeason.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Season p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            entities.ToList();
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

      
        public QueryResult<Season> Query(QueryBase query)
        {
            var q = query as QuerySeason;
            IQueryable<tblSeason> seasonQuery;
            if (q.ShowInactive)
                seasonQuery = _ctx.tblSeason.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                seasonQuery = _ctx.tblSeason.Where(s=>s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Season>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                seasonQuery = seasonQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = seasonQuery.Count();
            seasonQuery = seasonQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                seasonQuery = seasonQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = seasonQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<Season>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(Season itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblSeason.Where(p => p.IM_Status != (int)EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (query.Any(s => s.id != itemToValidate.Id && !string.IsNullOrEmpty(s.Code) && s.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));
            return vri;
        }

        public Season Map(tblSeason p)
        {
            var commodityproducer = _commodityProducerRepository.GetById(p.CommodityProducerId);
            var season = new Season(p.id)
            {
                Code = (p.Code ?? ""),
                Description = (p.Description??""),
                Name = p.Name,
                CommodityProducer = commodityproducer,
                StartDate=p.StartDate,
                EndDate=p.EndDate
            };
            season._SetDateCreated(p.IM_DateCreated);
            season._SetDateLastUpdated(p.IM_DateLastUpdated);
            season._SetStatus((EntityStatus)p.IM_Status);
            return season;
        }


        protected override string _cacheKey
        {
            get { return "Season-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "SeasonList"; }
        }

    }
}
