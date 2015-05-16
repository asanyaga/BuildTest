using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities;

using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.OutletVisitReasonsTypeRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.OutletVisitReasonsTypeRepositories
{
    class OutletVisitReasonsTypeRepository : RepositoryMasterBase<OutletVisitReasonsType>, IOutletVisitReasonsTypeRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public OutletVisitReasonsTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("OutletVisitReasonsType Repository Constractor");

        } 
        public Guid Save(OutletVisitReasonsType entity, bool? isSync = null)
        {

            _log.Debug("Saving OutletVisitReasonsType ");

            //validation
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
               
                _log.Debug("Saving invalid OutletVisitReasonsType Failed");
                throw new DomainValidationException(vri, "Failed to save, invalid OutletVisitReasonsType");
            }

            tblOutletVisitReasonType outletVrt =
                _ctx.tblOutletVisitReasonType.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (outletVrt == null)
            {
                outletVrt = new tblOutletVisitReasonType()
                                {
                                    IM_Status = (int) EntityStatus.Active,
                                    IM_DateCreated = dt,
                                    id = entity.Id,
                                    
                                
                                    
                                };
                _ctx.tblOutletVisitReasonType.AddObject(outletVrt);

            }
            var entityStatus = (entity._Status == EntityStatus.New)? EntityStatus.Active: entity._Status;
            if (outletVrt.IM_Status != (int)entityStatus )
                outletVrt.IM_Status = (int) entity._Status;
            outletVrt.Name = entity.Name;
            outletVrt.Description = entity.Description;
            outletVrt.IM_DateLastUpdated = DateTime.Now;
            outletVrt.Action = (int) entity.OutletVisitAction;
           
            _log.Debug("Saving OutletVisitReasonsType");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletVisitReasonType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select( s=> s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, outletVrt.id));
            _log.DebugFormat("Successfully saved item id:{0}", outletVrt.id);
            return outletVrt.id;


        }

        public void SetInactive(OutletVisitReasonsType entity)
        {
            _log.Debug("Inactive OutletVisitReasonsType");
            ValidationResultInfo vri = Validate(entity);
            bool hasOutletVisitReasonsType =
                _ctx.tblOutletType.Where(n => n.IM_Status != (int) EntityStatus.Active)
                                  .Any(p => p.id == entity.Id);
            if(hasOutletVisitReasonsType)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblOutletVisitReasonType outletVrt = _ctx.tblOutletVisitReasonType.FirstOrDefault(
                    p => p.id == entity.Id);
                if (outletVrt != null)
                {
                    outletVrt.IM_Status = (int) EntityStatus.Inactive;
                    outletVrt.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey,
                                       _ctx.tblOutletVisitReasonType.Where(
                                           n => n.IM_Status != (int) EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, outletVrt.id));

                }
            }
        }

        public void SetActive(OutletVisitReasonsType entity)
        {
            tblOutletVisitReasonType outletVrt = _ctx.tblOutletVisitReasonType.FirstOrDefault(p => p.id == entity.Id);
            if (outletVrt != null)
            {
                outletVrt.IM_Status = (int) EntityStatus.Active;
                outletVrt.IM_DateLastUpdated = DateTime.Now;

                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,
                                 _ctx.tblOutletVisitReasonType.Where(
                                     n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, outletVrt.id));
            }

        }

        public void SetAsDeleted(OutletVisitReasonsType entity)
        {
            _log.Debug("Set OutletVisitReasonsType as deleted; Delete OutletVisitReasonsType");
            ValidationResultInfo vri = Validate(entity);
            bool hasOutletVisitReasonsType =
                _ctx.tblOutletType.Where(n => n.IM_Status != (int)EntityStatus.Active)
                                  .Any(p => p.id == entity.Id);
            if (hasOutletVisitReasonsType)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblOutletVisitReasonType outletVrt = _ctx.tblOutletVisitReasonType.FirstOrDefault(
                    p => p.id == entity.Id);
                if (outletVrt != null)
                {
                    outletVrt.IM_Status = (int)EntityStatus.Deleted;
                    outletVrt.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey,
                                       _ctx.tblOutletVisitReasonType.Where(
                                           n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, outletVrt.id));

                }
            }
        }

        public OutletVisitReasonsType GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.Debug("Get OutletVisitReasonType By ID");
            OutletVisitReasonsType entity = (OutletVisitReasonsType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblOutletVisitReasonType.FirstOrDefault(s => s.id == Id);
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
            get { return "OutletVisitReasonsType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "OutletVisitReasonsTypeList"; }
        }

        public override IEnumerable<OutletVisitReasonsType> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Get all OutletVisitReasonsTypeList");
            IList<OutletVisitReasonsType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<OutletVisitReasonsType>(ids.Count);
                foreach (Guid id in ids)
                {
                    OutletVisitReasonsType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblOutletVisitReasonType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (OutletVisitReasonsType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }



        public ValidationResultInfo Validate(OutletVisitReasonsType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
            {
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            }
            if (itemToValidate.OutletVisitAction  == null )
            {
                itemToValidate.OutletVisitAction = OutletVisitAction.NoAction;
            }
            bool hasDuplicate = GetAll(true)
                .Where(n => n.Id != itemToValidate.Id)
                .Any(n => n.Name.ToLower() == itemToValidate.Name.ToLower() || n.Description.ToLower() == itemToValidate.Description.ToLower());
            if (hasDuplicate)
            {
                 itemToValidate.OutletVisitAction = OutletVisitAction.NoAction;
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
               
            }
            return vri;
        }


        public QueryResult<OutletVisitReasonsType> Query(QueryStandard query)
        {
            IQueryable<tblOutletVisitReasonType> outletVrtquery;
            if (query.ShowInactive)
                outletVrtquery = _ctx.tblOutletVisitReasonType.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                outletVrtquery = _ctx.tblOutletVisitReasonType.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<OutletVisitReasonsType>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                outletVrtquery = outletVrtquery.Where(p => p.Name.ToLower().Contains(query.Name.ToLower())
                                                       || p.Description.ToLower().Contains( query.Name.ToLower( )) );

            }

            outletVrtquery = outletVrtquery.OrderBy(p => p.Name).ThenBy(p => p.Description);
            queryResult.Count = outletVrtquery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
                outletVrtquery = outletVrtquery.Skip(query.Skip.Value).Take(query.Take.Value);
            queryResult.Data = outletVrtquery.Select(Map).OfType<OutletVisitReasonsType>().ToList();

            return queryResult;
        }


        public OutletVisitReasonsType Map(tblOutletVisitReasonType outletVisitReasonsType)
        {
            OutletVisitReasonsType retOutletVisitReasonsType = new OutletVisitReasonsType(outletVisitReasonsType.id)
            {

                Name = outletVisitReasonsType.Name,
                Description = outletVisitReasonsType.Description,
                OutletVisitAction = (OutletVisitAction) outletVisitReasonsType.Action,
                _DateCreated = outletVisitReasonsType.IM_DateCreated ,
                _DateLastUpdated = outletVisitReasonsType.IM_DateCreated 
                
               };
            retOutletVisitReasonsType._SetDateCreated(outletVisitReasonsType.IM_DateCreated);
            retOutletVisitReasonsType._SetDateLastUpdated(outletVisitReasonsType.IM_DateLastUpdated);
            retOutletVisitReasonsType._SetStatus((EntityStatus)outletVisitReasonsType.IM_Status);

            return retOutletVisitReasonsType;
        }

    }
}
