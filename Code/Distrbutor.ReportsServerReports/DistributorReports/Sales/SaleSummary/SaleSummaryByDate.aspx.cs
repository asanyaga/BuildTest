using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace Distrbutor.ReportsServerReports.DistributorReports.Sales.SaleSummary
{
    public partial class SaleSummaryByDate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                    ReportViewer1.ServerReport.ReportServerUrl = new Uri("http://10.0.0.19/reportserver");
                    ReportViewer1.ServerReport.ReportPath = "/DistributrReports/Sales summary by date";
                    ReportViewer1.ServerReport.Refresh();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
              



        }
    }
}