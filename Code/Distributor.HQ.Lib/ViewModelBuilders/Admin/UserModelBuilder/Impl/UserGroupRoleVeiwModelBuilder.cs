using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder.Impl
{
    public class UserGroupRoleVeiwModelBuilder : IUserGroupRoleVeiwModelBuilder
    {

        private IUserGroupRepository _userGroupRepository;
        private IUserGroupRolesRepository _userGroupRolesRepository;

        public UserGroupRoleVeiwModelBuilder(IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRolesRepository)
        {
            _userGroupRepository = userGroupRepository;
            _userGroupRolesRepository = userGroupRolesRepository;
        }

        public IList<UserGroupRoleVeiwModel> GetAll(bool inactive = false)
        {
            return _userGroupRolesRepository.GetAll(inactive).Select(p => Map(p)).ToList();
        }
        private UserGroupRoleVeiwModel Map(UserGroupRoles userGroupRole)
        {
            UserGroupRoleVeiwModel vm = new UserGroupRoleVeiwModel
            {
                GroupId = userGroupRole.UserGroup.Id,
                GroupName=userGroupRole.UserGroup.Name,
                RoleId=(int)userGroupRole.UserRole,
                RoleName=userGroupRole.UserRole.ToString(),
                Id = userGroupRole.Id,
                IsActive = userGroupRole._Status == EntityStatus.Active ? true : false
            };
            return vm;

        }
        public IList<UserGroupRoleVeiwModel> Search(string srchParam, bool inactive = false)
        {
            return _userGroupRolesRepository.GetAll(inactive).Where(s => s.UserRole.ToString() == srchParam).Select(p => Map(p)).ToList();
        }

        public UserGroupRoleVeiwModel Get(Guid Id)
        {
            UserGroupRoles userGroup = _userGroupRolesRepository.GetById(Id);
            if (userGroup != null)
                return Map(userGroup);
            return null;
        }

        public void Save(UserGroupRoleVeiwModel userGroupVeiwModel)
        {
            UserGroupRoles usergroup = new UserGroupRoles(userGroupVeiwModel.Id)
            {
                 UserGroup=_userGroupRepository.GetById(userGroupVeiwModel.GroupId),
                 UserRole = userGroupVeiwModel.RoleId

            };
            _userGroupRolesRepository.Save(usergroup);
        }

        public void SetInActive(Guid Id)
        {
            UserGroupRoles user = _userGroupRolesRepository.GetById(Id);
            _userGroupRolesRepository.SetInactive(user);
        }

        public void SetDeleted(Guid Id)
        {
            UserGroupRoles user = _userGroupRolesRepository.GetById(Id);
            _userGroupRolesRepository.SetAsDeleted(user);
        }

        public void SetActive(Guid Id)
        {
            UserGroupRoles user = _userGroupRolesRepository.GetById(Id);
            _userGroupRolesRepository.SetActive(user);
        }
        
    }
}
