using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.ServiceProviders
{
    public class ListingServiceProvidersViewModel:ListingsViewModelBase
    {
        private PagenatedList<ServiceProvider> _pagedServiceProvider;
        private IDistributorServiceProxy _proxy;

        public ListingServiceProvidersViewModel()
        {
            ListOfServiceProviders = new ObservableCollection<VmServiceProvider>();
        }

        #region Class Members
        public ObservableCollection<VmServiceProvider> ListOfServiceProviders { get; set; }
        #endregion

        #region Methods
        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        //if (isFirstLoad)

                            using (var c = NestedContainer)
                            {
                                var query = new QueryServiceProvider();
                                query.Take = ItemsPerPage;
                                query.Skip = ItemsPerPage * (CurrentPage - 1);
                                query.ShowInactive = ShowInactive;
                                if (!string.IsNullOrWhiteSpace(SearchText))
                                    query.Name = SearchText;

                                var rawList = Using<IServiceProviderRepository>(c).Query(query);
                                _pagedServiceProvider = new PagenatedList<ServiceProvider>(rawList.Data.OfType<ServiceProvider>().AsQueryable(),
                                                                                          CurrentPage,
                                                                                          ItemsPerPage,
                                                                                          rawList.Count, true);

                                ListOfServiceProviders.Clear();
                                int rownumber = 0;
                                _pagedServiceProvider.ToList().ForEach(n =>
                                                                       ListOfServiceProviders.Add(new VmServiceProvider
                                                                       {
                                                                           Id = n.Id,
                                                                           Code = n.Code,
                                                                           Name = n.Name,
                                                                           IdNo=n.IdNo,
                                                                           PinNo=n.PinNo,
                                                                           MobileNumber=n.MobileNumber,
                                                                           AccountName=n.AccountName,
                                                                           AccountNumber=n.AccountNumber,
                                                                           BankName=n.Bank!=null?n.Bank.Name:"",
                                                                           BankBranchName=n.BankBranch!=null?n.BankBranch.Name:"",
                                                                           Status = n._Status,
                                                                           Action = n._Status == EntityStatus.Active ? "Deactivate" : "Activate",
                                                                           RowNumber = ++rownumber
                                                                       }));
                                UpdatePagenationControl();
                            }
                    }));
        }

        protected override void EditSelected()
        {
            if (SelectedServiceProvider != null)
            {
                using (var c = NestedContainer)
                {

                    Messenger.Default.Send(new EditServiceProviderMessage
                    {
                        ServiceProviderId = SelectedServiceProvider.Id
                    });
                }
                SendNavigationRequestMessage(
                    new Uri("/Views/Admin/Farmactivities/ServiceProviders/EditServiceProvider.xaml", UriKind.Relative));
            }
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedServiceProvider.Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (MessageBox.Show("Are you sure you want to " + action + " this Service Provider?",
                                "Agrimanagr: Manage Service Provider", MessageBoxButton.YesNo) ==
                MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedServiceProvider == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ServiceProviderActivateOrDeactivateAsync(SelectedServiceProvider.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Service Provider", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this Service Provider?",
                                    "Agrimanagr: Delete Service Provider", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                
                var response = new ResponseBool() { Success = false };
                if (SelectedServiceProvider == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.ServiceProviderDeleteAsync(SelectedServiceProvider.Id);
                if (response.Success)
                {
                   var serviceProvider= Using<IServiceProviderRepository>(c).GetById(SelectedServiceProvider.Id);
                    Using<IServiceProviderRepository>(c).SetAsDeleted(serviceProvider);

                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Service Provider", MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                }
                    
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedServiceProvider.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedServiceProvider.PageNumber, _pagedServiceProvider.PageCount, _pagedServiceProvider.TotalItemCount,
                                     _pagedServiceProvider.IsFirstPage, _pagedServiceProvider.IsLastPage);
        }
        #endregion 

        #region Properties

        
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

        
        public const string SelectedServiceProviderPropertyName = "SelectedServiceProvider";
        private VmServiceProvider _selectedServiceProvider = null;
        public VmServiceProvider SelectedServiceProvider
        {
            get
            {
                return _selectedServiceProvider;
            }

            set
            {
                if (_selectedServiceProvider == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedServiceProviderPropertyName);
                _selectedServiceProvider = value;
                RaisePropertyChanged(SelectedServiceProviderPropertyName);
            }
        }

        #endregion

    }

    public class VmServiceProvider
    {
        public int RowNumber { get; set; }
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string IdNo { get; set; }
        public string PinNo { get; set; }
        public string MobileNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }
    }
}
