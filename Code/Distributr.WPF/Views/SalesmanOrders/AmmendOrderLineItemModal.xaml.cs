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
using System.Windows.Shapes;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.SalesmanOrders
{
    /// <summary>
    /// Interaction logic for AmmendOrderLineItemModal.xaml
    /// </summary>
    public partial class AmmendOrderLineItemModal : Window
    {
        AmmendSalesmanOrderLineItemViewModel _vm;
        private bool isInitialised = false;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public AmmendOrderLineItemModal()
        {
            InitializeComponent();
            isInitialised = true;
            LabelControls();
        }

        void LabelControls()
        {
            lblProduct.Content = _messageResolver.GetText("sl.order.addlineitems.modal.product");
            lblQty.Content = _messageResolver.GetText("sl.order.addlineitems.modal.requiredQty");
            lblUnitVat.Content = _messageResolver.GetText("sl.order.addlineitems.modal.unitVat");
            lblUnitPrice.Content = _messageResolver.GetText("sl.order.addlineitems.modal.unitPrice");
            lblTotalNet.Content = _messageResolver.GetText("sl.order.addlineitems.modal.totalNet");
            lblVatAmount.Content = _messageResolver.GetText("sl.order.addlineitems.modal.totalVat");
            lblTotalPrice.Content = _messageResolver.GetText("sl.order.addlineitems.modal.totalGross");
            btnCancel.Content = _messageResolver.GetText("sl.order.addlineitems.modal.cancel");
            btnOK.Content = _messageResolver.GetText("sl.order.ammendlineitems.modal.ok");

            lblAvailable.Content = _messageResolver.GetText("sl.order.ammendlineitems.modal.available");
            lblApprove.Content = _messageResolver.GetText("sl.order.ammendlineitems.modal.approve");
            lblBackOrder.Content = _messageResolver.GetText("sl.order.ammendlineitems.modal.backorder");
            lblLostSale.Content = _messageResolver.GetText("sl.order.ammendlineitems.modal.lostsale");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (
               MessageBox.Show("Unsaved changes will be lost.", "Distributr: Cancel Add Products",
                               MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _vm.LineItems.Clear();
                this.DialogResult = false;
            }
            else
            {
                //remain here
                return;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as AmmendSalesmanOrderLineItemViewModel;
            if (MessageBox.Show("Save these changes?", "Distributr: Ammending Order", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            if (!_vm.IsNew)
                txtQty_GotFocus(this, new RoutedEventArgs());

            decimal guardQty = 0;
            if (_vm.ProcessingBackOrder)
                guardQty = _vm.OriginalQty;
            else
                guardQty = _vm.Qty;

            bool isInvalid = false;
            if (cmbProducts.SelectedItem == null || cmbProducts.SelectedIndex == -1)
            //|| _vm.AwaitingStock+_vm.BackOrder+_vm.Qty+_vm.LostSale <= 0)
            {
                MessageBox.Show("Select a product in product dropdown list.",
                                "Distributr: Add Order Line Items", MessageBoxButton.OK);
                isInvalid = true;
            }

            if (!isInvalid)
            {
                _vm.AddOrUpdateProducts();
                this.DialogResult = true;
            }
        }

        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm = this.DataContext as AmmendSalesmanOrderLineItemViewModel;
            _vm.SelectedProduct = cmbProducts.SelectedItem as OrderLineItemProductLookupItem;
            _vm.ProductSelected.Execute(null);
        }

        private void txtQty_GotFocus(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as AmmendSalesmanOrderLineItemViewModel;
            _vm.txtQtyGotFocusCommand.Execute(null);
        }

        private void txtBackOrder_GotFocus(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as AmmendSalesmanOrderLineItemViewModel;
            _vm.txtBackOrderGotFocusCommand.Execute(null);
        }

        private void txtLostSale_GotFocus(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as AmmendSalesmanOrderLineItemViewModel;
            _vm.txtLostSaleGotFocusCommand.Execute(null);
        }

        private void txtApprove_GotFocus(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as AmmendSalesmanOrderLineItemViewModel;
            _vm.txtApproveGotFocusCommand.Execute(null);
        }

        private void txtHelp_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHelp.Text = "Click on a quantity field to view its pricing details.";
        }

        private void txtApprove_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!isInitialised)
                return;
            if (_vm == null)
                _vm = DataContext as AmmendSalesmanOrderLineItemViewModel;

            if (txtApprove.Text == "")
                txtApprove.Text = "0";

             
            decimal guardQty = 0;
            decimal txtapprove = 0;
                Decimal.TryParse(txtApprove.Text,out txtapprove);
            if (_vm.ProcessingBackOrder)
                guardQty = _vm.OriginalQty;
            else
                Decimal.TryParse(txtQty.Text, out guardQty);
            ;
            if (txtapprove > guardQty && _vm.DocumentStatus == "Order Pending Dispatch")
            {
                MessageBox.Show("The quantity to approve must not be more than the quantity originally required.",
                                "Distribtr: Ammend Order", MessageBoxButton.OK);
                txtApprove.Text = txtQty.Text;
            }
            else
            {
                _vm.Approve = txtapprove;//Convert.ToInt32(txtApprove.Text);
                //_vm.RecalcTotal();
                if (_vm.SellInBulk)
                {
                    _vm.SellInBulkSelected.Execute(null);
                    //_vm.RecalcTotal();
                }
                else if (_vm.SellInUnits)
                {
                    _vm.SellInUnitsSelected.Execute(null);
                }

                _vm.txtApproveGotFocusCommand.Execute(null);
            }
        }

        private void txtApprove_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab))

                e.Handled = false;

            else
            {
                e.Handled = true;

            }
        }

        private void cmbProducts_DropDownOpened(object sender, EventArgs e)
        {
            cmbProducts.IsDropDownOpen = false;
            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedProduct = (OrderLineItemProductLookupItem)popup.ShowDlg(sender);
        }
    }
}
