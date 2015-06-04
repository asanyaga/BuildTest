using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Distributr.Core.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private CustomIdentity _cidIdentity;
       
        public CustomPrincipal(CustomIdentity cidIdentity)
        {
            _cidIdentity = cidIdentity;
        }
     
        public IIdentity Identity    
        {
            get { return _cidIdentity; }
        }
    
        public bool IsInRole(string sRole)    
        {
            return _cidIdentity.Roles.Contains(sRole);
        }
    }
}
