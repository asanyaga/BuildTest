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
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.TransactionSatement;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.UI.Views.TransactionStatement
{
    public partial class TransactionStatement : Page
    {
        private TransactionStatementViewModel _vm;
        public TransactionStatement()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(TransactionStatement_Loaded);
            _vm = this.DataContext as TransactionStatementViewModel;
        }

        void TransactionStatement_Loaded(object sender, RoutedEventArgs e)
        {
            string orderId = NavigationService.Source.OriginalString.ParseQueryString("OrderId");
            _vm.OrderId = new Guid(orderId);

            _vm.LoadData();
        }

        private void hlInvRef_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/invoicedocument/invoicedocument.xaml?orderid=" + _vm.OrderId, UriKind.Relative));
        }

        private void btnreturnToList_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(OtherUtilities.StrBackUrl, UriKind.Relative));
            //NavigationService.GoBack();
        }

        private void hlRctRef2_Click(object sender, RoutedEventArgs e)
        {
            var hl = sender as Hyperlink;
            var rctId = new Guid(hl.Tag.ToString());
            NavigationService.Navigate(
                new Uri("/views/receiptdocument/receiptdocument.xaml?orderid=" + _vm.OrderId + "&receiptid=" + rctId,
                        UriKind.Relative));
        }

    }
}
