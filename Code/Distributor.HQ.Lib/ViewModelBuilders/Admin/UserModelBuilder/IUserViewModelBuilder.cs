using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.Outlets;
using Distributr.HQ.Lib.ViewModels.Admin.User;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.UserModelBuilder
{
    public interface IUserViewModelBuilder
    {
        List<UserViewModel> GetAll(bool inactive = false);
        IList<UserViewModel> Search(string srchParam, bool inactive = false);
        IList<UserViewModel> GetHqUsers(bool inactive = false);
        IList<UserViewModel> GetAllDistributorUsers(Guid costCentre, bool inactive = false);
        UserViewModel GetUserByUserType(UserType userType, bool inactive = false);
        IList<UserViewModel> GetAllUsers(bool inactive = false);
        IList<UserViewModel> SearchUsers(string searchParam, bool inactive = false);
        IList<UserViewModel> SearchDistributorUsers(string searchParam, Guid? costCentre, bool inactive = false);
        IList<UserViewModel> FilterUsers(int userType, bool inactive = false);
        IList<UserViewModel> FilterDistributrHqUsers(bool inactive = false);
        IList<UserViewModel> FilterAgrimanagrHqUsers(bool inactive = false);
        IList<UserViewModel> SearchHqUsers(UserType hqUserType, string searchParam, bool inactive = false);
        UserViewModel Get(Guid Id);
        void Save(UserViewModel userviewmodel);
        void AssignRole(UserViewModel userviewmodel);
        void SetInActive(Guid Id);
        void Activate(Guid id);
        void Delete(Guid id);
        Dictionary<Guid, string> CostCentre();
        Dictionary<Guid, string> CostCentreDistributors();
        Dictionary<Guid, string> Distributor();
        Dictionary<Guid, string> Producer();
        Dictionary<Guid, string> Suppliers();
        Dictionary<int, string> uts();
        Dictionary<int, string> utsDistributor();
        Dictionary<int, string> HqUserTypes();
        void ChangePassword(string oldPassword, string newPassword, string userName);
        void RegisterUser(string username, string password, string userPin, string mobileNo, int userType, int costCentre);
        void ResetPassword(Guid id);
        Dictionary<Guid, string> UserGroup();
        UserViewModel GetByUserName(string Username);
        void ImportUsers(string userName, UserViewModel uvm);
        List<UserViewModel> GetContactUsers(bool inactive = false);
        QueryResult<UserViewModel> Query(QueryUsers q);
        QueryResult<UserViewModel> SupplierQuery(QueryUsers q); //UserViewModel GetHqUsers(bool inactive = false);
        QueryResult<UserViewModel> DistributorQuery(QueryUsers q); //UserViewModel GetHqUsers(bool inactive = false);
        QueryResult<UserViewModel> HQQuery(QueryUsers q); //UserViewModel GetHqUsers(bool inactive = false);
        //UserViewModel GetAllDistributorUsers(Guid costCentre, bool inactive = false);
        //UserViewModel GetAllUsers(bool inactive = false);
        //UserViewModel SearchUsersSkipTake(string searchParam, bool inactive = false);
        //UserViewModel SearchHqUsersSkipTake(string searchParam, bool inactive = false);
        //UserViewModel FilterUsersSkipTake(int userType, bool inactive = false);
        //UserViewModel FilterHqUsersSkipTake(bool inactive = false);
        UserViewModel GetByCostCentreId(Guid id);
    }
}
