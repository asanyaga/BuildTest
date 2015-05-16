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

namespace Distributr.WPF.UI.Views.Payments
{
    /// <summary>
    /// Interaction logic for GetPaymentReferenceModal.xaml
    /// </summary>
    public partial class GetPaymentReferenceModal : Window
    {
        public GetPaymentReferenceModal()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtReference.Text.Trim() == "")
            {
                MessageBox.Show("You must enter the Transaction refernce number.", "Distributr: Payment Module", MessageBoxButton.OK);
                return;
            }
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
