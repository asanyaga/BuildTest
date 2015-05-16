using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using Distributr.WPF.UI.Views.Utils;
using StructureMap;

namespace Distributr.WPF.UI.Views.IAN
{
    public partial class IANLineItemModal : Window
    {
        private IMessageSourceAccessor messageResolver = null;
        private IANLineItemViewModel _vm;
        bool isInitialized = false;
        public IANLineItemModal()
        {
            isInitialized = false;
            InitializeComponent();
            isInitialized = true;
            this.Loaded += new RoutedEventHandler(IANLineItemModal_Loaded);
            _vm = this.DataContext as IANLineItemViewModel;
        }

        void IANLineItemModal_Loaded(object sender, RoutedEventArgs e)
        {
            messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            LocalizeLabels();
        }

        private void LocalizeLabels()
        {
            lblproduct.Content = messageResolver.GetText("sl.inventory.adjust.modal.product");
            lblexpected.Content = messageResolver.GetText("sl.inventory.adjust.modal.expected");
            lblvarience.Content = messageResolver.GetText("sl.inventory.adjust.modal.variance");
            lblreason.Content = messageResolver.GetText("sl.inventory.adjust.modal.reason");
            lblactual.Content = messageResolver.GetText("sl.inventory.adjust.modal.actual");


        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SelectedProduct == null || _vm.SelectedProduct.ProductId == Guid.Empty)
            {
                MessageBox.Show("Select product to  " + messageResolver.GetText("sl.inventory.adjust.title"), "Distributr:" + messageResolver.GetText("sl.inventory.adjust.title"), MessageBoxButton.OK);
                return;
            }

            if (Actual1.Text.Trim() == "")
            {
                MessageBox.Show("Actual field is required. Please enter the actual quantity.", "Distributr:" + messageResolver.GetText("sl.inventory.adjust.title"), MessageBoxButton.OK);
                return;
            }
            else if (_vm.Reason.Trim() == "")
            {
                MessageBox.Show("Enter reason for adjusting Product inventory ", "Distributr:" + messageResolver.GetText("sl.inventory.adjust.title"), MessageBoxButton.OK);
                return;

            }
             
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void Actual1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isInitialized)
                return;
            decimal actual = 0;
            bool isValidQuantity = decimal.TryParse(Actual1.Text, out actual);
            _vm.Actual = actual;
            if (!isValidQuantity)
            {
                _vm.Variance = 0;
                txtVariance.Text = "0";
                return;
            }
           


            decimal variance = (decimal.Parse(Actual1.Text)) - (decimal.Parse(Expected.Text));
            _vm.Variance = variance;
            txtVariance.Text = variance.ToString();
        }

        private void Actual1_KeyDown(object sender, KeyEventArgs e)
        {
           // e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }

      
    }
}
