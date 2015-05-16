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

namespace Distributr.WPF.UI.Views.SalesmanOrders
{
    public partial class RejectReasonModal : Window
    {
        public bool rejectingBackOrder = false;
        public RejectReasonModal()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtRejectReason.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(txtRejectReason.Text.Trim()))
                {
                    MessageBox.Show("You must enter the reason for rejecting the order.", "Distributr: Reject Order",
                                    MessageBoxButton.OKCancel);
                    return;
                }
            }
            if (
                MessageBox.Show("Are you sure you want to reject the order?", "Distributr: Reject Order",
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
