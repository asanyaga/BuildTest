using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace Distrbutor.ReportsServerReports.DistributorReports
{
    public partial class Sample : System.Web.UI.Page
    {
         string reportNames ;
         string reportPath ;
        protected void Page_Load(object sender, EventArgs e)
        {
          
            if (!IsPostBack)
            {
                reportNames = Request.QueryString["reportName"];
                switch (reportNames)
                {
                    case "closeOfDay":
                        reportPath = "/DistributrReports/StockByBrand";
                        break;
                    case "losses":
                        reportPath = "/DistributrReports/losses";
                        break;
                    default:
                        reportPath = "";
                        break;
                }
                ReportViewer1.ProcessingMode = ProcessingMode.Remote;
              //  IReportServerCredentials irsc = new re ;
                //ReportViewer1.ServerReport.ReportServerCredentials
                ReportViewer1.ServerReport.ReportServerUrl = new Uri("http://10.0.0.19/reportserver/");
                ReportViewer1.ServerReport.ReportPath = reportPath;
                ReportViewer1.ServerReport.Refresh();


            }

        }
    }
}