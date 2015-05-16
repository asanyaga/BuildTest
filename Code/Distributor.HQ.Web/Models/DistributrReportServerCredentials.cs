using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility;
using Microsoft.Reporting.WebForms;
using StructureMap;

namespace Agrimanagr.HQ.Models
{
    public class DistributrReportServerCredentials: IReportServerCredentials
    {
        public DistributrReportServerCredentials()
        {
        }
        public WindowsIdentity ImpersonationUser
        {
            get
            {
                return null;
            }
        }
        public ICredentials NetworkCredentials
        {
            get
            {
                string username = "";
                string password = "";
                var setting = ObjectFactory.GetInstance<ISettingsRepository>();
                var reportusername = setting.GetByKey(SettingsKeys.ReportServerUsername);
                if (reportusername != null)
                    username = reportusername.Value;
                var reportpassord = setting.GetByKey(SettingsKeys.ReportServerPassword);
                if (reportpassord != null)
                    password = VCEncryption.DecryptString(reportpassord.Value);// ConfigurationManager.AppSettings["PASSWORD"];
                return new NetworkCredential(username, password);
            }
        }
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {

           
            authCookie = null;
            user = "";
            password = "";
            authority = "";
            return false;
            //var reportusername = setting.GetByKey(SettingsKeys.ReportServerUsername);
            //if (reportusername != null)
            //    user = reportusername.Value;
            //var reportpassord = setting.GetByKey(SettingsKeys.ReportServerPassword);
            //if (reportpassord != null)
            //    password = VCEncryption.DecryptString(reportpassord.Value);// ConfigurationManager.AppSettings["PASSWORD"];
            //var reportserver = setting.GetByKey(SettingsKeys.ReportServerUrl);
            //if (reportserver != null)
            //{
            //    Uri uri = null;
            //    Uri.TryCreate(reportserver.Value, UriKind.Absolute, out uri);
            //    if (uri != null)
            //    {
            //        authority = uri.Authority; // ConfigurationManager.AppSettings["SERVER_NAME"];
            //    }
            //}
            //return true;
        }
    }
}