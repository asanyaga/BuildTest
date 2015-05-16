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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer;

namespace Agrimanagr.WPF.UI.Views.InventoryTransfer
{
    /// <summary>
    /// Interaction logic for CommodityRelease.xaml
    /// </summary>
    public partial class CommodityRelease : Page
    {
        private readonly CommodityReleaseViewModel _vm;
        public CommodityRelease()
        {
            InitializeComponent();
            _vm = this.DataContext as CommodityReleaseViewModel;
            _vm.SetUp();
        }
    }
}
