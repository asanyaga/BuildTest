using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.EagcLogin;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.SuppliersEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.SalesmanTargets;
using Distributr.WPF.Lib.ViewModels.Transactional.DispatchProducts;
using Distributr.WPF.Lib.ViewModels.Transactional.GRN;
using Distributr.WPF.Lib.ViewModels.Transactional.IAN;
using Distributr.WPF.Lib.ViewModels.Transactional.ITN;
using Distributr.WPF.Lib.ViewModels.Transactional.Order_Pos;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;

using StructureMap;

namespace Distributr.WPF.UI.Views.Utils
{
    /// <summary>
    /// Interaction logic for ItemLookUp.xaml
    /// </summary>
    public partial class ItemLookUp : Window, IItemsLookUp
    {
        private ItemLookUpsLookUpViewModel _vm;
        public ItemLookUp()
        {
            InitializeComponent();
            this.Topmost = true;
            this.CenterWindowOnScreen();
            _vm = DataContext as ItemLookUpsLookUpViewModel;
            _vm.SetUp();
            _vm.RequestClose += (s, e) => this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        void HideOrUnhide()
        {
            foreach (var dgColumn in dgItems.Columns.Where(dgColumn => dgColumn.Header.ToString() == "Code"))
            {
                dgColumn.Visibility = _vm.CodeVisible ? Visibility.Visible : Visibility.Collapsed;
                break;
            }
        }

        public Guid ShowDlg(Type objectType, int defaultTake = 50)
        {
            _vm.LoadedEntity = objectType.Name;
            _vm.LoadComboItems(defaultTake);
            _vm.CodeVisible = true;
            HideOrUnhide();
            this.Title = _vm.LoadedEntity + " List";
            ShowDialog();
            var selected=
            _vm.ReturnSelected();
            return selected !=null?selected.Id:Guid.Empty;
        }

        public CostCentre SelectUnderbankingCostCentre(Guid SalesmanId)
        {
            _vm.LoadedEntity = "UnderbankingCostCentre";
            _vm.Param1 = SalesmanId;
            _vm.LoadUnderbankingCostCentre(SalesmanId);
            HideOrUnhide();
            this.Title = "Underbanking Cost Centre List";
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
              var  id=selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id);
            }
            return null;
           
        }



     
        public CostCentre SelectDistributrSalesman()
        {
            _vm.LoadedEntity = "DistributrSalesman";
           // _vm.Param1 = SalesmanId;
            _vm.LoadDistributrSalesman();
            this.Title = "Distributor Salesman List";
            HideOrUnhide();
           ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id);
            }
            return null;
        }



      /*  public CostCentreRef SelectDistributrSalesman1()
        {
            _vm.LoadedEntity = "DistributrSalesman";
            // _vm.Param1 = SalesmanId;
            _vm.LoadDistributrSalesman();
            this.Title = "Distributor Salesman List";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>() as CostCentreRef;
            }
            return null;
        }*/




        public CostCentre SelectStockistDistributrSalesman()
        {
            _vm.LoadedEntity = "StockistDistributrSalesman";
            // _vm.Param1 = SalesmanId;
            _vm.LoadStockistDistributrSalesman();
            this.Title = "Distributor Stockist List";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id);
            }
            return null;
        }

        public Outlet SelectOutletToMapToSupplier()
        {
            _vm.LoadedEntity = "OutletToMapToSupplier";
            // _vm.Param1 = SalesmanId;
            this.Title = "Outlet List";
            _vm.LoadOutletToMapToSupplier();
            HideOrUnhide();
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

        public Outlet SelectOutletBySalesman(Guid salesmanId)
        {
            _vm.LoadedEntity = "OutletBySalesman";
            _vm.Param1 = salesmanId;
            this.Title = "Outlet List";
            _vm.LoadOutletsForSalesman(salesmanId);
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id) as Outlet;
            }
        }

     
        public Route SelectRoute(Guid salesmanId)
        {
            _vm.LoadedEntity = "Route";
             _vm.Param1 = salesmanId;
             _vm.LoadDistributrSalesmanRoute(salesmanId);
            this.Title = "Distributor Salesman Routes";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<IRouteRepository>().GetById(id);
            }
            return null;
        }

        public CostCentre SelectContactOwner(int contactOwnerType, Guid costCenId)
        {
            _vm.LoadedEntity = "AllContactsOwners";
            _vm.Param1 = contactOwnerType;
            _vm.Param2 = costCenId;
            _vm.LoadAllContactOwners(contactOwnerType, costCenId);
            this.Title = "Contacts Owners";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id);
            }
            return null;
        }

        public ContactType SelectContactType()
        {
            _vm.LoadedEntity = "AllContactsTypes";
            //_vm.Param1 = salesmanId;
            _vm.LoadAllContactTypes();
            this.Title = "Contact Types";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<IContactTypeRepository>().GetById(id);
            }
            return null;
        }


        public Guid IssueInventory(Guid? salesmanId =null)
        {
            _vm.Param1 = salesmanId;
            _vm.LoadedEntity = "Product";
            this.Title = "Product List";
            _vm.issueInventoryProducts = true;
            _vm.LoadComboItems(20);
            _vm.CodeVisible = true;
            HideOrUnhide();
            ShowDialog();
            var selected =
             _vm.ReturnSelected();
            _vm.issueInventoryProducts = false;
            return selected != null ? selected.Id : Guid.Empty;
        }


        public User SelectSelectedSalesman()
        {
            _vm.LoadedEntity = "Salesman";
           // _vm.Param1 = routeId;
            _vm.LoadSalesMen();
            this.Title = "SalesMen";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<IUserRepository>().GetById(id);
            }
            return null;
        }

        public Outlet SelectOutletByRoute(Guid routeId)
        {
            _vm.LoadedEntity = "Outlet";
            _vm.Param1 = routeId;
            _vm.LoadOutetsInRoute(routeId);
            this.Title = "Distributor Outlets";
            HideOrUnhide();
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

        public Route SelectRoute()
        {
            _vm.LoadedEntity = "LoadAllRoutes";
            //_vm.Param1 = salesmanId;
            _vm.LoadAllRoutes();
            this.Title = "Distributor Salesman Routes";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<IRouteRepository>().GetById(id);
            }
            return null;
        }

        public Guid SelectOutletShipToAddress(Guid outletId)
        {
            _vm.LoadedEntity = "ShipToAddress";
            _vm.Param1 = outletId;
            _vm.LoadOutletShipto(outletId);
            this.Title = "Outlet ShipTo Addresses";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            
            return selected==null?Guid.Empty:selected.Id;
        }

        public TargetPeriod SelectTargetPeriod()
        {
            _vm.LoadedEntity = "AllPeriods";
            //_vm.Param1 = salesmanId;
            _vm.LoadAllPeriods();
            this.Title = "APeriods";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ITargetPeriodRepository>().GetById(id);
            }
            return null;
        }

        public Warehouse SelectDistribtrWarehouse(Guid? parentCostCentreId)
        {
            _vm.LoadedEntity = "DistribtrWarehouse";
            _vm.Param1 = parentCostCentreId;
            _vm.LoadAllDistributrWarehouses(parentCostCentreId);
            this.Title = "Distribtr Warehouse";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ICostCentreRepository>().GetById(id)as Warehouse;
            }
            return null;
        }

        public Supplier SelectSupplier()
        {
            _vm.LoadedEntity = "DistribtrSuppliers";
            //_vm.Param1 = salesmanId;
            _vm.LoadDistribtrSuppliers();
            this.Title = "Distribtr Suppliers";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<ISupplierRepository>().GetById(id);
            }
            return null;
        }

        public Product SelectProduct(Guid? supplierId)
        {
            _vm.LoadedEntity = "ProSuppliers";
            //_vm.Param1 = salesmanId;
            _vm.LoadDistribtrSuppliers();
            this.Title = "Distribtr Suppliers";
            HideOrUnhide();
            ShowDialog();
            var selected =
            _vm.ReturnSelected();
            using (var container = ObjectFactory.Container.GetNestedContainer())
            {
                var id = selected != null ? selected.Id : Guid.Empty;
                return container.GetInstance<IProductRepository>().GetById(id);
            }
            return null;
        }



      

    }
}
