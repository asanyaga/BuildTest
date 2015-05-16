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
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;

namespace Distributr.WPF.UI.Views.CN
{
    /// <summary>
    /// Interaction logic for AddCreditNoteLineItemModal.xaml
    /// </summary>
    public partial class AddCreditNoteLineItemModal : Window
    {
        public AddCreditNoteLineItemModal()
        {
            InitializeComponent();
            this.Closing += AddCreditNoteLineItemModal_Closing;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {


            this.DialogResult = true;
        }

        void AddCreditNoteLineItemModal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AddCreditNoteLineViewModel vm = this.DataContext as AddCreditNoteLineViewModel;
            if (this.DialogResult == true)
            {
                if (vm.ProductLookUp == null)
                {
                    MessageBox.Show("Select Product", "Distributr Credit Note Product", MessageBoxButton.OK);
                    e.Cancel = true;
                }
                else if (vm.QuantityRequired <= 0)
                {
                    MessageBox.Show("Enter valid Required Product", "Distributr Credit Note Product", MessageBoxButton.OK);
                    e.Cancel = true;
                }
                else if (vm.QuantityRequired > vm.QuantityIssued)
                {
                    MessageBox.Show("Product quantity cant be more Invoice Product quantity", "Distributr Credit Note Product", MessageBoxButton.OK);
                    e.Cancel = true;
                }
                else if (vm.Reason.Trim() == "")
                {
                    MessageBox.Show("Enter Reason  ", "Distributr Credit Note Product", MessageBoxButton.OK);
                    e.Cancel = true;
                }
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to cancel", "Distributr Credit Note Product", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cboProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddCreditNoteLineViewModel vm = this.DataContext as AddCreditNoteLineViewModel;
            if (vm.ProductLookUp != null)
            {
                vm.SetupProductQuantity();
            }
        }



        private void txtQRequired_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = WPFValidation.AllowNumberOnlyOnKeyDown(e);
        }
    }
}
