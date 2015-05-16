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
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;

namespace Agrimanagr.WPF.UI.Views.Admin.Users
{
    public partial class AddUserContactsModal : Window
    {
        private EditAgriUserContactViewModel _vm = null;
        public AddUserContactsModal(Contact contact = null)
        {
            InitializeComponent();
            _vm = DataContext as EditAgriUserContactViewModel;
            _vm.Setup(contact);
        }

       private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.IsValid())
                this.DialogResult = true;
        }

       private void btnCancel_Click(object sender, RoutedEventArgs e)
       {
           this.DialogResult = false;
       }
    }
}
