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
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.ITN
{
    /// <summary>
    /// Interaction logic for ITNLineItemModal.xaml
    /// </summary>
    public partial class ITNLineItemModal : Window
    {
        private ITNLineItemViewModel _vm;
        bool isInitialized;

        public ITNLineItemModal()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            this.Owner = Application.Current.MainWindow;
            this.Loaded += new RoutedEventHandler(ITNLineItemModal_Loaded);
        }

        void ITNLineItemModal_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as ITNLineItemViewModel;
            IMessageSourceAccessor messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            this.Title = messageResolver.GetText("sl.inventory.issue.modal.title");
            lblproduct.Content = messageResolver.GetText("sl.inventory.issue.modal.product");
            lblavailableQuantity.Content = messageResolver.GetText("sl.inventory.issue.modal.availableqty");
            lblIssueQuantity.Content = messageResolver.GetText("sl.inventory.issue.modal.issuesqty");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValid(txtIssueAmount.Text))
            {
                if (Convert.ToDecimal(txtIssueAmount.Text) > 0)
                {
                    if (Convert.ToDecimal(txtAvailableQty.Text) >= Convert.ToDecimal(txtIssueAmount.Text))
                    {
                        if (_vm == null) { MessageBox.Show("Failed to add product"); return; }
                        _vm.AddProduct();
                        DialogResult = true;
                    }
                    else
                        MessageBox.Show("You cannot issue more inventory quantity than is available.");
                }
                else
                    MessageBox.Show("Issue inventory quantity cannot be zero.");
            }
            else
                MessageBox.Show("Issue inventory quantity must be a integer.");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Inventory Transfer", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                this.DialogResult = false;
        }

        bool isValid(string str)
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

        private void cmdDone_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ITNLineItemViewModel;
            if (vm == null) { MessageBox.Show("Failed to add product"); return; }
            vm.AddProduct();
            DialogResult = true;
        }

        private void btnAddSerial_Click(object sender, RoutedEventArgs e)
        {
            ITNLineItemViewModel vm = this.DataContext as ITNLineItemViewModel;
            vm.SerialTo = txtTo.Text;
            vm.AddSerialNumbers();
            txtFrom.Text = "";
            txtTo.Text = "";
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            ITNLineItemViewModel vm = this.DataContext as ITNLineItemViewModel;
            vm.EditSelectedSerial();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            ITNLineItemViewModel vm = this.DataContext as ITNLineItemViewModel;
            vm.DeleteSelectedSerial();
        }

        private void txtFrom_KeyUp(object sender, KeyEventArgs e)
        {
            WPFValidation.InvalidateSpecialCharactersOnKeyUp(sender);
            if (e.Key == Key.Enter)
            {
                txtTo.Focus();
            }
        }

        private void txtTo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAddSerial_Click(sender, null);
                txtFrom.Focus();
            }
        }

        private void txtIssueAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
            {
               
                return;
            }

            decimal quantity = 0;
            decimal.TryParse(txtIssueAmount.Text, out quantity);
            var vm = this.DataContext as ITNLineItemViewModel;
            if (vm != null)
            {
                vm.Qty = quantity;
               // vm.ValidQuantityCommand.Execute(e);
            }
        }
        
    }
}
