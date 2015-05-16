using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.MasterData;

namespace PaymentGateway.WSApi.Lib.Repository.MasterData.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User Login(string username, string password);
        User GetByUsername(string username);
    }
}
