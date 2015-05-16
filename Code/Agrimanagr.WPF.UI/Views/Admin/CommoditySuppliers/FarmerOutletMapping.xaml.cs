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
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.Admin.CommoditySuppliers
{
   
    public partial class FarmerOutletMapping : Window, IFarmerOutletMapping
    {
        private SupplierToOutletMappingViewModel _vm;
        public FarmerOutletMapping()
        {
            InitializeComponent();
            _vm = DataContext as SupplierToOutletMappingViewModel;
            _vm.RequestClose += (s, e) => this.Close();
        }




        public void SupplierToOutletToMappping(CommoditySupplier supplier)
        {
             this.CenterWindowOnScreen();
            _vm.Setup(supplier);
         _vm.ChoosenSupplier = supplier;
            ShowDialog();
           
          
          
        }
    }
}
