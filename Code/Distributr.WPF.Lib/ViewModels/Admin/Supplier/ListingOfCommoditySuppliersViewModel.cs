using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.Supplier
{
    public class ListingOfCommoditySuppliersViewModel:ListingsViewModelBase
    {
         private PagenatedList<CommoditySupplier> _pagedCommoditySuppliers;
        private IDistributorServiceProxy _proxy;

        public ListingOfCommoditySuppliersViewModel()
        {
            CommodityOwnersList = new ObservableCollection<VMCommodityOwnerItem>();
            CommoditySuppliersList = new ObservableCollection<VMCommoditySupplier>();
            MemberOwnersCommand = new RelayCommand(MemberOwners);
            MemberProducersCommand = new RelayCommand(MemberProducers);
            ContactsCommand = new RelayCommand(MemberContacts);
        }

       


        public RelayCommand MemberOwnersCommand { get; set; }
        public RelayCommand MemberProducersCommand { get; set; }
        public RelayCommand ContactsCommand { get; set; }
        #region properties

        public ObservableCollection<VMCommoditySupplier> CommoditySuppliersList { get; set; }
        public ObservableCollection<VMCommodityOwnerItem> CommodityOwnersList { get; set; }
        public RelayCommand EditSelectionCommand { get; set; }

        
        public const string SelectedCommoditySupplierPropertyName = "SelectedCommoditySupplier";
        private VMCommoditySupplier _selectedCommoditySupplier = null;
        public VMCommoditySupplier SelectedCommoditySupplier
        {
            get
            {
                return _selectedCommoditySupplier;
            }

            set
            {
                if (_selectedCommoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommoditySupplierPropertyName);
                _selectedCommoditySupplier = value;
                RaisePropertyChanged(SelectedCommoditySupplierPropertyName);
            }
        }

        public const string SelectedCommodityOwnerPropertyName = "SelectedCommodityOwner";
        private VMCommodityOwnerItem _selectedCommodityOwner = null;
        public VMCommodityOwnerItem SelectedCommodityOwner
        {
            get
            {
                return _selectedCommodityOwner;
            }

            set
            {
                if (_selectedCommodityOwner == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityOwnerPropertyName);
                _selectedCommodityOwner = value;
                RaisePropertyChanged(SelectedCommodityOwnerPropertyName);
            }
        }


        private CommoditySupplier _defaultCommoditySupplier;

        private CommoditySupplier DefaultCommoditySupplier
        {
            get
            {
                return _defaultCommoditySupplier ??
                       (_defaultCommoditySupplier =
                        new CommoditySupplier(Guid.Empty) {Name = "--Select commodity supplier--"});
            }
        }

        
        public const string MemberOwnerIsEnabledPropertyName = "MemberOwnerIsEnabled";
        private bool _memberOwnerIsEnabled = true;
        public bool MemberOwnerIsEnabled
        {
            get
            {
                return _memberOwnerIsEnabled;
            }

            set
            {
                if (_memberOwnerIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(MemberOwnerIsEnabledPropertyName);
                _memberOwnerIsEnabled = value;
                RaisePropertyChanged(MemberOwnerIsEnabledPropertyName);
            }
        }

        #endregion

        #region methods

        protected override void Load(bool isFirstLoad = false)
        {
            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                        {
                            if (isFirstLoad)
                                Setup();
                            using (var c = NestedContainer)
                            {
                                
                                var query = new QueryCommoditySupplier();
                                query.Skip = ItemsPerPage*(CurrentPage-1);
                                query.Take = ItemsPerPage;
                                query.ShowInactive = ShowInactive;

                                if (!string.IsNullOrWhiteSpace(SearchText))
                                    query.Name = SearchText;

                                var rawList = Using<ICommoditySupplierRepository>(c).Query(query);
                                var data = rawList.Data.OfType<CommoditySupplier>();

                                _pagedCommoditySuppliers = new PagenatedList<CommoditySupplier>(data.AsQueryable(),
                                                                                          CurrentPage,
                                                                                          ItemsPerPage,
                                                                                          rawList.Count,true);

                                CommoditySuppliersList.Clear();
                                int rownumber = 0;
                                _pagedCommoditySuppliers.Select((n, i) => Map(n, i + 1)).ToList().ForEach(
                                    n =>
                                        {
                                            if (n != null)
                                                CommoditySuppliersList.Add(new VMCommoditySupplier
                                                    {
                                                        SupplierId = n.CommoditySupplier.Id,
                                                        CostCenterCode = n.CommoditySupplier.CostCentreCode ?? "",
                                                        Name = n.CommoditySupplier.Name??"",
                                                        AccountName = n.CommoditySupplier.AccountName??"",
                                                        AccountNo = n.CommoditySupplier.AccountNo??"",
                                                        Bank =n.CommoditySupplier.BankId!=Guid.Empty? Using<IBankRepository>(c).GetById(n.CommoditySupplier.BankId).Name:"",
                                                        BankBranch =n.CommoditySupplier.BankBranchId!=Guid.Empty? Using<IBankBranchRepository>(c).GetById( n.CommoditySupplier.BankBranchId).Name:"",
                                                        Status=n.CommoditySupplier._Status,
                                                        Action=n.CommoditySupplier._Status==EntityStatus.Active?"Deactivate":"Activate",
                                                        RowNumber=++rownumber
                                                    });
                                        });
                                UpdatePagenationControl();
                            }
                        }));
        }

        private VMCommodityOwnerItem Map(CommoditySupplier commodityOwner, int i)
        {
            var mapped = new VMCommodityOwnerItem
                             {
                                 CommoditySupplier = commodityOwner,
                                 RowNumber = i
                             };
            if (commodityOwner._Status == EntityStatus.Active)
                mapped.HlkDeactivateContent = "Deactivate";
            else if (commodityOwner._Status == EntityStatus.Inactive)
                mapped.HlkDeactivateContent = "Activate";

            return mapped;
        }

        private void Setup()
        {
            PageTitle = "List of Hub Accounts";
            LoadVmCommoditySupplierList();
        }

        protected override void EditSelected()
        {
            if (SelectedCommoditySupplier != null)
            {
                 using (var c = NestedContainer)
                 {
                     var commodityProducer =
                         Using<ICommodityProducerRepository>(c).GetBySupplier(
                             SelectedCommoditySupplier.SupplierId).OrderBy(s => s._DateCreated).FirstOrDefault();// .OrderByDescending(s => s._DateCreated).FirstOrDefault();
                     var commodityOwner =
                               Using<ICommodityOwnerRepository>(c).GetBySupplier(
                                    SelectedCommoditySupplier.SupplierId).OrderBy(s => s._DateCreated).FirstOrDefault();//.OrderByDescending(s => s._DateCreated).FirstOrDefault();
                     var contact =
                                    Using<IContactRepository>(c).GetByContactsOwnerId(
                                         SelectedCommoditySupplier.SupplierId).OrderByDescending(s => s._DateCreated).FirstOrDefault();
              
                Messenger.Default.Send(new EditSupplierMessage
                    {
                        Id = SelectedCommoditySupplier.SupplierId,
                        CommodityOwnerId = commodityOwner == null ? Guid.NewGuid() : commodityOwner.Id,
                        CommodityProducerId = commodityProducer==null?Guid.NewGuid():commodityProducer.Id,
                        ContactId=contact==null?Guid.NewGuid():contact.Id
                    });
                        }
                SendNavigationRequestMessage(
                    new Uri("/views/admin/Supplier/AddEditFarmer.xaml", UriKind.Relative));

               
            }
        }

        private void MemberOwners()
        {
            if (SelectedCommoditySupplier != null)
            {
                using (var c = NestedContainer)
                {

                    Messenger.Default.Send(new MemberOwnersMessage
                        {
                            Id = SelectedCommoditySupplier.SupplierId,
                            
                        });
                }
                SendNavigationRequestMessage(
                    new Uri("/views/admin/Owner/ListingMemberCommodityOwner.xaml", UriKind.Relative));
            }
        }

        private void MemberContacts()
        {
            if(SelectedCommoditySupplier!=null)
                using(var c=NestedContainer)
                {
                    Messenger.Default.Send(new MemberContactsMessage
                    {
                        Id=SelectedCommoditySupplier.SupplierId
                    });
                    
                }
            SendNavigationRequestMessage(
                new Uri("/views/admin/SupplierContacts/ListingSupplierContacts.xaml", UriKind.Relative));
        }

        private void MemberProducers()
        {
            if(SelectedCommoditySupplier!=null)
            {
                using(var c=NestedContainer)
                {
                    Messenger.Default.Send(new MemberProducerMessage
                        {
                            Id=SelectedCommoditySupplier.SupplierId,
                        });
                }
                SendNavigationRequestMessage(
                    new Uri("/Views/Admin/Producer/ListingMemberCommodityProducers.xaml", UriKind.Relative));
            }
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedCommoditySupplier.Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity Supplier?",
                                    "Agrimanagr: Activate Commodity Supplier", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommoditySupplier.Status == EntityStatus.Active)
                {
                    var commoditySupplier =Using<ICommoditySupplierRepository>(c).GetById(SelectedCommoditySupplier.SupplierId) as CommoditySupplier;

                    if (Using<IMasterDataUsage>(c).CommoditySupplierHasOwnersOrProducers(commoditySupplier))
                    {
                        MessageBox.Show(
                            "Commodity Supplier " + SelectedCommoditySupplier.Name +
                            " has Owners and Producers in the system and thus cannot be deactivated.",
                            "Agrimanagr: Deactivate Commodity Supplier", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }


                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommoditySupplier == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommoditySupplierActivateOrDeactivateAsync(SelectedCommoditySupplier.SupplierId);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Owner", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
                MessageBox.Show("Are you sure you want to delete this commodity supplier?",
                                     "Agrimanagr: Delete Commodity Supplier", MessageBoxButton.YesNo) ==
                     MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommoditySupplier.Status==EntityStatus.Active)
                {

                    var commoditySupplier =Using<ICommoditySupplierRepository>(c).GetById(SelectedCommoditySupplier.SupplierId) as CommoditySupplier; 
                   
                    if (Using<IMasterDataUsage>(c).CommoditySupplierHasOwnersOrProducers(commoditySupplier))// 1SelectedCommodityOwner.CommoditySupplier))
                    {
                        MessageBox.Show(
                            "Commodity Supplier " + SelectedCommoditySupplier.Name +
                            " has purchases in the system and thus cannot be deleted.",
                            "Agrimanagr: Delete Commodity Supplier", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                ResponseBool response = new ResponseBool() { Success = false };
                // 1 if (SelectedCommodityOwner == null) return;
                if (SelectedCommoditySupplier == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommoditySupplierDeleteAsync(SelectedCommoditySupplier.SupplierId);
                if (response.Success)
                {
                    var commoditysupplier = Using<ICommoditySupplierRepository>(c).GetById(SelectedCommoditySupplier.SupplierId);
                    Using<ICommoditySupplierRepository>(c).SetAsDeleted(commoditysupplier);

                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Commodity Supplier", MessageBoxButton.OK,
                                   MessageBoxImage.Information);
                    Load();
                }
                   
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommoditySuppliers.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommoditySuppliers.PageNumber, _pagedCommoditySuppliers.PageCount, _pagedCommoditySuppliers.TotalItemCount,
                                      _pagedCommoditySuppliers.IsFirstPage, _pagedCommoditySuppliers.IsLastPage);
        }

       

        private void LoadVmCommoditySupplierList()
        {
            //VMCommoditySupplier supplierVM;
            CommoditySuppliersList.Clear();
            using (var c = NestedContainer)
            {
                //CommoditySuppliersList.Add(DefaultCommoditySupplier);
                var list = Using<ICommoditySupplierRepository>(c).GetAll().OfType<CommoditySupplier>().OrderBy(n => n.Name).ToList();
                //list.ForEach(n => CommoditySuppliersList.Add(n as CommoditySupplier));
                int rowNumber = 1;
                foreach(var n in list)
                {
                    if (n != null && n._Status==EntityStatus.Active)
                    {
                        var bank = Using<IBankRepository>(c).GetById(n.BankId);
                        var bankBranch = Using<IBankBranchRepository>(c).GetById(n.BankBranchId);
                           CommoditySuppliersList.Add(
                               new VMCommoditySupplier
                               {
                                   SupplierId=n.Id,
                                   RowNumber=rowNumber,
                                   Name=n.Name,
                                   CostCenterCode = n.CostCentreCode,
                                   AccountName = n.AccountName,
                                   AccountNo = n.AccountNo,
                                   Bank = bank!=null?bank.Name:"",
                                   BankBranch=bankBranch!=null?bankBranch.Name:"",
                                   Status=n._Status
                               });
                           
                        // CommoditySuppliersList.Add(supplierVM);
                    }
                    else
                        continue;
                    rowNumber++;
                }
               // SelectedCommoditySupplier = DefaultCommoditySupplier;
            }
        }
        #endregion

    }

    #region helpers

    public class VMCommodityOwnerItem
    {
        public CommoditySupplier CommoditySupplier { get; set; }
        public int RowNumber { get; set; }
        public string HlkDeactivateContent { get; set; }
        public bool IsDirty { get; set; }

    }

    public class VMCommoditySupplier
    {
        public Guid SupplierId { get; set; }
        public int RowNumber { get; set; }
        public string CostCenterCode { get; set; }
        public string Name { get; set; }
        public string AccountNo { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public string AccountName { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }
    }

    #endregion

}

