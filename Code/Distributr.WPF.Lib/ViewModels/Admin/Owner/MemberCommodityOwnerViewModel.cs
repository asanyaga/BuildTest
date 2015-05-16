using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight.Command;

using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Utility.MasterData;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.Owner
{
    public class MemberCommodityOwnerViewModel : ListingsViewModelBase
    {
        private PagenatedList<CommodityOwner> _pagedCommodityOwners;
        private IDistributorServiceProxy _proxy;

        public MemberCommodityOwnerViewModel()
        {
            //LoadListingPageCommand = new RelayCommand(Setup);
            EditSelectedCommand = new RelayCommand<VmCommodityOwner>(EditFarmer);
            AddFarmerCommand = new RelayCommand(AddFarmer);
            ListOfCommodityOwners = new ObservableCollection<VmCommodityOwner>();
           
        }

        #region Class Members
        //public RelayCommand LoadListingPageCommand { get; set; }
        public RelayCommand<VmCommodityOwner> EditSelectedCommand { get; set; }
        public RelayCommand AddFarmerCommand { get; set; }
        public ObservableCollection<VmCommodityOwner> ListOfCommodityOwners { get; set; }
        

      

        #endregion

        #region Methods

        protected override void Load(bool isFirstLoad = false)
        {
            LoadMemberFarmers();
            AddFarmerEnabler();

            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    delegate
                    {
                        if (isFirstLoad)
                            Setup();
                        using (var c = NestedContainer)
                        {
                            var query = new QueryCommodityOwner();
                            query.SupplierId = SupplierId;
                            query.Take = ItemsPerPage;
                            query.Skip = ItemsPerPage * (CurrentPage - 1);
                            query.ShowInactive = ShowInactive;
                            if (!string.IsNullOrWhiteSpace(SearchText))
                                query.Name = SearchText;

                            var rawList = Using<ICommodityOwnerRepository>(c).Query(query);
                            _pagedCommodityOwners = new PagenatedList<CommodityOwner>(rawList.Data.OfType<CommodityOwner>().AsQueryable(),
                                                                                      CurrentPage,
                                                                                      ItemsPerPage,
                                                                                      rawList.Count, true);

                            ListOfCommodityOwners.Clear();
                            int rownumber = 0;
                            _pagedCommodityOwners.ToList().ForEach(n =>
                                                                   ListOfCommodityOwners.Add(new VmCommodityOwner
                                                                   {
                                                                       Id = n.Id,
                                                                       Code = n.Code,
                                                                       Surname = n.Surname,
                                                                       FirstName = n.FirstName,
                                                                       LastName = n.LastName,
                                                                       IdNo = n.IdNo,
                                                                       Email = n.Email,
                                                                       PhoneNumber = n.PhoneNumber,
                                                                       BusinessNumber = n.BusinessNumber,
                                                                       CommodityOwnerType = n.CommodityOwnerType,
                                                                       Status = n._Status,
                                                                       Action=n._Status==EntityStatus.Active?"Deactivate":"Activate",
                                                                       RowNumber = ++rownumber
                                                                   }));
                            UpdatePagenationControl();
                        }
                    }));
        }

        protected override void EditSelected()
        {
            throw new NotImplementedException();
        }

        protected async override void ActivateSelected()
        {
            string action = SelectedCommodityOwner.Status == EntityStatus.Active
                               ? "deactivate"
                               : "activate";
            if (
                    MessageBox.Show("Are you sure you want to " + action + " this commodity Owner?",
                                    "Agrimanagr: Activate Commodity Owner", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityOwner.Status == EntityStatus.Active)
                {
                    var commodityOwner = Using<ICommodityOwnerRepository>(c).GetById(SelectedCommodityOwner.Id);

                    if (Using<IMasterDataUsage>(c).CommodityOwnerHasProducers(commodityOwner))
                    {
                        MessageBox.Show(
                            "Commodity Owner " + SelectedCommodityOwner.FirstName +
                            " has Purchases in the system and thus cannot be deactivated.",
                            "Agrimanagr: Deactivate Commodity Owner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }


                ResponseBool response = new ResponseBool() { Success = false };
                if (SelectedCommodityOwner == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityOwnerActivateOrDeactivateAsync(SelectedCommodityOwner.Id);

                MessageBox.Show(response.ErrorInfo, "Agrimangr: Manage Commodity Owner", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        protected async override void DeleteSelected()
        {
            if (
               MessageBox.Show("Are you sure you want to delete this commodity Owner?",
                                    "Agrimanagr: Delete Commodity Owner", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No) return;

            using (var c = NestedContainer)
            {
                if (SelectedCommodityOwner.Status == EntityStatus.Active)
                {

                    var commodityOwner = Using<ICommodityOwnerRepository>(c).GetById(SelectedCommodityOwner.Id);

                    if (Using<IMasterDataUsage>(c).CommodityOwnerHasProducers(commodityOwner))
                    {
                        MessageBox.Show(
                            "Commodity Owner " + SelectedCommodityOwner.FirstName +
                            " has purchases in the system and thus cannot be deleted.",
                            "Agrimanagr: Delete Commodity Owner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                var response = new ResponseBool() { Success = false };
                if (SelectedCommodityOwner == null) return;
                _proxy = Using<IDistributorServiceProxy>(c);
                response = await _proxy.CommodityOwnerDeleteAsync(SelectedCommodityOwner.Id);
                if (response.Success)
                {
                    var commodityowner = Using<ICommodityOwnerRepository>(c).GetById(SelectedCommodityOwner.Id);
                    Using<ICommodityOwnerRepository>(c).SetAsDeleted(commodityowner);
                    MessageBox.Show(response.ErrorInfo, "Agrimangr:Manage Commodity Owner", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                    Load();
                }
                    
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

        private void Setup()
        {
            LoadMemberFarmers();
            AddFarmerEnabler();
        }

        private void AddFarmerEnabler()
        {
            using(var c=NestedContainer)
            {
                var commoditySupplier = Using<ICommoditySupplierRepository>(c).GetById(SupplierId) as CommoditySupplier;
                if(commoditySupplier!=null && commoditySupplier.CommoditySupplierType==CommoditySupplierType.Individual)
                {
                    AddFarmerIsEnabled = false;
                }
                
            }
        }

        private void LoadMemberFarmers()
        {
            ListOfCommodityOwners.Clear();
           using(var c=NestedContainer)
           {
               var listOfFarmers = Using<ICommodityOwnerRepository>(c).GetBySupplier(SupplierId);
               int rowNumber = 0;
               foreach(var l in listOfFarmers)
               {
                   ListOfCommodityOwners.Add(new VmCommodityOwner
                       {
                           Id=l.Id,
                           RowNumber=rowNumber+1,
                           Code=l.Code,
                           Surname=l.Surname,
                           FirstName=l.FirstName,
                           LastName=l.LastName,
                           IdNo=l.IdNo,
                           Email=l.Email,
                           PhoneNumber=l.PhoneNumber,
                           BusinessNumber=l.BusinessNumber,
                           CommodityOwnerType=l.CommodityOwnerType
                       });
                   rowNumber++;
               }
           }
            AddFarmerIsEnabled = true;

        }


        public void SetSupplier(MemberOwnersMessage messageTo)
        {
            SupplierId = messageTo.Id;
        }

        private void AddFarmer()
        {
            Messenger.Default.Send(new AddCommodityOwnerMessage
                {
                    SupplierId = SupplierId
                });
            SendNavigationRequestMessage(new Uri("/Views/Admin/Owner/CommodityOwner.xaml", UriKind.Relative));
        }

        private void EditFarmer(VmCommodityOwner commodityOwner)
        {
            Messenger.Default.Send(new EditCommodityOwnerMessage
                {
                    SupplierId=SupplierId,
                    CommodityOwnerId=commodityOwner.Id
                });
            SendNavigationRequestMessage(new Uri("/Views/Admin/Owner/CommodityOwner.xaml", UriKind.Relative));
        }

        #endregion

        #region Properties
        public const string SupplierIdPropertyName = "SupplierId";
        private Guid _supplierId = Guid.NewGuid();
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


        
        public const string SelectedCommodityOwnerPropertyName = "SelectedCommodityOwner";
        private VmCommodityOwner _selectedCommodityOwner = null;
        public VmCommodityOwner SelectedCommodityOwner
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


        
        public const string AddFarmerIsEnabledPropertyName = "AddFarmerIsEnabled";
        private bool _addFarmerIsEnabled = true;
        public bool AddFarmerIsEnabled
        {
            get
            {
                return _addFarmerIsEnabled;
            }

            set
            {
                if (_addFarmerIsEnabled == value)
                {
                    return;
                }

                RaisePropertyChanging(AddFarmerIsEnabledPropertyName);
                _addFarmerIsEnabled = value;
                RaisePropertyChanged(AddFarmerIsEnabledPropertyName);
            }
        }
        #endregion

        
    }
    public class VmCommodityOwner
    {
        public Guid Id { get; set; }
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdNo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
        public CommodityOwnerType CommodityOwnerType { get; set; }
        public EntityStatus Status { get; set; }
        public string Action { get; set; }

    }

}
