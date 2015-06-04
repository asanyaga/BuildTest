using System;
using System.Collections.Generic;
using System.Linq;

using System.Security.Principal;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Security.Impl
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
            return _userRepository.Login(sUserName, sPassword) == null ;
        }

        public CustomPrincipal ConstructCustomPrincipal(IIdentity idIdentity)
        {
            User user = _userRepository.ConstructCustomPrincipal(idIdentity.Name);//FirstOrDefault(n => n.Username == idIdentity.Name &&( n.UserType == UserType.HQAdmin||n.UserType==UserType.SalesRep) );

            if (user == null)
                return null;
            Guid? supplerid = user.Supplier!=null?user.Supplier.Id:(Guid?) null;
            var customIdentity = new CustomIdentity(user.Username, idIdentity.IsAuthenticated, idIdentity.AuthenticationType, user.UserType, supplerid);

            foreach (var r in user.UserRoles)
                customIdentity.Roles.Add(r);
         

            return new CustomPrincipal(customIdentity);
        }

        public CustomPrincipal ConstructAgriCustomPrincipal(IIdentity identity)
        {
            User user = _userRepository.ConstructAgriCustomPrincipal(identity.Name);

            if (user == null)
                return null;

            var customIdentity = new CustomIdentity(user.Username, identity.IsAuthenticated, identity.AuthenticationType,user.UserType,null);

            foreach (var r in user.UserRoles)
                customIdentity.Roles.Add(r.ToString());

            return new CustomPrincipal(customIdentity);
        }
    }
}
