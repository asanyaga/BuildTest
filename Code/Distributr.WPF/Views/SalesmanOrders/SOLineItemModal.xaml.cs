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
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.ViewModels.Transactional.SalesmanOrders;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;
using System.Linq;

namespace Distributr.WPF.UI.Views.SalesmanOrders
{
    public partial class SOLineItemModal : Window
    {
        SOLineItemViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool isInitialized = false;
        public SOLineItemModal()
        {
            isInitialized = false;
            InitializeComponent();
            _vm = DataContext as SOLineItemViewModel;
            isInitialized = true;
            LabelControls();
            txtHelp.Text = "";
            rbUnits.IsChecked = true;
        }

        void LabelControls()
        {
            chkLoadFreeOfCharge.Content = _messageResolver.GetText("sl.order.addlineitem.modal.sellFreeofCharge");
            lblProduct.Content = _messageResolver.GetText("sl.order.addlineitems.modal.product");
            lblQty.Content = _messageResolver.GetText("sl.order.addlineitems.modal.requiredQty");
            lblSellby.Content = _messageResolver.GetText("sl.order.addlineitems.modal.sellByOptions");
            lblUnitVat.Content = _messageResolver.GetText("sl.order.addlineitems.modal.unitVat");
            lblUnitPrice.Content = _messageResolver.GetText("sl.order.addlineitems.modal.unitPrice");
            lblTotalNet.Content = _messageResolver.GetText("sl.order.addlineitems.modal.totalNet");
            lblVatAmount.Content = _messageResolver.GetText("sl.order.addlineitems.modal.totalVat");
            lblTotalPrice.Content = _messageResolver.GetText("sl.order.addlineitems.modal.totalGross");
            btnAddProduct.Content = _messageResolver.GetText("sl.order.addlineitems.modal.addProduct");
            btnOK.Content = _messageResolver.GetText("sl.order.addlineitems.modal.done");
            btnCancel.Content = _messageResolver.GetText("sl.order.addlineitems.modal.cancel");
            rbUnits.Content = _messageResolver.GetText("sl.order.addlineitems.modal.options.unit");
            rbBulk.Content = _messageResolver.GetText("sl.order.addlineitems.modal.options.bulk");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(/*"Unsaved changes will be lost."*/
                _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.cancelMsg")
                , _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.cancelCaption")/*"Distributr: Cancel Add Products"*/
                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
            AddProduct();

            this.DialogResult = true;
            _vm.ButtonNameText = "";
            _vm.ProductAddSummaries.Clear();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct();
            txtHelp.Text = _vm.ButtonNameText;
        }

        private void AddProduct()
        {
            _vm.SelectedProduct = cmbProducts.SelectedItem as POLineItemProductLookupItem;
            if (_vm.SelectedProduct == null)
            {
                MessageBox.Show(/*"Select a product in the products dropdown list."*/
                    _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.productNotSelectedErrorMsg")
                    , _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.errorCatpion")/*"Distributr: Add Order Line Items"*/
                    , MessageBoxButton.OK);
                return;
            }
            else if (_vm.Qty <= 0)
            {
                MessageBox.Show(/*"Quantity should not be zero or empty."*/
                    _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.zeroQtyErrorMsg")
                    , _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.errorCatpion") /*"Distributr: Add Order Line Items"*/
                    , MessageBoxButton.OK); ;
                return;
            }
            else
            {
                //string msg = "";
                //if (_vm.SelectedProduct.ProductId != Guid.Empty)
                //    msg = "Add Product : " + _vm.SelectedProduct.ProductDesc + " of quantity = " + _vm.Qty;
                //else
                //{
                //    msg = _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.confirmMsg");/* "Are you sure you want to insert added products";*/
                //}
                //MessageBoxResult isConfirmed = MessageBox.Show(msg + "?", _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.confirmCaption")
                //    /*"Distributr: Confirm Order"*/
                //    , MessageBoxButton.OKCancel);

                //if (isConfirmed == MessageBoxResult.OK)
                //{
                    MultipleAddProduct pro = new MultipleAddProduct()
                    {
                        Product = _vm.SelectedProduct,
                        Quantity = _vm.Qty,
                        TotalPrice = _vm.TotalPrice,
                        UnitPrice = _vm.UnitPrice,
                        Vat = _vm.VatAmount,
                        VatAmount = _vm.VatAmount,
                        LineItemVatValue = _vm.LineItemVatValue,
                        LineItemType = _vm.LineItemType,
                    };
                    _vm.AddProduct(pro);
                    _vm.RunClearAndSetup();
                    _vm.IsNew = true;
                    cmbProducts.SelectedIndex = 0;
                    rbUnits.IsChecked = true;
                //}
            }
        }

        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            try
            {
                _vm.RunProductSelected();
                if (_vm.IsNew == false)
                    _vm.IsEnabled = false;
            }
            catch
            {
                MessageBox.Show(_messageResolver.GetText("sl.order.addlineitem.modal.messageBox.valueTooLarge")/*"Value was too large."*/,
                    _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.caption")/*"Distribut: Order Module"*/
                    , MessageBoxButton.OK);
                txtQty.Text = "0";
            }
        }

        private void txtQty_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            try
            {
                if (txtQty.Text == "")
                    txtQty.Text = "0";
                _vm.Qty = Convert.ToDecimal(txtQty.Text);

                _vm.RecalcTotal();
            }
            catch
            {
                MessageBox.Show(_messageResolver.GetText("sl.order.addlineitem.modal.messageBox.valueTooLarge")/*"Value was too large."*/,
                    _messageResolver.GetText("sl.order.addlineitem.modal.messageBox.caption")/*"Distribut: Order Module"*/
                    , MessageBoxButton.OK);
                txtQty.Text = "0";
            }
        }

        private void rbUnits_Checked(object sender, RoutedEventArgs e)
        {
            _vm.LineItemType = Lib.UI.Hierarchy.LineItemType.Unit;
            _vm.RecalcTotal();
        }

        private void rbBulk_Checked(object sender, RoutedEventArgs e)
        {
            _vm.LineItemType = Lib.UI.Hierarchy.LineItemType.Bulk;
            _vm.RecalcTotal();
        }

        private void txtHelp_Click(object sender, RoutedEventArgs e)
        {
            ShowListDetails();
        }

        void ShowListDetails()
        {
            Utils.ListDetailsModal ldm = new Utils.ListDetailsModal();
            foreach (var item in _vm.ProductAddSummaries)
            {
                Product first = _vm.AllProducts.FirstOrDefault(prods => prods.Id == item.ProductId);
                if (first != null)
                {
                    string prodDesc = first.GetType().ToString().Split('.').Last();

                    ldm.ListItems.Add(new LineItem
                                          {
                                              ProductId = item.ProductId,
                                              Qty = item.Quantity,
                                              ProductDesc = item.ProductName,
                                              TotalPrice = item.TotalPrice,
                                              ProductType = prodDesc
                                          });
                }
            }
            ldm.Show();
        }

        private void cmbLoadFreeOfCharge_Click(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                _vm.ToggleFreeOfChargeProducts(true);
                cmbProducts.ItemsSource = _vm.Products;
                cmbProducts.SelectedIndex = 0;
            }
            else
            {
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
