using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Integration.Cussons.WPF.Lib.CussonsIntegrationViewModels;
using Integration.Cussons.WPF.Lib.Utils;

namespace Integration.Cussons.WPF.UI.Pages
{
    /// <summary>
    /// Interaction logic for AdjustInventory.xaml
    /// </summary>
    public partial class AdjustInventory : Window, IAdjustInventoryWindow
    {
        private CussonsMainWindowViewModel _vm;
        public AdjustInventory()
        {
            InitializeComponent();
            _vm = DataContext as CussonsMainWindowViewModel;
            _vm.AdjustQuantity = 0m;
            _vm.RequestClose += (s, e) => this.Close();
        }

        public decimal ShowAdjustDialog()
        {
            ShowDialog();
            return _vm.AdjustQuantity;
        }
    }
}
