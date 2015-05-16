using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;
using Distributr.Core.Domain.Transactional.AuditLogEntity;
using log4net;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using System.Reflection;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.Transactional.AuditLogRepositories
{
    internal class AuditLogRepository : RepositoryMasterBase<AuditLog>, IAuditLogRepository
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected CokeDataContext _ctx;
        protected ICostCentreRepository _costCentreRepository;
        protected IUserRepository _userRepository;
        ICacheProvider _cacheProvider;

        public AuditLogRepository(CokeDataContext ctx, ICostCentreRepository costCentreRepository, IUserRepository userRepository,ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _costCentreRepository = costCentreRepository;
            _userRepository = userRepository;
            _cacheProvider=cacheProvider;
        }

        #region IRepositoryTransactional<AuditLog> Members

        public void Save(AuditLog entity)
        {
            tblAuditLog docToSave = _ctx.tblAuditLog.FirstOrDefault(n => n.Id == entity.Id);
            if (docToSave == null)
            {
                docToSave = new tblAuditLog();
                docToSave.Id = entity.Id;
                docToSave.Action = entity.Action;
                docToSave.UserId = entity.ActionUser == null ? Guid.Empty.ToString() : entity.ActionUser.Id.ToString();
                docToSave.OwnerId =entity.ActionOwner == null ? Guid.Empty.ToString() : entity.ActionOwner.Id.ToString();
                docToSave.Module = entity.Module;
                docToSave.IM_DateCreated = entity.ActionTimeStamp;
                _ctx.tblAuditLog.AddObject(docToSave);
            }
            _ctx.SaveChanges();
        }

        public AuditLog GetById(Guid Id)
        {
            AuditLog entity = (AuditLog)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblAuditLog.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public List<AuditLog> GetAll()
        {
            IList<AuditLog> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<AuditLog>(ids.Count);
                foreach (Guid id in ids)
                {
                    AuditLog entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblAuditLog.ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (AuditLog p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            return entities.ToList();
        }

        public List<AuditLog> GetAll(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public int GetCount()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IValidation<AuditLog> Members

        public ValidationResultInfo Validate(AuditLog itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();

            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }

        #endregion

        AuditLog Map(tblAuditLog tblAl)
        {
            AuditLog aLog = new AuditLog(tblAl.Id)
            {
                 Action=tblAl.Action,
                  ActionOwner=_costCentreRepository.GetById(Guid.Parse(tblAl.OwnerId)),
                   ActionTimeStamp=tblAl.IM_DateCreated,
                    ActionUser=_userRepository.GetById(Guid.Parse(tblAl.UserId)),
                     Module=tblAl.Module
            };
            return aLog;
        }
       public void addAuditLog(CostCentre ActionOwner, User User, string Module, string Action, DateTime timeStamp)
        //public void addAuditLog(int ActionOwnerId, int UserId, string Module, string Action, DateTime timeStamp)
        {
            AuditLog aLog = new AuditLog(Guid.NewGuid()) 
            {
             Action=Action,
              ActionTimeStamp=timeStamp,
               Module=Module,
              ActionOwner=_costCentreRepository.GetById(User.CostCentre ),
               ActionUser=_userRepository.GetById(User.Id),
            };
            Save(aLog);
        }

       
        protected override string _cacheKey
        {
            get { return "AuditLog-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "AuditLogList"; }
        }

        public override IEnumerable<AuditLog> GetAll(bool includeDeactivated = false)
       {
           IList<AuditLog> entities = null;
           IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
           if (ids != null)
           {
               entities = new List<AuditLog>(ids.Count);
               foreach (Guid id in ids)
               {
                   AuditLog entity = GetById(id);
                   if (entity != null)
                       entities.Add(entity);
               }
           }
           else
           {
               entities = _ctx.tblAuditLog.ToList().Select(s => Map(s)).ToList();
               if (entities != null && entities.Count > 0)
               {
                   ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                   _cacheProvider.Put(_cacheListKey, ids);
                   foreach (AuditLog p in entities)
                   {
                       _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                   }

               }
           }

          
           return entities;
       }

         public List<AuditLog> GetByDate(DateTime from, DateTime to)
        {
            return _ctx.tblAuditLog.Where(s => s.IM_DateCreated >= from && s.IM_DateCreated <= to)
                .OrderByDescending(d => d.IM_DateCreated)
                .ToList().Select(n => Map(n)).ToList();
        }
       
    }
}
