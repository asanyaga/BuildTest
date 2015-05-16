using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Stores
{
    public class EditStoreViewModel : MasterEntityContactUtilsViewModel
    {
        private IDistributorServiceProxy _proxy;

        public EditStoreViewModel()
        {
            ContactsList = new ObservableCollection<VMContactItem>();
        }

        #region properties

        private RelayCommand _saveCmd;

        public RelayCommand SaveCommand
        {
            get { return _saveCmd ?? (_saveCmd = new RelayCommand(Save)); }
        }

        private RelayCommand _cancelCmd;

        public RelayCommand CancelCommand
        {
            get { return _cancelCmd ?? (_cancelCmd = new RelayCommand(Cancel)); }
        }

        public const string StorePropertyName = "Store";
        private Store _store = null;

        public Store Store
        {
            get { return _store; }

            set
            {
                if (_store == value)
                {
                    return;
                }

                RaisePropertyChanging(StorePropertyName);
                _store = value;
                RaisePropertyChanged(StorePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Edit Centre";

        public string PageTitle
        {
            get { return _pageTitle; }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        #endregion


        #region methods

        protected override void LoadPage(Page page)
        {
            Guid storeId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            ContactsList.Clear();
            using (var c = NestedContainer)
            {
                if (storeId == Guid.Empty)
                {
                    PageTitle = "Create Store";
                    Store = Using<CostCentreFactory>(c).CreateCostCentre(Guid.NewGuid(), CostCentreType.Store,
                                                                         Using<ICostCentreRepository>(c).GetById(
                                                                             GetConfigParams().CostCentreId))
                            as Store;
                }
                else
                {
                    var store = Using<IStoreRepository>(c).GetById(storeId) as Store;
                    Store = store.DeepClone<Store>();
                    PageTitle = "Edit Store";
                    LoadEntityContacts(Store);
                }
            }
        }

        public async void Save()
        {
            Store.Contact = new List<Contact>();
            Store.Contact.AddRange(ContactsList.Select(n => n.Contact));
            if (!IsValid())
                return;
            if (Store.Name == null )
            {
                MessageBox.Show( "Name field is Empty","Agrimanagr: Manage Stores", MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            using (var c = NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await _proxy.StoreAddAsync(Store);
                string log = string.Format("Created store: {0}; Code: {1};", Store.Name,
                                           Store.CostCentreCode);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Store Management", log);

                MessageBox.Show(response.ErrorInfo, "Agrimanagr: Manage Stores", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                {
                    SendNavigationRequestMessage(
                        new Uri("views/admin/stores/liststores.xaml", UriKind.Relative));
                }
            }
        }

        private void Cancel()
        {
            if (
                MessageBox.Show("Unsaved changes will be lost. Do you want to continue?",
                                "Agrimanagr: Edit Store", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(
                    new Uri("views/admin/stores/liststores.xaml", UriKind.Relative));
            }
        }

        protected override void AddOrEditContact(Button btnAdd)
        {
            base.AddOrEditContact(btnAdd, Store);
        }

        protected override void EditContact(VMContactItem contactItem)
        {
            base.EditContact(contactItem, Store);
        }

        #endregion

    }
}
