using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers.Contacts;

namespace Agrimanagr.WPF.UI.Views.Admin.Contacts
{
    public partial class AddUserContactsModal : Window, IEditContactModal
    {
        private EditAgriUserContactViewModel _vm = null;
        public AddUserContactsModal()
        {
            InitializeComponent();
            _vm = DataContext as EditAgriUserContactViewModel;
        }

        public bool AddUserContact(Contact contactToEdit, out Contact contactReturned)
        {
            _vm = DataContext as EditAgriUserContactViewModel;
            _vm.Setup(contactToEdit);
            ShowDialog();
            contactReturned = _vm.Contact;
            return DialogResult != null && (bool) DialogResult;
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
