using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using Distributr.WPF.Lib.ViewModels.Transactional.POS;
using StructureMap;
using DataGrid = System.Windows.Controls.DataGrid;

namespace Distributr.WPF.UI.Views.POS
{
    public partial class ListPOSSales : Page
    {
        ListPOSSalesViewModel _vm = null;
        ListInvoicesViewModel _livm = null;
        EditPOSOutletSaleViewModel _posvm = null;
        PaymentModeModal _paymentModeModal = null;
        private IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        private BackgroundWorker bwLoadSales = new BackgroundWorker();

        public ListPOSSales()
        {
            bwLoadSales.DoWork += new DoWorkEventHandler(bwLoadSales_DoWork);
            bwLoadSales.ProgressChanged += new ProgressChangedEventHandler(bwLoadSales_ProgressChanged);
            bwLoadSales.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwLoadSales_RunWorkerCompleted);

            _vm = DataContext as ListPOSSalesViewModel;
            InitializeComponent();
            LabelControls();

            SetUpDataPager();

            this.Loaded += new RoutedEventHandler(ListPOSSales_Loaded);

            double newLayoutRootWidth = OtherUtilities.ContentFrameWidth;
            newLayoutRootWidth = (newLayoutRootWidth * 0.95);

            double newLayoutRootHeight = OtherUtilities.ContentFrameHeight;
            newLayoutRootHeight = (newLayoutRootHeight * 0.95);

            double tabControlHeight = (newLayoutRootHeight * 0.825);

            LayoutRoot.Width = newLayoutRootWidth;
            LayoutRoot.Height = newLayoutRootHeight;
            SalesTabControl.Height = tabControlHeight;

#if (KEMSA)
            {
                BackOrdersTabItem.Visibility = Visibility.Collapsed;
                LostSalesTabItem.Visibility = Visibility.Collapsed;
                OutstandingTabItem.Visibility = Visibility.Collapsed;
            }
#endif
        }

        void ListPOSSales_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ListPOSSalesViewModel;
            _vm.ClearAndSetup();
            try
            {
                DataPager.txtTotal.Text = _vm.SalesCount.ToString();
                DataPager.txtPage.Text = _vm.CurrentPage.ToString();

                _posvm = ViewModelLocator.EditPOSOutletSaleViewModelPropertyNameStatic;
                _livm = ViewModelLocator.ListInvoicesViewModelStatic;

                SalesTabControl.SelectedIndex = OtherUtilities.SelectedTabPos;
                string _pendingSales = NavigationService.Source.OriginalString.ParseQueryString("PendingSales");
                if (!string.IsNullOrEmpty(_pendingSales))
                    _vm.PendingSales = Convert.ToBoolean(_pendingSales);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void bwLoadSales_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;
            //worker.ReportProgress((0));
            LoadSales();
            //bwLoadSales.ReportProgress(100);
        }

