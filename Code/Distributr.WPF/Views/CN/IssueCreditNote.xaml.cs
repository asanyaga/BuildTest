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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.ViewModels.Transactional.CN;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.UI.Views.CN
{
    /// <summary>
    /// Interaction logic for IssueCreditNote.xaml
    /// </summary>
    public partial class IssueCreditNote : Page
    {
        EditCNViewModel _vm;
        public IssueCreditNote()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(IssueCreditNote_Loaded);
        }

        void IssueCreditNote_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = DataContext as EditCNViewModel;
            txtCreditNoteValue.Text = "";
            txtReason.Text = "";
            _vm.CancelCommand.Execute(null);

            Guid invoiceNo = PresentationUtility.ParseIdFromUrl(NavigationService.CurrentSource);
            _vm.InvoiceNo = invoiceNo;
            _vm.LoadInvoiceCommand.Execute(null);
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid(txtCreditNoteValue.Text))
                {
                    if (Convert.ToDecimal(txtCreditNoteValue.Text) > 0)
                    {
                        if (Convert.ToDecimal(txtCreditNoteValue.Text) <= Convert.ToDecimal(lblInvoiceAmount.Content))
                        {
                            if (txtReason.Text.Trim() != null && txtReason.Text.Trim() != "")
                            {
                                if ((_vm.IssuedCreditNotes + Convert.ToDecimal(txtCreditNoteValue.Text)) > _vm.TotalGross)
                                {
                                    MessageBox.Show("Credit Notes value exceeds Invoice Value");
                                    return;
                                }
                                _vm.SaveCommand.Execute(null);
                                _vm.ConfirmCommand.Execute(null);
                                NavigationService.Navigate(new Uri("/CN/ListInvoices", UriKind.Relative));
                            }
                            else
                                MessageBox.Show("Please provide a reason for the issuance of a credit note");
                        }
                        else
                            MessageBox.Show("Credit Note Amount should not be greater than Invoice Amount");
                    }
                    else
                        MessageBox.Show("Ensure the credit Note value is greater than zero");
                }
                else
                    MessageBox.Show("credit note value should be numeric");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Cancel Credit note? All changes will be lost", "Distributr: Cancel Action",
                                    MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            NavigationService.Navigate(new Uri("/CN/ListInvoices", UriKind.Relative));
        }

        private void txtCreditNoteValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isValid(txtCreditNoteValue.Text))
            {
                if (Convert.ToDecimal(txtCreditNoteValue.Text) > Convert.ToDecimal(lblInvoiceAmount.Content))
                    MessageBox.Show("Credit Note Amount should not be greater than Invoice Amount");
            }
            else
                MessageBox.Show("credit note value should be numeric");
        }
    }
}
