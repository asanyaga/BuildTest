using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class TerritoryRepository :RepositoryMasterBase<Territory>,ITerritoryRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public TerritoryRepository(CokeDataContext ctx, ICacheProvider cacheProvider) {

            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("Territory repository Constructor Bootstrap");
        
        }


        protected override string _cacheKey
        {
            get { return "Territory-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "TerritoryList"; }
        }

        public override IEnumerable<Territory> GetAll(bool includeDeactivated = false)
        {
            IList<Territory> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Territory>(ids.Count);
                foreach (Guid id in ids)
                {
                    Territory entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblTerritory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Territory p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
            
        }

        public QueryResult<Territory> Query(QueryStandard query)
        {
            IQueryable<tblTerritory> territoryQuery;
            if (query.ShowInactive)
                territoryQuery = _ctx.tblTerritory.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                territoryQuery = _ctx.tblTerritory.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Territory>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                territoryQuery = territoryQuery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                         );

            }

            territoryQuery = territoryQuery.OrderBy(p => p.Name).ThenBy(p => p.id);
            queryResult.Count = territoryQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                territoryQuery = territoryQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = territoryQuery.Select(Map).ToList();

            return queryResult;
        }


        public Territory Map(tblTerritory territory)
        {
            Territory cntry = new Territory(territory.id)
            {
                Name = territory.Name
               
            };
            cntry._SetStatus((EntityStatus)territory.IM_Status);
            cntry._SetDateCreated(territory.IM_DateCreated);
            cntry._SetDateLastUpdated(territory.IM_DateLastUpdated);
            return cntry;

        }

        public Guid Save(Territory entity, bool? isSync = null)
        {
            tblTerritory pt  = _ctx.tblTerritory.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Territory not valid");
                throw new DomainValidationException(vri, "Territory Entity Not valid");
            }

            if (pt == null)
            {

                pt = new tblTerritory();
                pt.IM_Status = (int)EntityStatus.Active;// true;
                pt.IM_DateCreated = dt;
                pt.id = entity.Id;
                _ctx.tblTerritory.AddObject(pt);
            }
            else
            {
             

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (pt.IM_Status != (int)entityStatus)
                pt.IM_Status = (int)entity._Status;
            pt.Name = entity.Name;
            pt.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblTerritory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, pt.id));

            return pt.id;
        }

        public void SetInactive(Territory entity)
        {
            tblTerritory tblTerritory = _ctx.tblTerritory.FirstOrDefault(p => p.id == entity.Id);
            if (tblTerritory != null)
            {
                tblTerritory.IM_Status = (int)EntityStatus.Inactive;// false;
                tblTerritory.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblTerritory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblTerritory.id));
            }
        }

        public void SetActive(Territory entity)
        {
            tblTerritory tt = _ctx.tblTerritory.FirstOrDefault(p => p.id == entity.Id);
            if (tt != null)
            {
                tt.IM_Status = (int) EntityStatus.Active;
                tt.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,_ctx.tblTerritory.Where(n =>n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tt.id));
            }
        }

        public void SetAsDeleted(Territory entity)
        {
            tblTerritory tblTerritory = _ctx.tblTerritory.FirstOrDefault(n => n.id == entity.Id);
            if (tblTerritory != null)
            {
                tblTerritory.IM_Status = (int) EntityStatus.Deleted;
                tblTerritory.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblTerritory.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select(n => n.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblTerritory.id));
            }
        }

        public Territory GetById(Guid Id, bool includeDeactivated = false)
        {
            Territory entity = (Territory)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblTerritory.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity =tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity; 
        }

       

        public ValidationResultInfo Validate(Territory itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
              .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            return vri;
        }

        
    }
}
