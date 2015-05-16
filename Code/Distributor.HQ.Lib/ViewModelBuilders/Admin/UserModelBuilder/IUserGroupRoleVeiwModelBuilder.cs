using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder
{
   public interface IUserGroupRoleVeiwModelBuilder
    {
        IList<UserGroupRoleVeiwModel> GetAll(bool inactive = false);
        IList<UserGroupRoleVeiwModel> Search(string srchParam, bool inactive = false);
        UserGroupRoleVeiwModel Get(Guid Id);
        void Save(UserGroupRoleVeiwModel userGroupVeiwModel);
        void SetInActive(Guid Id);
        void SetActive(Guid Id);
        void SetDeleted(Guid id);
    }
}
