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
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.Agrimanagr
{
    public class InfectionRepository:RepositoryMasterBase<Infection>,IInfectionRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public InfectionRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            
        }

        public Guid Save(Infection entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("Infection.validation.error"));
                throw new DomainValidationException(vri, "");
            }
            DateTime dt = DateTime.Now;

            tblInfection Infection = _ctx.tblInfection.FirstOrDefault(n => n.id == entity.Id);
            if (Infection == null)
            {
                Infection = new tblInfection();
                Infection.id = entity.Id;
                Infection.IM_Status = (int)EntityStatus.Active;
                Infection.IM_DateCreated = dt;
                _ctx.tblInfection.AddObject(Infection);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (Infection.IM_Status != (int)entityStatus)
                Infection.IM_Status = (int)entity._Status;
            Infection.Code = entity.Code;
            Infection.Name = entity.Name;
            Infection.Type = (int) entity.InfectionType;
            Infection.Description = entity.Description;
            Infection.IM_DateLastUpdated = dt;


            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblInfection.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, Infection.id));
            return Infection.id; 
        }

        public void SetInactive(Infection entity)
        {
            var Infection = _ctx.tblInfection.FirstOrDefault(n => n.id == entity.Id);
            if (Infection != null)
            {
                Infection.IM_Status = (int)EntityStatus.Inactive;
                Infection.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblInfection.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, Infection.id));
            }
        }

        public void SetActive(Infection entity)
        {
            var infection = _ctx.tblInfection.FirstOrDefault(n => n.id == entity.Id);
            if (infection != null)
            {
                infection.IM_Status = (int)EntityStatus.Active;
                infection.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblInfection.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, infection.id));
            }
        }

        public void SetAsDeleted(Infection entity)
        {
            var infection = _ctx.tblInfection.FirstOrDefault(n => n.id == entity.Id);
            if (infection != null)
            {
                infection.IM_Status = (int)EntityStatus.Deleted;
                infection.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblInfection.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, infection.id));
            }
        }

        public Infection GetById(Guid Id, bool includeDeactivated = false)
        {
            Infection entity = (Infection)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblInfection.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Infection> GetAll(bool includeDeactivated = false)
        {
            IList<Infection> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Infection>(ids.Count);
                foreach (Guid id in ids)
                {
                    Infection entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblInfection.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Infection p in entities)
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

        public QueryResult<Infection> Query(QueryBase query)
        {
            var q = query as QueryInfection;
            IQueryable<tblInfection> infectionQuery;
            if (q.ShowInactive)
                infectionQuery = _ctx.tblInfection.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                infectionQuery = _ctx.tblInfection.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Infection>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                infectionQuery = infectionQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = infectionQuery.Count();
            infectionQuery = infectionQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                infectionQuery = infectionQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = infectionQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<Infection>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(Infection itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblInfection.Where(p => p.IM_Status != (int)EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (query.Any(s => s.id != itemToValidate.Id && !string.IsNullOrEmpty(s.Code) && s.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            if (query.Any(s => s.id != itemToValidate.Id && s.Name == itemToValidate.Name))
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            return vri;
        }

        public Infection Map(tblInfection s)
        {

            var infection = new Infection(s.id)
            {
                Code = (s.Code ?? ""),
                Description = (s.Description??""),
                InfectionType=(InfectionType)s.Type,
                Name = s.Name,
                
            };
            infection._SetDateCreated(s.IM_DateCreated);
            infection._SetDateLastUpdated(s.IM_DateLastUpdated);
            infection._SetStatus((EntityStatus)s.IM_Status);
            return infection;
        }
        protected override string _cacheKey
        {
            get { return "Infection-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "InfectionList"; }
        }

    }


}
