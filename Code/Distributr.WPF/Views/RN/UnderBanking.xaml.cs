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
using Distributr.WPF.Lib.ViewModels.Transactional.RN;

namespace Distributr.WPF.UI.Views.RN
{
    /// <summary>
    /// Interaction logic for UnderBanking.xaml
    /// </summary>
    public partial class UnderBanking : Window, IUnderBankingPopUp
    {
        private UnderBankingViewModel _vm;
        public UnderBanking()
        {
            InitializeComponent();
            _vm = DataContext as UnderBankingViewModel;
            _vm.RequestClose += (s, e) => this.Close();
        }

        public bool AddUnderBanking()
        {
            this.Owner = Application.Current.MainWindow;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.CenterWindowOnScreen();
            _vm.LineItems.Clear();
            this.ShowDialog();
            return _vm.Status;
        }
    }
}
