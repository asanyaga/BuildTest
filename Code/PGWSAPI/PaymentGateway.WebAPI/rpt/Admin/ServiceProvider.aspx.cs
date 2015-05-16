using System;
using System.Reflection;
using Microsoft.Reporting.WebForms;
using PaymentGateway.WSApi.Lib.Report;
using PaymentGateway.WSApi.Lib.Report.Services;
using StructureMap;

namespace PaymentGateway.WebAPI.rpt.Admin
{
    public partial class ServiceProvider : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonView_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            DateTime EndDate = DateTime.Now;
            DateTime StartDate = DateTime.Now.AddDays(-2);
         
            DateTime.TryParse(TextBoxFrom.Text, out EndDate);
            DateTime.TryParse(TextBoxTo.Text, out StartDate);
            IReportService _reportService = ObjectFactory.GetInstance<IReportService>();
            ReportViewer1.Reset();
            ReportViewer1.LocalReport.DataSources.Clear();
            ReportViewer1.ProcessingMode = ProcessingMode.Local;
            ReportViewer1.LocalReport.EnableHyperlinks = true;

            var stream = Assembly.GetAssembly(typeof(ReportBase)).GetManifestResourceStream(ReportCollective.ServiceProviderReport);
            ReportDataSource reportDataSource = new ReportDataSource("ServiceProviderReportDataSet", _reportService.GetServiceProviderReport(StartDate, EndDate));
            ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
            ReportViewer1.LocalReport.LoadReportDefinition(stream);

            ReportViewer1.LocalReport.DisplayName = "Service Provider Report";
            ReportViewer1.LocalReport.Refresh();
        }
    }
}