using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModels.Admin.User;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder
{
    public interface IUserTypeViewModelBuilder
    {
        IList<UserTypeViewModel> GetAll();
        UserTypeViewModel Get(Guid Id);
        void Save(UserTypeViewModel usertypeviewmodel);
        void SetInactive(Guid id);
    }
}
