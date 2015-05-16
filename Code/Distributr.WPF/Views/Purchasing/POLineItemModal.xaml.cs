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
using Distributr.WPF.Lib.ViewModels.Transactional.Purchasing;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.Purchasing
{
    /// <summary>
    /// Interaction logic for POLineItemModal.xaml
    /// </summary>
    public partial class POLineItemModal : Window
    {
        private POLineItemViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        bool isInitialzed = false;
        public POLineItemModal()
        {
            isInitialzed = false;
            InitializeComponent();
            isInitialzed = true;
            this.Loaded += new RoutedEventHandler(POLineItemModal_Loaded);
            _vm = this.DataContext as POLineItemViewModel;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            POLineItemViewModel vm = DataContext as POLineItemViewModel;
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            if (vm.SelectedProduct != null)
            {
                MessageBoxResult isConfirmed = MessageBox.Show("Are sure you want Cancel.Unsaved data will be lost ", "Distributr: " + messageResolver.GetText("sl.po.name"), MessageBoxButton.OKCancel);

                if (isConfirmed == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }
        void POLineItemModal_Loaded(object sender, RoutedEventArgs e)
        {
            LocalizeLables();
        }

        private void LocalizeLables()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            lblProduct.Content = messageResolver.GetText("sl.po.popmodal.product");
            lblQty.Content = messageResolver.GetText("sl.po.popmodal.qty");
            lblUnitPrice.Content = messageResolver.GetText("sl.po.popmodal.unitprice");
            lblVatAmount.Content = messageResolver.GetText("sl.po.popmodal.vatamount");
            lblTotalPrice.Content = messageResolver.GetText("sl.po.popmodal.totalprice");
            labelOrderby.Content = messageResolver.GetText("sl.po.popmodal.orderby");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as POLineItemViewModel;

            if (_vm.SelectedProduct != null && _vm.Qty > 0)
            {
                AddProduct();
            }

            this.DialogResult = true;
            _vm.ButtonNameText = "";
            _vm.ProductAddSummaries.Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = false;
        }

        private void cmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isInitialzed)
                return;
            POLineItemViewModel vm = DataContext as POLineItemViewModel;
            vm.SelectedProduct = cmbProducts.SelectedItem as POLineItemProductLookupItem;
            vm.ProductSelected.Execute(null);
            if (vm.IsNew == false)
                vm.IsEnabled = false;
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct();
        }

        private void AddProduct()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            POLineItemViewModel vm = DataContext as POLineItemViewModel;
            vm.SelectedProduct = cmbProducts.SelectedItem as POLineItemProductLookupItem;
            if (vm.SelectedProduct == null)
            {
                MessageBox.Show("Please select product ");
                return;
            }
            else if (vm.Qty <= 0)
            {
                MessageBox.Show("Please Enter valid Quantity ");
                return;
            }
            else
            {

                MessageBoxResult isConfirmed = MessageBox.Show("Add Product : " + vm.SelectedProduct.ProductDesc + " of quantity = " + vm.Qty, "Confirm " + messageResolver.GetText("sl.po.name"), MessageBoxButton.OKCancel);

                if (isConfirmed == MessageBoxResult.OK)
                {
                    MultipleAddProduct pro = new MultipleAddProduct()
                    {
                        Product = vm.SelectedProduct,
                        Quantity = vm.Qty,
                        TotalPrice = vm.TotalPrice,
                        UnitPrice = vm.UnitPrice,
                        Vat = vm.Vat,
                        VatAmount = vm.VatAmount,
                        LineItemVatValue = vm.LineItemVatValue,
                        LineItemType = vm.LineItemType,
                    };
                    vm.AddProduct(pro);
                    vm.ClearAndSetup.Execute(null);
                    vm.IsNew = true;
                    cmbProducts.SelectedItem = null;
                    rb1.IsChecked = true;
                }
            }
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);


        }

        private void rb1_Click(object sender, RoutedEventArgs e)
        {
            POLineItemViewModel vm = DataContext as POLineItemViewModel;
            vm.RecalcTotal();
        }

        private void rb2_Click(object sender, RoutedEventArgs e)
        {
            POLineItemViewModel vm = DataContext as POLineItemViewModel;
            vm.RecalcTotal();
        }

        private void cmbProducts_DropDownOpened(object sender, EventArgs e)
        {
            cmbProducts.IsDropDownOpen = false;
            ComboPopUp popup = new ComboPopUp();
            _vm.SelectedProduct = (POLineItemProductLookupItem)popup.ShowDlg(sender);
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
    }
}
