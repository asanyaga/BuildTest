using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class CostCentreApplicationRepository :RepositoryMasterBase<CostCentreApplication>, ICostCentreApplicationRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        //ICostCentreRepository _costCentreRepository;

        public CostCentreApplicationRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        public Guid Save(CostCentreApplication entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid) 
            {
                throw new DomainValidationException(vri, "Failed to save, invalid cost centre application");
            }
            tblCostCentreApplication cca = _ctx.tblCostCentreApplication.FirstOrDefault(n=> n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (cca == null)
            {
                entity._SetDateCreated(dt);
                entity._SetStatus(EntityStatus.Active);
                cca = new tblCostCentreApplication 
                {
                    IM_DateCreated = dt,
                    id= entity.Id
                };
                _ctx.tblCostCentreApplication.AddObject(cca);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (cca.IM_Status != (int)entityStatus)
                cca.IM_Status = (int)entity._Status;

            cca.IM_DateLastUpdated = dt;
            cca.IM_Status = (int)EntityStatus.Active;//true;
            cca.description = entity.Description;
            cca.costcentreid = entity.CostCentreId;
           
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentreApplication.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, cca.id));
            return cca.id;

        }

        public void SetInactive(CostCentreApplication entity)
        {
            var cca = _ctx.tblCostCentreApplication.FirstOrDefault(n => n.id == entity.Id);
            if (cca != null)
            {
                cca.IM_DateLastUpdated = DateTime.Now;
                cca.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentreApplication.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, cca.id));
            }
           

        }

        public void SetActive(CostCentreApplication entity)
        {
            var cca = _ctx.tblCostCentreApplication.FirstOrDefault(n => n.id == entity.Id);
            if (cca != null)
            {
                cca.IM_DateLastUpdated = DateTime.Now;
                cca.IM_Status = (int)EntityStatus.Active;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCostCentreApplication.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, cca.id));
            }
        }

        public void SetAsDeleted(CostCentreApplication entity)
        {
            throw new NotImplementedException();
        }

        public CostCentreApplication GetById(Guid Id, bool includeDeactivated = false)
        {
            CostCentreApplication entity = (CostCentreApplication)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCostCentreApplication.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "CostCentreApplication-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CostCentreApplicationList"; }
        }

        public override IEnumerable<CostCentreApplication> GetAll(bool includeDeactivated = false)
        {
            IList<CostCentreApplication> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CostCentreApplication>(ids.Count);
                foreach (Guid id in ids)
                {
                    CostCentreApplication entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCostCentreApplication.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CostCentreApplication p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        CostCentreApplication Map(tblCostCentreApplication app)
        {
            var ret =  new CostCentreApplication(app.id)
            {
                Description = app.description,
                CostCentreId = app.costcentreid
            };
            ret._SetDateCreated(app.IM_DateCreated);
            ret._SetDateLastUpdated(app.IM_DateLastUpdated);
            ret._SetStatus((EntityStatus)app.IM_Status);
            return ret;
        }

        public ValidationResultInfo Validate(CostCentreApplication itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }

       

        public IEnumerable<CostCentreApplication> GetByCostCentreId(Guid Id)
        {
            var tblapp =  _ctx.tblCostCentreApplication.Where(s => s.costcentreid == Id && s.IM_Status==(int)EntityStatus.Active);
            return tblapp.ToList().Select(s => Map(s)).AsEnumerable();
            //return GetAll().Where(n => n.CostCentreId == Id).AsEnumerable();
        }


        //public int GetSeedCostCentreApplicationId(int costCentreId)
        //{
        //    //if there is a preexisting ccappid for a costcentre then return the lowest ccappid for that cost centre
        //    if (GetAll().Any(n => n.CostCentreId == costCentreId))
        //    {
        //        return GetAll().Where(n => n.CostCentreId == costCentreId).Min(n => n.Id);
        //    }
        //    return 0;
        //}
    }
}
