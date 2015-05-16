using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Messages;
using GalaSoft.MvvmLight.Command;

using Distributr.Core.Domain.Master.CommodityEntity;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.Supplier
{
    public class MemberFarmersViewModel:DistributrViewModelBase
    {
        public MemberFarmersViewModel()
        {
            LoadListingPageCommand = new RelayCommand(Setup);
            EditSelectedCommand = new RelayCommand<CommodityOwner>(EditFarmer);
            AddFarmerCommand = new RelayCommand(AddFarmer);
            ListOfCommodityOwners = new ObservableCollection<CommodityOwner>();
           
        }

        #region Class Members
        public RelayCommand LoadListingPageCommand { get; set; }
        public RelayCommand<CommodityOwner>EditSelectedCommand { get; set; }
        public RelayCommand AddFarmerCommand { get; set; }
        public ObservableCollection<CommodityOwner> ListOfCommodityOwners { get; set; }
        
  
        #endregion

        #region Methods
        private void Setup()
        {
            LoadMemberFarmers();
        }

        private void LoadMemberFarmers()
        {
            ListOfCommodityOwners.Clear();
           using(var c=NestedContainer)
           {
               var listOfFarmers = Using<ICommodityOwnerRepository>(c).GetBySupplier(SupplierId);

               foreach(var l in listOfFarmers)
               {
                   ListOfCommodityOwners.Add(l);
               }
           }
        }


        public void SetSupplier(MemberFarmersMessage messageTo)
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

        private void EditFarmer(CommodityOwner commodityOwner)
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
        private CommodityOwner _selectedCommodityOwner = null;
        public CommodityOwner SelectedCommodityOwner
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
        
        #endregion


    }
}
