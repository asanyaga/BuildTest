using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using Distributr.WPF.Lib.ViewModels.MainPage;
using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Users
{
    public partial class UserRoutes : Page
    {
        private SalesmanRoutesViewModel _salesmanRoutesViewModel;
        private AddSalesmanRouteModal modal;
        bool confirmNavigateAway = true;
        private bool _isInitialized;
        readonly IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public UserRoutes()
        {

            _isInitialized = false;
            InitializeComponent();
            _isInitialized = true;
            _salesmanRoutesViewModel = DataContext as SalesmanRoutesViewModel;
            LabelControls();


        }

        void UserRoutes_Loaded(object sender, RoutedEventArgs e)
        {
            _salesmanRoutesViewModel = DataContext as SalesmanRoutesViewModel;
            _salesmanRoutesViewModel.ClearAndSetupCommand.Execute(null);
        }


        void LabelControls()
        {
            lblSalesman.Content = _messageResolver.GetText("sl.userRoutes.user_lbl");
            Addroute.Content = _messageResolver.GetText("sl.userroute.add");
            saveroutes.Content = _messageResolver.GetText("sl.userroute.confirm");

            //grid
            colRoute.Header = _messageResolver.GetText("sl.userroute.grid.col.route");
            colSalesman.Header = _messageResolver.GetText("sl.userroute.grid.col.salesman");
        }

        protected void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (confirmNavigateAway)
            {
                if (
                    MessageBox.Show(/*"Are you sure you want to move away from this page?" +
                    "Unsaved changes will be lost"*/
                    _messageResolver.GetText("sl.userroute.cancel.messagebox.prompt")
                                    , _messageResolver.GetText("sl.userroute.cancel.messagebox.caption")//"Distributr: Confirm Navigating Away"
                                    , MessageBoxButton.OKCancel) ==
                    MessageBoxResult.OK)
                {
                    //base.OnNavigatingFrom(e);
                }
                else
                    e.Cancel = true;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _salesmanRoutesViewModel.GetRoutesCommand.Execute(null);
        }

        private void saveroutes_Click(object sender, RoutedEventArgs e)
        {
            confirmNavigateAway = false;
            _salesmanRoutesViewModel.SaveRouteAssignmentCommand.Execute(null);
        }
        private MainPageViewModel _vm = new MainPageViewModel();

        private void Addroute_Click(object sender, RoutedEventArgs e)
        {
            bool canAccess = true;
            canAccess = _vm.CanAccess(UserRole.RoleCreateSalesmanRoute);

            if (canAccess == false)
            {
                MessageBox.Show("\n \tAccess Denied !!!! \n \tYou dont have permission access this module.\n \tPlease Contact the Administrator", "User Permissions");
            }
            else
            {
                if (_salesmanRoutesViewModel.UserLookup.Id == Guid.Empty)
                {
                    MessageBox.Show("Select Salesman first");
                    return;
                }
                else
                {
                    modal = new AddSalesmanRouteModal();
                    modal.Closed += new EventHandler(modal_Closed);

                    SalesmanRouteItemViewModel vmItem = modal.DataContext as SalesmanRouteItemViewModel;
                    _salesmanRoutesViewModel.GetRoutesNotAssignedCommand.Execute(null);
                    vmItem.RouteLookupList = _salesmanRoutesViewModel.RouteLookupList;
                    modal.cmbRoutes.ItemsSource = vmItem.RouteLookupList;

                    modal.ShowDialog();
                }
            }


        }


        void modal_Closed(object sender, EventArgs e)
        {
            bool result = modal.DialogResult.Value;
            if (result)
            {
                if (_salesmanRoutesViewModel.UserLookup.Id != Guid.Empty)
                {
                    SalesmanRouteItemViewModel vmItem = modal.DataContext as SalesmanRouteItemViewModel;
                    _salesmanRoutesViewModel.AddRoute(vmItem.RouteLookup);
                }
                else
                {
                    MessageBox.Show("Select Salesman first");
                }
            }
        }

        private void dgAssigned_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //StackPanel spEdit = dgAssigned.Columns.GetByName("colEdit").GetCellContent(dataGridrow) as StackPanel;

            //Hyperlink delete = spEdit.Children[2] as Hyperlink;
            //delete.Content = _messageResolver.GetText("sl.userroute.grid.col.delete");
        }

        private void hlDeactivate_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            string tag = hl.Tag.ToString();
            var selected = dgAssigned.SelectedItem as DistributorSalemanRoute;
            if (selected.EntityStatus == (int)EntityStatus.Active)
                _salesmanRoutesViewModel.Deactivate(Guid.Parse(tag));
            else if (selected.EntityStatus == (int)EntityStatus.Inactive)
                _salesmanRoutesViewModel.Activate(Guid.Parse(tag));

        }

        private SalesmanRoutesViewModel vm;
        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            var item = hl.DataContext as DistributorSalemanRoute;
            if (_salesmanRoutesViewModel.SalesmanRouteCheck(item.CostCentreId) == false)
            {
                _salesmanRoutesViewModel.Delete(item.MasterId);
            }
            _salesmanRoutesViewModel.DistributorSalemanRoute.Remove(item);

        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            _salesmanRoutesViewModel.ShowInactive = true;
            _salesmanRoutesViewModel.ReloadSalesmen();
            _salesmanRoutesViewModel.GetRoutesCommand.Execute(null);
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            _salesmanRoutesViewModel.ShowInactive = false;
            _salesmanRoutesViewModel.ReloadSalesmen();
            _salesmanRoutesViewModel.GetRoutesCommand.Execute(null);
        }
    }
}
