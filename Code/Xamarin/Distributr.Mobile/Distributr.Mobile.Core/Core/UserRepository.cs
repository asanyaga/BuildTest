using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Utility.MasterData;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly Database db;

        public UserRepository(Database db)
            : base(db)
        {
            this.db = db;
        }

        public List<User> GetByDistributor(Guid distributorId, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public User Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string oldPassword, string newPassword, string userName)
        {
            throw new NotImplementedException();
        }

        public void RegisterUser(string username, string password, string userPin, string mobileNo, int userType,
            int costCentre)
        {
            throw new NotImplementedException();
        }

        public void ResetUserPassword(Guid userId)
        {
            throw new NotImplementedException();
        }

        public User HqLogin(string username, string password)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public User AgriHqLogin(string username, string password)
        {
            throw new NotImplementedException();
        }

        public User ConstructCustomPrincipal(string userName)
        {
            throw new NotImplementedException();
        }

        public User ConstructAgriCustomPrincipal(string userName)
        {
            throw new NotImplementedException();
        }

        public List<User> GetByUserType(UserType userType, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public List<User> GetByCostCentre(Guid ccId, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public List<User> GetByCostCentreAndUserType(Guid ccId, UserType userType, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public User GetByCode(string code, bool includeDeactivated = false)
        {
            throw new NotImplementedException();
        }

        public QueryResult<User> Query(QueryUsers query)
        {
            throw new NotImplementedException();
        }

        public QueryResult<User> SupplierQuery(QueryUsers query)
        {
            throw new NotImplementedException();
        }

        public QueryResult<User> DistributorQuery(QueryUsers query)
        {
            throw new NotImplementedException();
        }

        public QueryResult<User> HQQuery(QueryUsers query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllInactiveUsers(bool Deactivated = false)
        {
            throw new NotImplementedException();
        }
    }
}