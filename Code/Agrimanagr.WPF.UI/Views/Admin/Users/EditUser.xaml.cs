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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Agrimanagr.WPF.UI.Views.Admin.Contacts;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;

namespace Agrimanagr.WPF.UI.Views.Admin.Users
{
    public partial class EditUser : PageBase
    {
        private EditAgriUsersViewModel _vm = null;
        private AddUserContactsModal modal = null;
        public EditUser()
        {
            InitializeComponent();
            _vm = DataContext as EditAgriUsersViewModel;
        }
         private void EditUserControl_Loaded(object sender, RoutedEventArgs e)
         {

             _vm = this.DataContext as EditAgriUsersViewModel;
             if (NavigationService != null)
             {
                 Guid userId = PresentationUtility.ParseIdFromUrl(NavigationService.CurrentSource);
                 string navParam = PresentationUtility.GetLastTokenFromUri(NavigationService.CurrentSource);
                 if (navParam != null && navParam.ToLower() == "driver")
                     _vm.CreateDriver = true;
                 else _vm.CreateDriver = false;
                 cmbUserType.IsEnabled = !_vm.CreateDriver;
                 _vm.Load(userId);
             }
         }
    }
}
