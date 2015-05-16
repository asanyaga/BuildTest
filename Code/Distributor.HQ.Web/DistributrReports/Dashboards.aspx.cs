using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Agrimanagr.HQ.Models;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Microsoft.Reporting.WebForms;
using StructureMap;

namespace Distributr.HQ.Web.DistributrReports
{
    public partial class Dashboards : System.Web.UI.Page
    {
        string reportName;
        string reportPath;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                reportName = Request.QueryString["reportName"];
                switch (reportName)
                {
                    case "Dashboard":
                        reportPath = "/DistributrReports/DB_Home";
                        break;


                    default:
                        reportPath = "";
                        break;
                }
                string server = "";
                string folder = "";
                var settings = ObjectFactory.GetInstance<ISettingsRepository>();
                var reportServer = settings.GetByKey(SettingsKeys.ReportServerUrl);
                var reportFolder = settings.GetByKey(SettingsKeys.ReportServerFolder);
                if (reportFolder != null)
                {
                    folder = "/" + reportFolder.Value;
                }
                if (reportServer != null) server = reportServer.Value;

                ReportViewer2.ProcessingMode = ProcessingMode.Remote;
                ReportViewer2.ServerReport.ReportServerUrl = new Uri(server);
                ReportViewer2.ServerReport.ReportPath = folder + reportPath;
                ReportViewer2.ServerReport.ReportServerCredentials = new DistributrReportServerCredentials();
                ReportViewer2.ServerReport.Refresh();


            }
        }
    }
}