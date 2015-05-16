using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.SupplierContacts
{
    public class ListingSupplierContactsViewModel:ListingsViewModelBase
    {
        private PagenatedList<Contact> _pagedContacts;
        private IDistributorServiceProxy _proxy;

        public ListingSupplierContactsViewModel()
        {
            ContactList = new ObservableCollection<VmContact>();
            AddContactCommand = new RelayCommand(AddContact);
        }

      

        #region Class Members
        public ObservableCollection<VmContact> ContactList { get; set; }
        public RelayCommand AddContactCommand { get; set; }
        #endregion

        #region Methods

        private void AddContact()
        {
            Messenger.Default.Send(new AddContactMessage
            {
                SupplierId = SupplierId
            });
            SendNavigationRequestMessage(new Uri("/Views/Admin/SupplierContacts/Contacts.xaml", UriKind.Relative));
        }


        public void SetSupplier(MemberContactsMessage messageFrom)
        {
            SupplierId = messageFrom.Id;
        }

        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                       
                        using (var c = NestedContainer)
                        {
                            var query = new QuerySupplierContact
                                {
                                    SupplierId = SupplierId,
                                    Take = ItemsPerPage,
                                    Skip = ItemsPerPage*(CurrentPage - 1),
                                    ShowInactive = ShowInactive
                                };
                            if (!string.IsNullOrWhiteSpace(SearchText))
                                query.Name = SearchText;

                            var rawList = Using<IContactRepository>(c).Query(query);
                            _pagedContacts = new PagenatedList<Contact>(rawList.Data.OfType<Contact>().AsQueryable(),
                                                                                      CurrentPage,
                                                                                      ItemsPerPage,
                                                                                      rawList.Count, true);

                            ContactList.Clear();
                            int rownumber = 0;
                            _pagedContacts.ToList().ForEach(n =>
                                                                   ContactList.Add(new VmContact
                                                                   {
                                                                       Id = n.Id,
                                                                       FirstName = n.Firstname,
                                                                       LastName = n.Lastname,
                                                                       Email = n.Email,
                                                                       MobilePhone=n.MobilePhone,
                                                                       PhysicalAddress=n.PhysicalAddress,
                                                                       PostalAddress=n.PostalAddress,
                                                                       Status=n._Status,
                                                                       Action=n._Status==EntityStatus.Active?"Deactivate":"Activate",
                                                                       RowNumber = ++rownumber
                                                                   }));

                            UpdatePagenationControl();
                        }
                    }));
        }

        protected override void EditSelected()
        {
            Messenger.Default.Send(new EditContactMessage()
                {
                    SupplierId = SupplierId,
                    ContactId = SelectedContact.Id
                });
            SendNavigationRequestMessage(new Uri("/Views/Admin/SupplierContacts/Contacts.xaml", UriKind.Relative));
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedContact.Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (MessageBox.Show("Are you sure you want to " + action + " this Contact?",
                                    "Agrimanagr: " + action + " Contact", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedContact == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);

                if (action == "activate")
                {
                    response = await _proxy.ContactActivateAsync(SelectedContact.Id);
                }
                else if (action == "deactivate")
                {
                    response = await _proxy.ContactDeactivateAsync(SelectedContact.Id);
                }
                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Contact", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (MessageBox.Show("Contact has been "+action+"d", "Agrimangr: "+action+" Contact", MessageBoxButton.OK,
                              MessageBoxImage.Information) == MessageBoxResult.OK)
                    Load();
            }
        }

        protected async override void DeleteSelected()
        {
            ResponseBool response = new ResponseBool() { Success = false };
            
            if (MessageBox.Show("Are you sure you want to delete this Contact", "Agrimanagr:Delete Contact", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            using(var c=NestedContainer)
            {
                _proxy = Using<IDistributorServiceProxy>(c);

                if (SelectedContact == null) return;
                response = await _proxy.ContactDeleteAsync(SelectedContact.Id);
                if(response.Success)
                {
                    var contactEntity = Using<IContactRepository>(c).GetById(SelectedContact.Id);
                    Using<IContactRepository>(c).SetAsDeleted(contactEntity);
                }
                MessageBox.Show("Contact Has Been Deleted", "Agrimangr: Delete Contact", MessageBoxButton.OK,
                               MessageBoxImage.Information);
                    Load();
                

            }

        }

        

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedContacts.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedContacts.PageNumber, _pagedContacts.PageCount, _pagedContacts.TotalItemCount,
                                      _pagedContacts.IsFirstPage, _pagedContacts.IsLastPage);
        }

        #endregion

        #region Properties

        
        public const string StatusPropertyName = "Status";
        private string _status = "";
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                RaisePropertyChanging(StatusPropertyName);
                _status = value;
                RaisePropertyChanged(StatusPropertyName);
            }
        }
        
        public const string SupplierIdPropertyName = "SupplierId";
        private Guid _supplierId = Guid.Empty;
        public Guid SupplierId
        {
            get
            {
                return _supplierId;
            }

            set
            {
                if (_supplierId == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierIdPropertyName);
                _supplierId = value;
                RaisePropertyChanged(SupplierIdPropertyName);
            }
        }

        
        public const string IdPropertyName = "Id";
        private Guid _id = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }
        
        public const string SelectedContactPropertyName = "SelectedContact";
        private VmContact _selectedContact;
        public VmContact SelectedContact
        {
            get
            {
                return _selectedContact;
            }

            set
            {
                if (_selectedContact == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContactPropertyName);
                _selectedContact = value;
                RaisePropertyChanged(SelectedContactPropertyName);
            }
        }
        #endregion


    }

    public class VmContact
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string PostalAddress { get; set; }
        public string PhysicalAddress { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }
    }
}
