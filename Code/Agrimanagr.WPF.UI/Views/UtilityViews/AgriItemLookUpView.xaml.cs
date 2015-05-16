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
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using StructureMap;

namespace Agrimanagr.WPF.UI.Views.UtilityViews
{
    /// <summary>
    /// Interaction logic for AgriItemLookUpView.xaml
    /// </summary>
    public partial class AgriItemLookUpView : Window, IAgriItemsLookUp
    {
     
         private ItemLookUpsLookUpViewModel _vm;
         public AgriItemLookUpView()
        {
            InitializeComponent();
            _vm = DataContext as ItemLookUpsLookUpViewModel;
            _vm.SetUp();
            _vm.RequestClose += (s, e) => this.Close();
        }

         void HideOrUnhide()
         {
             foreach (var dgColumn in dgItems.Columns.Where(dgColumn => dgColumn.Header.ToString() == "Code"))
             {
                 dgColumn.Visibility = _vm.CodeVisible ? Visibility.Visible : Visibility.Collapsed;
                 break;
             }
         }
        public Outlet SelectOutletToMapToSupplier()
        {
            _vm.LoadedEntity = "OutletToMapToSupplier";
            // _vm.Param1 = SalesmanId;
            _vm.LoadOutletToMapToSupplier();
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id) as Outlet;
            }
            return null;
        }

        public Warehouse SelectWarehouse()
        {
            _vm.LoadedEntity = "Warehouse";
            // _vm.Param1 = SalesmanId;
            _vm.LoadWarehouse();
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id) as Warehouse;
            }
            return null;
        }


        public CommoditySupplier SelectCommoditySupplier()
        {
            _vm.LoadedEntity = "CommoditySupplier";
            // _vm.Param1 = SalesmanId;
            _vm.LoadCommoditySupplier();
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id) as CommoditySupplier;
            }
            return null;
        }

        public Store SelectStore()
        {
            _vm.LoadedEntity = "Store";
            // _vm.Param1 = SalesmanId;
            _vm.LoadStore();
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id) as Store;
            }
            return null;
        }
        public Commodity SelectCommodity()
        {
            _vm.LoadedEntity = "Commodity";
            // _vm.Param1 = SalesmanId;
            _vm.LoadCommodity();
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICommodityRepository>().GetById(id);
            }
            return null;
        }

        public CommodityOwner SelectFarmer()
        {
            _vm.LoadedEntity = "CommodityOwner";
            // _vm.Param1 = SalesmanId;
            _vm.LoadCommodityOwner();
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICommodityOwnerRepository>().GetById(id);
            }
            return null;
        }

        public CommodityOwner SelectFarmersBySupplier(Guid supplierId)
        {
            _vm.LoadedEntity = "CommodityOwnersBySupplier";
             _vm.Param1 = supplierId;
            _vm.LoadCommodityOwnersBySupplier(supplierId);
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICommodityOwnerRepository>().GetById(id);
            }
            return null;
        }

        public CommodityGrade SelectGrade(Guid commodityId)
        {
            _vm.LoadedEntity = "CommodityGrade";
            _vm.Param1 = commodityId;
            _vm.LoadCommodityGrade(commodityId);
            HideOrUnhide();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICommodityRepository>().GetGradeByGradeId(id);
            }
            return null;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
        
            this.DialogResult = true;
       
        }
    }
}
