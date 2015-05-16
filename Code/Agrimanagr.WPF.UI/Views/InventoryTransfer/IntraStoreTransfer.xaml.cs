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
    /// Interaction logic for IntraStoreTransfer.xaml
    /// </summary>
    public partial class IntraStoreTransfer : Page
    {
        private readonly IntraStoreTransferViewModel _vm;
        public IntraStoreTransfer()
        {
            InitializeComponent();
            _vm = this.DataContext as IntraStoreTransferViewModel;
            _vm.SetUp();
            
        }
    }
}
