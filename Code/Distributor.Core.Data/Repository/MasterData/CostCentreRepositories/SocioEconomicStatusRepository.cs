using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class SocioEconomicStatusRepository : RepositoryMasterBase<SocioEconomicStatus>, ISocioEconomicStatusRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public SocioEconomicStatusRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("SocioEconomicStatus Repository Constructor Bootstrap");
        }

        //protected static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Guid Save(SocioEconomicStatus entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating SocioEconomicStatus");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid SocioEconomicStatus");
                throw new DomainValidationException(vri, "Failed to save invalid SocioEconomicStatus");
            }

            tblSocioEconomicStatus ses = _ctx.tblSocioEconomicStatus.FirstOrDefault(n => n.id == entity.Id);
            DateTime date = DateTime.Now;
            if (ses == null)
            {
                ses = new tblSocioEconomicStatus {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    id= entity.Id
                };
                _ctx.tblSocioEconomicStatus.AddObject(ses);
            }
            else
            {
             
               
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (ses.IM_Status != (int)entityStatus)
                ses.IM_Status = (int)entity._Status;
            ses.Status = entity.EcoStatus;

            ses.IM_DateLastUpdated = date;

            _log.Debug("Saving SocioEconomicStatus");
            _ctx.SaveChanges();
            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblSocioEconomicStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, ses.id));
            _log.DebugFormat("Successfully saved item id:{0}",ses.id );
            return ses.id;
        }

        public void SetAsDeleted(SocioEconomicStatus entity)
        {
            throw new NotImplementedException();
        }

        public SocioEconomicStatus GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting SocioEconomicStatus by ID: {0}",Id);


            SocioEconomicStatus entity = (SocioEconomicStatus)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblSocioEconomicStatus.FirstOrDefault(s => s.id == Id);
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
            get { return "SocioEconomicStatus-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "SocioEconomicStatusList"; }
        }

        public override IEnumerable<SocioEconomicStatus> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All SocioEconomicStatuss; include Deactivated: {0}", includeDeactivated);

            IList<SocioEconomicStatus> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<SocioEconomicStatus>(ids.Count);
                foreach (Guid id in ids)
                {
                    SocioEconomicStatus entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblSocioEconomicStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (SocioEconomicStatus p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public void SetInactive(SocioEconomicStatus entity)//To be done
        {
            //if(_ctx.tblSocioEconomicStatus.Where(s=>s.id == entity.Id).)
            _log.Debug("Inactivating SocioEconomicStatus");
            bool dependenciesPresent = false; 

            string failureReason = "";
            if (dependenciesPresent)
            {
                failureReason = "DEPENDENCIES FOUND:";//populate with ids
                throw new ArgumentException(failureReason);
            }
            else
            {
                tblSocioEconomicStatus reg = _ctx.tblSocioEconomicStatus.First(n => n.id == entity.Id);
                if(reg==null || reg.IM_Status==(int)EntityStatus.Inactive){//not existing or already deactivated.
                    return;
                }
                reg.IM_Status = (int)EntityStatus.Inactive;//false;
                reg.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblSocioEconomicStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, reg.id));
            }
          
        }

        public void SetActive(SocioEconomicStatus entity)
        {
            _log.Debug("Activating Social Economic Status ID: " + entity.Id);
            tblSocioEconomicStatus tblSocioEconomicStatus = _ctx.tblSocioEconomicStatus.FirstOrDefault(n => n.id == entity.Id);
            if (tblSocioEconomicStatus != null)
            {
                tblSocioEconomicStatus.IM_Status = (int)EntityStatus.Active;// false;
                tblSocioEconomicStatus.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblSocioEconomicStatus.id));
            }
        }

        public ValidationResultInfo Validate(SocioEconomicStatus itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicate = _ctx.tblSocioEconomicStatus.Where(n => n.id != itemToValidate.Id && n.IM_Status != (int)EntityStatus.Deleted).Any(n => n.Status == itemToValidate.EcoStatus);
            if (hasDuplicate)
            {
                vri.Results.Add(new ValidationResult("Duplicate Status found"));
            }
            return vri;
        }

       
    }
}
