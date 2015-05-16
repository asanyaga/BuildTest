using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Security
{
    public class CustomIdentity : IIdentity
    {
        private bool _bIsAuthenticated;
        private string _sAuthenticationType;
        private string _sUserName;
        private StringCollection _lstRoles = new StringCollection();


        public CustomIdentity(string sUserName, bool bIsAuthenticated, string sAuthenticationType)
        {
            _sUserName = sUserName;
            _bIsAuthenticated = bIsAuthenticated;
            _sAuthenticationType = sAuthenticationType;
        }

        public string AuthenticationType
        {
            get { return _sAuthenticationType; }
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
