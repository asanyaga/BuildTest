using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using Distributr.WPF.Lib.ViewModels.Transactional.POS;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.UI.Views.POS;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Distributr.WPF.UI.Views.SalesmanOrders
{
    public partial class ListSalesmanOrders : Page
    {
      private ListSalesmanOrdersViewModel _vm;
        private ListInvoicesViewModel _livm = null;
        private PaymentModeModal _paymentModeModal = null; 
        private EditPOSOutletSaleViewModel _posvm = null;
        private BackgroundWorker _bw;
        IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public ListSalesmanOrders()
        {
            _bw = new BackgroundWorker();
            _bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            _bw.WorkerReportsProgress = true;
            _bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            _bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            InitializeComponent();
            SetUpDataPager();
            LabelControls();
            this.Loaded += new RoutedEventHandler(ListSalesmasOrders_Loaded);
            this.Unloaded += new RoutedEventHandler(ListSalesManOrder_Unloaded);

            double newLayoutRootWidth = OtherUtilities.ContentFrameWidth;
            newLayoutRootWidth = (newLayoutRootWidth * 0.95);

            double newLayoutRootHeight = OtherUtilities.ContentFrameHeight;
            newLayoutRootHeight = (newLayoutRootHeight * 0.985);

            double tabControlHeight = (newLayoutRootHeight * 0.820);

            LayoutRoot.Width = newLayoutRootWidth;
            LayoutRoot.Height = newLayoutRootHeight;
            tabOrders.Height = tabControlHeight;
        }

        #region Localization & Datapager

        void LabelControls()
        {
            lblSearchText.Content = messageResolver.GetText("sl.orderSummary.searchby_lbl");
            btnSearch.Content = messageResolver.GetText("sl.orderSummary.search_btn");
            btnClear.Content = messageResolver.GetText("sl.orderSummary.clear_btn");
            btnAddItem.Content = messageResolver.GetText("sl.orderSummary.addOrder_btn");
            btnDispatch.Content = messageResolver.GetText("sl.orderSummary.dispatch_btn");

            //Pending Approval Tab
            tabItemPendingApprovals.Header = messageResolver.GetText("sl.orderSummary.pendingApproval.tab");
            coldgPendingApprovalDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.orderRef");
            coldgPendingApprovalDateRequired.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.dateRequired");
            coldgPendingApprovalSalesman.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.salesman");
            coldgPendingApprovalStatus.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.status");
            coldgPendingApprovalTotalNet.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.netAmount");
            coldgPendingApprovalTotalVat.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.totalVat");
            coldgPendingApprovalTotalGross.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.grossAmount");
            coldgPendingApprovalTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.amountPaid");
            coldgPendingApprovalTotalDue.Header = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.outstandingAmount");

            //Pending Dispatch Tab
            tabItemPendingDispatch.Header = messageResolver.GetText("sl.orderSummary.pendingDispatch.tab");
            coldgPendingDispatchDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.orderRef");
            coldgPendingDispatchDate.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.dateRequired");
            coldgPendingDispatchSalesman.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.salesman");
            coldgPendingDispatchStatus.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.status");
            coldgPendingDispatchTotalNet.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.netAmount");
            coldgPendingDispatchTotalVat.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.totalVat");
            coldgPendingDispatchTotalGross.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.grossAmount");
            coldgPendingDispatchTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.amountPaid");
            coldgPendingDispatchTotalDue.Header = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.outstandingAmount");

            //Pending Dispatched Tab
            tabItemDispatchedOrders.Header = messageResolver.GetText("sl.orderSummary.dispatched.tab");
            coldgDispatchedOrdersDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.orderRef");
            coldgDispatchedOrdersDate.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.dateRequired");
            coldgDispatchedOrdersSalesman.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.salesman");
            coldgDispatchedOrdersStatus.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.status");
            coldgDispatchedOrdersTotalNet.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.netAmount");
            coldgDispatchedOrdersTotalVat.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.totalVat");
            coldgDispatchedOrdersTotalGross.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.grossAmount");
            coldgDispatchedOrdersTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.amountPaid");
            coldgDispatchedOrdersTotalDue.Header = messageResolver.GetText("sl.ordersSummary.dispatched.grid.col.outstandingAmount");

            //Pending Incomplete Tab
            tabItemIncomplete.Header = messageResolver.GetText("sl.orderSummary.incomplete.tab");
            coldgIncompleteDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.orderRef");
            coldgIncompleteDate.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.dateRequired");
            coldgIncompleteSalesman.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.salesman");
            coldgIncompleteStatus.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.status");
            coldgIncompleteTotalNet.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.netAmount");
            coldgIncompleteTotalVat.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.totalVat");
            coldgIncompleteTotalGross.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.grossAmount");
            coldgIncompleteTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.amountPaid");
            coldgIncompleteTotalDue.Header = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.outstandingAmount");

            //Pending Delivered Tab
            tabItemDelivered.Header = messageResolver.GetText("sl.orderSummary.delivered.tab");
            coldgDeliveredDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.orderRef");
            coldgDeliveredDate.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.dateRequired");
            coldgDeliveredSalesman.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.salesman");
            coldgDeliveredStatus.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.status");
            coldgDeliveredTotalNet.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.netAmount");
            coldgDeliveredTotalVat.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.totalVat");
            coldgDeliveredTotalGross.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.grossAmount");
            coldgDeliveredTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.amountPaid");
            coldgDeliveredTotalDue.Header = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.outstandingAmount");

            //Pending Outstanding Payments Tab
            tabItemPartiallyPaidDeliveries.Header = messageResolver.GetText("sl.orderSummary.outstandingPayment.tab");
            coldgPartiallyPaidDeliveriesDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.orderRef");
            coldgPartiallyPaidDeliveriesDate.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.dateRequired");
            coldgPartiallyPaidDeliveriesSalesman.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.salesman");
            coldgPartiallyPaidDeliveriesStatus.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.status");
            coldgPartiallyPaidDeliveriesTotalNet.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.netAmount");
            coldgPartiallyPaidDeliveriesTotalVat.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.totalVat");
            coldgPartiallyPaidDeliveriesTotalGross.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.grossAmount");
            coldgPartiallyPaidDeliveriesTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.amountPaid");
            coldgPartiallyPaidDeliveriesTotalDue.Header = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.outstandingAmount");

            //Pending Fully Paid Tab
            tabItemFullyPaidDeliveries.Header = messageResolver.GetText("sl.orderSummary.fullyPaid.tab");
            coldgFullyPaidDeliveriesDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.orderRef");
            coldgFullyPaidDeliveriesDate.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.dateRequired");
            coldgFullyPaidDeliveriesSalesman.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.salesman");
            coldgFullyPaidDeliveriesStatus.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.status");
            coldgFullyPaidDeliveriesTotalNet.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.netAmount");
            coldgFullyPaidDeliveriesTotalVat.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.totalVat");
            coldgFullyPaidDeliveriesTotalGross.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.grossAmount");
            coldgFullyPaidDeliveriesTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.amountPaid");
            coldgFullyPaidDeliveriesTotalDue.Header = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.outstandingAmount");

            //Pending Back Orders Tab
            tabItemBackOrders.Header = messageResolver.GetText("sl.orderSummary.backOrders.tab");
            coldgBackOrdersDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.orderRef");
            coldgBackOrdersDate.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.dateRequired");
            coldgBackOrdersSalesman.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.salesman");
            coldgBackOrdersStatus.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.status");
            coldgBackOrdersTotalNet.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.netAmount");
            coldgBackOrdersTotalVat.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.totalVat");
            coldgBackOrdersTotalGross.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.grossAmount");
            coldgBackOrdersTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.amountPaid");
            coldgBackOrdersTotalDue.Header = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.outstandingAmount");

            //Pending Lost Sales Tab
            tabItemLostSales.Header = messageResolver.GetText("sl.orderSummary.lostSales.tab");
            coldgLostSalesDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.orderRef");
            coldgLostSalesDate.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.dateRequired");
            coldgLostSalesSalesman.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.salesman");
            coldgLostSalesStatus.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.status");
            coldgLostSalesTotalNet.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.netAmount");
            coldgLostSalesTotalVat.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.totalVat");
            coldgLostSalesTotalGross.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.grossAmount");
            coldgLostSalesTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.amountPaid");
            coldgLostSalesTotalDue.Header = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.outstandingAmount");

            //Pending Rejected Tab
            tabItemRejectedOrders.Header = messageResolver.GetText("sl.orderSummary.rejected.tab");
            coldgRejectedOrdersDocumentRef.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.orderRef");
            coldgRejectedOrdersDate.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.dateRequired");
            coldgRejectedOrdersSalesman.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.salesman");
            coldgRejectedOrdersStatus.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.status");
            coldgRejectedOrdersTotalNet.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.netAmount");
            coldgRejectedOrdersTotalVat.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.totalVat");
            coldgRejectedOrdersTotalGross.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.grossAmount");
            coldgRejectedOrdersTotalPaid.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.amountPaid");
            coldgRejectedOrdersTotalDue.Header = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.outstandingAmount");
        }

        void SetUpDataPager()
        {
            DataPager.btnFirst.Click += new RoutedEventHandler(btnFirst_Click);
            DataPager.btnPrevious.Click += new RoutedEventHandler(btnPrevious_Click);
            DataPager.btnNext.Click += new RoutedEventHandler(btnNext_Click);
            DataPager.btnLast.Click += new RoutedEventHandler(btnLast_Click);
            DataPager.btnGoTo.Click += new RoutedEventHandler(btnGoTo_Click);
        }

        void btnGoTo_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = Convert.ToInt32(DataPager.txtPage.Text);
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = 1;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        void btnLast_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = _vm.PageCount;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage++;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage--;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        #endregion

        void ListSalesManOrder_Unloaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.ClearViewModel();
        }

        void ListSalesmasOrders_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as ListSalesmanOrdersViewModel;
            _vm.SetUp();

            string navParam = PresentationUtility.GetLastTokenFromUri(NavigationService.CurrentSource);
            switch (navParam)
            {
                case "PendingApprovals":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemPendingApprovals);
                    break;
                case "Incomplete":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemIncomplete);
                    break;
                case "PendingDispatch":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemPendingDispatch);
                    break;
                case "DispatchedOrders":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemDispatchedOrders);
                    break;
                case "Delivered":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemDelivered);
                    break;
                case "PartiallyPaidDeliveries":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemPartiallyPaidDeliveries);
                    break;
                case "FullyPaidDeliveries":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemFullyPaidDeliveries);
                    break;
                case "BackOrders":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemBackOrders);
                    break;
                case "Rejected":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemRejectedOrders);
                    break;
                case "LostSales":
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemLostSales);
                    break;
                default:
                    tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemPendingApprovals);
                    break;
            }
        }

        void SelectPaymentMode(Guid orderId)
        {
            try
            {
                _posvm = ViewModelLocator.EditPOSOutletSaleViewModelPropertyNameStatic;
                _livm = ViewModelLocator.ListInvoicesViewModelStatic;
                _livm.InvoiceNo = _vm.getInvoiceId(orderId).ToString();

                _livm.LoadGetInvoiceAmountsCommand.Execute(null);
                _paymentModeModal = new PaymentModeModal();
                _paymentModeModal.Closed += paymentModeModal_Closed;
                var pvm = _paymentModeModal.DataContext as PaymentModeViewModel;
                pvm.RunClearAndSetup();
                _posvm.PaymentInfoList.Clear();
                pvm.AmountPaid = _livm.TotalPaid;
                pvm.GrossAmount = _livm.GrossTotal;
                pvm.SetAmntPaid(_livm.TotalPaid);
                //pvm.Setup();

                pvm.GetOrder(_livm.OrderId);
                pvm.InvoiceDocReference = _livm.InvoiceRef;
                pvm.OrderOutletId = pvm.TheOrder.IssuedOnBehalfOf.Id;
                pvm.GetOrderOutlet();
                pvm.SetUpSubscriber();

                _paymentModeModal.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void paymentModeModal_Closed(object sender, EventArgs e)
        {
            var pvm = _paymentModeModal.DataContext as PaymentModeViewModel;
            _posvm = ViewModelLocator.EditPOSOutletSaleViewModelPropertyNameStatic;
            try
            {
                if (_paymentModeModal.DialogResult.Value)
                {
                    if (pvm != null)
                    {
                        if (pvm.CashAmount + pvm.MMoneyAmount + pvm.ChequeAmount > 0)
                        {
                            throw new NotImplementedException();
                            //_posvm.AddPaymentInfo(pvm.CashAmount,
                            //                 pvm.CreditAmount,
                            //                 pvm.MMoneyAmount,
                            //                 pvm.ChequeAmount,
                            //                 pvm.AmountPaid,
                            //                 pvm.PaymentRef,
                            //                 pvm.ChequeNumber,
                            //                 pvm.GrossAmount,
                            //                 pvm.Change,
                            //                 pvm.SelectedBank,
                            //                 pvm.SelectedBankBranch,
                            //                 pvm.SelectedMMoneyOption != null ? pvm.SelectedMMoneyOption.Name : "",
                            //                 pvm.MMoneyIsApproved,
                            //                 pvm.PaymentTransactionRefId,
                            //                 pvm.AccountNo,
                            //                 pvm.SubscriberId,
                            //                 pvm.TillNumber,
                            //                 pvm.Currency,
                            //                 pvm.PaymentNotification,
                            //                 pvm.PaymentResponse
                                             //);
                            _posvm.OrderIdLookup = _livm.OrderId;
                            _posvm.DocumentRef = _livm.InvoiceRef;
                            _posvm.InvoiceIdLookUp = new Guid(_livm.InvoiceNo);
                            _posvm.ConfirmPaymentCommand.Execute(null);
                            //_livm.LoadUpaidInvoicesCommand.Execute(null);
                            //Reload();
                            LoadOrders(tabItemPartiallyPaidDeliveries);
                            if (MessageBox.Show(
                                "Payment Successful.\nClick 'OK' to view fully paid for orders, 'Cancel' to remain in this tab",
                                "Distributr: Make Payment", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                tabOrders.SelectedIndex = tabOrders.Items.IndexOf(tabItemFullyPaidDeliveries);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pvm.ClearAndSetup.Execute(null);
                MessageBox.Show(ex.Message);

            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtSearchText.Text = "";
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = 1;
            LoadOrders(tabOrders.SelectedItem as TabItem);
        }

        private void txtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearchText.Text.Trim() == "")
            {
                btnClear_Click(this, null);
            }
        }

        private void hlnkEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            var uri = "";
            var orderId = new Guid(hl.Tag.ToString());
            if (_vm == null)
                _vm = DataContext as ListSalesmanOrdersViewModel;

            if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemPendingDispatch)
                || tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemDelivered)
                || tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemBackOrders))
            {
                uri = "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + orderId + "&ProcessBackOrder";
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemPartiallyPaidDeliveries))
            {
                if (!_vm.CanReceivePayments)
                {
                    MessageBox.Show("Sorry, you do not have sufficient rights to perform this action.", "Distributr: POS",
                                    MessageBoxButton.OK);
                    return;
                }
                SelectPaymentMode(orderId);
            }
            if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemRejectedOrders))
            {
                var orderRef = _vm.Orders.First(n => n.OrderId == orderId);
                MessageBox.Show("", "Distributr: Reason for rejecting order "+orderRef+".", MessageBoxButton.OK);
            }
        }

        private void hlnkView_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink) sender;
            var uri = "";
            var orderId = new Guid(hl.Tag.ToString());
            _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.OrderIdLookup = new Guid(hl.Tag.ToString());

            if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemBackOrders))
            {
                //load back order only
                uri = "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + orderId + "&ViewBackOrder";
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemDispatchedOrders))
            {
                uri = "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + orderId + "&ViewDispatched";
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemPartiallyPaidDeliveries)
                     || tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemFullyPaidDeliveries))
            {
                uri = "/views/TransactionStatement/TransactionStatement.xaml?OrderId=" + orderId;
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemLostSales))
            {
                uri = "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + orderId + "&ViewLostSales";
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }
            else if(tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemDelivered))
            {
                uri = "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + orderId + "&ViewDelivered";
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            }

            else
                _vm.SelectViewerAndGoCommand.Execute(null);
        }

        private void hlDocRef_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (Hyperlink)sender;
            _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.OrderIdLookup = new Guid(hl.Tag.ToString());

            _vm.SelectViewerAndGoCommand.Execute(null);
        }

        private void hlnkProcess_Click(object sender, RoutedEventArgs e)
        {
            var hlk = sender as Hyperlink;
            var orderId = new Guid(hlk.Tag.ToString());

            if (!_vm.CanApproveOrder)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action.", "Distributr: POS",
                                MessageBoxButton.OK);
                return;
            }

            string url = "/views/salesmanorders/approvesalemanorders.xaml?orderid=" + orderId;
            if (tabOrders.SelectedIndex == tabOrders.Items.IndexOf(tabItemIncomplete))
                url = "/views/salesmanorders/editsalesmanorder.xaml?orderid=" + orderId;
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void tabOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;
            if (_vm == null)
                _vm = this.DataContext as ListSalesmanOrdersViewModel;
            _vm.CurrentPage = 1;
            _vm.SearchText = "";
            if (DataPager != null)
            {
                DataPager.EnableOrDisableButtons(_vm.CurrentPage, _vm.PageCount);
                DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            }

            ClearAllDataGrids();

            TabControl tabCntr = sender as TabControl;
            TabItem tabItem = tabCntr.SelectedItem as TabItem;

            LoadOrders(tabItem);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;
            //worker.ReportProgress((0));
            //LoadOrders();
            //_bw.ReportProgress(100);
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _vm.PageProgressBar = "Loading ....." + (e.ProgressPercentage.ToString() + "%");//report progress here
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadOrders();
            if (e.Cancelled)
            {
                MessageBox.Show("Chris Kali");
                return;
            }
            TabItem tabItem = tabOrders.SelectedItem as TabItem;
            var dataGrid = tabItem.Content as DataGrid;
            dataGrid.ItemsSource = _vm.Orders;

            DataPager.txtTotal.Text = _vm.PageCount.ToString();
            DataPager.lblTotalItems.Content = _vm.OrdersCount.ToString();
            DataPager.EnableOrDisableButtons(_vm.CurrentPage, _vm.PageCount);

            lblProgress.Visibility = Visibility.Collapsed;
        }

        void ClearAllDataGrids()
        {
            if (tabOrders != null)
                //foreach (var item in tabOrders.Items)
                //{
                //    var tab = item as TabItem;
                //    var dataGrid = tab.Content as DataGrid;

                //    if (dataGrid != null) dataGrid.ItemsSource = null;
                //}
                _vm.Orders.Clear();
        }

        void LoadOrders()
        {
            if (_vm == null)
                _vm = DataContext as ListSalesmanOrdersViewModel;
            _vm.LoadPendingOrdersCommand.Execute(null);
        }

        private void LoadOrders(ContentControl selectedTab)
        {
            if (_vm == null)
                _vm = DataContext as ListSalesmanOrdersViewModel;

            if (lblProgress != null)
                lblProgress.Visibility = Visibility.Visible;

            if (btnDispatch != null)
                btnDispatch.Visibility = Visibility.Collapsed;

           
            switch (selectedTab.Name)
            {
                case "tabItemPendingApprovals":
                    //not approved
                    if (PageTitle != null)
                    {
                        //PageTitle.Text = "List of Orders Awaiting Approval";
                        PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.pendingApproval");
                    }
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.PendingApproval;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?PendingApprovals";
                    break;
                case "tabItemPendingDispatch":
                    //approved not dispatched 
                    //PageTitle.Text = "List of Orders Pending Dispatch";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.pendingDispatch");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.PendingDispatch;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?PendingDispatch";
                    btnDispatch.Visibility = Visibility.Visible;
                    break;
                case "tabItemDispatchedOrders":
                    //of status = dispatched 
                    //PageTitle.Text = "List of Dispatched Orders";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.dispatched");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.DispatchedOrders;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?DispatchedOrders";
                    break;
                case "tabItemIncomplete":
                    //of status new
                    //PageTitle.Text = "List of Incomplete Orders";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.incomplete");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.Incomplete;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?Incomplete";
                    break;
                case "tabItemDelivered":
                    //all delivered of status closed
                    //PageTitle.Text = "List of Delivered Orders";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.delivered");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.Delivered;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?Delivered";
                    break;
                case "tabItemPartiallyPaidDeliveries":
                    //all delivered but not paid fully
                    //PageTitle.Text = "List of Outstanding Payments";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.outstandingPayment");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.PartiallyPaidDeliveries;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?PartiallyPaidDeliveries";
                    break;
                case "tabItemFullyPaidDeliveries":
                    //of status closed and fully paid for note payment tracked from receipts
                    //PageTitle.Text = "List of Fully Paid Deliveries";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.fullyPaid");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.FullyPaidDeliveries;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?FullyPaidDeliveries";
                    break;
                case "tabItemBackOrders":
                    //of status closed and payments made in full note payment tracked from receipts
                    //PageTitle.Text = "List of Orders With Back Order";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.backOrders");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.BackOrders;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?BackOrders";
                    break;
                case "tabItemRejectedOrders":
                    //of status rejected
                    //PageTitle.Text = "List of Rejected Orders";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.rejected");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.Rejected;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?Rejected";
                    break;
                case "tabItemLostSales":
                    //PageTitle.Text = "List of Lost Sales";
                    PageTitle.Text = messageResolver.GetText("sl.orderSummary.Title.lostSales");
                    _vm.OrdersToLoad = ListSalesmanOrdersViewModel.EnumOrdersToLoad.LostSales;
                    OtherUtilities.StrBackUrl = "/views/salesmanorders/listsalesmanorders.xaml?LostSales";
                    break;
            }
            //if (_bw != null)
            //{
            //    _bw.CancelAsync();
            //    _bw.Dispose();
            //}

            //_bw = CreateBackGroundWorker();
            if(!_bw.IsBusy)
            _bw.RunWorkerAsync();
        }

        private BackgroundWorker CreateBackGroundWorker()
        {
            var bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;

            return bw;
        }

        private void btnDispatch_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/DispatchPendingOrdersToPhone/DispatchPendingOrdersToPhone.xaml",
                                               UriKind.Relative));
        }

        private void dgPendingApproval_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            var item = coldgPendingApprovalView.GetCellContent(dataGridrow);
            ////Hyperlink hl = coldgPendingApprovalView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.view");

            ////Hyperlink hl2 = coldgPendingApprovalProcess").GetCellContent(dataGridrow) as Hyperlink;
            ////hl2.Content = messageResolver.GetText("sl.ordersSummary.pendingApproval.grid.col.process");
        }

        private void dgPendingDispatch_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgPendingDispatchView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.view");

            ////Hyperlink hl2 = coldgPendingDispatchEdit").GetCellContent(dataGridrow) as Hyperlink;
            ////hl2.Content = messageResolver.GetText("sl.ordersSummary.pendingDispatch.grid.col.processBackOrder");
        }

        private void dgIncomplete_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgIncompleteView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.view");

            ////Hyperlink hl2 = coldgIncompleteProcess").GetCellContent(dataGridrow) as Hyperlink;
            ////hl2.Content = messageResolver.GetText("sl.ordersSummary.incomplete.grid.col.confirm");

        }

        private void dgDelivered_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgDeliveredView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.delivered.grid.col.view");

        }

        private void dgPartiallyPaidDeliveries_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgPartiallyPaidDeliveriesView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.view");

            ////Hyperlink hl2 = coldgPartiallyPaidDeliveriesEdit").GetCellContent(dataGridrow) as Hyperlink;
            ////hl2.Content = messageResolver.GetText("sl.ordersSummary.outstandingPayment.grid.col.receivePayment");
        }

        private void dgFullyPaidDeliveries_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ////DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgFullyPaidDeliveriesView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.fullyPaid.grid.col.view");
        }

        private void dgBackOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgBackOrdersView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.view");

            ////Hyperlink hl2 = coldgBackOrdersEdit").GetCellContent(dataGridrow) as Hyperlink;
            ////hl2.Content = messageResolver.GetText("sl.ordersSummary.backOrders.grid.col.processBackOrders");
        }

        private void dgLostSales_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ////DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgLostSalesView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.lostSales.grid.col.view");
        }

        private void dgRejectedOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ////DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            ////Hyperlink hl = coldgRejectedOrdersView").GetCellContent(dataGridrow) as Hyperlink;
            ////hl.Content = messageResolver.GetText("sl.ordersSummary.rejected.grid.col.view");
        }

        private void cmdLoadSales_Click(object sender, RoutedEventArgs e)
        {
            btnFirst_Click(this, null);
        }
    }
}
