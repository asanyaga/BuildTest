using System;
using System.Collections.Generic;
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
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.SalesmanOrders
{
    public partial class ApproveSalemanOrders : PageBase
    {
        private ApproveSalesmanOrderViewModel _vm;
        private SOLineItemModal _newLineItemModal = null;
        private AmmendOrderLineItemModal _ammendApprovedOrderModal = null;
        private DistributrMessageBoxViewModel _distMsgBxVm;
        private DistributrMessageBox _distMsgBx;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public ApproveSalemanOrders()
        {
            InitializeComponent();
            _vm = this.DataContext as ApproveSalesmanOrderViewModel;
            this.Loaded += ApproveSalesmanOrder_Loaded;
            LabelControls();
        }

        private void LabelControls()
        {
            lblOrderId.Content = _messageResolver.GetText("sl.createOrder.saleid_lbl");
            lblDateRequired.Content = _messageResolver.GetText("sl.createOrder.date_lbl");
            lblSalesman.Content = _messageResolver.GetText("sl.createOrder.salesman_lbl");
            lblRoute.Content = _messageResolver.GetText("sl.createOrder.route_lbl");
            lblOutlet.Content = _messageResolver.GetText("sl.createOrder.outlet_lbl");
            lblSaleDiscount.Content = _messageResolver.GetText("sl.createOrder.saleDiscount_lbl");
            lblStatus.Content = _messageResolver.GetText("sl.createOrder.status_lbl");
            lblTotalProductDiscount.Content = _messageResolver.GetText("sl.createOrder.totalProductDiscount_lbl");
            lblTotalNet.Content = _messageResolver.GetText("sl.createOrder.totalNet_lbl");
            lblTotalVat.Content = _messageResolver.GetText("sl.createOrder.totalVat_lbl");
            lblTotalGross.Content = _messageResolver.GetText("sl.createOrder.totalGross_lbl");
            lblRejectReason.Content = _messageResolver.GetText("sl.approveOrder.rejectReason");
            lblReceivedReturnablesLegend.Content = _messageResolver.GetText("sl.approveOrder.receivedReturnablesLegend");

            btnViewInvoice.Content = _messageResolver.GetText("sl.approveOrder.viewInvoice");
            btnViewReceipts.Content = _messageResolver.GetText("sl.approveOrder.viewReceipt");
            btnAddLineItem.Content = _messageResolver.GetText("sl.createOrder.addProduct_btn");
            btnApprove.Content = _messageResolver.GetText("sl.approveOrder.approve");
            btnReject.Content = _messageResolver.GetText("sl.approveOrder.reject");
            btnProcessBackOrder.Content = _messageResolver.GetText("sl.approveOrder.processBackOrder");
            btnBackToList.Content = _messageResolver.GetText("sl.approveOrder.back");

            //Full Grid
            coldgLineItemsFullGridProduct.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.product");
            coldgLineItemsFullGridQty.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.required");
            coldgLineItemsFullGridProcessedQty.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.approve");
            coldgLineItemsFullGridAvailableProductInv.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.available");
            coldgLineItemsFullGridUnitPrice.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.unitPrice");
            coldgLineItemsFullGridNetAmount.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.netAmt");
            coldgLineItemsFullGridProductDiscount.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.unitDiscount");
            coldgLineItemsFullGridLineItemVatValue.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.unitVAT");
            coldgLineItemsFullGridVatAmount.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.totalVAT");
            coldgLineItemsFullGridTotalPrice.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.grossAmount");
            coldgLineItemsFullGridBackOrder.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.backOrder");
            coldgLineItemsFullGridLostSale.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.lostSale");
            coldgLineItemsFullGridProduct_Type.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.productType");

            //dgOrderLineItems
            coldgOrderLineItemsProduct.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.product");
            coldgOrderLineItemsQty.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.required");
            coldgOrderLineItemsAvailableProductInv.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.available");
            coldgOrderLineItemsUnitPrice.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.unitPrice");
            coldgOrderLineItemsNetAmount.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.netAmt");
            coldgOrderLineItemsProductDiscount.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.unitDiscount");
            coldgOrderLineItemsLineItemVatValue.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.unitVAT");
            coldgOrderLineItemsVatAmount.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.totalVAT");
            coldgOrderLineItemsTotalPrice.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.grossAmount");
            coldgOrderLineItemsProduct_Type.Header =
                _messageResolver.GetText("sl.approveOrder.lineItemsGrid.productType");

        }

        void Load()
        {
            _vm.RunClearAndSetup();
            string _orderId = NavigationService.Source.OriginalString.ParseQueryString("orderid");
            if (!string.IsNullOrEmpty(_orderId))
            {
                string orderId = _orderId;
                _vm.OrderIdLookup = new Guid(orderId);
            }
            string _loadforprocessing = NavigationService.Source.OriginalString.ParseQueryString("loadforprocessing");
            if (!string.IsNullOrEmpty(_loadforprocessing)) //cn: load for viewing
            {
                string loadForViewing = _loadforprocessing;
                _vm.LoadForProcessing = Convert.ToBoolean(loadForViewing);
                _vm.LoadForViewing = !_vm.LoadForProcessing;
                lblHeader.Content = "";
                //_vm.CancelButtonContent = "Back";//not used
                //_vm.CancelButtonContent = messageResolver.GetText("sl.approveOrder.back");//cn: not used
                _vm.ConfirmNavigatingAway = false;
            }
            else//load for processing
            {
                _vm.LoadForProcessing = true;
                //_vm.CancelButtonContent = "Cancel";//not used
                //_vm.CancelButtonContent = messageResolver.GetText("sl.approveOrder.cancel");//cn: not used
                _vm.ConfirmNavigatingAway = true;
            }
            string _processBackOrder = NavigationService.Source.OriginalString.ParseQueryString("ProcessBackOrder");
            string _viewBackOrder = NavigationService.Source.OriginalString.ParseQueryString("ViewBackOrder");
            string _viewDispatched = NavigationService.Source.OriginalString.ParseQueryString("ViewDispatched");
            string _viewDelivered = NavigationService.Source.OriginalString.ParseQueryString("ViewDelivered");
            string _viewLostSales = NavigationService.Source.OriginalString.ParseQueryString("ViewLostSales");
            if (!string.IsNullOrEmpty(_processBackOrder))//cn: processing back order
            {
                lblHeader.Content = _messageResolver.GetText("sl.approveOrder.approveBackOrder.title");
                processingBackOrder = true;
                _vm.ProcessBackOrdersCommand.Execute(null);
                _vm.LoadForProcessing = true;
                _vm.LoadForViewing = false;
                btnProcessBackOrder.Visibility = Visibility.Collapsed;
                btnAddLineItem.Visibility = Visibility.Collapsed;
                dgLineItemsFullGrid.ItemsSource = _vm.LineItems;

                dgLineItemsFullGrid.Visibility = Visibility.Visible;
                dgOrderLineItems.Visibility = Visibility.Collapsed;

                btnApprove.Content = _messageResolver.GetText("sl.approveOrder.viewBackOrder.approve_btn");/*"Approve Back Order";*/
                btnReject.Content = _messageResolver.GetText("sl.approveOrder.viewBackOrder.reject_btn");/*"Reject Back Order";*/

                //spFinancials.Visibility = Visibility.Collapsed;
                return;
            }
            else if (!string.IsNullOrEmpty(_viewBackOrder))//cn: viewing back order
            {
                _vm.ConfirmNavigatingAway = false;
                processingBackOrder = true;
                _vm.ProcessBackOrdersCommand.Execute(null);
                _vm.LoadForProcessing = false;
                _vm.LoadForViewing = true;
                btnProcessBackOrder.Visibility = Visibility.Collapsed;
                btnAddLineItem.Visibility = Visibility.Collapsed;
                dgLineItemsFullGrid.ItemsSource = _vm.LineItems;

                dgLineItemsFullGrid.Visibility = Visibility.Collapsed;
                dgOrderLineItems.Visibility = Visibility.Visible;

                btnApprove.Content = _messageResolver.GetText("sl.approveOrder.viewBackOrder.approve_btn");/*"Approve Back Order";*/
                btnReject.Content = _messageResolver.GetText("sl.approveOrder.viewBackOrder.reject_btn");/*"Cancel Back Order";*/
                lblHeader.Content = _messageResolver.GetText("sl.approveOrder.viewBackOrder.title");/*"Viewing Back Order for Order " +_vm.OrderId;*/

                //spFinancials.Visibility = Visibility.Collapsed;
                return;
            }
            else if (!string.IsNullOrEmpty(_viewDispatched))//cn: viewing dispatched
            {
                _vm.ConfirmNavigatingAway = false;
                _vm.ViewDispatched = true;
                _vm.LoadForProcessing = false;
                _vm.LoadForViewing = !_vm.LoadForProcessing;

                _vm.LoadOrderCommand.Execute(null);
                _vm.LoadInvoiceAndReceipts();
                lblHeader.Content = _messageResolver.GetText("sl.approveOrder.viewDispatchedOrder.title") + " "/*"Viewing Dispatched Items for Order"*/ + _vm.OrderId;

                _vm.ViewDispatched = false;
                //return;
            }
            else if (!string.IsNullOrEmpty(_viewDelivered))//cn: viewing delivered
            {
                _vm.ConfirmNavigatingAway = false;
                _vm.LoadForProcessing = false;
                _vm.LoadForViewing = !_vm.LoadForProcessing;
                _vm.LoadDelivered = true;
                _vm.LoadOrderCommand.Execute(null);
                _vm.LoadInvoiceAndReceipts();
                _vm.LoadDelivered = false;
                lblHeader.Content = _messageResolver.GetText("sl.approveOrder.viewDeliveredOrder.title") + " "/*"Viewing Delivered Items for Order "*/ + _vm.OrderId;

                dgLineItemsFullGrid.Visibility = Visibility.Collapsed;
                dgOrderLineItems.Visibility = Visibility.Visible;
                return;
            }
            else if (!string.IsNullOrEmpty(_viewLostSales))//cn: viewing lost sale
            {
                _vm.ConfirmNavigatingAway = false;
                _vm.LoadForProcessing = false;
                _vm.LoadForViewing = !_vm.LoadForProcessing;
                _vm.LoadLostSale = true;
                _vm.LoadOrderCommand.Execute(null);
                _vm.LoadInvoiceAndReceipts();
                _vm.LoadLostSale = false;
                lblHeader.Content = _messageResolver.GetText("sl.approveOrder.viewLostSale.title") + " "/*"Viewing Lost Sale for Order "*/ + _vm.OrderId;

                dgLineItemsFullGrid.Visibility = Visibility.Collapsed;
                dgOrderLineItems.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                _vm.LoadOrderCommand.Execute(null);
                _vm.LoadInvoiceAndReceipts();

                if (_vm.LoadForViewing)
                    lblHeader.Content = _messageResolver.GetText("sl.approveOrder.viewOrder.title") + " " /*"Viewing Details of Order "*/ + _vm.OrderId;
            }

            if (_vm.LineItems.Any(n => n.BackOrder > 0) || _vm.LineItems.Any(n => n.LostSale > 0))
            {
                dgLineItemsFullGrid.Visibility = Visibility.Visible;
                dgOrderLineItems.Visibility = Visibility.Collapsed;
            }
            else
            {
                dgLineItemsFullGrid.Visibility = Visibility.Collapsed;
                dgOrderLineItems.Visibility = Visibility.Visible;
            }

            //if (_vm.Status == "Confirmed")
            //{
            //    spFinancials.Visibility = Visibility.Collapsed;
            //}

            //if (_vm.Status == DocumentStatus.Rejected.ToString())
            //    spFinancials.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                string text = _messageResolver.GetText("sl.approveOrder.back.messageBox.confirmMessage.part1") + "\n" +
                              _messageResolver.GetText("sl.approveOrder.back.messageBox.confirmMessage.part2");
                if (
                    MessageBox.Show(
                    /*"Are you sure you want to move away from this page without completing the order processing?\n"+
                    "Unsaved changes will be lost"*/
                    text,
                    _messageResolver.GetText("sl.approveOrder.back.messageBox.caption")
                    /*"Distributr: Confirm Navigating Away"*/
                    , MessageBoxButton.OKCancel) ==
                    MessageBoxResult.OK)
                {
                    _vm.CancelCommand.Execute(null);
                }
                else
                    e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        private void ApproveSalesmanOrder_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private bool processingBackOrder = false;

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            bool showCompletedApprovalOptions = true;
            string btnApproveCaption =
                _messageResolver.GetText("sl.approveOrder.approve.messagebox.createBackOrderAndApprove_btn");
            /*"Create Back Order And Approve";*/
            string btnBackCaption = _messageResolver.GetText("sl.approveOrder.approve.messagebox.back_btn");
            string confirmationMsg = _messageResolver.GetText("sl.approveOrder.approve.messageBox.propmt") + " ";
            /*"Do you want to approve order ";*/
            string confirmationMsg2 = "";
            string newButtonToolTip =
                "Create back order of the uncovered quantities for each of the listed items then approve the whole order.";

            if (processingBackOrder)
            {
                btnApproveCaption = _messageResolver.GetText("sl.approveOrder.bo.messageBox.rejectBackOrder_btn");
                /*"Reject Back Order";*/
                newButtonToolTip = "Reject the whole back order and create a lost sale from back order items";
                confirmationMsg = _messageResolver.GetText("sl.approveOrder.bo.approve.messageBox.propmt") + " ";
                /*"Do you want to approve the back order for order ";*/

                if (_vm.Status == "OrderDispatchedToPhone")
                {
                    confirmationMsg2 = _messageResolver.GetText("sl.approveOrder.bo.approve.messageBox.propmt2");
                    /*"\nNOTE: To dispatch the back order go to dispatch orders module.";*/
                }

            }

            if (!processingBackOrder)
            {
                if (_vm.LineItems.Count < 1)
                {
                    MessageBox.Show(
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.nolineItemMessage")
                        /*"The order must have at least 1 line item."*/, "Order Line Items.",
                        MessageBoxButton.OK);
                    return;
                }
            }
            if (
                MessageBox.Show(confirmationMsg + _vm.OrderId + " ?" + confirmationMsg2,
                                _messageResolver.GetText("sl.approveOrder.approve.messageBox.caption")
                /*"Distributr: Confirm Action."*/,
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.Cursor = Cursors.Wait;

                //CN:  
                //first validate
                _vm.ValidateOrderForApprovalCommand.Execute(null);
                //if is valid then approve it
                if (_vm.Validated)
                {
                    _vm.ApproveCommand.Execute(null);
                }
                else
                {
                    //show invalid n give options here !
                    _vm.CreateInvalidOrdersMessageCommand.Execute(null);
                    _distMsgBxVm = new DistributrMessageBoxViewModel();

                    _distMsgBx = new DistributrMessageBox(true, false, false, false, true, btnApproveCaption,
                                                          null, null, null, btnBackCaption);

                    showCompletedApprovalOptions = false;
                    _distMsgBx.Closed += new EventHandler(_distMsgBx_PreApproval_Closed);
                    _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
                    _distMsgBxVm.MessageBoxContent =
                        /*"Order cannot be fulfilled.\nThe available inventory cannot satisfy the following order item(s):\n"*/
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.unsatiableOrder.message.part1") +
                        "\n" +
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.unsatiableOrder.message.part2") +
                        "\n" + _vm.Message +
                        /*"\nSelect an option to proceed."*/
                        "\n" +
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.unsatiableOrder.message.part3");
                    _distMsgBxVm.NewButtonToolTip = newButtonToolTip;
                    _distMsgBxVm.Action1ButtonToolTip = "Cancel the order processing and go back to order processing.";
                    _distMsgBxVm.MessageBoxTitle =
                        /*"Distributr: Approve Order*/
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.unsatiableOrder.title.part1") + " " +
                        _vm.OrderId +
                        /*" on behalf of "*/
                        " " + _messageResolver.GetText("sl.approveOrder.approve.messageBox.unsatiableOrder.title.part2") +
                        " " + _vm.SalesmanUsername;
                    _distMsgBx.ShowDialog();
                }

                this.Cursor = Cursors.Arrow;

                if (_vm.IsApproved)
                {
                    string msg = /*"Order "*/
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.message.part1") + " " +
                        _vm.OrderId
                        + " " /*" on behalf of "*/
                        + _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.message.part2")
                        + " "
                        + _vm.SalesmanUsername
                        + " "
                        + /*" was successfully approved."*/
                        _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.message.part3");

                    string caption = _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.caption");
                    /*"Distributr: Approve Order On Behalf of Salesman";*/

                    if (processingBackOrder)
                    {
                        msg = /*"Back Order for order "*/
                            _messageResolver.GetText("sl.approveOrder.bo.approve.messageBox.approved.message.part1") +
                            " "
                            + _vm.OrderId
                            + " " /*" on behalf of "*/
                            + _messageResolver.GetText("sl.approveOrder.bo.approve.messageBox.approved.message.part2") +
                            " "
                            + _vm.SalesmanUsername +
                            /* " was successfully approved."*/
                            _messageResolver.GetText("sl.approveOrder.bo.approve.messageBox.approved.message.part3");
                        caption = _messageResolver.GetText("sl.approveOrder.bo.approve.messageBox.approved.caption");
                        /*"Distributr: Approve Back Order";*/
                    }
                    if (showCompletedApprovalOptions)
                        ShowCompletedApprovalOptions(msg, caption);
                }
            }
            processingBackOrder = false;
        }

        private void ShowCompletedApprovalOptions(string message, string caption)
        {
            _vm.ConfirmNavigatingAway = false;
            string goDispatch = _messageResolver.GetText("sl.approveOrder.approve.completed.messageBox.gotoDispatch");
            /*"Go To Dispatch";*/
            string processBO = _messageResolver.GetText("sl.approveOrder.approve.completed.messageBox.processBO");
            /*"Process Back Orders";*/
            string pendingList = _messageResolver.GetText("sl.approveOrder.approve.completed.messageBox.pendingList");
            /*"Pending Approval List";*/
            _distMsgBx = new DistributrMessageBox(true, true, false, false, true, goDispatch,
                                                  processBO, null, null, pendingList);
            //_distMsgBx = null;
            _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            _distMsgBx.Closed += new EventHandler(_distMsgBx_PostApproval_Closed);

            _distMsgBxVm.ClearToolTips();

            _distMsgBxVm.NewButtonToolTip = "Dispatch orders pending dispatch.";
            _distMsgBxVm.HomeButtonTooTip = "Go to list of back orders and process back order.";
            _distMsgBxVm.Action1ButtonToolTip = "View List of orders pending approval";
            _distMsgBx.Width = _distMsgBx.MinWidth;
            _distMsgBx.Height = _distMsgBx.MinHeight;


            _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            _distMsgBxVm.MyListUri = "/views/salesmanorders/listsalesmanorder.xaml?PendingApprovals";
            _distMsgBxVm.NewUriString = "/views/salesmanorders/listsalesmanorder.xaml?BackOrders";
            _distMsgBxVm.MessageBoxContent = message;
            _distMsgBxVm.MessageBoxTitle = caption;
            _distMsgBx.ShowDialog();
        }

        private void _distMsgBx_PostApproval_Closed(object sender, EventArgs e)
        {
            _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            if (_distMsgBxVm.DialogResult)
            {
                switch (_distMsgBxVm.Command)
                {
                    case DistributrMessageBoxViewModel.CommandToExcecute.NewButtonClickedCommand: //dispatch list
                        string urlDisp = "/views/DispatchPendingOrdersToPhone/DispatchPendingOrdersToPhone.xaml";
                        _vm.ConfirmNavigatingAway = false;
                        NavigationService.Navigate(new Uri(urlDisp, UriKind.Relative));
                        break;
                    case DistributrMessageBoxViewModel.CommandToExcecute.HomeButtonClickedCommand: //Process BO
                        string urlBO = "/views/salesmanorders/listsalesmanorders.xaml?BackOrders";
                        _vm.ConfirmNavigatingAway = false;
                        NavigationService.Navigate(new Uri(urlBO, UriKind.Relative));
                        break;
                    case DistributrMessageBoxViewModel.CommandToExcecute.Action1ButtonClickedCommand:
                        //pending approval list
                        _vm.ConfirmNavigatingAway = false;
                        NavigationService.Navigate(new Uri(OtherUtilities.StrBackUrl, UriKind.Relative));
                        break;
                }
            }
            else //default action
            {
                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri(OtherUtilities.StrBackUrl, UriKind.Relative));
            }

        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            RejectMessageBox();
        }

        private void RejectMessageBox()
        {
            string message = /*"Are you sure you want to reject order "*/
                _messageResolver.GetText("sl.approveOrder.reject.messageBox.message.part1") + " "
                + _vm.OrderId + " ?\n" +
                /*"If yes please enter the reason for rejecting the order";*/
                _messageResolver.GetText("sl.approveOrder.reject.messageBox.message.part2");
            if (processingBackOrder)
            {
                message = /*"Are you sure you want to reject back order of order "*/
                    _messageResolver.GetText("sl.approveOrder.bo.reject.messageBox.message.part1") + " "
                    + _vm.OrderId + " ?\n"
                    + /*"Note: A lost sale equivalent to the back order will be created.*/
                    _messageResolver.GetText("sl.approveOrder.bo.reject.messageBox.message.part2") + "\n";
            }

            RejectReasonModal rejectModal = new RejectReasonModal();
            rejectModal.Closing += rejectModal_Closing;
            rejectModal.Title = /*"Reject Order"*/
                _messageResolver.GetText("sl.approveOrder.reject.messageBox.message.title") + "\n" + _vm.OrderId;
            rejectModal.txtCaption.Text = message;
            if (processingBackOrder)
            {
                rejectModal.rejectingBackOrder = true;
                rejectModal.txtRejectReason.Visibility = Visibility.Collapsed;
            }
            rejectModal.ShowDialog();
        }

        private void rejectModal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RejectReasonModal rejectModal = sender as RejectReasonModal;
            string msg = /*"Order " */
                _messageResolver.GetText("sl.approveOrder.rejected.messageBox.message.part1") + " "
                + _vm.OrderId +
                /*" on behalf of " +*/
                " " + _messageResolver.GetText("sl.approveOrder.rejected.messageBox.message.part2") + " "
                + _vm.SalesmanUsername +
                /*" was successfully rejected."*/
                _messageResolver.GetText("sl.approveOrder.rejected.messageBox.message.part3");
            string caption = _messageResolver.GetText("sl.approveOrder.rejected.messageBox.message.title");
            /*"Distributr: Reject Order On Behalf of Salesman";*/

            if (rejectModal.rejectingBackOrder)
            {
                msg = /*"Back order in the order "*/
                    _messageResolver.GetText("sl.approveOrder.bo.rejected.messageBox.message.part1") + " "
                    + _vm.OrderId +
                    /*" on behalf of " +*/
                    " " + _messageResolver.GetText("sl.approveOrder.bo.rejected.messageBox.message.part2") + " " +
                    _vm.SalesmanUsername +
                    /*" was successfully rejected and lost sale created.";*/
                    " " + _messageResolver.GetText("sl.approveOrder.bo.rejected.messageBox.message.part3");
                caption = _messageResolver.GetText("sl.approveOrder.bo.rejected.messageBox.message.title");
                /*"Distributr: Reject Back Order fo Order on Behalf of Salesman";*/
            }


            if (rejectModal.DialogResult == true)
            {
                _vm.RejectReason = rejectModal.txtRejectReason.Text.Trim();
                _vm.RejectCommand.Execute(null);
                rejectModal.rejectingBackOrder = false;
                ShowCompletedApprovalOptions(msg, caption);
            }
        }

        private void btnBackToList_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(OtherUtilities.StrBackUrl, UriKind.Relative));
        }

        private void btnAddLineItem_Click(object sender, RoutedEventArgs e)
        {
            AddNewLineItem();
        }

        private void AddNewLineItem()
        {
            _newLineItemModal = new SOLineItemModal();
            _newLineItemModal.Closed += new EventHandler(modal_Closed);
            _newLineItemModal.cmbProducts.IsEnabled = true;
            _newLineItemModal.btnAddProduct.IsEnabled = true;
            SOLineItemViewModel vmLineItem = _newLineItemModal.DataContext as SOLineItemViewModel;
            vmLineItem.SelectedOutletId = _vm.OutletId;
            vmLineItem.ModalCrumbs =
                _messageResolver.GetText("sl.order.addlineitem.modal.title") /*"Add Product to Order on Behalf of"*/+
                _vm.SalesmanUsername;
            vmLineItem.RunClearAndSetup();
            vmLineItem.IsNew = true;
            vmLineItem.IsAdd = true;
            vmLineItem.IsEnabled = true;

            _newLineItemModal.ShowDialog();
        }

        private void _distMsgBx_PreApproval_Closed(object sender, EventArgs e)
        {
            _distMsgBxVm = _distMsgBx.DataContext as DistributrMessageBoxViewModel;
            _vm = this.DataContext as ApproveSalesmanOrderViewModel;
            string msg = /*"Order "*/
                _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.message.part1") + " "
                + _vm.OrderId +
                /*" on behalf of "*/
                " " + _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.message.part2") + " "
                + _vm.SalesmanUsername +
                /*" was successfully approved.";*/
                " " + _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.message.part3") + " ";
            string caption = _messageResolver.GetText("sl.approveOrder.approve.messageBox.approved.caption");
            /*"Distributr: Approve Order On Behalf of Salesman";*/

            if (_distMsgBxVm.DialogResult)
            {
                switch (_distMsgBxVm.Command)
                {
                    case DistributrMessageBoxViewModel.CommandToExcecute.NewButtonClickedCommand:
                        if (_vm.ProcessingBackOrder)
                        {
                            caption = _messageResolver.GetText("sl.approveOrder.rejected.messageBox.message.title");
                            /* "Distributr: Reject Order On Behalf of Salesman";*/
                            msg = /*"Back order of order " */
                                _messageResolver.GetText("sl.approveOrder.bo.rejected.messageBox.message.part1") + " "
                                + _vm.OrderId +
                                /*" on behalf of " +*/
                                " " + _messageResolver.GetText("sl.approveOrder.bo.rejected.messageBox.message.part2") +
                                " "
                                + _vm.SalesmanUsername +
                                /*" was rejected and a lost sale created.";*/
                                _messageResolver.GetText("sl.approveOrder.rejected.messageBox.message.part3");
                            this.Cursor = Cursors.Wait;
                            _vm.RejectCommand.Execute(null);
                            this.Cursor = Cursors.Arrow;
                        }
                        else
                        {
                            this.Cursor = Cursors.Wait;
                            _vm.RunCreateBackOrderAndApprove();
                            this.Cursor = Cursors.Arrow;
                        }
                        ShowCompletedApprovalOptions(msg, caption);
                        break;
                    case DistributrMessageBoxViewModel.CommandToExcecute.Action1ButtonClickedCommand:
                        return;
                }
            } //default action
            if (!_distMsgBxVm.DialogResult) return;
        }

        private void modal_Closed(object sender, EventArgs e)
        {
            bool result = false;
            object vmItem = null;
            //AmmendSalesmanOrderLineItemViewModel avmItem = null;
            if (sender.GetType() == typeof(SOLineItemModal))
            {
                vmItem = _newLineItemModal.DataContext as SOLineItemViewModel;
                result = _newLineItemModal.DialogResult.Value;
            }
            else if (sender.GetType() == typeof(AmmendOrderLineItemModal))
            {
                vmItem = _ammendApprovedOrderModal.DataContext as AmmendSalesmanOrderLineItemViewModel;
                result = _ammendApprovedOrderModal.DialogResult.Value;
            }
            if (!result) return;
            if (sender.GetType() == typeof(SOLineItemModal))
            {
                AddNewLineItemModalClosed();
            }
            else if (sender.GetType() == typeof(AmmendOrderLineItemModal))
            {
                foreach (var item in ((AmmendSalesmanOrderLineItemViewModel)vmItem).LineItems.Where(n => n.Qty != 0))
                {
                    if (item.IsNew && item.ProductType != "ReturnableProduct")
                    {
                        AddLineItem(sender, item);
                        _vm.AddedNewLineItem = true;
                    }
                    else
                    {
                        if (item.ProductType == "ReturnableProduct")
                        {
                            var lineItemToUpdate = _vm.LineItems.FirstOrDefault(n => n.ProductId == item.ProductId);
                            //returnables are grouped by product id!
                            item.SequenceNo = lineItemToUpdate.SequenceNo;
                            item.Qty = lineItemToUpdate.Qty + item.Qty;
                            item.VatAmount = lineItemToUpdate.TotalLineItemVatAmount + item.VatAmount;
                            item.Vat = lineItemToUpdate.LineItemVatValue;
                            item.TotalPrice = lineItemToUpdate.TotalPrice + item.TotalPrice;

                            UpdateLineItem(sender, item);
                        }
                        else
                            UpdateLineItem(sender, item);
                    }

                    var targetLineItem =
                        _vm.LineItems.First(
                            n => n.ProductId == item.ProductId && n.OrderLineItemType != OrderLineItemType.Discount);
                    if (targetLineItem != null)
                        if (targetLineItem.Qty <= 0)
                            _vm.RemoveLineItem(targetLineItem.SequenceNo);
                }
            }

            if (_vm.LineItems.Any(n => n.BackOrder > 0) || _vm.LineItems.Any(n => n.LostSale > 0))
            {
                dgLineItemsFullGrid.Visibility = Visibility.Visible;
                dgOrderLineItems.Visibility = Visibility.Collapsed;
            }
            else
            {
                dgLineItemsFullGrid.Visibility = Visibility.Collapsed;
                dgOrderLineItems.Visibility = Visibility.Visible;
            }
        }

        private void AddNewLineItemModalClosed()
        {
            SOLineItemViewModel vmLineItem = _newLineItemModal.DataContext as SOLineItemViewModel;

            bool result = _newLineItemModal.DialogResult.Value;
            if (result)
            {
                if (vmLineItem.IsNew)
                {
                    _vm.UpdateOrAddLineItemFromPoductSummary(vmLineItem.ProductAddSummaries, vmLineItem.IsAdd);
                    vmLineItem.MultipleProduct.Clear();
                    vmLineItem.ProductAddSummaries.Clear();
                }
                else
                {
                    //can only update qry???
                    throw new Exception("Was called");
                    ////_vm.UpdateLineItem(vmLineItem.SequenceNo, vmLineItem.Qty);
                }
            }
        }

        private void AddLineItem(object sender, LineItem item)
        {
            if (_vm == null)
                _vm = DataContext as ApproveSalesmanOrderViewModel;

            _vm.AddAmmendedLineItem(item.ProductId, item.Qty);

            if (sender.GetType() == typeof(AmmendOrderLineItemModal))
            {
                var lineItem =
                    _vm.LineItems.First(
                        n => n.ProductId == item.ProductId && n.OrderLineItemType != OrderLineItemType.Discount);

                _vm.AddAmmendedLineItem(lineItem.ProductId, lineItem.Qty, lineItem.SequenceNo);
            }

            _vm.ProcessDiscounts();
        }

        private void UpdateLineItem(object sender, LineItem item)
        {
            if (_vm == null)
                _vm = DataContext as ApproveSalesmanOrderViewModel;
            if (sender.GetType() == typeof(SOLineItemModal))
            {
                _vm.UpdateLineItem(item.SequenceNo,
                                   item.Qty,
                                   item.LineItemVatValue,
                                   item.VatAmount,
                                   item.TotalPrice,
                                   item.BackOrder,
                                   item.LostSale,
                                   true);
            }
            else if (sender.GetType() == typeof(AmmendOrderLineItemModal))
            {
                _vm.UpdateLineItem(item.SequenceNo,
                                   item.Qty,
                                   item.LineItemVatValue,
                                   item.VatAmount,
                                   item.TotalPrice,
                                   item.BackOrder,
                                   item.LostSale,
                                   true);
            }
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            string[] tag = hl.Tag.ToString().Split(',');
            var item = hl.DataContext as ApproveSalesmanOrderViewModel.ApproveSalesmanOrderItem;
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString()); //int.Parse(tag[0].ToString());
            LineItemType lit = LineItemType.Unit; //(LineItemType) int.Parse(tag[1].ToString());
            if (item.ProductType == "ReturnableProduct")
            {
                //string msg = string.Format("\n\t{0} of {1} will be deleted", item.Qty, item.Product);
                string msg = string.Format("\n\t{0} " +
                                           _messageResolver.GetText(
                                               "sl.approveOrder.lineItems.delete.messageBox.productDetails1")
                                           + " {1} " +
                                           _messageResolver.GetText(
                                               "sl.approveOrder.lineItems.delete.messageBox.productDetails2")
                                           , item.Qty, item.Product);
                if (MessageBox.Show( /*"Are sure you want to delete the following product(s)"*/
                        _messageResolver.GetText("sl.approveOrder.lineItems.delete.messageBox.text")
                        + msg,
                    /*"Delete Order Line item"*/
                        _messageResolver.GetText("sl.approveOrder.lineItems.delete.messageBox.title"),
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _vm.RemoveLineItem(item.SequenceNo);
                    _vm.ProcessDiscounts();
                }
            }
            else
                _vm.RemoveLineItem(ParentProductid, lit);
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            int sequenceNo = (int)hl.Tag;
            var lineItem = _vm.LineItems.First(n => n.SequenceNo == sequenceNo);
            //if (lineItem.LineItemId != Guid.Empty)
            //{
            EditApprovedOrderLineItem(sequenceNo);
            //}
            //else
            //{
            //    EditConfirmedOrderLineItem(sequenceNo);
            //}
        }

        private void EditApprovedOrderLineItem(int sequenceNo)
        {
            _ammendApprovedOrderModal = new AmmendOrderLineItemModal();
            
            _ammendApprovedOrderModal.cmbProducts.IsEnabled = false;
            _ammendApprovedOrderModal.lblBackOrder.IsEnabled = true;
            _ammendApprovedOrderModal.txtBackOrder.IsEnabled = true;
            _ammendApprovedOrderModal.lblLostSale.IsEnabled = true;
            _ammendApprovedOrderModal.txtLostSale.IsReadOnly = true;
            _ammendApprovedOrderModal.txtApprove.IsEnabled = true;
            _ammendApprovedOrderModal.txtQty.IsReadOnly = true;
            _ammendApprovedOrderModal.txtAvailableQuantity.IsReadOnly = true;
            _ammendApprovedOrderModal.txtBackOrder.IsReadOnly = true;
          //  _ammendApprovedOrderModal.t
            _ammendApprovedOrderModal.Closed += new EventHandler(modal_Closed);


            ApproveSalesmanOrderViewModel.ApproveSalesmanOrderItem lineItem = null;
            //if (!_vm.ProcessingBackOrder)
            lineItem = _vm.LineItems.First(n => n.SequenceNo == sequenceNo);
            //else
            //    lineItem = _vm.OriginalLineItems.First(n => n.SequenceNo == sequenceNo);

            AmmendSalesmanOrderLineItemViewModel vmLineItem =
                _ammendApprovedOrderModal.DataContext as AmmendSalesmanOrderLineItemViewModel;
            vmLineItem.InstanciateLineItems();
            vmLineItem.DocumentStatus = _vm.Status;
            vmLineItem.SelectedOutletId = _vm.OutletId;
            if (!_vm.ProcessingBackOrder)
            {
                vmLineItem.OriginalQty = lineItem.Qty;
            }
            else
            {
                vmLineItem.OriginalQty = _vm.OriginalLineItems.First(n => n.Key == sequenceNo).Value;
            }

            vmLineItem.ModalCrumbs = /*"Edit Product for Order on Behalf of " */
                _messageResolver.GetText("sl.order.editlineitem.modal.title") + " "
                + _vm.SalesmanUsername;

            vmLineItem.ProductTypeToLoad = AmmendSalesmanOrderLineItemViewModel.ProducTypeToLoad.AllProducts;
            vmLineItem.ClearAndSetup.Execute(null);
            vmLineItem.ProcessingBackOrder = _vm.ProcessingBackOrder;
            vmLineItem._atApproval = true;
            vmLineItem.LoadForEdit(
                lineItem.ProductId,
                lineItem.UnitPrice,
                lineItem.LineItemVatValue,
                lineItem.TotalPrice,
                lineItem.TotalLineItemVatAmount,
                lineItem.SequenceNo,
                lineItem.Qty,
                lineItem.AvailableProductInv,
                lineItem.BackOrder,
                lineItem.Approved
                );

            _ammendApprovedOrderModal.ShowDialog();
        }

        private void btnProcessBackOrder_Click(object sender, RoutedEventArgs e)
        {
            processingBackOrder = true;
            if (MessageBox.Show("Process back order?", "Distributr: Back Order", MessageBoxButton.OKCancel) ==
                MessageBoxResult.OK)
            {
                lblHeader.Content = "Process Back Order";
                _vm = DataContext as ApproveSalesmanOrderViewModel;

                _vm.ProcessBackOrdersCommand.Execute(null);
                _vm.LoadForProcessing = true;
                btnProcessBackOrder.Visibility = Visibility.Collapsed;
                btnAddLineItem.Visibility = Visibility.Collapsed;
                btnReject.Content = "Cancel Back Order";
                dgLineItemsFullGrid.ItemsSource = _vm.LineItems;
            }
        }

        private void dgLineItemsFullGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            StackPanel stackPanel = null;
            ////if (((DataGrid) sender).Name == dgLineItemsFullGrid.Name)
            ////{
            ////    stackPanel =
            ////        coldgLineItemsFullGridEdit").GetCellContent(dataGridrow) as
            ////        StackPanel;
            ////}
            ////else //dgOrderLineItems
            ////{
            ////    stackPanel =
            ////        coldgOrderLineItemsEdit").GetCellContent(dataGridrow) as
            ////        StackPanel;
            ////    ////Hyperlink hl2 = stackPanel.Children[2] as Hyperlink;
            ////    ////hl2.Content = _messageResolver.GetText("sl.approveOrder.lineItemsGrid.deleteLineItem");
            ////}

            
            ////Hyperlink hl = stackPanel.Children[0] as Hyperlink;
            ////hl.Content = _messageResolver.GetText("sl.approveOrder.lineItemsGrid.editLineItem");

        }

    }
}

