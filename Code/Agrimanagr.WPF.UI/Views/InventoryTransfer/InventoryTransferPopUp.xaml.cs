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
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer;

namespace Agrimanagr.WPF.UI.Views.InventoryTransfer
{
    /// <summary>
    /// Interaction logic for InventoryTransferPopUp.xaml
    /// </summary>
    public partial class InventoryTransferPopUp : Page, IInventoryTransferPopUp
    {
        private InventoryTransferViewModelPopUp _vm;

        public InventoryTransferPopUp()
        {
            InitializeComponent();
            _vm = this.DataContext as InventoryTransferViewModelPopUp;
            _vm.SetUp();
           // this.CenterWindowOnScreen();
           // this.Owner = Application.Current.MainWindow;
           // _vm = this.DataContext as InventoryTransferViewModelPopUp;
           // _vm.RequestClose += (s, e) => this.Close();
        }

        public void ShowCommodityTransfer()
        {
            _vm.SetUp();
           // this.ShowDialog();
        }
    }
}