        void bwLoadSales_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TabItem tabItem = SalesTabControl.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var dataGrid = tabItem.Content as DataGrid;
                if (dataGrid != null) dataGrid.ItemsSource = _vm.Sales;
            }
            DataPager.txtTotal.Text = _vm.PageCount.ToString();
            DataPager.lblTotalItems.Content = _vm.SalesCount.ToString();
            DataPager.EnableOrDisableButtons(_vm.CurrentPage, _vm.PageCount);
            lblProgress.Visibility = Visibility.Collapsed;
        }

        void bwLoadSales_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _vm.PageProgressBar = "Loading ..."/* + (e.ProgressPercentage.ToString() + "%")*/;//report progress here
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
            LoadSales(SalesTabControl.SelectedItem as TabItem);
        }

        void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = 1;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadSales(SalesTabControl.SelectedItem as TabItem);
        }

        void btnLast_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage = _vm.PageCount;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadSales(SalesTabControl.SelectedItem as TabItem);
        }

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage++;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadSales(SalesTabControl.SelectedItem as TabItem);
        }

        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            _vm.CurrentPage--;
            DataPager.txtPage.Text = _vm.CurrentPage.ToString();
            LoadSales(SalesTabControl.SelectedItem as TabItem);
        }

        void LabelControls()
        {
            CompleteSalesTabItem.Header = messageResolver.GetText("sl.listPosSales.completeSalesTab");
            IncompleteTabItem.Header = messageResolver.GetText("sl.listPosSales.incompleteTab");
            BackOrdersTabItem.Header = messageResolver.GetText("sl.listPosSales.backOrdersTab");
            LostSalesTabItem.Header = messageResolver.GetText("sl.listPosSales.lostSalesTab");
            OutstandingTabItem.Header = messageResolver.GetText("sl.listPosSales.outstandingTab");

            //summary
            lblSearchBy.Content = messageResolver.GetText("sl.listPosSales.searchBy_lbl");
            cmdSearch.Content = messageResolver.GetText("sl.listPosSales.search_btn");
            cmdClear.Content = messageResolver.GetText("sl.listPosSales.clear_btn");
            btnAddItem.Content = messageResolver.GetText("sl.listPosSales.addNewSale_btn");

            //Complete Sales Tab
            colDgSalesSaleRef.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.saleNumber");
            colDgSalesDate.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.saleDate");
            colDgSalesStatus.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.status");
            colDgSalesNetAmount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.netAmt");
            colDgSalesVatAmount.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.vatAmt");
            colDgSalesGrossTotal.Header = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.grossTotal");

            //Incomplete Sales Tab
            coldgIncompleteSaleRef.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.saleNumber");
            coldgIncompleteDate.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.saleDate");
            coldgIncompleteStatus.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.status");
            coldgIncompleteNetAmount.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.netAmt");
            coldgIncompleteVatAmount.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.vatAmt");
            coldgIncompleteTotalGross.Header = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.grossTotal");

            //Back Orders Tab
            coldgBackOrdersSaleRef.Header = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.saleNumber");
            coldgBackOrdersDate.Header = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.saleDate");
            coldgBackOrdersStatus.Header = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.status");
            coldgBackOrdersTotalNet.Header = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.netAmt");
            coldgBackOrdersTotalVat.Header = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.vatAmt");
            coldgBackOrdersTotalGross.Header = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.grossTotal");

            //Lost Sales / Unfilled Orders Tab
            coldgLostSalesSalesRef.Header = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.saleNumber");
            coldgLostSalesDate.Header = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.saleDate");
            coldgLostSalesStatus.Header = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.status");
            coldgLostSalesNetAmount.Header = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.netAmt");
            coldgLostSalesVatAmount.Header = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.vatAmt");
            coldgLostSalesGrossAmount.Header = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.grossTotal");

            //Outstanding Payments Tab
            coldgoutStandingPaymentsSalesRef.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.saleNumber");
            coldgoutStandingPaymentsSalesRef.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.saleDate");
            coldgoutStandingPaymentsStatus.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.status");
            coldgoutStandingPaymentsNetAmount.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.netAmt");
            coldgoutStandingPaymentsVatAmount.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.vatAmt");
            coldgoutStandingPaymentsGrossAmount.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.grossTotal");
            coldgoutStandingPaymentsAmountPaid.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.amountPaid");
            coldgoutStandingPaymentsAmountDue.Header = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.outstandingAmount");
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadSales(SalesTabControl.SelectedItem as TabItem);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            LoadSales(SalesTabControl.SelectedItem as TabItem);
            txtSearch.Text = "";
        }

        private void hlnkView_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hpb = (Hyperlink)sender;
            _vm = DataContext as ListPOSSalesViewModel;
            var orderId = new Guid(hpb.Tag.ToString());
            var uri = "";
            if (_vm == null) return;

            if (SalesTabControl.SelectedIndex == SalesTabControl.Items.IndexOf(OutstandingTabItem))
            {
                uri = "/views/TransactionStatement/TransactionStatement.xaml?OrderId=" + orderId;
                OtherUtilities.SelectedTabPos = 4;
                OtherUtilities.StrBackUrl = "/views/pos/listpossales.xaml";
                NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                return;
            }
            _vm.OrderIdLookup = orderId;
            _vm.SelectViewerAndGoCommand.Execute(null);
        }

        private void SalesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source.GetType() != typeof(TabControl))
                return;
            if (_vm == null)
                _vm = DataContext as ListPOSSalesViewModel;
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
            OtherUtilities.SelectedTabPos = tabCntr.SelectedIndex;

            LoadSales(tabItem);
        }

        void LoadSales()
        {
            _vm.LoadSales();
        }

        void LoadSales(ContentControl selectedTab)
        {
            if (_vm == null)
                _vm = DataContext as ListPOSSalesViewModel;

            if (lblProgress != null)
                lblProgress.Visibility = Visibility.Visible;

            var dataGrid = selectedTab.Content as DataGrid;

            switch (selectedTab.Name)
            {
                case "CompleteSalesTabItem":
                    _vm.reportType = ListPOSSalesViewModel.ReportType.Complete;
                    break;
                case "IncompleteTabItem":
                    _vm.reportType = ListPOSSalesViewModel.ReportType.Incomplete;
                    break;
                case "BackOrdersTabItem":
                    _vm.reportType = ListPOSSalesViewModel.ReportType.BackOrders;
                    break;
                case "LostSalesTabItem":
                    _vm.reportType = ListPOSSalesViewModel.ReportType.LostSales;
                    break;
                case "OutstandingTabItem":
                    _vm.reportType = ListPOSSalesViewModel.ReportType.OutstandingPayment;
                    break;
            }
            //if (!loadSearched)
            //{
            //    if (!bwLoadSales.IsBusy)
            //        bwLoadSales.RunWorkerAsync();
            //}
            //else
            //{
            //    _vm.LoadSalesBySearchTextCommand.Execute(null);
            //    dataGrid.ItemsSource = _vm.Sales;
            //    lblProgress.Visibility = Visibility.Collapsed;
            //}

            if (!bwLoadSales.IsBusy)
                bwLoadSales.RunWorkerAsync();
        }

        void ClearAllDataGrids()
        {
            if (SalesTabControl != null)
                foreach (var item in SalesTabControl.Items)
                {
                    var tab = item as TabItem;
                    var dataGrid = tab.Content as DataGrid;

                    if (dataGrid != null) dataGrid.ItemsSource = null;
                }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text)) return;
            btnClear_Click(this, null);
        }

        private void hlView_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.CanReceivePayments)
            {
                MessageBox.Show("Sorry, you do not have sufficient rights to perform this action.", "Distributr: POS",
                                MessageBoxButton.OK);
                return;
            }
            _livm.InvoiceNo = ((Hyperlink)sender).Tag.ToString();
            SelectPaymentMode();
        }

        void SelectPaymentMode()
        {
            try
            {
                _livm.LoadGetInvoiceAmountsCommand.Execute(null);
                _paymentModeModal = new PaymentModeModal();
                _paymentModeModal.Closed += new EventHandler(paymentModeModal_Closed);
                var pvm = _paymentModeModal.DataContext as PaymentModeViewModel;
                pvm.ClearAndSetup.Execute(null);
                _posvm.PaymentInfoList.Clear();
                pvm.AmountPaid = _livm.TotalPaid;
                pvm.GrossAmount = _livm.GrossTotal;
                pvm.SetAmntPaid(_livm.TotalPaid);

                pvm.GetOrder(_livm.OrderId);
                pvm.OrderOutletId = pvm.TheOrder.IssuedOnBehalfOf.Id;
                pvm.GetOrderOutlet();
                pvm.SetUpSubscriber();
                //pvm.Setup();
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
                            //                 );
                            _posvm.OrderIdLookup = _livm.OrderId;
                            _posvm.DocumentRef = _livm.InvoiceRef;
                            _posvm.InvoiceIdLookUp = new Guid(_livm.InvoiceNo);
                            _posvm.ConfirmPaymentCommand.Execute(null);
                            //_livm.LoadUpaidInvoicesCommand.Execute(null);
                            MessageBox.Show("Payment Successful");
                            LoadSales(SalesTabControl.SelectedItem as TabItem);
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

        private void dgSales_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //WPF Hyperlink hl = dgSales.Columns.GetByName("colDgSalesView").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hl.Content = messageResolver.GetText("sl.listPosSales.completeSales.grid.col.view_lnk");
        }

        private void dgIncomplete_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //WPF Hyperlink hl = dgIncomplete.Columns.GetByName("coldgIncompleteView").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hl.Content = messageResolver.GetText("sl.listPosSales.incompleteSales.grid.col.view_lnk");
        }

        private void dgBackOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //WPF Hyperlink hl = dgSales.Columns.GetByName("coldgBackOrdersView").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hl.Content = messageResolver.GetText("sl.listPosSales.backOrders.grid.col.view_lnk");

        }

        private void dgLostSales_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //WPF Hyperlink hl = dgLostSales.Columns.GetByName("coldgLostSalesView").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hl.Content = messageResolver.GetText("sl.listPosSales.unfilledOrders.grid.col.view_lnk");

        }

        private void dgoutStandingPayments_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //WPF Hyperlink hl = dgoutStandingPayments.Columns.GetByName("coldgoutStandingPaymentsView").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hl.Content = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.view_lnk");

            //WPF Hyperlink hl2 = dgoutStandingPayments.Columns.GetByName("coldgoutStandingPaymentsReceivePayments").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hl2.Content = messageResolver.GetText("sl.listPosSales.outstandingPayments.grid.col.receivePayments_lnk");

        }

        private void cmdLoadSales_Click(object sender, RoutedEventArgs e)
        {
            btnFirst_Click(this, null);
        }
    }
}
