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
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.ViewModels.Admin.Routes;

using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Routes
{
    public partial class ListRoutes : Page
    {
        private ListRoutesViewModel _vm;
        private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();

        public ListRoutes()
        {
            _vm = this.DataContext as ListRoutesViewModel;
            InitializeComponent();
            LabelControls();
            this.Loaded += new RoutedEventHandler(ListRoutes_Loaded);

            double height = OtherUtilities.ContentFrameHeight * 0.8;
            dgRoutes.MaxHeight = height;
        }

        void LabelControls()
        {
            btnAdd.Content = _messageResolver.GetText("sl.routes.list.add");
            //grid
            colCode.Header = _messageResolver.GetText("sl.routes.list.grid.col.code");
            colName.Header = _messageResolver.GetText("sl.routes.list.grid.col.name");
        }

        void ListRoutes_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as ListRoutesViewModel;
            _vm.RunGetRoutes();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/views/administration/routes/editroute.xaml?" + Guid.Empty, UriKind.Relative));
        }

        private void hlDeactivate_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hb = sender as Hyperlink;
            Guid id = (Guid)hb.Tag;
            var route = (ListRoutesViewModel.ListRouteItem)dgRoutes.SelectedItem;
            if (route.EntityStatus == (int)EntityStatus.Active)
            {
                _vm.DeactivateRoute(id);
            }
            else if (route.EntityStatus == (int)EntityStatus.Inactive)
            {
                _vm.ActivateRoute(id);
            }
        }

        private void dgRoutes_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);

            //HyperlinkButton edit = dgRoutes.Columns.GetByName("colEdit").GetCellContent(dataGridrow) as HyperlinkButton;
            //edit.Content = _messageResolver.GetText("sl.routes.list.grid.col.edit");
            //HyperlinkButton deactivate = dgRoutes.Columns.GetByName("colDeactivate").GetCellContent(dataGridrow) as HyperlinkButton;
            //deactivate.Content = _messageResolver.GetText("sl.routes.list.grid.col.deactivate");

            ////StackPanel stackPanel = dgRoutes.Columns.GetByName("colManageRoute").GetCellContent(dataGridrow) as StackPanel;

            //HyperlinkButton edit = stackPanel.Children[0] as HyperlinkButton;
            //edit.Content = _messageResolver.GetText("sl.routes.list.grid.col.edit");

            //HyperlinkButton delete = stackPanel.Children[4] as HyperlinkButton;
            //delete.Content = _messageResolver.GetText("sl.routes.list.grid.col.delete");
        }
        private void hpActivate_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hb = sender as Hyperlink;
            Guid id = (Guid)hb.Tag;
            ListRoutesViewModel vm = this.DataContext as ListRoutesViewModel;
            vm.RouteId = id;
            var route = (ListRoutesViewModel.ListRouteItem)dgRoutes.SelectedItem;
            if (route.EntityStatus != (int)EntityStatus.Deleted)
            {
                _vm.ActivateRoute(id);
            }
            _vm.RunGetRoutes();

        }

        private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hb = sender as Hyperlink;
            Guid id = (Guid) hb.Tag;
            ListRoutesViewModel vm = this.DataContext as ListRoutesViewModel;
            vm.DeleteRoute(id);
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as ListRoutesViewModel;
            _vm.RunGetRoutes(true);
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as ListRoutesViewModel;
            _vm.RunGetRoutes();
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = (sender as Hyperlink);
            string url = "views/administration/routes/editroute.xaml?" + hl.Tag.ToString();
            NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }
       
    }
}
