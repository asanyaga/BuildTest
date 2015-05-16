using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using ValidationResult = System.Windows.Controls.ValidationResult;

namespace Distributr.WPF.UI.Views.SalesmanOrders
{
    public partial class EditSalesmanOrder : PageBase
    {
        private EditSalesmanOrderViewModel _vm;
        private DistributrMessageBoxViewModel _distMsgBxVm;
        private DistributrMessageBox _distMsgBx;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool isInitialized = false;

        public EditSalesmanOrder()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            this.Loaded += new RoutedEventHandler(EditSalesman_Loaded);
            
            LabelControls();
        }

        void LabelControls()
        {
            lblOrderId.Content = _messageResolver.GetText("sl.createOrder.saleid_lbl");
            lblDateRequired.Content = _messageResolver.GetText("sl.createOrder.date_lbl");
            lblSalesman.Content = _messageResolver.GetText("sl.createOrder.salesman_lbl");
            lblRoute.Content = _messageResolver.GetText("sl.createOrder.route_lbl");
            lblOutlet.Content = _messageResolver.GetText("sl.createOrder.outlet_lbl");
            lblTotalNet.Content = _messageResolver.GetText("sl.createOrder.totalNet_lbl");
            lblTotaProductDiscount.Content = _messageResolver.GetText("sl.createOrder.totalProductDiscount_lbl");
            lblTotalVat.Content = _messageResolver.GetText("sl.createOrder.totalVat_lbl");
            lblSaleDiscount.Content = _messageResolver.GetText("sl.createOrder.saleDiscount_lbl");
            lblTotalGross.Content = _messageResolver.GetText("sl.createOrder.totalGross_lbl");
            lblStatus.Content = _messageResolver.GetText("sl.createOrder.status_lbl");

            btnAddLineItem.Content = _messageResolver.GetText("sl.createOrder.addProduct_btn");
            btnSaveOrder.Content = _messageResolver.GetText("sl.createOrder.saveAndContinueLater_btn");
            btnConfirmOrder.Content = _messageResolver.GetText("sl.createOrder.completeSale_btn");
            btnGoHome.Content = _messageResolver.GetText("sl.createOrder.cancelOrder");//cn: Cancel Order
            btnViewList.Content = _messageResolver.GetText("sl.createOrder.viewList");

            //cn: datagrid
            colProduct.Header = _messageResolver.GetText("sl.createOrder.grid.col.product");
            colQty.Header = _messageResolver.GetText("sl.createOrder.grid.col.qty");
            colUnitPrice.Header = _messageResolver.GetText("sl.createOrder.grid.col.unitprice");
            colUnitDisc.Header = _messageResolver.GetText("sl.createOrder.grid.col.unitdiscount");
            colNetAmt.Header = _messageResolver.GetText("sl.createOrder.grid.col.netamount");
            colUnitVat.Header = _messageResolver.GetText("sl.createOrder.grid.col.unitvat");
            colTotalVat.Header = _messageResolver.GetText("sl.createOrder.grid.col.totalvat");
            colGrossAmt.Header = _messageResolver.GetText("sl.createOrder.grid.col.grossamount");
            colProductType.Header = _messageResolver.GetText("sl.createOrder.grid.col.producttype");
        }

