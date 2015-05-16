using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Resources.Util;
using Distributr.WPF.Lib.Impl.Services.Utility;
using Distributr.WPF.Lib.UI.UI_Utillity;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;

using StructureMap;

namespace Distributr.WPF.UI.Views.Administration.Contacts
{
    /// <summary>
    /// Interaction logic for ListContacts.xaml
    /// </summary>
    public partial class ListContacts : Page
    {
        private NewListContactViewModel _vm;
        private bool _isInitialized;
        IMessageSourceAccessor _messageResolver = ObjectFactory.GetInstance<IMessageSourceAccessor>();
        public ListContacts()
        {
            _isInitialized = false;
            InitializeComponent();
            _isInitialized = true;
            LabelControls();
           
            double width = OtherUtilities.ContentFrameWidth * 0.9;
            Width = width;
            LayoutRoot.Width = width;
            double width2 = width * 0.9;

            dgContacts.Width = width;
            spBottom.Width = width2;
           
        }

        void LabelControls()
        {
            lblFilter.Content = _messageResolver.GetText("");
            lblOwnerType.Content = _messageResolver.GetText("sl.contacts.edit.contactOwnerType");
            lblOwner.Content = _messageResolver.GetText("sl.contacts.edit.contactOwner");
            btnAdd.Content = _messageResolver.GetText("sl.contacts.list.add");

            
        }
        
        void ListContacts_Loaded(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as NewListContactViewModel;
          // _vm.NewListContactsLoadedCommand.Execute(null);
           
            
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
                NavigationService.Navigate(new Uri("views/administration/contacts/editcontact.xaml?" + Guid.Empty, UriKind.Relative));
        }

        /* private void cmbContactOwnerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (cmbContactOwnerType.SelectedItem != ContactOwnerType.Distributor.ToString())
               //_vm.SelectedContactOwnerType = ContactOwnerType.Distributor;
            {
                _vm.ContactOwnerTypeSelectionChangedCommand.Execute(null);
            }
           
           // cmbContactOwner.ItemsSource = _vm.ContactOwners;
        }*/

       /* private void cmbContactOwner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
          
            _vm.ContactOwnerSelectionChangedCommand.Execute(null);
          
        }*/

        private void hlDeactivate_Click(object sender, RoutedEventArgs e)
        {
          
            var hlb = sender as Hyperlink;
            _vm.ContactId = new Guid(hlb.Tag.ToString());

            var contact = (ListContactViewModel.ContactLineItem)dgContacts.SelectedItem;
            if (contact.EntityStatus == (int)EntityStatus.Active)
            {
                if (MessageBox.Show("Are you sure you want to deactivate this contact?",
                                    "Distributr: Deactivate Contact",
                                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                   
                    _vm.DeactivateConctact();
                    _vm.Loading = true;
                    dgContacts.ItemsSource = _vm.Contacts;
                    _vm.Loading = false;
                }
            }
            else if (contact.EntityStatus == (int)EntityStatus.Inactive)
            {
                if (MessageBox.Show("Are you sure you want to activate this contact?",
                                    "Distributr: Activate Contact",
                                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    _vm.ContactId = new Guid(hlb.Tag.ToString());
                    _vm.ActivateContact();
                    _vm.Loading = true;
                    dgContacts.ItemsSource = _vm.Contacts;
                    _vm.Loading = false;
                }
            }

        }

      /*  private void hlDelete_Click(object sender, RoutedEventArgs e)
        {
            var hl = sender as Hyperlink;
            if (true)
                if (hl != null) _vm.Id = (Guid)hl.Tag;
            if (MessageBox.Show("Are you sure you want to delete this contact?",
                                    "Distributr: Delete Contact",
                                    MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _vm.DeleteContact();
                _vm.Loading = true;
                dgContacts.ItemsSource = _vm.Contacts;
                _vm.Loading = false;
            }
        }*/

        private void dgContacts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            /*DataGridRow dataGridrow = DataGridRow.GetRowContainingElement(e.Row);
            StackPanel stackPanel = dgContacts.Columns.GetByName("colManageContact").GetCellContent(dataGridrow) as StackPanel;
*/
            //Hyperlink edit = stackPanel.Children[0] as Hyperlink;
           // edit.t = _messageResolver.GetText("sl.users.grid.col.edit");

           // Hyperlink delete = stackPanel.Children[4] as Hyperlink;
            //delete.Content = _messageResolver.GetText("sl.users.grid.col.delete");
        }

        private void chkShowInactive_Checked(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as NewListContactViewModel;
            _vm.ShowInactive = true;
            _vm.GetContacts();
        }

        private void chkShowInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            _vm = this.DataContext as NewListContactViewModel;
            _vm.ShowInactive = false;
            _vm.GetContacts();
        }

        private void hlEdit_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hl = sender as Hyperlink;
            if (hl != null)
            {
                string url = "views/administration/contacts/editcontact.xaml?" + hl.Tag;

                if (NavigationService != null) NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

      

       /* private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }*/

      
    }
}
