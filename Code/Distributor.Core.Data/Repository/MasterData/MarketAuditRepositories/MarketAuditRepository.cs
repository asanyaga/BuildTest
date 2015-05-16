using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.MarketAuditRepositories
{
    internal  class MarketAuditRepository : RepositoryMasterBase<MarketAudit>, IMarketAuditRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public MarketAuditRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }
        public Guid Save(MarketAudit entity, bool? isSync = null)
        {
            ValidationResultInfo vri = Validate(entity);
            DateTime dt = DateTime.Now;
            if (!vri.IsValid)
            {
                string moreInfo = string.Join(",", vri.Results.Select(n => n.ErrorMessage));
                _log.Debug("Market Audit not valid-->" + moreInfo);
                throw new DomainValidationException(vri, "Market Audit Entity Not valid -->" + moreInfo );
            }
            tblMarketAudit marketAudit = _ctx.tblMarketAudit.FirstOrDefault(p => p.Id == entity.Id);
            if (marketAudit == null)
            {

                marketAudit = new tblMarketAudit();
                marketAudit.IM_DateCreated = dt;
                marketAudit.IM_Status = (int)EntityStatus.Active;// true;
                marketAudit.Id = entity.Id;
                _ctx.tblMarketAudit.AddObject(marketAudit);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (marketAudit.IM_Status != (int)entityStatus)
                marketAudit.IM_Status = (int)entity._Status;
            marketAudit.Question  = entity.Question ;
            marketAudit.StartDate = entity.StartDate ;
            marketAudit.EndDate = entity.EndDate ;
            marketAudit.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblMarketAudit.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, marketAudit.Id));
            return marketAudit.Id;
        }

        public void SetInactive(MarketAudit entity)
        {
            tblMarketAudit  marketAudit = _ctx.tblMarketAudit .FirstOrDefault(p => p.Id == entity.Id);
            if (marketAudit != null)
            {
                marketAudit.IM_Status = (int)EntityStatus.Inactive;// false;
                marketAudit.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblMarketAudit.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, marketAudit.Id));
            }
        }

        public void SetActive(MarketAudit entity)
        {
            throw new NotImplementedException();
        }

        public void SetAsDeleted(MarketAudit entity)
        {
            throw new NotImplementedException();
        }

        public MarketAudit GetById(Guid Id, bool includeDeactivated = false)
        {
            MarketAudit entity = (MarketAudit)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblMarketAudit.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(MarketAudit itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            //start date should be before end date
            if (itemToValidate.StartDate > itemToValidate.EndDate)
                vri.Results.Add(new ValidationResult("Start date must be before end date"));

            //check for duplicate names
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Question == itemToValidate.Question);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            //check for dates within an existing period
            if (DateAlreadyInPeriod(itemToValidate.StartDate, itemToValidate.Id))
                vri.Results.Add(new ValidationResult("Start date already exists in an existing defined period"));
            if (DateAlreadyInPeriod(itemToValidate.EndDate, itemToValidate.Id))
                vri.Results.Add(new ValidationResult("End date already exists in an existing defined period"));

            return vri;
        }
        bool DateAlreadyInPeriod(DateTime dateTime, Guid idToValidate)
        {
            return GetAll().Where(n => n.Id != idToValidate).Select(n => n.IsWithinDateRange(dateTime)).Count() > 0;
        }


        protected override string _cacheKey
        {
            get { return "MarketAudit-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "MarketAuditList"; }
        }

        public override IEnumerable<MarketAudit> GetAll(bool includeDeactivated = false)
        {
            IList<MarketAudit> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<MarketAudit>(ids.Count);
                foreach (Guid id in ids)
                {
                    MarketAudit entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblMarketAudit.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (MarketAudit p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<MarketAudit> Query(QueryStandard query)
        {
            IQueryable<tblMarketAudit> marketAuditQuery;
            if (query.ShowInactive)
                marketAuditQuery =
                    _ctx.tblMarketAudit.Where(b => b.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                marketAuditQuery =
                    _ctx.tblMarketAudit.Where(k => k.IM_Status == (int) EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                marketAuditQuery = marketAuditQuery.Where(l => l.Question.ToLower().Contains(query.Name.ToLower()));

            marketAuditQuery = marketAuditQuery.OrderBy(j => j.Question).ThenBy(j => j.StartDate);

            var result = new QueryResult<MarketAudit>();
            result.Count = marketAuditQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                marketAuditQuery = marketAuditQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            var queryResult = marketAuditQuery.ToList();

            result.Data = queryResult.Select(Map).ToList();
            
            query.ShowInactive = false;

            return result;
        }

        public MarketAudit Map(tblMarketAudit marketAudit)
        {
            MarketAudit audit = new MarketAudit(marketAudit.Id )
            {
                Question=marketAudit.Question,
                StartDate=marketAudit.StartDate,
                EndDate=marketAudit.EndDate 
            };
            audit._SetDateCreated(marketAudit.IM_DateCreated );
            audit._SetDateLastUpdated(marketAudit.IM_DateLastUpdated );
            audit._SetStatus((EntityStatus)marketAudit.IM_Status );
            return audit;
        }
    }
}
