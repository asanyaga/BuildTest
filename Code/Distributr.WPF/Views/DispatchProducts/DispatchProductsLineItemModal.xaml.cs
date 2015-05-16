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
using Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.DispatchProducts
{
    public partial class DispatchProductsLineItemModal : Window
    {
        private DPLineItemViewModel _vm;
        bool isInitialized = false;
        public DispatchProductsLineItemModal()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            _vm = this.DataContext as DPLineItemViewModel;
            this.Loaded += new RoutedEventHandler(DispatchProductsLineItemModal_Loaded);
            //this.HasCloseButton = false;
        }

        void DispatchProductsLineItemModal_Loaded(object sender, RoutedEventArgs e)
        {
            LocalizeLabels();
        }

        private void LocalizeLabels()
        {
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            this.Title = messageResolver.GetText("sl.inventory.dispatch.modal.title");
            this.lblproduct.Content = messageResolver.GetText("sl.inventory.dispatch.modal.product");
            this.lblavailable.Content = messageResolver.GetText("sl.inventory.dispatch.modal.available");
            this.lblquantity.Content = messageResolver.GetText("sl.inventory.dispatch.modal.quantity");
            this.lblreason.Content = messageResolver.GetText("sl.inventory.dispatch.modal.reason");
            this.lblotherReason.Content = messageResolver.GetText("sl.inventory.dispatch.modal.otherreason");

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as DPLineItemViewModel;
            string errorMsg = string.Empty;
            if (_vm.SelectedProduct.Id == Guid.Empty)
            {
                errorMsg += "You must select a product.\n";
            }
            if (_vm.SelectedReason.Id == Guid.Empty)
            {
                errorMsg += "You must select a reason\n";
            }
            if (_vm.Qty == 0)
            {
                errorMsg += "The quantity entered should be greater than 0.\n";
            }
            if ((_vm.AvailableQty - _vm.Qty) < 0)
            {
                errorMsg += "The quantity entered is more than available inventory.\n";
            }
            if (_vm.SelectedReason.Reason == "Others")
            {
                if (txtOtherReason.Text.Trim() == "")
                {
                    errorMsg += "You must specify other reasons.";
                }
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(errorMsg, "Distributr: Dispatch Products", MessageBoxButton.OK);
                return;
            }
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?\nUnsaved changes will be lost.", "Distributr: Dispatch Products", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                this.DialogResult = false;
        }

        private void txtQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            _vm = DataContext as DPLineItemViewModel;
            decimal quantity = 0;
            bool isValidQuantity = decimal.TryParse(txtQty.Text, out quantity);
            _vm.Qty = quantity;
            try
            {
                _vm.Qty = Convert.ToDecimal(txtQty.Text);
                if ((_vm.AvailableQty - _vm.Qty) < 0)
                {
                    MessageBox.Show("The quantity entered is more than available inventory");
                    _vm.Qty = 0;
                    return;
                }
            }
            catch
            {
                MessageBox.Show("You have enter a digit that is too big");
            }
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
           // e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

        
       

    }
}
