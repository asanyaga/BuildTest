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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers;

namespace Agrimanagr.WPF.UI.Views.Admin.CommoditySuppliers
{
    public partial class EditCommodityProducerModal : Window, IEditCommodityProducerModal
    {
        private EditCommodityProducerModalViewModel _vm;

        public EditCommodityProducerModal()
        {
            InitializeComponent();
        }

        public bool AddCommodityProducer(CommodityProducer commodityProducerToEdit, out CommodityProducer commodityProducerReturned)
        {
            _vm = DataContext as EditCommodityProducerModalViewModel;
            _vm.Load(commodityProducerToEdit);
            _vm.CloseDialog += (s, e) => this.Close();
            ShowDialog();
            commodityProducerReturned = _vm.CommodityProducer;
            return _vm.DialogResult; ;
        }
    }
}
