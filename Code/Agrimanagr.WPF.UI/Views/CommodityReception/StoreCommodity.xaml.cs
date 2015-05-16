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
using Agrimanagr.WPF.UI.Views.UtilityViews;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception;

namespace Agrimanagr.WPF.UI.Views.CommodityReception
{
    public partial class StoreCommodity : Window, IStoreCommodityPopUp
    {
        private StoreCommodityViewModel _vm;
        public StoreCommodity()
        {
            InitializeComponent();
            _vm = this.DataContext as StoreCommodityViewModel;
            _vm.SetUp();
            _vm.RequestClose += (s, e) => this.Close();
           
        }
      
        public void ShowCommodityToStore(List<Guid> items)
        {
            _vm.GetItemsToStore(items);
            _vm.StorageCommodityPageLoadedCommand.Execute(null);
            this.ShowDialog();
        }
    }
}
