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
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.UserRepository
{
    internal class UserGroupRolesRepository : RepositoryMasterBase<UserGroupRoles>, IUserGroupRolesRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public UserGroupRolesRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

       

        protected override string _cacheKey
        {
            get { return "UserGroupRoles-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "UserGroupRolesList"; }
        }

        public override IEnumerable<UserGroupRoles> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All UserGroupRoles; include Deactivated: {0}", includeDeactivated);


            IList<UserGroupRoles> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<UserGroupRoles>(ids.Count);
                foreach (Guid id in ids)
                {
                    UserGroupRoles entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (UserGroupRoles p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public List<UserGroupRoles> GetByGroup(Guid GroupId)
        {
            return GetAll().Where(n => n.UserGroup.Id == GroupId).ToList().ToList();
        }

        public List<AgriUserGroupRoles> GetByAgriGroup(Guid goupid)
        {
            return GetAllAgri().Where(n => n.UserGroup.Id == goupid).ToList();
        }

        public Guid SaveAgriRole(AgriUserGroupRoles entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating UserGroupRoles");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = ValidateAgri(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("hq.user.validation.invusergroup"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.user.validation.invusergroup"));
            }

            tblUserGroupRoles usrgroup = _ctx.tblUserGroupRoles.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (usrgroup == null)
            {
                usrgroup = new tblUserGroupRoles
                {
                    IM_DateCreated = date,
                    IM_Status = (int)EntityStatus.Active,// true,
                    CanAccess = false,
                    Id = entity.Id
                };

                _ctx.tblUserGroupRoles.AddObject(usrgroup);
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (usrgroup.IM_Status != (int)entityStatus)
                usrgroup.IM_Status = (int)entity._Status;

            usrgroup.IM_DateLastUpdated = date;
            usrgroup.GroupId = entity.UserGroup.Id;
            usrgroup.RoleId = (int)entity.UserRole;
            usrgroup.CanAccess = entity.CanAccess;

            _log.Debug("Saving usrgroup");
            _ctx.SaveChanges();

            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, usrgroup.Id));
            _log.DebugFormat("Successfully saved item id:{0}", usrgroup.Id);
            return usrgroup.Id;
        }

        private ValidationResultInfo ValidateAgri(AgriUserGroupRoles itemToValidate)
        {
            _log.InfoFormat("Validating UserGroupRoles" + itemToValidate.UserGroup.Id);
            //need to add validation for username duplicates???
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            bool alreadyExists = GetAllAgri()
                .Where(p => p.Id != itemToValidate.Id)
                .Any(p => p.UserGroup == itemToValidate.UserGroup && p.UserRole == itemToValidate.UserRole);
            if (alreadyExists)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.user.validation.duprole")));
            return vri;
        }

        private IEnumerable<AgriUserGroupRoles> GetAllAgri()
        {
            IList<AgriUserGroupRoles> entities = null;
            /*IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<AgriUserGroupRoles>(ids.Count);
                foreach (Guid id in ids)
                {
                    AgriUserGroupRoles entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (UserGroupRoles p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();*/
            entities = _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(MapAgri).Where(k=>k!=null).ToList();
            /*if (entities != null && entities.Count > 0)
            {
                ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                _cacheProvider.Put(_cacheListKey, ids);
                foreach (AgriUserGroupRoles p in entities)
                {
                    _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                }

            }*/
            return entities;
        }

        private AgriUserGroupRoles MapAgri(tblUserGroupRoles tblUserGroupRoles)
        {
            if (
                !Enum.GetValues(typeof (AgriUserRole))
                    .Cast<AgriUserRole>()
                    .ToList()
                    .Contains((AgriUserRole) tblUserGroupRoles.RoleId))
                return null;
            var usergroup = new AgriUserGroupRoles(tblUserGroupRoles.Id)
            {
                UserGroup = MapUserGroup(tblUserGroupRoles.tblUserGroup),
                UserRole = (AgriUserRole)tblUserGroupRoles.RoleId,
                CanAccess = tblUserGroupRoles.CanAccess
            };
            usergroup._SetStatus((EntityStatus)tblUserGroupRoles.IM_Status);
            usergroup._SetDateCreated(tblUserGroupRoles.IM_DateCreated);
            usergroup._SetDateLastUpdated(tblUserGroupRoles.IM_DateLastUpdated);
            return usergroup;
        }

        private UserGroup MapUserGroup(tblUserGroup tblusergroup)
        {
            if (tblusergroup == null)
                return null;

            UserGroup usergroup = new UserGroup(tblusergroup.Id)
            {
                Descripition = tblusergroup.Description,
                Name = tblusergroup.Name,
            };
            usergroup._SetStatus((EntityStatus)tblusergroup.IM_Status);
            usergroup._SetDateCreated(tblusergroup.IM_DateCreated);
            usergroup._SetDateLastUpdated(tblusergroup.IM_DateLastUpdated);
            return usergroup;
        }

        public Guid Save(UserGroupRoles entity, bool? isSync = null)
        {
            _log.Debug("Saving/Updating UserGroupRoles");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("hq.user.validation.invusergroup"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.user.validation.invusergroup"));
            }

            tblUserGroupRoles usrgroup = _ctx.tblUserGroupRoles.FirstOrDefault(n => n.Id == entity.Id);
            DateTime date = DateTime.Now;
            if (usrgroup == null)
            {
                usrgroup = new tblUserGroupRoles
                               {
                                   IM_DateCreated = date,
                                   IM_Status =(int)EntityStatus.Active,// true,
                                   CanAccess = false,
                                   Id = entity.Id
                               };

                _ctx.tblUserGroupRoles.AddObject(usrgroup);
            }

            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (usrgroup.IM_Status != (int)entityStatus)
                usrgroup.IM_Status = (int)entity._Status;
            
            usrgroup.IM_DateLastUpdated = date;
            usrgroup.GroupId = entity.UserGroup.Id;
            usrgroup.RoleId = (int)entity.UserRole;
            usrgroup.CanAccess = entity.CanAccess;
          
            _log.Debug("Saving usrgroup");
            _ctx.SaveChanges();

            _log.Debug("Invalidating cache");
            _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, usrgroup.Id));
            _log.DebugFormat("Successfully saved item id:{0}", usrgroup.Id);
            return usrgroup.Id;
        }

        public void SetInactive(UserGroupRoles entity)
        {
            _log.Debug("InActivating UserGroupRoles");
            tblUserGroupRoles usergroup = _ctx.tblUserGroupRoles.First(n => n.Id == entity.Id);
            if (usergroup != null)
            {
                usergroup.IM_Status = (int)EntityStatus.Inactive;// false;
                usergroup.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, usergroup.Id));
            }
            
        }

        public void SetActive(UserGroupRoles entity)
        {
            _log.Debug("Activating UserGroupRoles");
            tblUserGroupRoles usergroup = _ctx.tblUserGroupRoles.First(n => n.Id == entity.Id);
            if (usergroup != null)
            {
                usergroup.IM_Status = (int)EntityStatus.Active;// false;
                usergroup.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, usergroup.Id));
            }
        }

        public void SetAsDeleted(UserGroupRoles entity)
        {
            _log.Debug("Deleting UserGroupRoles");
            tblUserGroupRoles usergroup = _ctx.tblUserGroupRoles.First(n => n.Id == entity.Id);
            if (usergroup != null)
            {
                usergroup.IM_Status = (int)EntityStatus.Deleted;// false;
                usergroup.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblUserGroupRoles.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, usergroup.Id));
            }
        }

        public UserGroupRoles GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting UserGroupRoles by ID: {0}", Id);
            UserGroupRoles entity = (UserGroupRoles)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblUserGroupRoles.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

       
        public ValidationResultInfo Validate(UserGroupRoles itemToValidate)
        {
            _log.InfoFormat("Validating UserGroupRoles" + itemToValidate.UserGroup.Id);
            //need to add validation for username duplicates???
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            
            bool alreadyExists = GetAll(true)
                .Where(p => p.Id != itemToValidate.Id)
                .Any(p => p.UserGroup == itemToValidate.UserGroup && p.UserRole==itemToValidate.UserRole);
            if (alreadyExists)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.user.validation.duprole")));
            return vri;
        }

    
    }
}
