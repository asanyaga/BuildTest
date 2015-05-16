using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Security
{
   public interface ISecurityService
    {
        bool Authenticate(string sUserName, string sPassword);

        /// <summary>
        /// Constructs an instance a CustomPrincipal object from a supplied IIdentity.
        /// </summary>
        /// <param name="idIdentity">The IIdentity object containing user information.</param>
        /// <returns>A CustomPrincipal object for the user identified in the 
        /// IIdentity object.</returns>
        CustomPrincipal ConstructCustomPrincipal(IIdentity idIdentity);
    }
}
