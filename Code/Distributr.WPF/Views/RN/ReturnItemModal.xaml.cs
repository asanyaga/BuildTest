using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.RN;
using StructureMap;

namespace Distributr.WPF.Views.RN
{
    public partial class ReturnItemModal : Window
    {
        private ReturnItemViewModel _rivm;
        public ReturnItemModal()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ReturnItemModal_Loaded);
        }

        void ReturnItemModal_Loaded(object sender, RoutedEventArgs e)
        {
            LocalizeLabels();
            _rivm = this.DataContext as ReturnItemViewModel;
        }

        private void LocalizeLabels()
        {
            IMessageSourceAccessor   messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
            this.Title = messageResolver.GetText("sl.returns.modal.title");
            labelactual.Content = messageResolver.GetText("sl.returns.modal.actual");
            labelReturnType.Content = messageResolver.GetText("sl.returns.modal.returntype");
            labelProduct.Content = messageResolver.GetText("sl.returns.modal.product");
            labelexpected.Content = messageResolver.GetText("sl.returns.modal.expected");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtActual.Text))
            {
                MessageBox.Show("Expected cannot be null");
                return;
            }
            if (!isValid(txtActual.Text))
            {
                MessageBox.Show("Expected has to be numeric");
                return;
            }
            this.DialogResult = true;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel?", "Returns Serials", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.DialogResult = false;
            }
        }

        bool isValid(string str)
        {
            try
            {
                Decimal.Parse(str);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void btnAdSerial_Click(object sender, RoutedEventArgs e)
        {

        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            _rivm.EditSerial();
        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            _rivm.DeleteSerial();
        }

        private void txtFrom_KeyDown(object sender, KeyEventArgs e)
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
                _rivm.EndSerialNo = txtTo.Text;
                btnAdSerial.Command.Execute(_rivm.AddSerialsCommand);
                txtFrom.Focus();
            }
        }

        private void txtFrom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtTo.Focus();
            }
        }

        private void txtTo_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}

