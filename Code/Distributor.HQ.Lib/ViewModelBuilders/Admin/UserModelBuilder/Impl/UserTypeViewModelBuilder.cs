using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder
{
    //public class UserTypeViewModelBuilder:IUserTypeViewModelBuilder
    //{
    //    IUserTypeRepository _iusertyperepository;

    //    public UserTypeViewModelBuilder(IUserTypeRepository iusertyperepository)
    //    {
    //        _iusertyperepository = iusertyperepository;
    //    }

    //    public IList<UserTypeViewModel> GetAll()
    //    {
    //        var types = _iusertyperepository.GetAll();
    //        return types
    //            .Select(n => new UserTypeViewModel
    //            {
    //                Id = n.Id,
    //                Name = n.Name,
    //                Description = n.Description
    //            })
    //            .ToList();
    //    }

    //    UserTypeViewModel Map(UserType usertype)
    //    {
    //        return new UserTypeViewModel
    //        {
    //            Id = usertype.Id,
    //            Name = usertype.Name,
    //            Description = usertype.Description
    //        };
    //    }

    //    public UserTypeViewModel Get(int Id)
    //    {
    //        UserType usertype = new UserType(0);
    //        if (Id > 0)
    //            usertype = _iusertyperepository.GetById(Id);
    //        return Map(usertype);

    //    }

    //    public void Save(UserTypeViewModel usertypeviewmodel)
    //    {
    //        UserType usertype = new UserType(usertypeviewmodel.Id)
    //        {
    //            Name = usertypeviewmodel.Name,
    //            Description = usertypeviewmodel.Description,
    //        };
    //        _iusertyperepository.Save(usertype);
    //    }

    //    public void SetInactive(int id)
    //    {
    //        UserType usertype = _iusertyperepository.GetById(id);
    //        _iusertyperepository.SetInactive(usertype);

    //    }
    //}
}
