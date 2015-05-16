using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.DispatchPendingOrdersToPhone
{
    public partial class DispatchPendingOrdersToPhone : PageBase
    {
        private IConfigService _configService;
        private ListSalesmanOrdersViewModel _vm;
        private DistributrMessageBox _distributrMessageBox;
        private DistributrMessageBoxViewModel _distributrMessageBoxViewModel;
        bool isInitialized = false;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool reloadListOnSalesmanChanged = false;
        public DispatchPendingOrdersToPhone()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            Loaded += new RoutedEventHandler(DispatchPendingOrdersToPhone_Loaded);
            chkAssignOverallRecipient.IsChecked = false;

            double newLayoutRootWidth = OtherUtilities.ContentFrameWidth;
            newLayoutRootWidth = (newLayoutRootWidth * 0.90);

            double newLayoutRootHeight = OtherUtilities.ContentFrameHeight;
            newLayoutRootHeight = (newLayoutRootHeight * 0.90);

            double tabControlHeight = (newLayoutRootHeight * 0.825);

            LayoutRoot.Width = newLayoutRootWidth;
            //LayoutRoot.Height = newLayoutRootHeight;
            _configService = ObjectFactory.GetInstance<IConfigService>();

            LabelControls();
        }

        void LabelControls()
        {
            lblSalesman.Content = _messageResolver.GetText("sl.dispatchOrders.salesman_lbl");
            lblRoute.Content = _messageResolver.GetText("sl.dispatchOrders.route");
            chkSelectAll.Content = _messageResolver.GetText("sl.dispatchorders.selectall");
            lblLegend.Content = _messageResolver.GetText("sl.dispatchOrders.legend");
            btnDispatch.Content = _messageResolver.GetText("sl.dispatchOrders.dispatch");
            btnBack.Content = _messageResolver.GetText("sl.dispatchOrders.back");

            //grid
            colOrderRef.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.orderref");
            colDateRequired.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.required");
            colSalesman.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.salesman");
            colStatus.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.status");
            colTotalNet.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.net");
            colVat.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.vat");
            colTotalGross.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.gross");
            colDispatch.Header = _messageResolver.GetText("sl.dispatchOrders.grid.col.select");
        }

        void DispatchPendingOrdersToPhone_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.SetupForDispatchCommand.Execute(null);
            reloadListOnSalesmanChanged = false;

            if (!_configService.ViewModelParameters.CurrentUserRights.CanDispatchOrder)
                btnDispatch.Visibility = Visibility.Collapsed;
        }

        private void cmbSalesmen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm = DataContext as ListSalesmanOrdersViewModel;
            if (_vm.SelectedSalesman == null)
                return;
            if (_vm.SelectedSalesman.Id != Guid.Empty)
                reloadListOnSalesmanChanged = true;
            if (reloadListOnSalesmanChanged)
            {
                _vm.LoadSalesmansPendingOrdersForDispatchCommand.Execute(null);
                dgOrders.ItemsSource = _vm.Orders;
            }
        }

        private void btnDispatch_Click_Revised(object sender, RoutedEventArgs e)
        {

            if ((from ListSalesmanOrdersViewModel.ListOrderViewModelItem item in _vm.Orders
                 where item.Dispatch
                 select item).ToList().Any())
            {
                try
                {
                    if (MessageBox.Show(_messageResolver.GetText("sl.dispatchOrders.dispatch.messagebox.prompt")/*"Dispatch selected orders?"*/
                        , _messageResolver.GetText("sl.dispatchOrders.dispatch.messagebox.caption")/* "Distributr: Dispatch Orders"*/
                        , MessageBoxButton.OKCancel)
                        == MessageBoxResult.OK)
                    {
                        //check for back orders
                        _vm.RunValidateBackOrdersForDispatch();
                        if (_vm.OrdersWithBackOrder.Any())
                        {
                            var msg = CreateSummary();

                            CreateDispatchRadioOptions(msg);
                        }
                        else
                        {
                            JustDispatch();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(_messageResolver.GetText("sl.dispatchOrders.dispatch.error.messagebox.text1")/*"Orders were not successfully dispatched."*/
                        + "\n " +
                        _messageResolver.GetText("sl.dispatchOrders.dispatch.error.messagebox.text2")/*"Error details"*/
                        + ":\n" + ex.Message
                        , "!" + _messageResolver.GetText("sl.dispatchOrders.dispatch.messagebox.caption")/* "Distributr: Dispatch Orders"*/
                        , MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(
                    _messageResolver.GetText("sl.dispatchOrders.dispatch.ordernotselected.messagebox.prompt")/*"You have not selected any order to dispatch."*/
                    , _messageResolver.GetText("sl.dispatchOrders.dispatch.messagebox.caption")/* "Distributr: Dispatch Orders"*/
                    , MessageBoxButton.OK);
            }
        }

        void CreateDispatchRadioOptions(string summary)
        {
            var dispatchOptions = new DispatchOptions();
            dispatchOptions.txtSummary.Text = summary;
            dispatchOptions.Closed += new EventHandler(dispatchOptions_Closed);
            dispatchOptions.ShowDialog();
        }

        void dispatchOptions_Closed(object sender, EventArgs e)
        {
            var dispatchOptions = sender as DispatchOptions;
            _vm = this.DataContext as ListSalesmanOrdersViewModel;
            if ((bool)dispatchOptions.DialogResult)
            {
                _vm.SelectedDispatchMode = dispatchOptions.SelectedDispatchOption;
                this.Cursor = Cursors.Wait;
                _vm.DispatchCommand.Execute(null);
                this.Cursor = Cursors.Arrow;

                if (!_vm.ProcessBackOrder)
                    PostDispatchMessageBox();
            }
        }

        string CreateSummary()
        {
            string backOrders = "";
            string fulfilledOrders = "";
            string fulfillableBO = "";
            string unfulfillableBO = "";
            string partiallyDispatchable = "";
            string msg = "";

            if (_vm.FulfilledOrders.Count() > 0)
            {

                msg += _messageResolver.GetText("sl.dispatchOrders.summary.fulfilled")/*"The following orders have been fulfilled and can be dispatched."*/
                    + "\n";
                fulfilledOrders = _vm.FulfilledOrders.Aggregate(fulfilledOrders,
                                                                (current, item) =>
                                                                current +
                                                                ("\t-" + item.DocumentRef + ".\n"));
                msg += fulfilledOrders + "\n";
            }
            if (_vm.OrdersWithBackOrder.Count > 0)
            {
                msg += _messageResolver.GetText("sl.dispatchOrders.summary.havebackorder")/*"The following orders have back order."*/
                    + "\n";
                backOrders = _vm.OrdersWithBackOrder.Aggregate(backOrders,
                                                               (current, item) =>
                                                               current + ("\t-" + item.DocumentRef + ".\n"));
                msg += backOrders + "\n";
            }
            if (_vm.FulfillableOrdersWithBackOrder.Count() > 0)
            {
                msg += _messageResolver.GetText("sl.dispatchOrders.summary.canbeProcessedandDelivered")/*"The following orders can be processed and delivered."*/
                    + "\n";
                fulfillableBO = _vm.FulfillableOrdersWithBackOrder.Aggregate(fulfillableBO,
                                    (current, item) =>
                                    current + ("\t-" + item.DocumentRef + ".\n"));

                msg += fulfillableBO;
            }

            if (_vm.UnFulfillableOrdersWithBackOrder.Count > 0)
            {
                msg += "\n" +
                    _messageResolver.GetText("sl.dispatchOrders.summary.cannotbeFulfilled")/*"The following orders cannot be fulfilled."*/
                    + "\n";
                unfulfillableBO += _vm.UnFulfillableOrdersWithBackOrder.Aggregate(unfulfillableBO,
                                    (current, item) =>
                                    current + ("\t-" + item.DocumentRef + ".\n"));
                msg += unfulfillableBO;
            }

            if (_vm.PartiallyDispatchableOrders.Count > 0)
            {
                msg += "\n" +
                    _messageResolver.GetText("sl.dispatchOrders.summary.canbePartiallDispatched")/*"The following orders can be partially dispatched."*/
                    + "\n";
                partiallyDispatchable += _vm.PartiallyDispatchableOrders.Aggregate(partiallyDispatchable,
                                    (current, item) =>
                                    current + ("\t-" + item.DocumentRef + ".\n"));
                msg += partiallyDispatchable;
            }

            return msg;
        }

        void JustDispatch()
        {
            this.Cursor = Cursors.Wait;
            _vm.DispatchAnyway = true;
            _vm.DispatchCommand.Execute(null);
            dgOrders.ItemsSource = _vm.Orders;
            this.Cursor = Cursors.Arrow;

            PostDispatchMessageBox();
        }

        void PostDispatchMessageBox()
        {
            string msg = "";
            string ordersThatCntBeDispatched = "";
            string post_dispatch_msg = "";
            //new, home, ok, cancel, action1
            if (_vm != null)
                _vm = this.DataContext as ListSalesmanOrdersViewModel;

            post_dispatch_msg = _vm.dispatchCount + " "
                + _messageResolver.GetText("sl.dispatchOrders.dispatch.sucess.messagebox.text")/*"order(s) were dispatched successfully."*/
                + "\n\n";
            if (_vm.OrdersThatCannotBeDispatched != null)
            {
                if (_vm.OrdersThatCannotBeDispatched.Count > 0)
                {
                    msg +=
                        _messageResolver.GetText("sl.dispatchOrders.dispatch.fail.messagebox.text1")/*"The following"*/
                        + " " + ordersThatCntBeDispatched.Count() + " " +
                        _messageResolver.GetText("sl.dispatchOrders.dispatch.fail.messagebox.text2")
                        /*"orders could not be dispatched because insuficient inventory on all of the order items."*/
                        + "\n";
                    ordersThatCntBeDispatched = _vm.OrdersThatCannotBeDispatched.Aggregate(ordersThatCntBeDispatched,
                                                                                           (current, order) =>
                                                                                           current +
                                                                                           ("\t" +
                                                                                            order.DocumentReference +
                                                                                            "\n"));

                    msg += ordersThatCntBeDispatched;
                }
            }

            bool showDispatchOption = _vm.Orders.Count() > 0;

            string btnCont = _messageResolver.GetText("sl.dispatchOrders.dispatch.sucess.messagebox.option.continue")/*"Continue Dispatching Orders"*/;
            string btnApprove = _messageResolver.GetText("sl.dispatchOrders.dispatch.sucess.messagebox.option.approve")/*"Approve Orders"*/;
            string btnView = _messageResolver.GetText("sl.dispatchOrders.dispatch.sucess.messagebox.option.view")/*"View Dispatched Orders"*/;
            _distributrMessageBox = new DistributrMessageBox(showDispatchOption, true, true, false, false, btnCont,
                                                             btnApprove, btnView);
            _distributrMessageBoxViewModel = _distributrMessageBox.DataContext as DistributrMessageBoxViewModel;
            _distributrMessageBox.Closed += new EventHandler(_distributrMessageBoxRevisedPostDispatch_Closed);
            _distributrMessageBoxViewModel.MessageBoxTitle = _messageResolver.GetText("sl.dispatchOrders.dispatch.messagebox.caption")/* "Distributr: Dispatch Orders"*/;
            _distributrMessageBoxViewModel.NewButtonToolTip =
                "Continue dispatching orders";
            _distributrMessageBoxViewModel.HomeButtonTooTip =
                "Go to orders pending approval list and approve orders";
            _distributrMessageBoxViewModel.OKButtonToolTip =
                "Go to list of dispatched orders.";

            post_dispatch_msg += msg;
            _distributrMessageBoxViewModel.MessageBoxContent = post_dispatch_msg;
            _distributrMessageBox.ShowDialog();
        }

        void _distributrMessageBoxRevisedPostDispatch_Closed(object sender, EventArgs e)
        {

            //new, home, ok, cancel, action1
            _distributrMessageBoxViewModel = _distributrMessageBox.DataContext as DistributrMessageBoxViewModel;
            switch (_distributrMessageBoxViewModel.Command)
            {
                case DistributrMessageBoxViewModel.CommandToExcecute.NewButtonClickedCommand: //Continue Dispatching Orders
                    return;
                    break;
                case DistributrMessageBoxViewModel.CommandToExcecute.HomeButtonClickedCommand: //Approve Orders
                    string uri = "views/salesmanorders/listsalesmanorders.xaml?PendingApprovals";
                    NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                    break;
                case DistributrMessageBoxViewModel.CommandToExcecute.OKButtonClickedCommand: //View Dispatched Orders
                    string uri2 = "views/salesmanorders/listsalesmanorders.xaml?DispatchedOrders";
                    NavigationService.Navigate(new Uri(uri2, UriKind.Relative));
                    break;
            }
            if (!_distributrMessageBoxViewModel.DialogResult) //close box??
                return; //do a lot of nothing
        }

        private void chkAssignOverallRecipient_Checked(object sender, RoutedEventArgs e)
        {
            cmbOverallRecipient.IsEnabled = chkAssignOverallRecipient.IsChecked != false;
            _vm.SetRecipientCommand.Execute(null);
            dgOrders.ItemsSource = _vm.Orders;
        }

        private void chkAssignOverallRecipient_Unchecked(object sender, RoutedEventArgs e)
        {
            cmbOverallRecipient.IsEnabled = false;
            cmbOverallRecipient.SelectedIndex = 0;
            _vm.UnsetRecipientCommand.Execute(null);
            dgOrders.ItemsSource = _vm.Orders;
        }

        private void cmbOverallRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            chkAssignOverallRecipient_Checked(this, new RoutedEventArgs());
        }

        private void btnResetReciptient_Click(object sender, RoutedEventArgs e)
        {
            if (chkAssignOverallRecipient.IsChecked == true)
                chkAssignOverallRecipient.IsChecked = false;
            else
                chkAssignOverallRecipient_Unchecked(this, new RoutedEventArgs());
        }

        private void cmbRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm.RouteChangedCommand.Execute(null);
            reloadListOnSalesmanChanged = false;
            _vm.LoadSalesmansPendingOrdersForDispatchCommand.Execute(null);
            dgOrders.ItemsSource = _vm.Orders;
        }

        private void chkSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.SelectAllCommand.Execute(null);
            dgOrders.ItemsSource = _vm.Orders;
        }

        private void chkSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.UnSelectAllCommand.Execute(null);
            dgOrders.ItemsSource = _vm.Orders;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void chkDispact_Checked(object sender, RoutedEventArgs e)
        {
            var item = ((ListSalesmanOrdersViewModel.ListOrderViewModelItem)dgOrders.SelectedItem);
            var item2 = (sender as FrameworkElement).DataContext as ListSalesmanOrdersViewModel.ListOrderViewModelItem;
            if (!_vm.RunOrderIsSelected(item2))
                (sender as CheckBox).IsChecked = false;
        }

        private void chkDispact_Unchecked(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as ListSalesmanOrdersViewModel.ListOrderViewModelItem;
            _vm.RunOrderIsUnSelected(item);
        }
    }
}
