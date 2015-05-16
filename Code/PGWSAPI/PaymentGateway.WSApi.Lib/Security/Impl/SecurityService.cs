using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Repository.MasterData.Users;

namespace PaymentGateway.WSApi.Lib.Security.Impl
{
    public class SecurityService : ISecurityService
    {
        IUserRepository _userRepository;
        public SecurityService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Authenticate(string sUserName, string sPassword)
        {
            return _userRepository.Login(sUserName, sPassword) == null;
        }

        public CustomPrincipal ConstructCustomPrincipal(IIdentity idIdentity)
        {
            User user = _userRepository.GetByUsername(idIdentity.Name);//FirstOrDefault(n => n.Username == idIdentity.Name &&( n.UserType == UserType.HQAdmin||n.UserType==UserType.SalesRep) );

            if (user == null)
                return null;

            var customIdentity = new CustomIdentity(user.Username, idIdentity.IsAuthenticated, idIdentity.AuthenticationType);

            //foreach (var r in user.UserRoles)
            customIdentity.Roles.Add("ROLE_ADMIN");

            return new CustomPrincipal(customIdentity);
        }
    }
}
