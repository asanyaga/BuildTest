using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Resources.Util;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.Hierarchy;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using Distributr.WPF.Lib.ViewModels.Transactional.POS;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.UI.Views.Payments;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.POS.PostOrderRefactoring
{
    public partial class EditPOS : PageBase
    {
        EditPOSOutletSaleViewModel _vm;
        POSLineItemModal _lineItemModal = null;
        PaymentModeModal _paymentModeModal = null;
        readonly IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool go_back_on_btnCancel_click = true;
        private bool isInitialized = false;
        public EditPOS()
        {
            isInitialized = false;
            InitializeComponent();
            _vm = DataContext as EditPOSOutletSaleViewModel;
            isInitialized = true;
            LabelControls();
            go_back_on_btnCancel_click = true;
            this.Loaded += new RoutedEventHandler(AddPOSSale_Loaded);

#if (KEMSA)
            {
    lblSaleDiscount.Visibility = Visibility.Collapsed;
    txtSalesDiscount.Visibility = Visibility.Collapsed;
    lblReturnablesValue.Visibility = Visibility.Collapsed;
    txtReturnables.Visibility = Visibility.Collapsed;
    lblAmountPaid.Visibility = Visibility.Collapsed;
    txtAmountPaid.Visibility = Visibility.Collapsed;
    PaymentsDataGrid.Visibility = Visibility.Collapsed;
}
#endif
        }



        void LabelControls()
        {
            lblSalesId.Content = _messageResolver.GetText("sl.createSale.saleid_lbl");
            lblDateRequired.Content = _messageResolver.GetText("sl.createSale.date_lbl");
            lblSalesman.Content = _messageResolver.GetText("sl.createSale.salesman_lbl");
            lblRoute.Content = _messageResolver.GetText("sl.createSale.route_lbl");
            lblOutlet.Content = _messageResolver.GetText("sl.createSale.outlet_lbl");
            lblStatus.Content = _messageResolver.GetText("sl.createSale.status_lbl");
            lblTotalDiscount.Content = _messageResolver.GetText("sl.createSale.totalProductDiscount_lbl");
            lblTotalNetAmnt.Content = _messageResolver.GetText("sl.createSale.totalNet_lbl");
            lblTotalVat.Content = _messageResolver.GetText("sl.createSale.totalVat_lbl");
            lblSaleValue.Content = _messageResolver.GetText("sl.createSale.saleValue_lbl");
            lblReturnablesValue.Content = _messageResolver.GetText("sl.createSale.returnablesValue_lbl");
            lblSaleDiscount.Content = _messageResolver.GetText("sl.createSale.saleDiscount_lbl");
            lblTotalGross.Content = _messageResolver.GetText("sl.createSale.totalGross_lbl");
            lblAmountPaid.Content = _messageResolver.GetText("sl.createSale.amountPaid_lbl");

            //cn: buttons
            btnAddLineItem.Content = _messageResolver.GetText("sl.createSale.addProduct_btn");
            btnConfirmOrder.Content = _messageResolver.GetText("sl.createSale.completeSale_btn");
            btnReceiveReturnables.Content = _messageResolver.GetText("sl.createSale.receiveReturnables_btn");
            btnViewInvoice.Content = _messageResolver.GetText("sl.createSale.viewInvoice_btn");
            btnViewReceipt.Content = _messageResolver.GetText("sl.createSale.viewReceipt_btn");
            btnNewSale.Content = _messageResolver.GetText("sl.pos.newsale_btn");
            btnReceivePayments.Content = _messageResolver.GetText("sl.createSale.receivePayments_btn");
            btnSave.Content = _messageResolver.GetText("sl.createSale.saveAndContinueLater_btn");

            //payment grid
            var item = PaymentsDataGrid.Columns.GetByName("colPaymentType");
            colPaymentType.Header = _messageResolver.GetText("sl.pos.payment.grid.col.paymenttype");
            colAmount.Header = _messageResolver.GetText("sl.pos.payment.grid.col.amount");
            colConfirmed.Header = _messageResolver.GetText("sl.pos.payment.grid.col.confirmed");

            //Line Items grid
            colProduct.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.product");
            colQty.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.qty");
            colUnitPrice.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.unitPrice");
            colUnitVat.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.unitVat");
            colUnitDisc.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.unitDisc");
            colTotal.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.total");
            colProductType.Header = _messageResolver.GetText("sl.pos.lineitems.grid.col.producttype");


        }

        void AddPOSSale_Loaded(object sender, RoutedEventArgs e)
        {
            Load();

            _vm.FireSalesmanChangedCmd = true;
            _vm.FireRouteChangedCmd = true;
            _vm.FireOutletChangeCmd = true;

        }

        private void Load()
        {
            _vm.RunClearAndSetup();
            cmbOutlets.IsEnabled = true;
            cmbRoutes.IsEnabled = true;
            cmbSalesman.IsEnabled = true;
            string _orderid = NavigationService.Source.OriginalString.ParseQueryString("orderid");
            string _loadforviewing = NavigationService.Source.OriginalString.ParseQueryString("loadforviewing");
            if (!string.IsNullOrEmpty(_orderid))
            {
                string orderId = _orderid;
                _vm.OrderIdLookup = new Guid(orderId);
            }
            if (!string.IsNullOrEmpty(_loadforviewing))
            {
                string loadForViewing = _loadforviewing;
                _vm.LoadForViewing = Convert.ToBoolean(loadForViewing);
                _vm.LoadForEditing = false;
                _vm.CancelButtonContent = _messageResolver.GetText("sl.pos.back_btn");/*"Back";*/
                go_back_on_btnCancel_click = true;
                _vm.LoadInvoiceAndReceipts();
                btnNewSale.Visibility = Visibility.Visible;
                _vm.ConfirmNavigatingAway = false;
            }
            else
            {
                if (_vm.OrderIdLookup != Guid.Empty)
                {
                    cmbOutlets.IsEnabled = false;
                    cmbRoutes.IsEnabled = false;
                    cmbSalesman.IsEnabled = false;
                }
                _vm.LoadForEditing = true;
                _vm.LoadForViewing = false;
                _vm.CancelButtonContent = _messageResolver.GetText("sl.pos.cancel_btn");/*"Cancel Sale";*/
                go_back_on_btnCancel_click = false;
                btnReceivePayments.Visibility = Visibility.Visible;
                btnNewSale.Visibility = Visibility.Collapsed;
            }

            _vm.LoadOrderCommand.Execute(null);
            if (_vm.LoadForViewing)
            {
                btnReceivePayments.Visibility = Visibility.Collapsed;
            }
        }

        private void cmbSalesman_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;

            if (!_vm.FireSalesmanChangedCmd)
                return;
            _vm.SalesManChangedCommand.Execute(null);
        }

        private void cmbRoutes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            if (!_vm.FireRouteChangedCmd)
            {
                return;
            }
            _vm.RouteChangedCommand.Execute(null);
        }

        private void cmbOutlets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            if (_vm.SelectedOutlet != null && _vm.SelectedOutlet.Id != Guid.Empty && _vm.IsEditing == false)
                _vm.GenerateSaleId();
        }

        private void btnAddLineItem_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.IsValid())
            {
                return;
            }
            _lineItemModal = new POSLineItemModal();
            _lineItemModal.Closed += new EventHandler(lineItemModal_Closed);
            _lineItemModal.cmbProducts.IsEnabled = true;
            _lineItemModal.rbUnits.IsChecked = true;
            _lineItemModal.cmdAddProduct.Visibility = Visibility.Visible;
            _lineItemModal.lblAvailable.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.availableInv");
            /*"Available Inventory";*/
            var vmItem = _lineItemModal.DataContext as POSLineItemViewModel;
            _vm.CreateNewOrder();
            _vm.ReceiveReturnable = false;
            vmItem.ReceiveReturnables = false;
            vmItem.LineItemType = LineItemType.Unit;
            vmItem.LineItems.Clear();
            vmItem.AddedLineItems = new List<OrderLineItemProductLookupItem>();
            vmItem.ModalCrumbs =
                _messageResolver.GetText("sl.pos.addlineitem.modal.title") /*"Add Product(s) To Order"*/+ " " +
                _vm.OrderId;
            if (_vm.LineItems.Count > 0)
            {
                vmItem.AddedLineItems = _vm.LineItems.Select(n => new OrderLineItemProductLookupItem
                {
                    ProductId = n.ProductId,
                    LineItemQty = n.Qty
                }).ToList();
            }
            vmItem.ProductTypeToLoad = POSLineItemViewModel.ProducTypeToLoad.NonReturnables;
            vmItem.IsNew = true;
            vmItem.IsAdd = true;
            vmItem.Salesman = _vm.SelectedSalesman;
            vmItem.SelectedOutletId = _vm.SelectedOutlet.Id;
            vmItem.ClearAndSetupCommand.Execute(null);
            _lineItemModal.ShowDialog();
        }

        void lineItemModal_Closed(object sender, EventArgs e)
        {
            POSLineItemViewModel vmLineItem = _lineItemModal.DataContext as POSLineItemViewModel;

            bool result = _lineItemModal.DialogResult.Value;
            if (result)
            {
                if (!_vm.ReceiveReturnable)
                {
                    _vm.UpdateOrAddLineItemFromPoductSummary(vmLineItem.ProductAddSummaries, vmLineItem.IsAdd);
                    vmLineItem.MultipleProduct.Clear();
                    vmLineItem.ProductAddSummaries.Clear();
                }
                else
                {
                    if (vmLineItem.IsNew)
                    {
                        _vm.ReceiveReturnableLineItem(vmLineItem.ProductAddSummaries, vmLineItem.IsNew);
                        vmLineItem.MultipleProduct.Clear();
                        vmLineItem.ProductAddSummaries.Clear();
                    }
                    else
                    {
                        _vm.ReceiveReturnableLineItem(vmLineItem.ProductAddSummaries, false);
                        vmLineItem.MultipleProduct.Clear();
                        vmLineItem.ProductAddSummaries.Clear();
                    }
                }
            }

            _vm.ReceiveReturnable = false;
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
            // _vm.LoadOrderCommand.Execute(null);
        }

        protected override void OnNavigatingFrom(object sender, NavigatingCancelEventArgs e)
        {
            if (_vm.ConfirmNavigatingAway)
            {
                if (
                    MessageBox.Show( /*"Are you sure you want to move away from this page without completing the Sale?" */
                        _messageResolver.GetText("sl.pos.navigateaway.messagebox.part1")
                        + _messageResolver.GetText("sl.pos.navigateaway.messagebox.part2")
                    /*"Unsaved changes will be lost"*/
                        , "!" + _messageResolver.GetText("sl.pos.messagebox.caption") /*"Distributr: Point of Sale"*/
                        , MessageBoxButton.YesNo) == MessageBoxResult.No)
                    e.Cancel = true;
            }
            base.OnNavigatingFrom(sender, e);
        }

        private void btnConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.LineItems.Count < 1)
            {
                MessageBox.Show(_messageResolver.GetText("sl.pos.completesale.messagebox.nolineitems")/*"The sale must have at least 1 line item."*/
                    , "!" + _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OK);
                return;
            }
            if (_vm.IsReturnableRequired())
            {
                MessageBoxResult isResult = MessageBox.Show(/*"Are you sure you want to complete sales without receiving returnables."*/
                    _messageResolver.GetText("sl.pos.completesale.messagebox.notReceivedReturnables")
                    , "!" + _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OKCancel);
                if (isResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            if (_vm.IsPaymentRequired())
            {
                MessageBoxResult isResult = MessageBox.Show(/*"Are you sure you want to complete sales without receiving payment."*/
                    _messageResolver.GetText("sl.pos.completesale.messagebox.notReceivedPayment")
                    , "! " + _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OKCancel);
                if (isResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            //select payment mode
            if (_vm.IsValid())
                if (_vm.ValidateOrderAmounts())
                {
                    if (
                        MessageBox.Show(_messageResolver.GetText("sl.pos.completesale.messagebox.prompt")/*"Complete Sale*/
                        + " " + _vm.OrderId + "?"
                        , "!" + _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/,
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {

                        Cursor = Cursors.Wait;
                        _vm.ConfirmOrder(); //cn:fanya mambo
                        btnViewInvoice.IsEnabled = true;
                        btnViewReceipt.IsEnabled = true;
                        btnAddLineItem.IsEnabled = false;
                        btnConfirmOrder.IsEnabled = false;
                        btnCancelOrder.IsEnabled = false;
                        btnReceiveReturnables.IsEnabled = false;
                        if (!(Convert.ToDecimal(txtAmountPaid.Text) > 0))
                            btnViewReceipt.IsEnabled = false;
                        else
                            btnViewReceipt.IsEnabled = true;
                        Cursor = Cursors.Arrow;
                        MessageBox.Show(_messageResolver.GetText("sl.pos.saved.success.part1") /*"Sale"*/
                                        + " " + _vm.OrderId + " "
                                        + _messageResolver.GetText("sl.pos.completesale.success.messagebox")/*"successfully Completed."*/
                                        , _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                                        + " " + _vm.OrderId
                                        , MessageBoxButton.OK);
                        OtherUtilities.SelectedTabPos = 0;
                        Cursor = Cursors.Wait;
                        _vm.ConfirmNavigatingAway = false;
                        NavigationService.Navigate(new Uri("/views/pos/addpossale.xaml?orderid=" + _vm.OrderIdLookup + "&loadforviewing=true", UriKind.Relative));
                        Cursor = Cursors.Arrow;
                    }
                }
                else
                    MessageBox.Show("A sale with negative value is not allowed.\nIf the value of returnables exceed the sum value of sale items, use receive returnables as cash module.", "Distributr: POS", MessageBoxButton.OK);

        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString());
            LineItemType lit = LineItemType.Unit; //(LineItemType) int.Parse(tag[1].ToString());
            var lineItem = hl.DataContext as EditPOSSaleLineItem;

            if (lineItem.IsReceivedReturnable)
            {
                _vm.RemoveLineItem(lineItem.SequenceNo);
            }
            else
            {
                _vm.RemoveLineItem(ParentProductid, lit, (lineItem.TotalPrice < 0));
            }

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

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            var hl = sender as Hyperlink;
            _lineItemModal = new POSLineItemModal();
            Guid ParentProductid = Guid.Parse(hl.Tag.ToString());
            var lineItemList = _vm.LineItems.Where(n => n.ProductId == ParentProductid);
            //var lineItem = lineItemList.First(p => p.ProductId == ParentProductid);

            EditPOSSaleLineItem lineItem = hl.DataContext as EditPOSSaleLineItem;

            _lineItemModal.cmbProducts.IsEnabled = false;
            _lineItemModal.cmdAddProduct.Visibility = Visibility.Collapsed;
            _lineItemModal.Closed += new EventHandler(lineItemModal_Closed);

            var vmLineItem = _lineItemModal.DataContext as POSLineItemViewModel;
            if (lineItem.IsReceivedReturnable)
            {
                vmLineItem.ProductTypeToLoad = POSLineItemViewModel.ProducTypeToLoad.Returnables;
                _lineItemModal.lblAvailable.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.pendingQty");/*"Pending Qty";*/
                vmLineItem.ReceiveReturnables = true;
            }
            else
            {
                _lineItemModal.lblAvailable.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.availableInv");//"Available Inventory";
                vmLineItem.ProductTypeToLoad = POSLineItemViewModel.ProducTypeToLoad.AllProducts;
                vmLineItem.ReceiveReturnables = false;
            }
            vmLineItem.Salesman = _vm.SelectedSalesman;
            vmLineItem.SelectedOutletId = _vm.SelectedOutlet.Id;
            vmLineItem.LineItemType = LineItemType.Bulk;
            vmLineItem.ModalCrumbs = _messageResolver.GetText("sl.pos.editlineitem.modal.title")/*"Edit Product of Order"*/+ " " + _vm.OrderId;
            vmLineItem.RunClear();

            if (lineItem.TotalPrice < 0)
                _vm.ReceiveReturnable = true;
            else _vm.ReceiveReturnable = false;

            vmLineItem.LoadForEdit(lineItem.ProductId,
                lineItem.UnitPrice,
                lineItem.LineItemVatValue,
                lineItem.TotalPrice,
                lineItem.VatAmount,
                lineItem.SequenceNo,
                //lineItemList.Min(m => m.Qty)//cn
                lineItem.Qty
                );
            vmLineItem.IsNew = false;
            _lineItemModal.ShowDialog();
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!go_back_on_btnCancel_click)
            {

                if (MessageBox.Show( /*"Cancel Transaction? This sale will be deleted!"*/
                        _messageResolver.GetText("sl.pos.cancel.messagebox")
                        , "!" + _messageResolver.GetText("sl.pos.messagebox.caption") /*"Distributr: Point of Sale"*/,
                        MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _vm.CancelCommand.Execute(null);
                }
            }

            _vm.ConfirmNavigatingAway = false;
            NavigationService.Navigate(new Uri("/views/pos/listpossales.xaml", UriKind.Relative));
        }

        private void btnReceiveReturnables_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.IsValid())
            {
                _vm.ReceiveReturnable = true;
                _lineItemModal = new POSLineItemModal();
                POSLineItemViewModel vmItem = _lineItemModal.DataContext as POSLineItemViewModel;
                vmItem.ReturnableIn = _vm.GetReturnableIn();
                if (vmItem.ReturnableIn.Count == 0)
                {
                    MessageBox.Show(
                        /*"There are no returnables to receive either because returnables for all line items have been received or there are no line items with returnables."*/
                        _messageResolver.GetText("sl.pos.receivereturnables.messagebox.noreturnables")
                        , _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                        , MessageBoxButton.OK);
                    return;
                }
                _lineItemModal.Closed += new EventHandler(lineItemModal_Closed);
                _lineItemModal.cmdAddProduct.Visibility = Visibility.Visible;


                if (vmItem == null) return;
                vmItem.LineItems.Clear();
                vmItem.IsNew = true;
                vmItem.ReceiveReturnables = true;
                _lineItemModal.lblAvailable.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.pendingQty");/*"Pending Qty";*/
                vmItem.Salesman = _vm.SelectedSalesman;
                vmItem.SelectedOutletId = _vm.SelectedOutlet.Id;
                vmItem.ProductTypeToLoad = POSLineItemViewModel.ProducTypeToLoad.Returnables;

                vmItem.ClearAndSetupCommand.Execute(null);
                _lineItemModal.ShowDialog();
            }
        }

        private void cmdReceivePayments_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.TotalGross > 0)
            {
                _vm.RaiseInvoice();
                if (_vm.LoadForViewing)
                {
                    if (_vm.AmountPaid >= _vm.TotalGross)
                    {
                        MessageBox.Show( /*"Payment already made in full"*/
                            _messageResolver.GetText("sl.pos.receivepayments.messagebox.alreadypaid")
                            , _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/
                            , MessageBoxButton.OK);
                        return;
                    }
                }

                SelectPaymentMode(null);
            }
            else
                MessageBox.Show( /*"Gross amount should be greater than zero"*/
                    _messageResolver.GetText("sl.pos.receivepayments.messagebox.grossamountiszero")
                    , "!" + _messageResolver.GetText("sl.payment.title")/*"Distributr: Payment Module"*/
                    , MessageBoxButton.OK);
        }

        void SelectPaymentMode(PaymentInfo itemToEdit)
        {
            _paymentModeModal = new PaymentModeModal();
            _paymentModeModal.Closed += new EventHandler(paymentModeModal_Closed);
            var pvm = _paymentModeModal.DataContext as PaymentModeViewModel;
            pvm.RunClearAndSetup();
            pvm.AmountPaid = _vm.AmountPaid;//carry this accross
            pvm.GrossAmount = _vm.TotalGross - _vm.AmountPaid;

            if (itemToEdit != null)
            {
                PaymentMode itemMode = itemToEdit.PaymentModeUsed;
                switch (itemMode)
                {
                    case PaymentMode.Cash:
                        pvm.CashAmount = itemToEdit.Amount;
                        pvm.GrossAmount += itemToEdit.Amount;
                        break;
                    case PaymentMode.Cheque:
                        pvm.ChequeAmount = itemToEdit.Amount;
                        pvm.GrossAmount += itemToEdit.Amount;
                        break;
                   
                }
            }
           
            pvm.TheOrder = _vm.OrderDocument;
            pvm.OrderOutletId = _vm.SelectedOutlet.Id;
            pvm.OrderDocReference = _vm.OrderId;
            pvm.InvoiceDocReference = _vm.InvoiceDocument.DocumentReference;
            pvm.SetUpSubscriber();
            pvm.CalcAmountPaid();

            if (_vm.bankBranch != null)
            {
                pvm.LoadForEditing(_vm.bankBranch);
            }

            _paymentModeModal.ShowDialog();
        }

        PaymentInfo itemEdited = null;
        void paymentModeModal_Closed(object sender, EventArgs e)
        {
            var pvm = _paymentModeModal.DataContext as PaymentModeViewModel;
            if (_paymentModeModal.DialogResult.Value)
            {
                if (itemEdited != null)
                {
                    if (itemEdited.PaymentModeUsed == PaymentMode.Cash)
                    {
                        itemEdited.Amount = pvm.CashAmount;
                    }
                    else if (itemEdited.PaymentModeUsed == PaymentMode.Cheque)
                    {
                        itemEdited.Amount = pvm.ChequeAmount;
                    }
                    _vm.UpdatePaymentInfo(itemEdited.Id, itemEdited.Amount, pvm.CreditAmount);
                }
                else
                {
                    if (pvm != null)
                    {
                        throw new NotImplementedException();
                        //_vm.AddPaymentInfo(pvm.CashAmount,
                        //                   pvm.CreditAmount,
                        //                   pvm.MMoneyAmount,
                        //                   pvm.ChequeAmount,
                        //                   pvm.AmountPaid,
                        //                   pvm.PaymentRef,
                        //                   pvm.ChequeNumber,
                        //                   pvm.GrossAmount,
                        //                   pvm.Change,
                        //                   pvm.SelectedBank,
                        //                   pvm.SelectedBankBranch,
                        //                   pvm.SelectedMMoneyOption != null ? pvm.SelectedMMoneyOption.Name : "",
                        //                   pvm.MMoneyIsApproved,
                        //                   pvm.PaymentTransactionRefId,
                        //                   pvm.AccountNo,
                        //                   pvm.SubscriberId,
                        //                   pvm.TillNumber,
                        //                   pvm.Currency,
                        //                   pvm.PaymentNotification,
                        //                   pvm.PaymentResponse
                            //);
                    }
                }
            }
           
            itemEdited = null;
        }

        private void hlRemovePaymentItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(/*"Are you sure you want to remove this payment information?"*/
                _messageResolver.GetText("sl.pos.removepayment.messagebox.prompt")
                , "!" + _messageResolver.GetText("sl.payment.title")/*"!Distributr: Payment Module"*/
                , MessageBoxButton.OKCancel)
                 == MessageBoxResult.OK)
            {
                _vm.RemovePaymentIfo(new Guid(((Hyperlink)sender).Tag.ToString()));
                PaymentsDataGrid.ItemsSource = null;
                PaymentsDataGrid.ItemsSource = _vm.PaymentInfoList;
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.LineItems.Count < 1)
            {
                MessageBox.Show(/*"The sale must have at least 1 line item."*/
                    _messageResolver.GetText("sl.pos.save.nolineitems.messagebox")
                    , _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(_messageResolver.GetText("sl.pos.save.prompt")/*"Save Sale"*/+ " " + _vm.OrderId + "?"
                , _messageResolver.GetText("sl.pos.messagebox.caption")/*"Distributr: Point of Sale"*/
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Cursor = Cursors.Wait;
                _vm.SaveOrderToContinue();
                Cursor = Cursors.Arrow;
                MessageBox.Show(
                    _messageResolver.GetText("sl.pos.saved.success.part1") /*"Sale"*/+ " " + _vm.OrderId + " "
                    + _messageResolver.GetText("sl.pos.saved.success.part2") /*"was successfully saved"*/
                    , _messageResolver.GetText("sl.pos.messagebox.caption") /*"Distributr: Point of Sale"*/
                    , MessageBoxButton.OK);
                Cursor = Cursors.Wait;
                OtherUtilities.SelectedTabPos = 1;
                _vm.ConfirmNavigatingAway = false;
                NavigationService.Navigate(new Uri("/views/pos/ListPOSSales.xaml", UriKind.Relative));
                Cursor = Cursors.Arrow;
            }
        }

        private void cmdNewSale_Click(object sender, RoutedEventArgs e)
        {
            //this.Cursor = Cursors.Wait;
            //_vm.CheckBusyStatus();
            //this.Cursor = Cursors.Arrow;
            NavigationService.Navigate(new Uri("/views/pos/addpossale.xaml", UriKind.Relative));
        }

        private void btnViewReceipt_Click(object sender, RoutedEventArgs e)
        {
            OtherUtilities.StrBackUrl = "/views/pos/listpossales.xaml?PendingSales=true";
        }

        private void hlConfirmPaymentItem_Click(object sender, RoutedEventArgs e)
        {
            PaymentInfo pi = ((Hyperlink)sender).DataContext as PaymentInfo;
            if (pi.IsConfirmed)
            {
                MessageBox.Show("This payment has already been confirmed.", "Distributr: Payment Module", MessageBoxButton.OK);
                return;
            }
            _vm.MMoneyAmount = pi.Amount;
            ListInvoicesViewModel.UnconfirmedReceiptPayment payment
                = new ListInvoicesViewModel.UnconfirmedReceiptPayment
                {
                    LineItemId = pi.Id,
                    InvoiceDocReference = pi.InvoiceDocRef,
                    InvoiceId = pi.InvoiceId
                };
            _vm.ConfirmThisPayment(payment, pi.ReceiptDocRef, pi.MMoneyPaymentType, pi.MMoneyPaymentType == "Buy Goods" ? GetTransactionRefNum() : "");
        }

        string GetTransactionRefNum()
        {
            GetPaymentReferenceModal obj = new GetPaymentReferenceModal();
            obj.ShowDialog();
            if ((bool)obj.DialogResult)
            {
                return obj.txtReference.Text.Trim();
            }
            return "";
        }

        private void PaymentsDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            StackPanel stackPanel = null;
            ////stackPanel = PaymentsDataGrid.Columns.GetByName("colEdit").GetCellContent(dataGridrow) as StackPanel;
            //WPF Hyperlink hlConfirmPaymentItem = stackPanel.Children[0] as Hyperlink;
            //WPFHyperlink hlRemovePaymentItem = stackPanel.Children[2] as Hyperlink;

            //WPF hlConfirmPaymentItem.Content = _messageResolver.GetText("sl.pos.payment.grid.col.edit.confirm");//cn: Confirm
            //WPF hlRemovePaymentItem.Content = _messageResolver.GetText("sl.pos.payment.grid.col.edit.remove");//cn: Remove
        }

        private void dgLineItems_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //WPF Hyperlink hlEdit = dgLineItems.Columns.GetByName("colLineItemsEdit").GetCellContent(dataGridrow) as Hyperlink;
            //WPF Hyperlink hlDelete = dgLineItems.Columns.GetByName("colLineItemsDelete").GetCellContent(dataGridrow) as Hyperlink;
            //WPF hlEdit.Content = _messageResolver.GetText("sl.pos.lineitems.grid.col.edit");
            //WPF hlDelete.Content = _messageResolver.GetText("sl.pos.lineitems.grid.col.delete");
        }

        private void cmbSalesman_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void cmbSalesman_DropDownOpened(object sender, EventArgs e)
        {
            cmbSalesman.IsDropDownOpen = false;

            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedSalesman = (User)popup.ShowDlg(sender);
        }

        private void cmbRoutes_DropDownOpened(object sender, EventArgs e)
        {
            cmbRoutes.IsDropDownOpen = false;
            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedRoute = (Route)popup.ShowDlg(sender);
        }

        private void cmbOutlets_DropDownOpened(object sender, EventArgs e)
        {
            cmbOutlets.IsDropDownOpen = false;
            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedOutlet = (Outlet)popup.ShowDlg(sender);
        }
    }
}
