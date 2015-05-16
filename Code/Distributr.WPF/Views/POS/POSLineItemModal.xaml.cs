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
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.POS;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.POS
{
    public partial class POSLineItemModal : Window
    {
        POSLineItemViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool isInitialized = false;
        public POSLineItemModal()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            Loaded += POSLineItemModal_Loaded;
            LabelControls();
        }

        void LabelControls()
        {
            chkLoadFreeOfCharge.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.sellfreeofcharge");
            lblProduct.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.product");
            lblSellby.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.sellby");
            rbUnits.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.sellby.units");
            rbBulk.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.sellby.bulk");
            lblAvailable.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.availableInv");
            lblQty.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.required");
            lblUnitVat.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.unitvat");
            lblUnitPrice.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.unitprice");
            lblTotalNet.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.totalnet");
            lblVatAmount.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.totalvat");
            lblTotalPrice.Content = _messageResolver.GetText("sl.pos.addlineitem.modal.totalgross");
            hlViewAdded.Text = _messageResolver.GetText("sl.pos.addlineitem.modal.viewAdded");
        }

        void POSLineItemModal_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as POSLineItemViewModel;
            rbUnits.IsChecked = true;
            this.cmbProducts.ItemsSource = _vm.Products;
            
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducts.SelectedIndex == 0)
            {
                if (MessageBox.Show(/*"This will close the window. Do you want to proceed?"*/
                    _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.propmt"),
                    _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                    , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    DialogResult = true;

            }
            else
            {

                AddProduct(sender);
                this.DialogResult = true;
                _vm.ButtonNameText = "";
                _vm.ProductAddSummaries.Clear();
            }
        }

        bool fireQtyChangedEvent = true;
        private void AddProduct(object sender)
        {
            try
            {
                _vm = DataContext as POSLineItemViewModel;
                bool isInvalid = false;
                Button cmd = (Button)sender;
                if (cmbProducts.SelectedItem == null || cmbProducts.SelectedIndex == -1 || cmbProducts.SelectedIndex == 0)
                {
                    MessageBox.Show(
                        _messageResolver.GetText("sl.pos.addlineitem.modal.message.productnotselected")/*"Select a product in product dropdown list."*/
                        , _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                        , MessageBoxButton.OK);
                    isInvalid = true;
                }
                if (Convert.ToDouble(txtQty.Text) <= 0 || txtQty.Text.Trim() == "")
                {
                    MessageBox.Show(
                        _messageResolver.GetText("sl.pos.addlineitem.modal.message.quantityzero")/*"Quantity should not be zero or empty."*/
                        , _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                        , MessageBoxButton.OK); ;
                    isInvalid = true;
                }

                if (!isInvalid)
                {
                    if (_vm.Qty > _vm.AvailableProductInv)
                    {
                        string msg = _messageResolver.GetText("sl.pos.addlineitem.modal.message.sellMoreThanAvailable");
                        /*"Sorry, you cannot sell more than the available inventory.";*/
                        if (_vm.ReceiveReturnables)
                            msg = _messageResolver.GetText("sl.pos.addlineitem.modal.message.receiveMoreThanRequired");
                        /*"Sorry, you cannot receive more than required returnables for this sale.";*/

                        MessageBox.Show(msg,
                            "!" + _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                            , MessageBoxButton.OK);
                        txtQty.Focus();
                        txtQty.SelectAll();
                        isInvalid = true;
                    }
                }

                if (!isInvalid)
                {
                    MultipleAddProduct pro = new MultipleAddProduct()
                    {
                        Product = _vm.SelectedProduct,
                        Quantity = _vm.Qty,
                        TotalPrice = _vm.TotalPrice,
                        UnitPrice = _vm.UnitPrice,
                        Vat = _vm.VatAmount,
                        VatAmount = _vm.VatAmount,
                        LineItemVatValue = _vm.LineItemVatValue,
                        LineItemType = _vm.LineItemType
                    };
                   
                    _vm.AddProduct(pro);

                    fireQtyChangedEvent = false;
                    _vm.ClearViewModel();
                    if (cmbProducts.Items.Count > 0)
                        cmbProducts.SelectedIndex = 0;
                    hlViewAdded.Text = _vm.ButtonNameText;

                    rbUnits.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message
                    , _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                    , MessageBoxButton.OK);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_vm == null)
                _vm = DataContext as POSLineItemViewModel;
            if (
                MessageBox.Show(
                _messageResolver.GetText("sl.pos.addlineitem.modal.message.unsaved")/*"Unsaved changes will be lost."*/
                , "!" + _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _vm.LineItems.Clear();
                this.DialogResult = false;
            }
            else
            {
                return;
            }
        }

        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            try
            {
                if (cmbProducts.SelectedItem != null)
                {
                    _vm = DataContext as POSLineItemViewModel;
                    if (cmbProducts.SelectedIndex != 0)
                    {
                        _vm.RunProductSelected();
                    }
                    else
                    {
                        _vm.ClearViewModel();
                        cmbProducts.SelectedIndex = 0;
                    }
                }
            }
            catch
            {
                MessageBox.Show(
                    _messageResolver.GetText("sl.pos.addlineitem.modal.message.valueTooLarge")/*"Value was too large."*/
                    , "!" + _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                    , MessageBoxButton.OK);
                _vm.Qty = 0;
            }
        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            if (!fireQtyChangedEvent)
            {
                fireQtyChangedEvent = true;
                return;
            }
            try
            {
                if (txtQty.Text == "")
                    txtQty.Text = "0";
                if (_vm == null)
                    _vm = DataContext as POSLineItemViewModel;

                _vm.Qty = Convert.ToDecimal(txtQty.Text);

                if (_vm.Qty > _vm.AvailableProductInv)
                {
                    string msg = _messageResolver.GetText("sl.pos.addlineitem.modal.message.sellMoreThanAvailable");
                    /*"Sorry, you cannot sell more than the available inventory.";*/
                    if (_vm.ReceiveReturnables)
                        msg = _messageResolver.GetText("sl.pos.addlineitem.modal.message.receiveMoreThanRequired");
                    /*"Sorry, you cannot receive more than required returnables for this sale.";*/

                    MessageBox.Show(msg, _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/, MessageBoxButton.OK);
                    _vm.Qty = _vm.AvailableProductInv;
                }
             
                _vm.RecalcTotal();
            }
            catch
            {
                MessageBox.Show(_messageResolver.GetText("sl.pos.addlineitem.modal.message.valueTooLarge")/*"Value was too large."*/
                    , _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                    , MessageBoxButton.OK);
                txtQty.Text = "0";

            }
        }

        private void txtQty_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

        private void rbUnits_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm = DataContext as POSLineItemViewModel;
            _vm.LineItemType = Lib.UI.Hierarchy.LineItemType.Unit;
            _vm.ChangeAvailableQty();
            _vm.RecalcTotal();
        }

        private void rbBulk_Checked(object sender, RoutedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm = DataContext as POSLineItemViewModel;
            _vm.LineItemType = Lib.UI.Hierarchy.LineItemType.Bulk;
            _vm.ChangeAvailableQty();
            _vm.RecalcTotal();
        }

        private void cmdAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct(sender);
        }

        private void txtQty_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        bool IsValid(string str)
        {
            try
            {
                decimal.Parse(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            string msg = "";
            if (_vm.ProductAddSummaries.Count > 0)
            {
                msg = _vm.ProductAddSummaries.Aggregate(msg, (current, item) => current + (item.ProductName + "   \tQty:" + item.Quantity + "\tTotal Price: " + item.TotalPrice + ";\n"));
            }
            MessageBox.Show(msg
                , _messageResolver.GetText("sl.pos.addlineitem.modal.ok.message.caption")/*"Distributr: Add Order Line Items"*/
                , MessageBoxButton.OK);
        }

        private void cmbLoadFreeOfCharge_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                _vm.SellFreeOfCharge = true;
                _vm.ToggleFreeOfChargeProducts(true);
                cmbProducts.ItemsSource = _vm.Products;
                cmbProducts.SelectedIndex = 0;
            }
            else
            {
                _vm.SellFreeOfCharge = false;
                _vm.ToggleFreeOfChargeProducts(false);
                cmbProducts.ItemsSource = _vm.Products;
                cmbProducts.SelectedIndex = 0;
            }
        }

        private void cmbProducts_DropDownOpened(object sender, EventArgs e)
        {
            cmbProducts.IsDropDownOpen = false;

            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedProduct = (POLineItemProductLookupItem)popup.ShowDlg(sender);
        }
    }
}
