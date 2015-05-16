using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.MarketAuditRepositories;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Data.IOC;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.MarketAuditRepositories
{
    internal  class OutletAuditRepository : RepositoryMasterBase<OutletAudit>, IOutletAuditRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public OutletAuditRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }
        public Guid Save(OutletAudit entity, bool? isSync = null)
        {
            ValidationResultInfo vri = Validate(entity);
            DateTime dt = DateTime.Now;
            if (!vri.IsValid)
            {
                _log.Debug("Outlet Audit not valid");
                throw new DomainValidationException(vri, "Outlet Audit Entity Not valid");
            }
            tblOutletAudit outletAudit = _ctx.tblOutletAudit.FirstOrDefault(p => p.Id == entity.Id);
            if (outletAudit == null)
            {

                outletAudit = new tblOutletAudit();
                outletAudit.IM_DateCreated = dt;
                outletAudit.IM_Status = (int)EntityStatus.Active;// true;
                outletAudit.Id = entity.Id;
                _ctx.tblOutletAudit.AddObject(outletAudit);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (outletAudit.IM_Status != (int)entityStatus)
                outletAudit.IM_Status = (int)entity._Status;
              
            outletAudit.Question = entity.Question;
            
            outletAudit.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletAudit.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, outletAudit.Id));
            return outletAudit.Id;

        }

        public void SetInactive(OutletAudit entity)
        {
            //string msg = "";
            tblOutletAudit outletAudit = _ctx.tblOutletAudit.FirstOrDefault(o=>o.Id==entity.Id );
            if (outletAudit != null)
            {
                outletAudit.IM_DateLastUpdated = DateTime.Now;
                outletAudit.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletAudit.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, outletAudit.Id));
            }
        }

        public void SetActive(OutletAudit entity)
        {
            throw new NotImplementedException();
        }

        public void SetAsDeleted(OutletAudit entity)
        {
            throw new NotImplementedException();
        }

        public OutletAudit GetById(Guid Id, bool includeDeactivated = false)
        {
            OutletAudit entity = (OutletAudit)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblOutletAudit.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(OutletAudit itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

           //check for duplicate names
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Question == itemToValidate.Question);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;
        }


        protected override string _cacheKey
        {
            get { return "OutletAudit-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "OutletAuditList"; }
        }

        public override IEnumerable<OutletAudit> GetAll(bool includeDeactivated = false)
        {
            IList<OutletAudit> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<OutletAudit>(ids.Count);
                foreach (Guid id in ids)
                {
                    OutletAudit entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblOutletAudit.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (OutletAudit p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }
        public OutletAudit Map(tblOutletAudit outletAudit)
        {
            OutletAudit outlet = new OutletAudit(outletAudit.Id )
            {
                
                Question=outletAudit.Question 

            };
            outlet._SetDateCreated(outletAudit.IM_DateCreated );
            outlet._SetDateLastUpdated(outletAudit.IM_DateLastUpdated );
            outlet._SetStatus((EntityStatus)outletAudit.IM_Status );
            return outlet;
        }
    }
}
