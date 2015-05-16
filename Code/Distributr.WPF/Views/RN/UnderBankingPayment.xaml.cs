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
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;

namespace Distributr.WPF.UI.Views.RN
{
    /// <summary>
    /// Interaction logic for UnderBankingPayment.xaml
    /// </summary>
    public partial class UnderBankingPayment : Window, IUnderBankingConfirmationPopUp
    {
        public UnderBankingPayment()
        {
            InitializeComponent();
        }

        public bool ShowPaymentInfor()
        {
            this.Owner = Application.Current.MainWindow;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.CenterWindowOnScreen();
            this.ShowDialog();
            return true;
        }
    }
}
