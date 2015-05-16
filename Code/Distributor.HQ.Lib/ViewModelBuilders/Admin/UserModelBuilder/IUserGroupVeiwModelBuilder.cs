using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder.Impl;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder
{
   public interface IUserGroupVeiwModelBuilder
    {
       IList<UserGroupVeiwModel> GetAll(bool inactive = false);
       IList<UserGroupVeiwModel> Search(string srchParam, bool inactive = false);
       UserGroupVeiwModel Get(Guid Id);
       void Save(UserGroupVeiwModel userGroupVeiwModel);
       void SetInActive(Guid Id);
       UserGroupRolesPlaceHolder GetRoles(Guid goupid);
       void Save(string[] roles, Guid groupId);
       void SetActive(Guid Id);
       void SetDeleted(Guid id);

       QueryResult<UserGroupVeiwModel> Query(QueryStandard q);
       UserGroupRolesPlaceHolder GetAgriUserRoles(Guid id);
       void SaveAgri(string[] roles, Guid groupId);
    }

}
