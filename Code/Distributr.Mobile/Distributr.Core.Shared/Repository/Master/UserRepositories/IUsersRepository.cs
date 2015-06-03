using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;

namespace Distributr.Core.Repository.Master.UserRepositories
{
    public interface IUserRepository : IRepositoryMaster<User>
    {
        List<User> GetByDistributor(Guid distributorId, bool includeDeactivated = false);
        User Login(string username, string password);
        void ChangePassword(string oldPassword, string newPassword, string userName);
        void RegisterUser(string username, string password, string userPin, string mobileNo, int userType, int costCentre);
        void ResetUserPassword(Guid userId);
        //void ResetAllPasswords();
        User HqLogin(string username, string password);
        User GetUser(string username);
        User ConstructCustomPrincipal(string userName);
        User ConstructAgriCustomPrincipal(string userName);
        List<User> GetByUserType(UserType userType, bool includeDeactivated = false);
        List<User> GetByCostCentre(Guid ccId, bool includeDeactivated = false);
        List<User> GetByCostCentreAndUserType(Guid ccId, UserType userType, bool includeDeactivated = false);
        User GetByCode(string code, bool includeDeactivated = false);
        QueryResult<User> Query(QueryUsers query);
        QueryResult<User> SupplierQuery(QueryUsers query);
        QueryResult<User> DistributorQuery(QueryUsers query);
        QueryResult<User> HQQuery(QueryUsers query);
        IEnumerable<User> GetAllInactiveUsers(bool Deactivated = false);
   }
}
