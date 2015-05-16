using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Resources.Util;
using Distributr.Core.Security;
using StructureMap;

namespace Distributr.HQ.Lib.Helper
{
   public static class ResourceHelper
    {
       public static string GetText(this HtmlHelper htmlHelper, string code)
       {
           IMessageSourceAccessor msa = ObjectFactory.GetInstance<IMessageSourceAccessor>();
           if (msa != null)
               return msa.GetText(code);
           else
               return code;

       }
       public static bool IsSystemAdmin(this HtmlHelper htmlHelper)
       {
           if (HttpContext.Current.User.Identity is CustomIdentity)
           {
               
                   var user = (CustomIdentity)HttpContext.Current.User.Identity;
                   if (user.UserType == UserType.HubManager || user.UserType == UserType.HQAdmin)
                       return true;
                   else
                       return false;
           }
           return false;
       }
     
       public static string GetLogo(this HtmlHelper htmlHelper)
       {

#if(KEMSA)
            string host = GetHost();
           return host+"Content/images/distributr-logo-kemsa.png";
#else
           string host = GetHost();
           return host+"Content/images/distributr-logo.png";
#endif

       }
       private static string GetHost()
       {
           string auth = HttpContext.Current.Request.Url.Authority;
           string scheme = HttpContext.Current.Request.Url.Scheme;
           string webroot = HttpContext.Current.Request.ApplicationPath.TrimEnd('/');
           return  string.Format("{0}://{1}{2}/",scheme,auth,webroot);
       }

       public static bool IsGlobalSettingUrlActive(this HtmlHelper htmlHelper, string code)
       {
           bool isactive = true;
         #if(KEMSA)
           switch(code)
           {
               case "GlobalSettingDiscount":
                   isactive = false;
                   break;
               case "GlobalSettingChannelPackaging":
                   isactive = false;
                   break;
               case "GlobalSettingBank":
                   isactive = false;
                   break;
               case "GlobalSettingMarketAudit":
                   isactive = false;
                   break;
               case "GlobalSettingReorderlevel":
                   isactive = false;
                   break;
               case "GlobalSettingDistributorTargets":
                   isactive = false;
                   break;
               case "GlobalSettingCompetitormgt":
                   isactive = false;
                   break;
               default:
                   isactive = true;
                   break;
           }
#else
         
#endif
           return isactive;
       }
       public static bool IsDashBoardUrlActive(this HtmlHelper htmlHelper, string code)
       {
           bool isactive = true;
#if(KEMSA)
           switch (code)
           {
               
               case "DashboardTarget":
                   isactive = false;
                   break;
               case "DashboardReorderlevel":
                   isactive = false;
                   break;
               case "DashboardSaleValueDiscount":
                   isactive = false;
                   break;
               case "DashboardFreeOfChargeDicount":
                   isactive = false;
                   break;
               case "DashboardDiscountGroup":
                   isactive = false;
                   break;
               case "DashboardPromotionDiscount":
                   isactive = false;
                   break;
               case "DashboardProductDiscount":
                   isactive = false;
                   break;
               case "DashboardCertainValueCertainProduct":
                   isactive = false;
                   break;
               case "DashboardBank":
                   isactive = false;
                   break;
               case "DashboardBranch":
                   isactive = false;
                   break;
               case "DashboardChannelPackaging":
                   isactive = false;
                   break;
               case "DashboardReportOrderSummary":
                   isactive = false;
                   break;
               case "DashboardReportOrderByBrand":
                   isactive = false;
                   break;
               case "DashboardReportOrderByDistributor":
                   isactive = false;
                   break;
               case "DashboardReportOrderBySubBrand":
                   isactive = false;
                   break;
               case "DashboardReportOrderByPackaging":
                   isactive = false;
                   break;
               default:
                   isactive = true;
                   break;
           }
#else
         
#endif
           return isactive;
       }
    }
}
