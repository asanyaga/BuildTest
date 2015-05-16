using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;

namespace Distributr.WPF.UI.Views.Order_Pos
{
    /// <summary>
    /// Interaction logic for PaymentModeModalPopup.xaml
    /// </summary>
    public partial class PaymentModeModalPopup : Window, IPaymentPopup
    {
        private PaymentModePopUpViewModel _vm;
        public PaymentModeModalPopup()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
            _vm = DataContext as PaymentModePopUpViewModel;
            _vm.SetupCommand.Execute(null);
            _vm.RequestClose += (s, e) => this.Close();

        }

        public List<PaymentInfo> GetPayments(decimal amountToPay, Guid orderId)
        {
            _vm.GrossAmount = amountToPay;
             _vm.OrderId = orderId;
             _vm.InitOrderInforCommand.Execute(null);
              ShowDialog();
            return _vm.GetPayMentInformation();
        }

        public List<PaymentInfo> GetPayments(decimal amountToPay, string orderdocumentReference = "",string invoiceref="")
        {
            _vm.GrossAmount = amountToPay;
            _vm.OrderDocReference = orderdocumentReference;
            _vm.InvoiceDocReference = invoiceref;
            _vm.InitOrderInforCommand.Execute(null);
            ShowDialog();
            return _vm.GetPayMentInformation();
        }
    }
}
