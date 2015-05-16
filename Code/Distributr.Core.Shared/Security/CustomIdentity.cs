using System;
using System.Collections.Specialized;
using System.Security.Principal;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Security
{
    public class CustomIdentity : IIdentity
    {
        private bool _bIsAuthenticated;
        private string _sAuthenticationType;
        private string _sUserName;
        private StringCollection _lstRoles = new StringCollection();
        private UserType _userType;
        private Guid? _supplierId;


        public CustomIdentity(string sUserName, bool bIsAuthenticated, string sAuthenticationType, UserType userType,Guid? supplierId)
        {
            _sUserName = sUserName;
            _bIsAuthenticated = bIsAuthenticated;
            _sAuthenticationType = sAuthenticationType;
            _userType = userType;
            _supplierId = supplierId;
        }
        public UserType UserType
        {
            get { return _userType; }
        }
        public string AuthenticationType
        {
            get { return _sAuthenticationType; }
        }
        public Guid? SupplierId 
        {
            get { return _supplierId; }
        }
        public bool IsAuthenticated
        {
            get { return _bIsAuthenticated; }
        }

        public string Name
        {
            get { return _sUserName; }
        }

        public StringCollection Roles
        {
            get { return _lstRoles; }
        }
    }
}
