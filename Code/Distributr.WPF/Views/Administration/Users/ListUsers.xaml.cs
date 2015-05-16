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
using Distributr.WPF.Lib.Data.IOC;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.ViewModels.Admin.Users;

using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Users
{
    public partial class ListUsers : Page
    {
       private IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public ListUsers()
        {
            InitializeComponent();
            LabelControls();
          
        }

        void LabelControls()
        {
            btnAddUser.Content = _messageResolver.GetText("sl.users.add");

            //grid
            colUsername.Header = _messageResolver.GetText("sl.users.grid.col.name");
            colCode.Header = _messageResolver.GetText("sl.users.grid.col.code");
            colCostCentre.Header = _messageResolver.GetText("sl.users.grid.col.costcentre");
            colPin.Header = _messageResolver.GetText("sl.users.grid.col.pin");
            colTill.Header = _messageResolver.GetText("sl.users.grid.col.till");
            colUserType.Header = _messageResolver.GetText("sl.users.grid.col.type");
            colMobile.Header = _messageResolver.GetText("sl.users.grid.col.mobile");
        }
        

       

        private void hlResetPassword_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            //if(!_isInitialised)return;
            //lblWait.Visibility = Visibility.Visible;
            //_vm = this.DataContext as ListUsersViewModel;
            //_vm.ShowInactive = true;
            //_vm.GetUsers();
            //lblWait.Visibility = Visibility.Collapsed;
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            //lblWait.Visibility = Visibility.Visible;
            //_vm = this.DataContext as ListUsersViewModel;
            //_vm.ShowInactive = false;
            //_vm.GetUsers();
            //lblWait.Visibility = Visibility.Collapsed;
        }

        private void dgUsers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            //StackPanel stackPanel = dgUsers.Columns.GetByName("colManageUser").GetCellContent(dataGridrow) as StackPanel;

            //Hyperlink edit = stackPanel.Children[0] as Hyperlink;
            //edit.Content = _messageResolver.GetText("sl.users.grid.col.edit");

            //HyperlinkButton resetpwd = stackPanel.Children[2] as HyperlinkButton;
            //resetpwd.Content = _messageResolver.GetText("sl.users.grid.col.resetpwd");

            ////HyperlinkButton deactivate = stackPanel.Children[4] as HyperlinkButton;
            ////deactivate.Content = _messageResolver.GetText("sl.users.grid.col.deactivate");

            //HyperlinkButton delete = stackPanel.Children[6] as HyperlinkButton;
            //delete.Content = _messageResolver.GetText("sl.users.grid.col.delete");
        }
    }
}
