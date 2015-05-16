using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Agrimanagr.WPF.UI.Views.Admin;
using Agrimanagr.WPF.UI.Views.Settings;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.MainViews
{
    public partial class MainPage : Page
    {
        public static string ShowDeliveredBy;
        public static string ShowTransporter;
        private static string _hubName = "";

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            
            Messenger.Default.Register<bool>(this, "UpdateOnlineStatusIndicator", UpdateOnlineStatus);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            CommodityHome commodityHome = new CommodityHome();
            ctrlCommodity.Content = commodityHome;

            SetProductInfo();
        }

        private void hlkLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult boxResult = MessageBox.Show("Do you want to log out", "Log Out", MessageBoxButton.YesNo,
                                                         MessageBoxImage.Information);
            if (boxResult == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new LoginPage());
            }
            else return;

        }

        void SetProductInfo()
        {
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                IConfigService configService = container.GetInstance<IConfigService>();
                string version = "Version No.: " + ParseVersionNumber(Assembly.GetExecutingAssembly()).ToString();
                string product = "Product Name: ";
                string copyright = "Copy Right : ";
                object[] prodAttributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                object[] coprAttributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                string ass = ((AssemblyProductAttribute)prodAttributes[0]).Product;
                product += ass;
                string copr = ((AssemblyCopyrightAttribute)coprAttributes[0]).Copyright;
                copyright += copr;
                Config c = configService.Load();
                string server = "Web Service : " + c.WebServiceUrl;

                string appstatus = "App Status : ";
                if (c.ApplicationStatus == 1)
                    appstatus += "Active";
                else if (c.ApplicationStatus == 3)
                    appstatus += "Locked";
                else
                    appstatus += "Inactive";
                lblUserLogin.Content = configService.ViewModelParameters.CurrentUsername;
                lblLoginDate.Content = DateTime.Now.ToShortDateString(); //todo:get logindate from WSAPI.
                var distributrId = configService.Load().CostCentreId;
                var dist = container.GetInstance<ICostCentreRepository>().GetById(distributrId);
                if (dist != null)
                {
                    _hubName = dist.Name;

                }
                lblBoxHubName.Content += _hubName;

                TxtProduct.Text = product;
                TxtVersion.Text = version;
                TxtWebService.Text = server;
                TxtAppStatus.Text = appstatus;

            }
        }

        void UpdateOnlineStatus(bool isOnline)
        {
            //avoid setting same val
            if (lblStatus.Content.ToString() == "Online" && isOnline) return;
            if (lblStatus.Content.ToString() == "Offline" && !isOnline) return;
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            if (isOnline)
                            {
                                lblStatus.Content = "Online";
                                imgStatus.Source =
                                    new BitmapImage(new Uri("../../Resources/images/online.png", UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                lblStatus.Content = "Offline";
                                imgStatus.Source =
                                    new BitmapImage(new Uri("../../Resources/images/offline.png", UriKind.RelativeOrAbsolute));
                            }
                        }));
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        private void ctrlAdmin_Navigating_1(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (((Frame)sender).Source.OriginalString != "../Admin/AdminHome.xaml")
                e.Cancel = true;
        }

        private void TcSettings_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;
            if (tcSettings.SelectedIndex == tcSettings.Items.IndexOf(tabSyncSettings))
            {
                ccSyncSettings.Content = new AgriSyncSettings();
            }
            if (tcSettings.SelectedIndex == tcSettings.Items.IndexOf(tabGeneralSettings))
            {
                ccGeneralSettings.Content = new AgriGeneralSettings();
            }
        }
    }
}
