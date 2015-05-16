using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.MarketAudit;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Admin.ChannelPackagingsViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder.Impl
{
    public class UserGroupVeiwModelBuilder : IUserGroupVeiwModelBuilder
    {
        private IUserGroupRepository _userGroupRepository;
        private IUserGroupRolesRepository _userGroupRolesRepository;

        public UserGroupVeiwModelBuilder(IUserGroupRepository userGroupRepository, IUserGroupRolesRepository userGroupRolesRepository)
        {
            _userGroupRepository = userGroupRepository;
            _userGroupRolesRepository = userGroupRolesRepository;
        }

        public IList<UserGroupVeiwModel> GetAll(bool inactive = false)
        {
            return _userGroupRepository.GetAll(inactive).Select(p=>Map(p)).ToList();
        }
        private UserGroupVeiwModel Map(UserGroup userGroup)
        {
            UserGroupVeiwModel vm = new UserGroupVeiwModel
                                        {
                                            Description=userGroup.Descripition,
                                            Id=userGroup.Id,
                                            Name=userGroup.Name,
                                            IsActive = userGroup._Status == EntityStatus.Active ? true : false
                                        };
            return vm;

        }
        public IList<UserGroupVeiwModel> Search(string srchParam, bool inactive = false)
        {
            return _userGroupRepository.GetAll(inactive).Where(s=>(s.Name.ToLower().StartsWith(srchParam.ToLower()))||(s.Descripition.ToLower().StartsWith(srchParam.ToLower()))).Select(p =>Map(p)).ToList();
        }

        public UserGroupVeiwModel Get(Guid Id)
        {
            UserGroup userGroup = _userGroupRepository.GetById(Id);
            if (userGroup != null)
                return Map(userGroup);
            return null;
        }

        public void Save(UserGroupVeiwModel userGroupVeiwModel)
        {
            UserGroup usergroup = new UserGroup(userGroupVeiwModel.Id)
            {
                Name = userGroupVeiwModel.Name,
                Descripition = userGroupVeiwModel.Description,
               
            };
            _userGroupRepository.Save(usergroup);
        }

        public UserGroupRolesPlaceHolder GetRoles(Guid goupid)
        {
            MakeplaceHolder(goupid);
          UserGroupRolesPlaceHolder place = new UserGroupRolesPlaceHolder(); /* 
            List<UserGroupRolesView> list = _userGroupRolesRepository.GetByGroup(goupid)
                .Select(p => new UserGroupRolesView
                                 {
                                     Id = p.Id,
                                     RoleId = (int) p.UserRole,
                                     Checked=p.CanAccess,
                                     RoleName=p.UserRole.ToString()
                                 })
                                 .ToList();*/
            var roles = _userGroupRolesRepository.GetByGroup(goupid);
            UserGroup group = _userGroupRepository.GetById(goupid);
            List<UserGroupRolesView> rolelList = new List<UserGroupRolesView>();
            foreach (var val in RolesHelper.GetRoles())
            {
                

                Guid id = Guid.NewGuid();
                bool canAcess = false;
                if (roles.Any(a => a.UserGroup.Id == group.Id && a.UserRole == val.Id))
                {
                    UserGroupRoles xrole = roles.First(a => a.UserGroup.Id == group.Id && a.UserRole == val.Id);
                    id = xrole.Id;
                    canAcess = xrole.CanAccess;
                }
                UserGroupRolesView r = new UserGroupRolesView()
                {
                   
                     Id = id,
                                     RoleId = val.Id,
                                     Checked=canAcess,
                                     RoleName=val.ToString()
                    
                };
                rolelList.Add(r);
            }
          
            place.GroupId = goupid;
            place.Rows = rolelList;
            place.GroupName = group.Name;
            return place;
        }

        private void MakeplaceHolder(Guid goupid)
        {
            UserGroup group = _userGroupRepository.GetById(goupid);


            List<UserGroupRoles> roles = _userGroupRolesRepository.GetByGroup(goupid);
            if (group != null)
            {
                foreach (var val in RolesHelper.GetRoles())
                {
                    Guid id = Guid.NewGuid();
                    bool canAcess = false;
                    if (roles.Any(a => a.UserGroup.Id == group.Id && a.UserRole == val.Id))
                    {
                        UserGroupRoles xrole = roles.First(a => a.UserGroup.Id == group.Id && a.UserRole == val.Id);
                        id = xrole.Id;
                        canAcess = xrole.CanAccess;
                    }                  
                        UserGroupRoles r = new UserGroupRoles(id)
                                               {
                                                   UserGroup = group,
                                                   UserRole = val.Id,
                                                   CanAccess = canAcess
                                               };
                        _userGroupRolesRepository.Save(r);
                  
                }
            }



        }
        public void SetInActive(Guid Id)
        {
            UserGroup user = _userGroupRepository.GetById(Id);
            _userGroupRepository.SetInactive(user);
        }

        public void SetDeleted(Guid Id)
        {
            UserGroup user = _userGroupRepository.GetById(Id);
            _userGroupRepository.SetAsDeleted(user);
        }

        public QueryResult<UserGroupVeiwModel> Query(QueryStandard q)
        {
            var queryResult = _userGroupRepository.Query(q);

            var result = new QueryResult<UserGroupVeiwModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public UserGroupRolesPlaceHolder GetAgriUserRoles(Guid goupid)
        {
            MakeAgriPlaceHolder(goupid);
            UserGroupRolesPlaceHolder place = new UserGroupRolesPlaceHolder();  
            var roles = _userGroupRolesRepository.GetByAgriGroup(goupid);
            UserGroup group = _userGroupRepository.GetById(goupid);
            List<UserGroupRolesView> rolelList = new List<UserGroupRolesView>();
            foreach (AgriUserRole val in Enum.GetValues(typeof(AgriUserRole)))
            {


                Guid id = Guid.NewGuid();
                bool canAcess = false;
                if (roles.Any(a => a.UserGroup.Id == group.Id && a.UserRole == val))
                {
                    AgriUserGroupRoles xrole = roles.First(a => a.UserGroup.Id == group.Id && a.UserRole == val);
                    id = xrole.Id;
                    canAcess = xrole.CanAccess;
                }
                UserGroupRolesView r = new UserGroupRolesView()
                {

                    Id = id,
                    RoleId = (int)val,
                    Checked = canAcess,
                    RoleName = val.ToString()

                };
                rolelList.Add(r);
            }

            place.GroupId = goupid;
            place.Rows = rolelList;
            place.GroupName = group.Name;
            return place;
        }

        private void MakeAgriPlaceHolder(Guid goupid)
        {
            UserGroup group = _userGroupRepository.GetById(goupid);

            List<AgriUserGroupRoles> roles = _userGroupRolesRepository.GetByAgriGroup(goupid);
            if (group != null)
            {
                foreach (AgriUserRole val in Enum.GetValues(typeof(AgriUserRole)))
                {
                    Guid id = Guid.NewGuid();
                    bool canAcess = false;
                    if (roles.Any(a => a.UserGroup.Id == group.Id && a.UserRole == val))
                    {
                        AgriUserGroupRoles xrole = roles.First(a => a.UserGroup.Id == group.Id && a.UserRole == val);
                        id = xrole.Id;
                        canAcess = xrole.CanAccess;
                    }
                    var r = new AgriUserGroupRoles(id)
                    {
                        UserGroup = group,
                        UserRole = val,
                        CanAccess = canAcess
                    };
                    _userGroupRolesRepository.SaveAgriRole(r);
                }
            }
        }

        public void SaveAgri(string[] roles, Guid groupId)
        {
            List<UserGroupRoleVeiwModel> vwRoles = new List<UserGroupRoleVeiwModel>();
            if (roles != null)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    UserGroupRoleVeiwModel cpvmmoto = new UserGroupRoleVeiwModel();
                    string[] s = roles[i].ToString().Split(',');
                    string UserGroupRoleId = s[0];

                    string RoleId = s[1];

                    cpvmmoto.GroupId = groupId;
                    cpvmmoto.Id = Guid.Parse(UserGroupRoleId);
                    cpvmmoto.RoleId = int.Parse(RoleId);
                    vwRoles.Add(cpvmmoto);
                }
                var userGroupRoles = _userGroupRolesRepository.GetByAgriGroup(groupId);
                foreach (AgriUserGroupRoles rolesitem in userGroupRoles)
                {
                    if (vwRoles.Any(p => p.RoleId == (int)rolesitem.UserRole && p.GroupId == rolesitem.UserGroup.Id))
                    {
                        rolesitem.CanAccess = true;
                    }
                    else
                    {
                        rolesitem.CanAccess = false;
                    }
                    _userGroupRolesRepository.SaveAgriRole(rolesitem);
                }
            }
        }

        public void SetActive(Guid Id)
        {
            UserGroup user = _userGroupRepository.GetById(Id);
            _userGroupRepository.SetActive(user);
        }



        #region IUserGroupVeiwModelBuilder Members


        public void Save(string[] roles, Guid groupId)
        {
            List<UserGroupRoleVeiwModel> vwRoles = new List<UserGroupRoleVeiwModel>();
            if (roles != null)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    UserGroupRoleVeiwModel cpvmmoto = new UserGroupRoleVeiwModel();
                    string[] s = roles[i].ToString().Split(',');
                    string UserGroupRoleId = s[0];
                    
                    string RoleId = s[1];

                    cpvmmoto.GroupId =groupId;
                    cpvmmoto.Id = Guid.Parse(UserGroupRoleId);
                    cpvmmoto.RoleId = int.Parse(RoleId);
                    vwRoles.Add(cpvmmoto);
                }
                foreach (UserGroupRoles rolesitem in _userGroupRolesRepository.GetByGroup(groupId))
                {
                    if (vwRoles.Any(p => p.RoleId == (int) rolesitem.UserRole && p.GroupId == rolesitem.UserGroup.Id))
                    {
                        rolesitem.CanAccess = true;
                    }
                    else
                    {
                        rolesitem.CanAccess = false;
                    }
                    _userGroupRolesRepository.Save(rolesitem);
                }
            }

        }

        #endregion


       
    }
    public class UserGroupRolesView
    {
        public Guid Id { get; set; }
        public int RoleId { get; set; }
        public bool Checked { get; set; }
        public string RoleName { get; set; }
        public string Value { get { return Id.ToString() + "," + RoleId.ToString(); } }
        public string Check { get { if (Checked) return "CHECKED"; else return ""; } }
    }
    public class UserGroupRolesPlaceHolder
    {
        public UserGroupRolesPlaceHolder()
        {
            Rows = new List<UserGroupRolesView>();

        }
        public List<UserGroupRolesView> Rows { get; set; }
        public Guid GroupId { get; set; }
        public string  GroupName { get; set; }
    }

    public class  SysModule
    {
        public int SysModuleId { get; set; }
        public string SysModuleName { get; set; }
    }
}
