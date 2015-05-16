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
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers;

namespace Agrimanagr.WPF.UI.Views.Admin.CommoditySuppliers
{public partial class EditCommodityOwnerModal : Window, IEditCommodityOwnerModal
    {
    private EditCommodityOwnerModalViewModel _vm;

    public EditCommodityOwnerModal()
        {
            InitializeComponent();
        }
        public bool AddCommodityOwner(CommodityOwner commodityOwnerToEdit, out CommodityOwner commodityOwnerReturned)
        {
            _vm = DataContext as EditCommodityOwnerModalViewModel;
            _vm.Load(commodityOwnerToEdit);
            _vm.CloseDialog += (s, e) => this.Close();
            ShowDialog();
            commodityOwnerReturned = _vm.CommodityOwner;
            return _vm.DialogResult; ;
        }
    }
}
