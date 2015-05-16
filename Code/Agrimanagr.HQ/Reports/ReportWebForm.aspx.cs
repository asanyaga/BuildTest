using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Agrimanagr.HQ.Models;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Microsoft.Reporting.WebForms;
using StructureMap;

namespace Agrimanagr.HQ.Reports
{
    public partial class ReportWebForm : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            string server = "";
             string folder = "";
            var setting = ObjectFactory.GetInstance<ISettingsRepository>();
            var reportserver = setting.GetByKey(SettingsKeys.ReportServerUrl);
            if (reportserver != null)
                server = reportserver.Value;
            var reportfolder = setting.GetByKey(SettingsKeys.ReportServerFolder);
            if (reportfolder != null)
                folder = "/" + reportfolder.Value;
            
            string uri = Request.QueryString["reporturi"];
            ReportViewer1.ServerReport.ReportServerUrl = new Uri(server);
            ReportViewer1.ProcessingMode = ProcessingMode.Remote;
            ReportViewer1.ServerReport.ReportPath = folder+uri;
            ReportViewer1.ServerReport.ReportServerCredentials =  new AgrimanagrReportServerCredentials();
            ReportViewer1.ServerReport.Refresh();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}