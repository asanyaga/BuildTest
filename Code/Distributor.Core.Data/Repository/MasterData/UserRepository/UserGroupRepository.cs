using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.UserRepository
{
     internal class UserGroupRepository:RepositoryMasterBase<UserGroup>, IUserGroupRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

         public UserGroupRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
         {
             _ctx = ctx;
             _cacheProvider = cacheProvider;
             _log.Debug("User UserGroupRepository Constructor Bootstrap");
         }

        

         protected override string _cacheKey
         {
             get { return "UserGroup-{0}"; }
         }

         protected override string _cacheListKey
         {
             get { return "UserGroupList"; }
         }

         public override IEnumerable<UserGroup> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All UserGroup; include Deactivated: {0}", includeDeactivated);


            IList<UserGroup> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<UserGroup>(ids.Count);
                foreach (Guid id in ids)
                {
                    UserGroup entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblUserGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (UserGroup p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

         public QueryResult<UserGroup> Query(QueryStandard q)
         {
             IQueryable<tblUserGroup> userGroupQuery;

             if (q.ShowInactive)
                 userGroupQuery = _ctx.tblUserGroup.Where(l => l.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
             else
                 userGroupQuery = _ctx.tblUserGroup.Where(l => l.IM_Status == (int)EntityStatus.Active).AsQueryable();

             if (!string.IsNullOrWhiteSpace(q.Name))
                 userGroupQuery = userGroupQuery.Where(l => l.Name.ToLower().Contains(q.Name.ToLower()));

             var queryResult = new QueryResult<UserGroup>();
             queryResult.Count = userGroupQuery.Count();

             userGroupQuery = userGroupQuery.OrderBy(m => m.Name).ThenBy(p => p.Description);

             if (q.Take.HasValue && q.Skip.HasValue)
                 userGroupQuery = userGroupQuery.Skip(q.Skip.Value).Take(q.Take.Value);

             var result = userGroupQuery.ToList();

             queryResult.Data = result.Select(l => l.Map()).ToList();

             q.ShowInactive = false;

             return queryResult;
         }


         public Guid Save(UserGroup entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating UserGroup");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug("Failed to save invalid UserGroup");
                throw new DomainValidationException(vri, "Failed to save invalid UserGroup");
            }

            tblUserGroup usrgroup = _ctx.tblUserGroup.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (usrgroup ==null)
            {
                usrgroup = new tblUserGroup
                {
                    IM_DateCreated = date,
                    IM_Status =(int)EntityStatus.Active,// true,
                    Id= entity.Id
                };
                _ctx.tblUserGroup.AddObject(usrgroup);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (usrgroup.IM_Status != (int)entityStatus)
                usrgroup.IM_Status = (int)entity._Status;
            
            //usr.GroupId = roles;
            usrgroup.IM_DateLastUpdated = date;
            usrgroup.Description = entity.Descripition;
            usrgroup.Name = entity.Name;
            _log.Debug("Saving usrgroup");
            _ctx.SaveChanges();

            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, usrgroup.Id));
            _log.DebugFormat("Successfully saved item id:{0}", usrgroup.Id);
            return usrgroup.Id;
        }

        public void SetInactive(UserGroup entity)
        {
            ValidationResultInfo vri = Validate(entity);
            _log.Debug("InActivating UserGroup");
            var hasUserDependency = false;
            hasUserDependency = _ctx.tblUsers.Where(u => u.IM_Status==(int)EntityStatus.Active).Any(u => u.GroupId == entity.Id);

            if (hasUserDependency)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblUserGroup user = _ctx.tblUserGroup.First(n => n.Id == entity.Id);
                if (user != null)
                {
                    user.IM_Status = (int)EntityStatus.Inactive;// false;
                    user.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, user.Id));
                }
            }
        }

         public void SetActive(UserGroup entity)
         {
             _log.Debug("Activating an UserGroup ID: " + entity.Id);
             ValidationResultInfo vri = Validate(entity);
             tblUserGroup userGroup = _ctx.tblUserGroup.FirstOrDefault(n => n.Id == entity.Id);
             if (userGroup != null)
             {
                 userGroup.IM_Status = (int)EntityStatus.Active;
                 userGroup.IM_DateLastUpdated = DateTime.Now;
                 _ctx.SaveChanges();
                 _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                 _cacheProvider.Remove(string.Format(_cacheKey, userGroup.Id));

             }
         }

         public void SetAsDeleted(UserGroup entity)
         {
             ValidationResultInfo vri = Validate(entity);
             _log.Debug("Deleting UserGroup");
             var hasUserDependency = false;
             hasUserDependency = _ctx.tblUsers.Where(u => u.IM_Status != (int)EntityStatus.Deleted)
                 .Any(u => u.GroupId == entity.Id);
             var hasUserGroupRoleDependency = _ctx.tblUserGroupRoles
                 .Where(n => n.IM_Status != (int)EntityStatus.Deleted)
                 .Any(n => n.GroupId == entity.Id);
             if (hasUserDependency || hasUserGroupRoleDependency)
             {
                 throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
             }
             else
             {
                 tblUserGroup user = _ctx.tblUserGroup.First(n => n.Id == entity.Id);
                 if (user != null)
                 {
                     user.IM_Status = (int)EntityStatus.Deleted;// false;
                     user.IM_DateLastUpdated = DateTime.Now;
                     _ctx.SaveChanges();
                     _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                     _cacheProvider.Remove(string.Format(_cacheKey, user.Id));
                 }
             }
         }

         public UserGroup GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting UserGroup by ID: {0}", Id);

            UserGroup entity = (UserGroup)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblUserGroup.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

       

        public ValidationResultInfo Validate(UserGroup itemToValidate)
        {
            _log.InfoFormat("Validating UserGroup" + itemToValidate.Id);
            //need to add validation for username duplicates???
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            
            bool alreadyExists = GetAll(true)
                .Where(p => p.Id != itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower() );
            if (alreadyExists)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.user.validation.dupgroup")));
            return vri;
        }
    }
}
