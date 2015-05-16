using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Utility;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.Services.Service.Utility;
using Microsoft.Reporting.WinForms;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.Reports
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : Page
    {
        public ReportView()
        {
            InitializeComponent();
            this.Loaded += ReportView_Loaded;
        }

        void ReportView_Loaded(object sender, RoutedEventArgs e)
        {
            string uri = NavigationService.CurrentSource.ToString();
            var report =uri.Substring(uri.IndexOf("?")+1);
            ShowsReport(report);
        }

        private void ShowsReport(string report)
        {
            switch (report)
            {
                case "CommodityPurchaseSummary":
                    RenderCommodityPurchaseSummary("terst",report);
                    break;
                default:
                    MessageBox.Show("Unknown Report");
                    break;
                    
            }
        }

        private void RenderCommodityPurchaseSummary(string report)
        {
            
        }
        void RenderCommodityPurchaseSummary(string title,string reportPath)
        {
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            var reportViewer = new ReportViewer();
            //reportViewer.ProcessingMode = ProcessingMode.Remote;
            //reportViewer.ServerReport.ReportPath = reportPath;// "/DistributrReports/Losses";
            //string url = "";
            //string password = "";
            //string username = "";
            //using (var c = ObjectFactory.Container.GetNestedContainer())
            //{
            //    var settingrepo = c.GetInstance<IGeneralSettingRepository>();
            //    GeneralSetting passsetting = settingrepo.GetByKey(GeneralSettingKey.ReportPassword);
            //    if (passsetting != null) password = VCEncryption.DecryptString(passsetting.SettingValue);
            //    GeneralSetting usersetting = settingrepo.GetByKey(GeneralSettingKey.ReportUsername);
            //    if (usersetting != null) username = usersetting.SettingValue;
            //    GeneralSetting reportUrl = settingrepo.GetByKey(GeneralSettingKey.ReportUrl);
            //    if (reportUrl != null) url = reportUrl.SettingValue;

            //}
            //reportViewer.ServerReport.ReportServerUrl = new Uri(url);
            //reportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new NetworkCredential(username, password);
            windowsFormsHost.Child = reportViewer;


            TabItem tab = new TabItem();
            tab.Header = title;
            Style style = this.FindResource("ClosableTabItemStyle") as Style;
            tab.Style = style;

            

            tab.Content = windowsFormsHost;
            int i = ReportTab.Items.Add(tab);
            ReportTab.SelectedIndex = i;
            reportViewer.RefreshReport();
        }

    }
}
