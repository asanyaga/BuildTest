using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
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

namespace Distributr.WPF.Lib.ViewModels.Admin.CommodityOwners
{
    public class ListCommodityOwnersViewModel : ListingsViewModelBase
    {
        private PagenatedList<CommoditySupplier> _pagedCommodityOwners;
        private IDistributorServiceProxy _proxy;

        public ListCommodityOwnersViewModel()
        {
            CommodityOwnersList = new ObservableCollection<VMCommodityOwnerItem>();
            CommoditySuppliersList = new ObservableCollection<VMCommoditySupplier>();
            MemberOwnersCommand = new RelayCommand(MemberOwners);
            MemberProducersCommand = new RelayCommand(MemberProducers);
        }



        public RelayCommand MemberOwnersCommand { get; set; }
        public RelayCommand MemberProducersCommand { get; set; }
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
                                var rawList = Using<ICommoditySupplierRepository>(c).GetAll(ShowInactive);
                                    //.Where(n => n.CostCentreCode.ToLower().Contains(SearchText.ToLower()) ||
                                    //            n.Name.ToLower().Contains(SearchText.ToLower()));

                                //var rawVmList =new List<VMCommoditySupplier>();

                                //foreach(var commoditySupplier in rawList)
                                //{
                                //    rawVmList.Add(new VMCommoditySupplier
                                //        {
                                //            SupplierId=commoditySupplier.Id,
                                //            AccountName=commoditySupplier.Name,
                                            
                                //        });
                                //}
                              
                                rawList = rawList.OrderBy(n => n.Name).ThenBy(n => n.CostCentreCode);
                                _pagedCommodityOwners = new PagenatedList<CommoditySupplier>(rawList.OfType<CommoditySupplier>().AsQueryable(),
                                                                                          CurrentPage,
                                                                                          ItemsPerPage,
                                                                                          rawList.Count());

                                CommodityOwnersList.Clear();
                                _pagedCommodityOwners.Select((n, i) => Map(n, i + 1)).ToList().ForEach(
                                    n => CommodityOwnersList.Add(n));
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
            PageTitle = "List Farmers";
           // LoadCommoditySupplierList();
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
            string action = SelectedCommodityOwner.CommoditySupplier._Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity owner?",
                                    "Agrimanagr: Activate Commodity Owner", MessageBoxButton.OKCancel) ==
                    MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                if(SelectedCommodityOwner.CommoditySupplier._Status == EntityStatus.Active)
                {
                    if(Using<IMasterDataUsage>(c).CommodityOwnerHasPurchases(SelectedCommodityOwner.CommoditySupplier))
                    {
                        MessageBox.Show(
                            "Commodity owner " + SelectedCommodityOwner.CommoditySupplier.Name +
                            " has purchases in the system and thus cannot be deactivated.",
                            "Agrimanagr: Deactivate Commodity Owner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }


                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommodityOwner == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityOwnerActivateOrDeactivateAsync(SelectedCommodityOwner.CommoditySupplier.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Owner", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
                     MessageBox.Show("Are you sure you want to delete this commodity owner?",
                                     "Agrimanagr: Activate Commodity Owner", MessageBoxButton.OKCancel) ==
                     MessageBoxResult.Cancel) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityOwner.CommoditySupplier._Status == EntityStatus.Active)
                {
                    if (Using<IMasterDataUsage>(c).CommodityOwnerHasPurchases(SelectedCommodityOwner.CommoditySupplier))
                    {
                        MessageBox.Show(
                            "Commodity owner " + SelectedCommodityOwner.CommoditySupplier.Name +
                            " has purchases in the system and thus cannot be deleted.",
                            "Agrimanagr: Delete Commodity Owner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommodityOwner == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityOwnerDeleteAsync(SelectedCommodityOwner.CommoditySupplier.Id);
                //if (response.Success)
                //    Using<ICommodityOwnerRepository>(c).SetAsDeleted(SelectedCommodityOwner.CommoditySupplier.);
                //MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Owner", MessageBoxButton.OK,
                //                MessageBoxImage.Information);
            }
        }

        protected override void GoToPage(PageDestinations page)
        {
            GoToPageBase(page, _pagedCommodityOwners.PageCount);
        }

        protected override void ComboPageSizesSelectionChanged(int take)
        {
            ItemsPerPage = take;
            Load();
        }

        protected override void UpdatePagenationControl()
        {
            UpdatePagenationControlBase(_pagedCommodityOwners.PageNumber, _pagedCommodityOwners.PageCount, _pagedCommodityOwners.TotalItemCount,
                                      _pagedCommodityOwners.IsFirstPage, _pagedCommodityOwners.IsLastPage);
        }

        //private void LoadCommoditySupplierList()
        //{
        //    CommoditySuppliersList.Clear();
        //    using (var c = NestedContainer)
        //    {
        //        CommoditySuppliersList.Add(DefaultCommoditySupplier);
        //        var list = Using<ICommoditySupplierRepository>(c).GetAll().OrderBy(n => n.Name).ToList();
        //        list.ForEach(n => CommoditySuppliersList.Add(n as CommoditySupplier));
        //        SelectedCommoditySupplier = DefaultCommoditySupplier;
        //    }
        //}

        private void LoadVmCommoditySupplierList()
        {
            VMCommoditySupplier supplierVM;
            CommoditySuppliersList.Clear();
            using (var c = NestedContainer)
            {
                //CommoditySuppliersList.Add(DefaultCommoditySupplier);
                var list = Using<ICommoditySupplierRepository>(c).GetAll().OfType<CommoditySupplier>().OrderBy(n => n.Name).ToList();
                //list.ForEach(n => CommoditySuppliersList.Add(n as CommoditySupplier));
                int rowNumber = 1;
                foreach(var n in list)
                {
                    if (n != null)
                    {
                        var bank = Using<IBankRepository>(c).GetById(n.BankId);
                        var bankBranch = Using<IBankBranchRepository>(c).GetById(n.BankBranchId);
                           CommoditySuppliersList.Add(
                               new VMCommoditySupplier
                               {
                                   SupplierId=n.Id,
                                   RowNumber=rowNumber,
                                   Name=n.Name,
                                   Code=n.CostCentreCode,
                                   AccountName = n.AccountName,
                                   AccountNo = n.AccountNo,
                                   Bank = bank!=null?bank.Name:"",
                                   BankBranch=bankBranch!=null?bankBranch.Name:"",
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
        public string Code { get; set; }
        public string Name { get; set; }
        public string AccountNo { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public string AccountName { get; set; }
    }

    #endregion

}
