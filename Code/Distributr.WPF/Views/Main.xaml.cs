using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.ViewModels.MainPage;
using Distributr.WPF.UI.Views.LoginViews;
using Distributr.WPF.UI.Views.ReconcileGenerics;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views
{
    public partial class Main : Window
    {
        private MainPageViewModel _vm;
        public Main()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Main_Loaded);
            this.Closing += Main_Closing;
            _vm = DataContext as MainPageViewModel;
            Messenger.Default.Register<Uri>(this, "NavigationRequest", (uri) =>
                                                                           {
                                                                               this.ContentFrame.Navigate(uri);
                                                                           });
            Messenger.Default.Register<bool[]>(this, "LoginMessage", (x) =>
                                                                         {
                                                                             _vm.IsLoggedIn = x[0];
                                                                             if (x[1])
                                                                                 SetProductInfo();
                                                                         });
            this.ContentFrame.Navigating += ContentFrame_Navigating;

        }

        void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            bool canAccess = CheckAccessPermission(e.Uri.OriginalString);
            string line1 = "                          Access Denied !!!!";
            string line2 = "                          tYou dont have permission access this module !!!!";

            if (!canAccess)
            {
                MessageBox.Show("\n \tAccess Denied !!!! \n \tYou dont have permission access this module.\n \tPlease Contact the Administrator", "User Permissions");

                e.Cancel = true;
                return;
            }
        }
        public  bool CheckAccessPermission(string originalString)
        {
            bool canAccess = true;

            string end = originalString;
            if (originalString.Contains("?"))
            {
                end = originalString.Substring(0, originalString.LastIndexOf('?'));
            }

            string uri = end.Substring(end.LastIndexOf('/')).ToLower();
            switch (uri)
            {
                case "/createorder.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreateOrder);
                    break;
                case "/editianview.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleAdjustInventory);
                    break;
                case "/createpurchaseorder.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreatePurchaseOrder);
                    break;
                case "/addpos.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreatePOSSale);
                    break;
                case "/edititnview.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleIssueInventory);
                    break;
                case "/editcontact.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreateContacts);
                    break;
                    
                case "/listcontacts.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewContacts);
                    break;

                case "/listpossales.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewPOS);
                    break;

                case "/recievereturnable.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleReceiveReturnables);
                    break;

                case "/retunslist.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewReturnsList);
                    break;

                case "/listgrn.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewInventory);
                    break;

                case "/listgenericinventory.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleReconcileGenericReturnables);
                    break;

                case "/createstockistorder.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreateStockistOrder);
                    break;

                case "/stockistpurchaseorderlisting.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewStockistOrder);
                    break;

                case "/purchaseorderlisting.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewPurchaseOrder);
                    break;

                case "/salesmanorderslisting.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewOrder);
                    break;

                case "/orderdispatch.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleDispatchOrder);
                    break;

                case "/listpendingpayment.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewOutstandingPayments);
                    break;

                case "/listinvoices.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleIssueCreditNote);
                    break;

                case "/underbankinglist.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewUnderbankinglist);
                    break;

                case "/listroutes.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewRoutes);
                    break;

                case "/editroute.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreateRoutes);
                    break;

                case "/listoutlets.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewOutlet);
                    break;

                case "/editoutlet.xaml":
                    canAccess = _vm.CanAccess( UserRole.RoleAddOutlet);
                    break;

                case "/listusers.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewUser);
                    break;

                case "/edituser.xaml":
                    canAccess = _vm.CanAccess( UserRole.RoleAddUser );
                    break;

                case "/addsalesmanroutemodal.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreateSalesmanRoute);
                    break;

                case "/userroutes.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewSalesmanRoute);
                    break;

                case "/dispatchproducts.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleDispatchProducts);
                    break;

                case "/editoutletvisit.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleCreateOutletVisitDays);
                    break;

                case "/editoutletpriority.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewOutletPriority);
                    break;


                case "/editoutlettargets.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewOutletTargets);
                    break;

                case "/editsalesmantargets.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleViewSalesManTarget);
                    break;
                      case "/importsalesmaninventoryview.xaml":
                    canAccess = _vm.CanAccess(UserRole.RoleImportSalesmanInventory);
                    break;
                    



                    
                default:
                    break;
            }
            return canAccess;
        }
        private void Main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this application?"
                                , "Distributr: Exit Application", MessageBoxButton.YesNo) == MessageBoxResult.No)

                e.Cancel = true;
            else
                Application.Current.Shutdown();
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeControls();
            OtherUtilities.ContentFrameHeight = ContentBorder.Height;
            OtherUtilities.ContentFrameWidth = colMainContent.ActualWidth;
        }

        private void ResizeControls()
        {
            double mainCanvasHeight = mainCanvas.ActualHeight;
            //double canvasTop = gridMainContent.GetValue(Canvas.TopProperty);
            double canvasTop = Canvas.GetTop(gridMainContent);
            double canvasBottom = Canvas.GetBottom(gridMainContent);
            double finalHeight = mainCanvasHeight - (canvasTop + canvasBottom);
            gridMainContent.Height = finalHeight;

            double breadCrumbH = lblWhereAt.ActualHeight;
            double contentHeight = gridMainContent.Height - breadCrumbH;
            double navMenuH = gridMainContent.Height - bdQuickLinks.Height;
            contentHeight -= 20;
            ContentBorder.Height = contentHeight;
            bdLeftNavMenu.Height = navMenuH;
        }

        private void SetProductInfo()
        {
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                IConfigService configService = container.GetInstance<IConfigService>();
                //Config conf = _configService.Load();
                string version = "Version: " + ParseVersionNumber(Assembly.GetExecutingAssembly()).ToString();
                string product = "Product: ";
                string copyright = "Copy Right: ";
                // Get all Product attributes on this assembly
                object[] prodAttributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                object[] coprAttributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Product attributes, return an empty string
                // If there is a Product attribute, return its value
                string ass = ((AssemblyProductAttribute)prodAttributes[0]).Product;
                product += ass;
                string copr = ((AssemblyCopyrightAttribute)coprAttributes[0]).Copyright;
                copyright += copr;
                // Config agr = configService.LoadAgrimangr();
                Config c = configService.Load();
                //lblWCFRemoteAdd.Content = "Server: " + _configService.WCFRemoteAddress.Remove(0, 7).Split('/').First();//(_configService.WCFRemoteAddress.LastIndexOf('/'));
                string server = "Server: " + c.WebServiceUrl;

                string appstatus = "App Status : ";
                if (c.ApplicationStatus == 1)
                    appstatus += "Active";
                else if (c.ApplicationStatus == 3)
                    appstatus += "Locked";
                else
                    appstatus += "Inactive";

                lblProductInfo.Content = product + "\t::\t" + version + "\t::\t" + server + "\t::\t" + appstatus +
                                         "\t::\t" + copr;
                //lblappstatus.Content = appstatus;}
            }
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var uri = e.Uri == null ? e.Content : e.Uri.OriginalString;
            _vm.SetBreadCrumb(uri.ToString());
        }

        private void hlVMenu_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;

            _vm.Navigate(hl.NavigateUri.OriginalString);
        }



        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            tvi.IsExpanded = !tvi.IsExpanded;
            ToggleTreeViewItems(sender as TreeView, tvi);
        }

        private void ToggleTreeViewItems(TreeView tv, TreeViewItem selected)
        {
            foreach (var item in tv.Items)
            {
                TreeViewItem tvi = (TreeViewItem)(tv.ItemContainerGenerator.ContainerFromIndex(tv.Items.IndexOf(item)));
                if (!tvi.IsSelected)
                    tvi.IsExpanded = false;
            }
        }

       
    }
}
