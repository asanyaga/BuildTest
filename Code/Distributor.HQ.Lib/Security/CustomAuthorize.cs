using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Security;

namespace Distributr.HQ.Lib.Security
{
    public class CustomAuthorize :AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
            
           
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            
            if (filterContext.HttpContext.User.Identity is CustomIdentity )
            {
                CustomIdentity user = (CustomIdentity)filterContext.HttpContext.User.Identity;
                var area = filterContext.RouteData.Values["area"] != null ? filterContext.RouteData.Values["area"].ToString() : "";
                if ( user.UserType == UserType.OutletUser && area.ToLower() != "outletmanager")
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        area = "OutletManager",
                        controller = "OutletHome",
                        action="index"
                        
                    }));
                }
                
            }
            base.OnAuthorization(filterContext);
        }

    }
}
