using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.CompetitorManagement;
using Distributr.Core.Repository.Master.CompetitorManagement;
using Distributr.Core.Utility;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.CompetitorManagement
{
   internal class CompetitorRepository:RepositoryMasterBase<Competitor>,ICompetitorRepository
    {
       CokeDataContext _ctx;
       ICacheProvider _cacheProvider;
       public CompetitorRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;
       }
       public Guid Save(Competitor entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
            
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Failed to validate competitor");
            }
            tblCompetitor tblCompe = _ctx.tblCompetitor.FirstOrDefault(n => n.id == entity.Id); ;
            DateTime dt = DateTime.Now;
            if (tblCompe == null)
            {
                tblCompe = new tblCompetitor();
                tblCompe.id = entity.Id;
                tblCompe.IM_DateCreated = dt;
                tblCompe.IM_Status = (int)EntityStatus.Active;// true;
                _ctx.tblCompetitor.AddObject(tblCompe);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblCompe.IM_Status != (int)entityStatus)
                tblCompe.IM_Status = (int)entity._Status;
            tblCompe.IM_DateLastUpdated = dt;
            tblCompe.Name = entity.Name;
            tblCompe.PhysicalAddress = entity.PhysicalAddress;
            tblCompe.PostaAddress = entity.PostalAddress;
            tblCompe.Telephone = entity.Telephone;
            tblCompe.Lattitude = entity.Lattitude;
            tblCompe.Longitude = entity.Longitude;
            tblCompe.ContactPerson = entity.ContactPerson;
            tblCompe.City = entity.City;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitor.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblCompe.id));
            return tblCompe.id;
           
        }

        public void SetInactive(Competitor entity)
        {
             ValidationResultInfo vri = Validate(entity);
            bool hasCompetitorDependencies = _ctx.tblCompetitorProducts
                .Where(s => s.IM_Status != (int)EntityStatus.Deleted)
                .Any(p => p.CompetitorId == entity.Id);

            if (hasCompetitorDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete - has a competitor product attached.");
            }
            else
            {
                tblCompetitor tblCompe = _ctx.tblCompetitor.FirstOrDefault(n => n.id == entity.Id);
                if (tblCompe != null)
                {
                    tblCompe.IM_Status = (int)EntityStatus.Inactive;// false;
                    tblCompe.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitor.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, tblCompe.id));
                }
            }
        }

       public void SetActive(Competitor entity)
       {
           ValidationResultInfo vri = Validate(entity);
           bool hasCompetitorDependencies = _ctx.tblCompetitorProducts.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CompetitorId == entity.Id);

          
               tblCompetitor tblCompe = _ctx.tblCompetitor.FirstOrDefault(n => n.id == entity.Id);
               if (tblCompe != null)
               {
                   tblCompe.IM_Status = (int)EntityStatus.Active;
                   tblCompe.IM_DateLastUpdated = DateTime.Now;
                   _ctx.SaveChanges();
                   _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitor.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                   _cacheProvider.Remove(string.Format(_cacheKey, tblCompe.id));
               }
           
       }

       public void SetAsDeleted(Competitor entity)
       {
           ValidationResultInfo vri = Validate(entity);
           bool hasCompetitorDependencies = _ctx.tblCompetitorProducts
               .Where(s => s.IM_Status != (int)EntityStatus.Deleted)
               .Any(p => p.CompetitorId == entity.Id);

           if (hasCompetitorDependencies)
           {
               throw new DomainValidationException(vri, "Cannot delete - has a competitor product attached.");
           }
           else
           {
               tblCompetitor tblCompe = _ctx.tblCompetitor.FirstOrDefault(n => n.id == entity.Id);
               if (tblCompe != null)
               {
                   tblCompe.IM_Status = (int)EntityStatus.Deleted;
                   tblCompe.IM_DateLastUpdated = DateTime.Now;
                   _ctx.SaveChanges();
                   _cacheProvider.Put(_cacheListKey, _ctx.tblCompetitor.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                   _cacheProvider.Remove(string.Format(_cacheKey, tblCompe.id));
               }
           }
       }

       public Competitor GetById(Guid Id, bool includeDeactivated = false)
        {
            Competitor p = (Competitor)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (p == null)
            {
                var tbl = _ctx.tblCompetitor.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    p = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                }

            }
            return p;
        }

        public ValidationResultInfo Validate(Competitor itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name == itemToValidate.Name);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            bool hasDuplicateCode = GetAll(true)
                .Where(p => !string.IsNullOrEmpty(p.Telephone) && p.Id != itemToValidate.Id)
                .Any(x => x.Telephone == itemToValidate.Telephone);
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Telephone found"));

            return vri;
        }


       protected override string _cacheKey
       {
           get { return "Competitor-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "CompetitorList"; }
       }

       public override IEnumerable<Competitor> GetAll(bool includeDeactivated = false)
        {
            IList<Competitor> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Competitor>(ids.Count);
                foreach (Guid id in ids)
                {
                    Competitor entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCompetitor.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Competitor p in entities)
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

       public QueryResult<Competitor> Query(QueryStandard query)
       {
           IQueryable<tblCompetitor> competitorQuery;
               if (query.ShowInactive)
                   competitorQuery = _ctx.tblCompetitor.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                   competitorQuery = _ctx.tblCompetitor.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Competitor>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                competitorQuery = competitorQuery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                         || p.PhysicalAddress.ToLower().Contains(query.Name.ToLower()));

            }
            competitorQuery = competitorQuery.OrderBy(p => p.Name).ThenBy(p => p.PhysicalAddress);
            queryResult.Count = competitorQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                competitorQuery = competitorQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = competitorQuery.Select(Map).OfType<Competitor>().ToList();

            return queryResult;
             
       }


       protected Competitor Map(tblCompetitor tblCompe)
        {
            Competitor compe = new Competitor(tblCompe.id) 
            { 
             Lattitude=tblCompe.Lattitude,
             Longitude=tblCompe.Longitude,
             ContactPerson=tblCompe.ContactPerson,
             City=tblCompe.City,
             Name=tblCompe.Name,
             PhysicalAddress=tblCompe.PhysicalAddress,
             PostalAddress=tblCompe.PostaAddress,
              Telephone=tblCompe.Telephone
            };
            compe._SetDateCreated(tblCompe.IM_DateCreated);
            compe._SetDateLastUpdated(tblCompe.IM_DateLastUpdated);
            compe._SetStatus((EntityStatus)tblCompe.IM_Status);
            return compe;

        }
    }
}
