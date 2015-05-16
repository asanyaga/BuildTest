using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers;
using Distributr.WPF.Lib.ViewModels.Admin.AgriUsers.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Centres;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityOwners;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers;
using Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers;
using Distributr.WPF.Lib.ViewModels.Admin.Contacts;
using Distributr.WPF.Lib.ViewModels.Admin.Containers;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Activity;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Infections;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Seasons;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.ServiceProviders;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Services;
using Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Shifts;
using Distributr.WPF.Lib.ViewModels.Admin.Owner;
using Distributr.WPF.Lib.ViewModels.Admin.Producer;
using Distributr.WPF.Lib.ViewModels.Admin.Routes;
using Distributr.WPF.Lib.ViewModels.Admin.Stores;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityOwners;
using Distributr.WPF.Lib.ViewModels.Admin.Owner;
using Distributr.WPF.Lib.ViewModels.Admin.Supplier;
using Distributr.WPF.Lib.ViewModels.Admin.SupplierContacts;
using Distributr.WPF.Lib.ViewModels.Admin.Users;
using Distributr.WPF.Lib.ViewModels.Admin.Vehicles;
using Distributr.WPF.Lib.ViewModels.Admin.WeighingScales;
using Distributr.WPF.Lib.ViewModels.ApplicSettings;
using Distributr.WPF.Lib.ViewModels.IntialSetup;
using Distributr.WPF.Lib.ViewModels.MainPage;
using Distributr.WPF.Lib.ViewModels.Reports;
using Distributr.WPF.Lib.ViewModels.Sync;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase;
using Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception;
using Distributr.WPF.Lib.ViewModels.Transactional.InventoryTransfer;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders.OrderProduct;
using Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument;
using Distributr.WPF.Lib.ViewModels.Utils;
using Distributr.WPF.Lib.ViewModels.Warehousing;
using Distributr.WPF.Lib.ViewModels.Warehousing.EagcClient;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Distributr.WPF.Lib.Data.IOC
{
    public class AgriViewModelLocator
    {
        static AgriViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<EditCommodityPurchaseViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<ListFarmersViewModel>();
            SimpleIoc.Default.Register<ConfigurationViewModel>();
            SimpleIoc.Default.Register<ComboPopUpViewModel>();
            SimpleIoc.Default.Register<AgrimanagrReceiptDocumentViewModel>();
            SimpleIoc.Default.Register<ListCommodityReceptionViewModel>();
            SimpleIoc.Default.Register<StoreCommodityViewModel>();
            SimpleIoc.Default.Register<DocumentDetailsViewModel>();
            SimpleIoc.Default.Register<EquipmentSetupViewModel>();
            SimpleIoc.Default.Register<ListWeingScalesViewModel>();
            SimpleIoc.Default.Register<SyncViewModel>();
            SimpleIoc.Default.Register<GeneralSettingsViewModel>();
            SimpleIoc.Default.Register<ListAgriUsersViewModel>();
            SimpleIoc.Default.Register<EditAgriUsersViewModel>();
            SimpleIoc.Default.Register<EditAgriUserContactViewModel>();
            SimpleIoc.Default.Register<AgrimanagrMainPageViewModel>();
            SimpleIoc.Default.Register<EditCommoditySupplierViewModel>();
            SimpleIoc.Default.Register<EditRouteViewModel>();
            SimpleIoc.Default.Register<ListRoutesViewModel>();
            SimpleIoc.Default.Register<AwaitingStorageViewModel>();
            SimpleIoc.Default.Register<AwaitingReceptionViewModel>();
            SimpleIoc.Default.Register<WeighReceiveDeliveryViewModelPopUp>();
            SimpleIoc.Default.Register<ListCommoditySuppliersViewModel>();
            SimpleIoc.Default.Register<EditCommodityProducerViewModel>();
            SimpleIoc.Default.Register<ListCommodityProducersViewModel>();
            SimpleIoc.Default.Register<EditCommodityOwnerViewModel>();
            SimpleIoc.Default.Register<ListCommodityOwnersViewModel>();
            SimpleIoc.Default.Register<CompletedAndStoredViewModel>();
            SimpleIoc.Default.Register<EditStoreViewModel>();
            SimpleIoc.Default.Register<ListStoresViewModel>();
            SimpleIoc.Default.Register<EditCentreViewModel>();
            SimpleIoc.Default.Register<ListCentresViewModel>();
            SimpleIoc.Default.Register<EditContainerTypeViewModel>();
            SimpleIoc.Default.Register<ListContainerTypesViewModel>();
            SimpleIoc.Default.Register<EditContainerViewModel>();
            SimpleIoc.Default.Register<ListContainersViewModel>();
            SimpleIoc.Default.Register<AgriListContactsViewModel>();
            SimpleIoc.Default.Register<EditVehicleViewModel>();
            SimpleIoc.Default.Register<ListVehiclesViewModel>();
            SimpleIoc.Default.Register<EditCommodityProducerModalViewModel>();
            SimpleIoc.Default.Register<EditCommodityOwnerModalViewModel>();
            SimpleIoc.Default.Register<ListWeighingScaleViewModel>();
            SimpleIoc.Default.Register<EditWeighingScaleViewModel>();
            SimpleIoc.Default.Register<AgrimanagrReportsViewModel>();
            SimpleIoc.Default.Register<EditIndividualFarmerViewModel>();
            SimpleIoc.Default.Register<SupplierToOutletMappingViewModel>();
            SimpleIoc.Default.Register<ItemLookUpsLookUpViewModel>();
            SimpleIoc.Default.Register<InventoryInStorageViewModel>();
            SimpleIoc.Default.Register<TransferedInventoryViewModel>();
            SimpleIoc.Default.Register<ToHqInventoryTransferViewModel>();
            SimpleIoc.Default.Register<IntraStoreTransferViewModel>();
            SimpleIoc.Default.Register<InventoryLevelViewModel>();
            SimpleIoc.Default.Register<AddEditSupplierViewModel>();
            SimpleIoc.Default.Register<ListingOfCommoditySuppliersViewModel>();
            SimpleIoc.Default.Register<MemberCommodityOwnerViewModel>();
            SimpleIoc.Default.Register<MemberCommodityProducerViewModel>();
            SimpleIoc.Default.Register<CommodityOwnerViewModel>();
            SimpleIoc.Default.Register<CommodityProducerViewModel>();
            SimpleIoc.Default.Register<ListingSupplierContactsViewModel>();
            SimpleIoc.Default.Register<ContactsViewModel>();
            SimpleIoc.Default.Register<ListingInfectionsViewModel>();
            SimpleIoc.Default.Register<InfectionsViewModel>();
            SimpleIoc.Default.Register<SeasonViewModel>();
            SimpleIoc.Default.Register<ListingSeasonsViewModel>();
            SimpleIoc.Default.Register<ServiceViewModel>();
            SimpleIoc.Default.Register<ListingServicesViewModel>();
            SimpleIoc.Default.Register<ShiftViewModel>();
            SimpleIoc.Default.Register<ListingShiftsViewModel>();
            SimpleIoc.Default.Register<ServiceProviderViewModel>();
            SimpleIoc.Default.Register<ListingServiceProvidersViewModel>();
            SimpleIoc.Default.Register<ActivityListingViewModel>();
            SimpleIoc.Default.Register<ActivityDetailsPopUp>();
            SimpleIoc.Default.Register<WeighScaleDefaultViewModel>();
            SimpleIoc.Default.Register<WarehouseMenuViewModel>();
            SimpleIoc.Default.Register<AddWarehouseEntryViewModel>();
            SimpleIoc.Default.Register<WarehouseDepositorViewModel>();
            SimpleIoc.Default.Register<WarehouseEntryListingViewModel>();
            SimpleIoc.Default.Register<WarehouseExitViewModel>();
            SimpleIoc.Default.Register<WarehouseExitListingViewModel>();
            SimpleIoc.Default.Register<WarehouseStoreViewModel>();
            SimpleIoc.Default.Register<WarehousePendingStorageListingViewModel>();
            SimpleIoc.Default.Register<WarehouseInventoryLevelsListingViewModel>();
            SimpleIoc.Default.Register<WarehouseGoodsReceivedViewModel>();
            SimpleIoc.Default.Register<WarehouseReceiptViewModel>();
            SimpleIoc.Default.Register<CommodityReleaseViewModel>();
            SimpleIoc.Default.Register<CommodityReleaseDocumentViewModel>();
            SimpleIoc.Default.Register<EagcServiceProxy>();
            SimpleIoc.Default.Register<EditUsersViewModel>();
          

            
            
            RegisterViewModelMessage();
        }

        #region RegisterViewModelMessage

        private static void RegisterViewModelMessage()
        {
            var commodityPurchaseVM = SimpleIoc.Default.GetInstance<EditCommodityPurchaseViewModel>();
            Messenger.Default.Register<ViewModelMessage>(commodityPurchaseVM, commodityPurchaseVM.SetId);

            var receiptdoc = SimpleIoc.Default.GetInstance<AgrimanagrReceiptDocumentViewModel>();
            Messenger.Default.Register<ViewModelMessage>(receiptdoc, receiptdoc.SetId);

            var docdetails = SimpleIoc.Default.GetInstance<DocumentDetailsViewModel>();
            Messenger.Default.Register<DocumentDetailMessage>(docdetails, docdetails.SetId);

            var sync = SimpleIoc.Default.GetInstance<SyncViewModel>();
            Messenger.Default.Register<string>(sync, sync.ReceiveMessage);

            var addEditSupplierViewModel = SimpleIoc.Default.GetInstance<AddEditSupplierViewModel>();
            Messenger.Default.Register<EditSupplierMessage>(addEditSupplierViewModel, addEditSupplierViewModel.SetForEdit);

            var memberOwnersViewModel = SimpleIoc.Default.GetInstance<MemberCommodityOwnerViewModel>();
            Messenger.Default.Register<MemberOwnersMessage>(memberOwnersViewModel, memberOwnersViewModel.SetSupplier);
            
            var addOwnerViewModel = SimpleIoc.Default.GetInstance<CommodityOwnerViewModel>();
            Messenger.Default.Register<AddCommodityOwnerMessage>(addOwnerViewModel, addOwnerViewModel.AddOwner);
            Messenger.Default.Register<EditCommodityOwnerMessage>(addOwnerViewModel, addOwnerViewModel.EditOwner);

            var memberProducerViewModel = SimpleIoc.Default.GetInstance<MemberCommodityProducerViewModel>();
            Messenger.Default.Register<MemberProducerMessage>(memberProducerViewModel,memberProducerViewModel.SetSupplier);

            var addProducerViewModel = SimpleIoc.Default.GetInstance<CommodityProducerViewModel>();
            Messenger.Default.Register<AddCommodityProducerMessage>(addProducerViewModel,addProducerViewModel.AddProducer);
            Messenger.Default.Register<EditCommodityProducerMessage>(addProducerViewModel,addProducerViewModel.EditProducer);

            var memberContactsViewModel = SimpleIoc.Default.GetInstance<ListingSupplierContactsViewModel>();
            Messenger.Default.Register<MemberContactsMessage>(memberContactsViewModel,memberContactsViewModel.SetSupplier);

            var addEditContactViewModel = SimpleIoc.Default.GetInstance<ContactsViewModel>();
            Messenger.Default.Register<AddContactMessage>(addEditContactViewModel, addEditContactViewModel.AddContact);
            Messenger.Default.Register<EditContactMessage>(addEditContactViewModel, addEditContactViewModel.EditContact);

            var editInfectionViewModel = SimpleIoc.Default.GetInstance<InfectionsViewModel>();
            Messenger.Default.Register<EditInfectionsMessage>(editInfectionViewModel,editInfectionViewModel.EditInfection);

            var editSeasonViewModel = SimpleIoc.Default.GetInstance<SeasonViewModel>();
            Messenger.Default.Register<EditSeasonMessage>(editSeasonViewModel, editSeasonViewModel.EditSeason);

            var editServiceViewModel = SimpleIoc.Default.GetInstance<ServiceViewModel>();
            Messenger.Default.Register<EditCommodityProducerServiceMessage>(editServiceViewModel, editServiceViewModel.EditService);

            var editShiftViewModel = SimpleIoc.Default.GetInstance<ShiftViewModel>();
            Messenger.Default.Register<EditShiftMessage>(editShiftViewModel, editShiftViewModel.EditShift);

            var editServiceProvider = SimpleIoc.Default.GetInstance<ServiceProviderViewModel>();
            Messenger.Default.Register<EditServiceProviderMessage>(editServiceProvider,editServiceProvider.EditServiceProvider);

            var activityDetailsPopUp = SimpleIoc.Default.GetInstance<ActivityDetailsPopUp>();
            Messenger.Default.Register<DetailsPopUpMessage>(activityDetailsPopUp, activityDetailsPopUp.SetActivity);

           
            var warehouseExitViewModel = SimpleIoc.Default.GetInstance<WarehouseExitViewModel>();
            Messenger.Default.Register<WarehouseEntryUpdateMessage>(warehouseExitViewModel, warehouseExitViewModel.SetDocumentId);

            var warehouseGoodsReceivedViewModel = SimpleIoc.Default.GetInstance<WarehouseGoodsReceivedViewModel>();
            Messenger.Default.Register<WarehouseEntryUpdateMessage>(warehouseGoodsReceivedViewModel, warehouseGoodsReceivedViewModel.SetDocumentId);

            var warehouseStoreViewModel = SimpleIoc.Default.GetInstance<WarehouseStoreViewModel>();
            Messenger.Default.Register<WarehouseEntryUpdateMessage>(warehouseStoreViewModel, warehouseStoreViewModel.SetDocumentId);
        }

        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseMenuViewModel WarehouseMenuViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseMenuViewModel>();
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public CommodityReleaseDocumentViewModel CommodityReleaseDocumentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CommodityReleaseDocumentViewModel>();
            }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public CommodityReleaseViewModel CommodityReleaseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CommodityReleaseViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public InventoryLevelViewModel InventoryLevelViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<InventoryLevelViewModel>();
            }
        }

        /// <summary>   
        /// Gets the ItemLookUpsLookUpViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ItemLookUpsLookUpViewModel ItemLookUpsLookUpViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ItemLookUpsLookUpViewModel>();
            }
        }

        /// <summary>   
        /// Gets the SupplierToOutletMappingViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SupplierToOutletMappingViewModel SupplierToOutletMappingViewModel    
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SupplierToOutletMappingViewModel>();
            }
        }

        #region WeighReceiveDeliveryViewModelPopUp


        /// <summary>
        /// Gets the WeighReceiveDeliveryViewModelPopUp property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WeighReceiveDeliveryViewModelPopUp WeighReceiveDeliveryViewModelPopUp
        {
            get { return ServiceLocator.Current.GetInstance<WeighReceiveDeliveryViewModelPopUp>(); }
        }

        #endregion

        #region AwaitingReceptionViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AwaitingReceptionViewModel AwaitingReceptionViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AwaitingReceptionViewModel>(); }
        }

        #endregion

        #region CompletedAndStoredViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CompletedAndStoredViewModel CompletedAndStoredViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CompletedAndStoredViewModel>();
            }
        }
        #endregion

        #region AwaitingStorageViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AwaitingStorageViewModel AwaitingStorageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AwaitingStorageViewModel>();
            }
        }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AgrimanagrReportsViewModel AgrimanagrReportsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AgrimanagrReportsViewModel>();
            }
        }

        #region EditCommodityPurchaseViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCommodityPurchaseViewModel EditCommodityPurchaseViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditCommodityPurchaseViewModel>();
            }
        }
        #endregion

        #region  AgrimanagrMainPageViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AgrimanagrMainPageViewModel AgrimanagrMainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AgrimanagrMainPageViewModel>();
            }
        }
        #endregion

        #region AgrimanagrReceiptDocumentViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public AgrimanagrReceiptDocumentViewModel AgrimanagrReceiptDocumentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AgrimanagrReceiptDocumentViewModel>();
            }
        }
        #endregion

        #region ListFarmersViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListFarmersViewModel ListFarmersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListFarmersViewModel>();
            }
        }
        #endregion

        #region AgriLoginViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public LoginViewModel LoginViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }
        #endregion

        #region EditUsersViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditUsersViewModel EditUsersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditUsersViewModel>();
            }
        }
        #endregion

        #region AgriConfigurationViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ConfigurationViewModel ConfigurationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConfigurationViewModel>();
            }
        }

        #endregion

        #region ComboPopUpViewModel

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ComboPopUpViewModel ComboPopUpViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ComboPopUpViewModel>();
            }
        }
        #endregion

        #region ReceiptDocumentViewModel

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ReceiptDocumentViewModel ReceiptDocumentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReceiptDocumentViewModel>();
            }
        }
        #endregion

        #region ListCommodityReceptionViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListCommodityReceptionViewModel ListCommodityReceptionViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListCommodityReceptionViewModel>();
            }
        }
        #endregion

        #region StoreCommodityViewModel

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public StoreCommodityViewModel StoreCommodityViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<StoreCommodityViewModel>();
            }
        }

        #endregion

        #region DocumentDetailsViewModel

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public DocumentDetailsViewModel DocumentDetailsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DocumentDetailsViewModel>();
            }
        }
        #endregion

        #region EquipmentSetupViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public EquipmentSetupViewModel EquipmentSetupViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EquipmentSetupViewModel>();
            }
        }
        #endregion

        #region ListWeingScalesViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1822:MarkMembersAsStatic",
           Justification = "This non-static member is needed for data binding purposes.")]
        public ListWeingScalesViewModel ListWeingScalesViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListWeingScalesViewModel>();
            }
        }
        #endregion

        #region SyncViewModel

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SyncViewModel SyncViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SyncViewModel>();
            }
        }
        #endregion

        #region GeneralSettingsViewModel

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public GeneralSettingsViewModel GeneralSettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GeneralSettingsViewModel>();
            }
        }
        #endregion

        #region EditAgriUsersViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public EditAgriUsersViewModel EditAgriUsersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditAgriUsersViewModel>();
            }
        }
        #endregion

        #region EditAgriUserContactViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public EditAgriUserContactViewModel EditAgriUserContactViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditAgriUserContactViewModel>();
            }
        }
        #endregion

        #region EditCommoditySupplierViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCommoditySupplierViewModel EditCommoditySupplierViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditCommoditySupplierViewModel>();
            }
        }
        #endregion

        #region   ListAgriUsersViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1822:MarkMembersAsStatic",
          Justification = "This non-static member is needed for data binding purposes.")]
        public ListAgriUsersViewModel ListAgriUsersViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ListAgriUsersViewModel>();
            }
        }

        #endregion

        #region EditRouteViewModel
        /// <summary>
        /// Gets the EditRouteViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditRouteViewModel EditRouteViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<EditRouteViewModel>();
            }
        }

        #endregion

        #region ListRoutesViewModel

        /// <summary>
        /// Gets the ListRoutesViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListRoutesViewModel ListRoutesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListRoutesViewModel>(); }
        }

        #endregion

        #region ListCommoditySuppliersViewModel

        /// <summary>
        /// Gets the ListCommoditySuppliersViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListCommoditySuppliersViewModel ListCommoditySuppliersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListCommoditySuppliersViewModel>(); }
        }

        #endregion

        #region EditCommodityProducerViewModel

        /// <summary>
        /// Gets the EditCommodityProducerViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCommodityProducerViewModel EditCommodityProducerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditCommodityProducerViewModel>(); }
        }

        #endregion

        #region ListCommodityProducersViewModel

        /// <summary>
        /// Gets the ListCommodityProducersViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListCommodityProducersViewModel ListCommodityProducersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListCommodityProducersViewModel>(); }
        }

        #endregion

        #region EditCommodityOwnerViewModel

        /// <summary>
        /// Gets the EditCommodityOwnerViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCommodityOwnerViewModel EditCommodityOwnerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditCommodityOwnerViewModel>(); }
        }

        #endregion

        #region EditIndividualFarmerViewModel

        /// <summary>
        /// Gets the EditIndividualFarmerViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditIndividualFarmerViewModel EditIndividualFarmerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditIndividualFarmerViewModel>(); }
        }

        #endregion

        #region ListCommodityOwnersViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListCommodityOwnersViewModel ListCommodityOwnersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListCommodityOwnersViewModel>(); }
        }

        
        #endregion

        #region EditCentreViewModel

        /// <summary>
        /// Gets the EditCentreViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCentreViewModel EditCentreViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditCentreViewModel>(); }
        }

        #endregion

        #region ListCentresViewModel

        /// <summary>
        /// Gets the ListCentresViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListCentresViewModel ListCentresViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListCentresViewModel>(); }
        }

        #endregion

        #region EditStoreViewModel

        /// <summary>
        /// Gets the EditStoreViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditStoreViewModel EditStoreViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditStoreViewModel>(); }
        }

        #endregion

        #region ListStoresViewModel

        /// <summary>
        /// Gets the ListStoresViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListStoresViewModel ListStoresViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListStoresViewModel>(); }
        }

        #endregion

        #region EditContainerTypeViewModel

        /// <summary>
        /// Gets the EditContainerTypeViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditContainerTypeViewModel EditContainerTypeViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditContainerTypeViewModel>(); }
        }

        #endregion

        #region ListContainerTypesViewModel

        /// <summary>
        /// Gets the ListContainerTypesViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListContainerTypesViewModel ListContainerTypesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListContainerTypesViewModel>(); }
        }

        #endregion

        #region EditContainerViewModel

        /// <summary>
        /// Gets the EditContainerViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditContainerViewModel EditContainerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditContainerViewModel>(); }
        }

        #endregion

        #region ListContainersViewModel

        /// <summary>
        /// Gets the ListContainersViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListContainersViewModel ListContainersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListContainersViewModel>(); }
        }

        #endregion

        #region AgriListContactsViewModel

        /// <summary>
        /// Gets the AgriListContactsViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AgriListContactsViewModel AgriListContactsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AgriListContactsViewModel>(); }
        }

        #endregion

        #region EditVehicleViewModel

        /// <summary>
        /// Gets the EditVehicleViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditVehicleViewModel EditVehicleViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditVehicleViewModel>(); }
        }

        #endregion

        #region ListVehiclesViewModel

        /// <summary>
        /// Gets the ListVehiclesViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListVehiclesViewModel ListVehiclesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListVehiclesViewModel>(); }
        }

        #endregion

        #region EditCommodityProducerModalViewModel

        /// <summary>
        /// Gets the EditCommodityProducerModalViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCommodityProducerModalViewModel EditCommodityProducerModalViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditCommodityProducerModalViewModel>(); }
        }

        #endregion

        #region EditCommodityOwnerModalViewModel

        /// <summary>
        /// Gets the EditCommodityOwnerModalViewModel property.
        /// </summary>
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditCommodityOwnerModalViewModel EditCommodityOwnerModalViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditCommodityOwnerModalViewModel>(); }
        }

        #endregion

        #region EditWeighingScaleViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditWeighingScaleViewModel EditWeighingScaleViewModel
        {
            get { return ServiceLocator.Current.GetInstance<EditWeighingScaleViewModel>(); }
        }

        #endregion

        #region ListWeighingScaleViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListWeighingScaleViewModel ListWeighingScaleViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListWeighingScaleViewModel>(); }
        }

        #endregion

        #region IntraStoreTransferViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public IntraStoreTransferViewModel IntraStoreTransferViewModel
        {
            get { return ServiceLocator.Current.GetInstance<IntraStoreTransferViewModel>(); }
        }
        #endregion

        #region ToHqInventoryTransferViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ToHqInventoryTransferViewModel ToHqInventoryTransferViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ToHqInventoryTransferViewModel>(); }
        }
        #endregion
        
        #region InventoryInStorageViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public InventoryInStorageViewModel InventoryInStorageViewModel
        {
            get { return ServiceLocator.Current.GetInstance<InventoryInStorageViewModel>(); }
        }

        #endregion

        #region TransferedInventoryViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public TransferedInventoryViewModel TransferedInventoryViewModel
        {
            get { return ServiceLocator.Current.GetInstance<TransferedInventoryViewModel>(); }
        }

        #endregion

        #region AddEditSupplierViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AddEditSupplierViewModel AddEditSupplierViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AddEditSupplierViewModel>(); }
        }

        #endregion

        #region ListingOfCommoditySuppliersViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListingOfCommoditySuppliersViewModel ListingOfCommoditySuppliers
        {
            get { return ServiceLocator.Current.GetInstance<ListingOfCommoditySuppliersViewModel>(); }
        }
        
        #endregion

        #region MemberCommodityOwnerViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MemberCommodityOwnerViewModel MemberOwnersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MemberCommodityOwnerViewModel>(); }
        }
        #endregion

        #region MemberCommodityProducerViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MemberCommodityProducerViewModel MemberProducersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MemberCommodityProducerViewModel>(); }
        }
        #endregion

        #region MemberSupplierContactViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListingSupplierContactsViewModel MemberSupplierContacts
        {
            get { return ServiceLocator.Current.GetInstance<ListingSupplierContactsViewModel>(); }
        }

        #endregion

        #region CommodityOwnerViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CommodityOwnerViewModel CommodityOwnerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CommodityOwnerViewModel>(); }
        }
        
        #endregion

        #region CommodityProducerViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public CommodityProducerViewModel CommodityProducerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<CommodityProducerViewModel>(); }
        }
        #endregion

        #region ContactsViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ContactsViewModel ContactsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ContactsViewModel>(); }
        }

        #endregion

        #region InfectionsViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public InfectionsViewModel InfectionsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<InfectionsViewModel>(); }
        }
        #endregion

        #region ListingInfectionsViewModels
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListingInfectionsViewModel ListingInfectionsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListingInfectionsViewModel>(); }
        }
        #endregion

        #region SeasonViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SeasonViewModel SeasonViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SeasonViewModel>(); }
        }
        #endregion

        #region ListingSeasonsViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public ListingSeasonsViewModel ListingSeasonsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListingSeasonsViewModel>(); }
        }
        #endregion

        #region ServiceViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ServiceViewModel ServiceViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ServiceViewModel>(); }
        }
        #endregion

        #region ListingServiceViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public ListingServicesViewModel ListingCommodityProducerServicesViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListingServicesViewModel>(); }
        }
        #endregion

        #region ShiftViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ShiftViewModel ShiftViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ShiftViewModel>(); }
        }
        #endregion

        #region ListingShiftsViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public ListingShiftsViewModel ListingShiftsViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListingShiftsViewModel>(); }
        }
        #endregion

        #region ServiceProviderViewModel

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ServiceProviderViewModel ServiceProviderViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ServiceProviderViewModel>(); }
        }
        #endregion

        #region ListingServiceProvidersViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public ListingServiceProvidersViewModel ListingServiceProvidersViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ListingServiceProvidersViewModel>(); }
        }
        #endregion

        #region ActivityListingViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public ActivityListingViewModel ActivityListingViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ActivityListingViewModel>(); }
        }
        #endregion
        #region WeighScaleDefaultViewModel
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public WeighScaleDefaultViewModel WeighScaleDefaultViewModel
        {
            get { return ServiceLocator.Current.GetInstance<WeighScaleDefaultViewModel>(); }
        }
        #endregion

        
        #region ActivityDetailsPopUp
        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]

        public ActivityDetailsPopUp ActivityDetailsPopUp
        {
            get { return ServiceLocator.Current.GetInstance<ActivityDetailsPopUp>(); }
        }
        #endregion

        #region AddWarehouseEntryViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AddWarehouseEntryViewModel AddWarehouseEntryViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AddWarehouseEntryViewModel>();
            }
        }
        #endregion

        #region WarehouseDepositorViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseDepositorViewModel WarehouseDepositorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseDepositorViewModel>();
            }
        }
        #endregion

        #region WarehouseEntryListingViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseEntryListingViewModel WarehouseEntryListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseEntryListingViewModel>();
            }
        }
        #endregion

        #region WarehouseExitViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseExitViewModel WarehouseExitViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseExitViewModel>();
            }
        }
        #endregion

        #region WarehouseExitListingViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseExitListingViewModel WarehouseExitListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseExitListingViewModel>();
            }
        }
        #endregion

        #region WarehouseStoreViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseStoreViewModel WarehouseStoreViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseStoreViewModel>();
            }
        }
        #endregion

        #region WarehouseGoodsReceivedViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseGoodsReceivedViewModel WarehouseGoodsReceivedViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseGoodsReceivedViewModel>();
            }
        }
        #endregion

        #region WarehousePendingStorageListingViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehousePendingStorageListingViewModel WarehousePendingStorageListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehousePendingStorageListingViewModel>();
            }
        }
        #endregion
        



        #region WarehouseInventoryLevelsListingViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseInventoryLevelsListingViewModel WarehouseInventoryLevelsListingViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseInventoryLevelsListingViewModel>();
            }
        }
        #endregion


        #region WarehouseReceiptViewModel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public WarehouseReceiptViewModel WarehouseReceiptViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WarehouseReceiptViewModel>();
            }
        }
        #endregion

      
        
    }
}