        void EditSalesman_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
            cmbRoutes.SelectedItem = _vm.SelectedRoute;
            _vm.FireSalesmanChangedCmd = true;
            _vm.FireRouteChangedCmd = true;
            _vm.FireOutletChangeCmd = true;
        }

        private void Load()
        {
            _vm = this.DataContext as EditSalesmanOrderViewModel;
            _vm.RunClearAndSetup();
            string _orderId = NavigationService.Source.OriginalString.ParseQueryString("orderid");
            if (!string.IsNullOrEmpty( _orderId))
            {
                _vm.OrderIdLookup = new Guid(_orderId);
            }
            string _loadForViewing = NavigationService.Source.OriginalString.ParseQueryString("loadforviewing");
            if (!string.IsNullOrWhiteSpace(_loadForViewing))
            {
                
                _vm.LoadForViewing = Convert.ToBoolean(_loadForViewing); //always true anyway.
                _vm.PostConfirmVisible = true;
                _vm.LoadForEditing = false;
                _vm.CancelButtonContent = _messageResolver.GetText("sl.createOrder.back");
                _vm.ConfirmNavigatingAway = false;
                lblPageHeader.Content = "";
                btnCancelOrder.Visibility = Visibility.Collapsed;
            }
            else
            {
                _vm.LoadForEditing = true;
                _vm.LoadForViewing = false;
                _vm.PostConfirmVisible = false;
                _vm.CancelButtonContent = _messageResolver.GetText("sl.createOrder.cancel");
                _vm.ConfirmNavigatingAway = true;
                lblPageHeader.Content = _messageResolver.GetText("sl.createOrder.title.new");
            }

            _vm.LoadOrderCommand.Execute(null);

            if (_vm.LoadForEditing)
            {
                if (_vm.LineItems.Count > 0)
                {
                    dtDateRequired.IsEnabled = false;
                    cmbSalesman.IsEnabled = false;
                    cmbRoutes.IsEnabled = false;
                    cmbOutlets.IsEnabled = false;
                }

                if (_vm.OrderIdLookup == Guid.Empty)
                    btnGoHome.Visibility = Visibility.Collapsed;
            }
            else if (_vm.LoadForViewing)
            {
                lblPageHeader.Content = _messageResolver.GetText("sl.createOrder.title.edit.part1")/*"Viewing Order"*/+ " "
                    + _vm.OrderId + " "
                    + _messageResolver.GetText("sl.createOrder.title.edit.part2");/*"Details";*/
            }
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                if (MessageBox.Show(
                    /*"Are you sure you want to move away from this page without completing the order?"*/
                    _messageResolver.GetText("sl.createOrder.navigateAway.messagebox.prompt.part1") + "\n"
                    + _messageResolver.GetText("sl.createOrder.navigateAway.messagebox.prompt.part2")/*"Unsaved changes will be lost"*/
                    , _messageResolver.GetText("sl.createOrder.navigateAway.messagebox.prompt.caption")/*"Distributr: Confirm Navigating Away"*/
                    , MessageBoxButton.YesNo)
                    == MessageBoxResult.No)
                    e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {

            if (_vm.LoadForViewing)
            {
                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri("/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals", UriKind.Relative));
            }
            else
            {
                _vm.ConfirmNavigatingAway = false;
                if (
                    MessageBox.Show(/*"Are you sure you want to cancel this order process? Unsaved changes will be lost."*/
                    _messageResolver.GetText("sl.createOrder.cancel.messagebox")
                    , _messageResolver.GetText("sl.createOrder.navigateAway.messagebox.prompt.caption")/*"Distributr: Confirm Navigating Away"*/
                    , MessageBoxButton.OKCancel)
                    == MessageBoxResult.OK)
                {
                    _vm.CancelCommand.Execute(null);

                    _vm.ConfirmNavigatingAway = false;
                    NavigationService.Navigate(new Uri("/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals", UriKind.Relative));
                }
            }
        }

        private void btnConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.LineItems.Count < 1)
            {
                MessageBox.Show(
                    _messageResolver.GetText("sl.createOrder.confirm.messagebox.nolineItems")/*"The order must have at least 1 line item."*/
                    , "!" + _messageResolver.GetText("sl.createOrder.confirm.messagebox.nolineItems.caption")/*Distributr: Confirm Order*/
                    , MessageBoxButton.OK);
                return;
            }
            if (
                MessageBox.Show(_messageResolver.GetText("sl.createOrder.confirm.messagebox") /*"Confirm order" */
                + " " + _vm.OrderId + "?"
                , _messageResolver.GetText("sl.createOrder.confirm.messagebox.nolineItems.caption")/*Distributr: Confirm Order*/
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.Cursor = Cursors.Wait;
                if (_vm.IsValid())
                {
                    this.Cursor = Cursors.Wait;
                    _vm.showConfirmMsg = false;
                    _vm.ConfirmCommand.Execute(null);
                    _vm.showConfirmMsg = true;
                    this.Cursor = Cursors.Arrow;

                    string newOrder = _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.options.newOrder");/* "New Order";*/
                    string home = _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.options.home");/*"Home Page";*/
                    string viewList = _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.options.viewList");/*"View List";*/

                    _distMsgBxVm = new DistributrMessageBoxViewModel();
                    _distMsgBx = new DistributrMessageBox(true, true, false, false, true, newOrder,
                                                                     home, null, viewList);

                    CompletedActionOptions(
                        _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.text1") /*"Order"*/
                        + " " + _vm.OrderId + " "
                        + _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.text2") /*"on behalf of"*/
                        + " " + _vm.SelectedSalesman.Username + " "
                        + _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.text3")/*"was successfully confirmed."*/
                        );
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        //cn: not in use currently
        private void btnConfirmAndApprove_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.LineItems.Count < 1)
            {
                MessageBox.Show("The order must have at least 1 line item.", "Distributr: Order Line Items.", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show("Confirm and Approve order " + _vm.OrderId + "?", "Distributr: Confirm Action.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (_vm.IsValid())
                {
                    this.Cursor = Cursors.Wait;
                    //_vm.ConfirmAndApproveCommand.Execute(null);
                    //confirm
                    _vm.showConfirmMsg = false;
                    _vm.ConfirmCommand.Execute(null);
                    //validate for approval
                    _vm.ValidateOrderForApprovalCommand.Execute(null);
                    //if valid approve
                    if (_vm.OrderIsValidForApproval)
                    {
                        _vm.ApproveOrderCommand.Execute(null);
                    }
                    else
                    {//cn: mambo mbiad
                        this.Cursor = Cursors.Wait;
                        _vm.CreateInvalidOrdersMessageCommand.Execute(null);

                        this.Cursor = Cursors.Arrow;
                        _distMsgBxVm = new DistributrMessageBoxViewModel();
                        _distMsgBx = new DistributrMessageBox(true, true, false, true, false, "Create Back Order",
                                                              "Edit Order", null, "Cancel");
                        //_distMsgBx.Width = _distMsgBx.MinWidth;
                        //_distMsgBx.Height = _distMsgBx.MinHeight;

                        _distMsgBx.Closed += new EventHandler(_distMsgBx_PreApproval_Closed);
                        _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
                        _distMsgBxVm.MyListUri = "";//"/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals"
                        _distMsgBxVm.NewUriString = ""; //"/views/salesmanorders/listsalesmanorders.xaml?orderid=" + Guid.Empty
                        _distMsgBxVm.MessageBoxContent =
                            "Order cannot be fulfilled.\nThe available inventory cannot satisfy the following order item(s):\n" +
                            _vm.Message +
                            "\nSelect an option to proceed.";

                        _distMsgBxVm.MessageBoxTitle = "Distributr: Approve Order " + _vm.OrderId + " on behalf of " +
                                                       _vm.SelectedSalesman.Username;
                        _distMsgBx.ShowDialog();
                    }
                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?Incomplete";
            _vm.ConfirmNavigatingAway = false;
            if (_vm == null)
                _vm = this.DataContext as EditSalesmanOrderViewModel;
            if (_vm.IsValid())
            {
                this.Cursor = Cursors.Wait;
                _vm.SaveToContinue();
                this.Cursor = Cursors.Arrow;
                CompletedActionOptions(_messageResolver.GetText("sl.createOrder.postconfirm.messagebox.text1") /*"Order"*/
                                       + " " + _vm.OrderId + " "
                                       + _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.text2") /*"on behalf of"*/
                                       + " " + _vm.SelectedSalesman.Username + " "
                                       + _messageResolver.GetText("sl.createOrder.save.messagebox.text3") /*"was successfully saved."*/
                    );
            }
        }

        SOLineItemModal _newLineItemModal = null;
        private void btnAddLineItem_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.IsValid())
            {
                AddNewLineItem();
            }
        }

        void AddNewLineItem()
        {
            _newLineItemModal = new SOLineItemModal();
            _newLineItemModal.Closed += new EventHandler(modal_Closed);
            _newLineItemModal.cmbProducts.IsEnabled = true;
            _newLineItemModal.btnAddProduct.IsEnabled = true;
            SOLineItemViewModel vmLineItem = _newLineItemModal.DataContext as SOLineItemViewModel;
            vmLineItem.Salesman = _vm.SelectedSalesman;
            vmLineItem.SelectedOutletId = _vm.SelectedOutlet.Id;
            vmLineItem.ModalCrumbs = _messageResolver.GetText("sl.order.addlineitem.modal.title")/*"Add Product to Order on Behalf of"*/
                + (vmLineItem.Salesman.Id == Guid.Empty ? "Salesman" : vmLineItem.Salesman.Username);
            vmLineItem.RunClearAndSetup();
            vmLineItem.IsNew = true;
            vmLineItem.IsAdd = true;
            vmLineItem.IsEnabled = true;

            _newLineItemModal.ShowDialog();
        }

        void modal_Closed(object sender, EventArgs e)
        {
            SOLineItemViewModel vmLineItem = _newLineItemModal.DataContext as SOLineItemViewModel;

            bool result = _newLineItemModal.DialogResult.Value;
            if (result)
            {
                _vm.UpdateOrAddLineItemFromPoductSummary(vmLineItem.ProductAddSummaries, vmLineItem.IsAdd);
                vmLineItem.MultipleProduct.Clear();
                vmLineItem.ProductAddSummaries.Clear();
            }

            if (_vm.LineItems.Count > 0)
            {
                dtDateRequired.IsEnabled = false;
                cmbSalesman.IsEnabled = false;
                cmbRoutes.IsEnabled = false;
                cmbOutlets.IsEnabled = false;
            }
        }

        void _distMsgBx_Closed(object sender, EventArgs e)
        {
            DistributrMessageBoxViewModel vmMsgBx = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            if (vmMsgBx.Command == DistributrMessageBoxViewModel.CommandToExcecute.NewButtonClickedCommand)
            {
                _vm.OrderIdLookup = Guid.Empty;
                _vm.LoadOrderCommand.Execute(null);

                dtDateRequired.IsEnabled = true;
                cmbSalesman.IsEnabled = true;
                cmbRoutes.IsEnabled = true;
                cmbOutlets.IsEnabled = true;
            }
            else if (vmMsgBx.Command == DistributrMessageBoxViewModel.CommandToExcecute.HomeButtonClickedCommand)
            {
                string urlPendApp = "/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals";
                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri(urlPendApp, UriKind.Relative));
            }
            else if (vmMsgBx.Command == DistributrMessageBoxViewModel.CommandToExcecute.Action1ButtonClickedCommand)
            {
                string urlPendApp = "/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals";
                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri(urlPendApp, UriKind.Relative));
            }
            else
                vmMsgBx.ExecuteCommand.Execute(null);
            cmbSalesman.SelectedIndex = 0;
        }

        //used in confirm and approve which is not is use currently
        void _distMsgBx_PreApproval_Closed(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Wait;
            _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            _vm = this.DataContext as EditSalesmanOrderViewModel;
            _distMsgBxVm.DialogResult = true;
            switch (_distMsgBxVm.Command)
            {
                case DistributrMessageBoxViewModel.CommandToExcecute.NewButtonClickedCommand://create back orders and approve them
                    this.Cursor = Cursors.Wait;
                    _vm.CreateBackOrderAndApproveCommand.Execute(null);
                    this.Cursor = Cursors.Arrow;
                    CompletedActionOptions("Order " + _vm.OrderId + " on behalf of " +
                                                     _vm.SelectedSalesman.Username +
                                                     "\nwas successfully confirmed after creation of backorder.");
                    break;
                case DistributrMessageBoxViewModel.CommandToExcecute.HomeButtonClickedCommand://go back to page
                    _distMsgBxVm.DialogResult = false;
                    this.Cursor = Cursors.Arrow;
                    break;
                case DistributrMessageBoxViewModel.CommandToExcecute.CancelButtonClickedCommand://go back to page
                    _distMsgBxVm.DialogResult = false;
                    this.Cursor = Cursors.Arrow;
                    break;
            }
            if (!_distMsgBxVm.DialogResult) return;
            this.Cursor = Cursors.Arrow;
        }

        void CompletedActionOptions(string msg1)
        {
            string newOrder = _messageResolver.GetText("sl.createOrder.postconfirm.messagebox.options.newOrder");/*"New Order";*/
            string approveOrders = _messageResolver.GetText("sl.createOrder.save.messagebox.options.approveOrders"); /*"Approve Orders";*/
            string summary = _messageResolver.GetText("sl.createOrder.save.messagebox.options.ordersSummary");/*"Orders Summary"*/
            _distMsgBxVm = new DistributrMessageBoxViewModel();
            _distMsgBx = new DistributrMessageBox(true, true, false, false, true, newOrder,
                                                             approveOrders, null, null, summary);

            _distMsgBx.Closed += new EventHandler(_distMsgBx_Closed);
            _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            if (OtherUtilities.StrBackUrl.Trim() == "")
                OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals";
            _distMsgBxVm.MyListUri = OtherUtilities.StrBackUrl;
            _distMsgBxVm.NewUriString = "/views/salesmanorders/listsalesmanorders.xaml?orderid=" + Guid.Empty;
            _distMsgBxVm.MessageBoxContent = msg1;
            _distMsgBxVm.MessageBoxTitle = "Distributr: Create Order On Behalf of Salesman";
            _distMsgBxVm.Action1ButtonToolTip = "Go to list of orders";
            _distMsgBxVm.HomeButtonTooTip = "Approve orders";
            _distMsgBxVm.NewButtonToolTip = "Create a new order";
            _distMsgBx.ShowDialog();
        }

        private void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink  hl = sender as Hyperlink ;
            string[] tag = hl.Tag.ToString().Split(',');
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString());
            _newLineItemModal = new SOLineItemModal();
            _newLineItemModal.Closed += new EventHandler(modal_Closed);
            _newLineItemModal.cmbProducts.IsEnabled = false;
            _newLineItemModal.btnAddProduct.IsEnabled = false;

            LineItemType lit = LineItemType.Unit;///int.Parse(tag[1].ToString());
            var lineItemList = _vm.LineItems.Where(n => n.ProductId == ParentProductid && n.OrderLineItemType != OrderLineItemType.Discount);
            var lineItem = lineItemList.First(p => p.ProductId == ParentProductid);
            SOLineItemViewModel vmLineItem = _newLineItemModal.DataContext as SOLineItemViewModel;
            vmLineItem.ModalCrumbs = _messageResolver.GetText("sl.createOrder.lineitemmodal.edit.title");/* "Edit Product Quantity";*/
            vmLineItem.RunClearAndSetup();
            vmLineItem.SelectedOutletId = _vm.SelectedOutlet.Id;

            vmLineItem.LoadForEdit(lineItem.ProductId,
                lineItem.UnitPrice,
                lineItem.LineItemUnitVatValue,
                lineItem.TotalPrice,
                lineItem.TotalLineItemVatAmount,
                lineItem.SequenceNo,
                lineItemList.Where(n => n.OrderLineItemType != OrderLineItemType.Discount).Min(m => m.Qty)
                );
            vmLineItem.LineItemType = lit;
            _newLineItemModal.ShowDialog();
        }

        private void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink ;
            string[] tag = hl.Tag.ToString().Split(',');
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString()); //int.Parse(tag[0].ToString());
            LineItemType lit = LineItemType.Unit; //(LineItemType) int.Parse(tag[1].ToString());
            _vm.RemoveLineItem(ParentProductid, lit);

            if (_vm.LineItems.Count > 0)
            {
                cmbOutlets.IsEnabled = false;
                cmbRoutes.IsEnabled = false;
                cmbSalesman.IsEnabled = false;
            }
            else
            {
                cmbOutlets.IsEnabled = true;
                cmbRoutes.IsEnabled = true;
                cmbSalesman.IsEnabled = true;
            }
        }

        private void cmbRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm.RouteChangedCommand.Execute(null);
            cmbOutlets.ItemsSource = _vm.RouteOutlets;

            if (!_vm.IsEditing)
            {
                cmbOutlets.SelectedItem = _vm.RouteOutlets.First(n => n.Id == Guid.Empty);
                _vm.IsEditing = false;
            }
            else
            {
                cmbOutlets.SelectedItem = _vm.RouteOutlets.FirstOrDefault(n => n.Id == _vm.setSelectedId);
                if (cmbOutlets.SelectedItem == null) //force #@$%%^#^
                    cmbOutlets.SelectedItem = _vm.RouteOutlets.First(n => n.Id == Guid.Empty);
            }

        }

        private void cmbOutlets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            if (_vm.SelectedOutlet != null && _vm.SelectedOutlet.Id != Guid.Empty)
            {
                _vm.OutletChanged();
            }
        }

        private void cmbSalesman_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm.SalesmanChangedCommand.Execute(null);
            cmbRoutes.ItemsSource = _vm.DistributorRoutes;
            cmbRoutes.SelectedIndex = 0;
        }

        private void btnViewList_Click(object sender, RoutedEventArgs e)
        {
            _vm.ConfirmNavigatingAway = false;
            NavigationService.Navigate(new Uri(OtherUtilities.StrBackUrl, UriKind.Relative));
        }

        private void btnGoHome_Click(object sender, RoutedEventArgs e)
        {
            if (
                       MessageBox.Show(/*"Are you sure you want to cancel this order?"*/
                       _messageResolver.GetText("sl.createOrder.cancel.messagebox"),
                       _messageResolver.GetText("sl.createOrder.navigateAway.messagebox.prompt.caption")/*"Distributr: Confirm Navigating Away"*/
                       , MessageBoxButton.OKCancel)
                       == MessageBoxResult.OK)
            {
                _vm.ConfirmNavigatingAway = false;
                if (_vm == null)
                    _vm = DataContext as EditSalesmanOrderViewModel;
                _vm.CancelCommand.Execute(null);
                _vm.CancelOrderCommand.Execute(null);

                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri("/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals", UriKind.Relative));
            }
        }

        private void btnCreateNewOrder_Click(object sender, RoutedEventArgs e)
        {
            _vm.LoadForEditing = true;
            _vm.LoadForViewing = false;
            _vm.PostConfirmVisible = false;
            _vm.CancelButtonContent = "Cancel";
            _vm.OrderIdLookup = Guid.Empty;
            _vm.LoadOrderCommand.Execute(null);
        }

        private void dgOrderLineItems_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            StackPanel stackPanel = null;
            //stackPanel = dgOrderLineItems.Columns.GetByName("colEdit").GetCellContent(dataGridrow) as StackPanel;
            ////Hyperlink edit = stackPanel.Children[0] as Hyperlink ;
            ////edit.Content = _messageResolver.GetText("sl.createOrder.grid.col.edit.edit");
            ////Hyperlink delete = stackPanel.Children[2] as Hyperlink ;
            ////delete.Content = _messageResolver.GetText("sl.createOrder.grid.col.edit.delete");
        }

        private void cmbSalesman_DropDownOpened(object sender, EventArgs e)
        {
            cmbSalesman.IsDropDownOpen = false;

            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedSalesman = (User) popup.ShowDlg(sender);
        }

        private void cmbRoutes_DropDownOpened(object sender, EventArgs e)
        {
            cmbRoutes.IsDropDownOpen = false;
            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedRoute = (Route) popup.ShowDlg(sender);
        }

        private void cmbOutlets_DropDownOpened(object sender, EventArgs e)
        {
            cmbOutlets.IsDropDownOpen = false;
            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedOutlet = (Outlet) popup.ShowDlg(sender);
        }
    }
}
