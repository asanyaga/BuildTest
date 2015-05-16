using System.Collections.Generic;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.Util;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Distributr.Core.Domain.Master.CostCentreEntities;
using System;
using System.Windows;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Workflow.Impl.AuditLogs;
using ShipToAddress = Distributr.Core.MasterDataDTO.DataContracts.ShipToAddress;

namespace Distributr.WPF.Lib.ViewModels.Admin.Outlets
{

    public class EditOutletViewModel : DistributrViewModelBase
    {
        internal IPagenatedList<Core.Domain.Master.CostCentreEntities.ShipToAddress> PagedList;
        public EditOutletViewModel()
        {
            DistributorRoutes = new ObservableCollection<Route>();
            DistributorOutletCategories = new ObservableCollection<OutletCategory>();
            DistributorOutletType = new ObservableCollection<OutletType>();
            DistributorProductPricingTier = new ObservableCollection<ProductPricingTier>();
            ShippingAddresses = new ObservableCollection<ShipAddressItem>();
            VATClasses = new ObservableCollection<VATClass>();
            DiscountGroups = new ObservableCollection<DiscountGroup>();

            SaveCommand = new RelayCommand(DoSave);
            CancelCommand = new RelayCommand(CancelRequest);
            Load = new RelayCommand(DoLoad);
            AddAddressCommand = new RelayCommand(DoAddAddress);
            EditAddressItemCommand = new RelayCommand<ShipAddressItem>(DoEditAddress);
            DeleteAddressItemCommand = new RelayCommand<ShipAddressItem>(DoRemoveAddress);
            LoadShipToAddressCommand= new RelayCommand(LoadShipToAddress);
        }

       

        public RelayCommand Load { get; set; }
        void DoLoad()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Config config = GetConfigParams();
                DistributorRoutes.Clear();
                var route = new Route(Guid.Empty)
                    {
                        Name = GetLocalText("sl.outlet.edit.route.default")
                        /* "--Please Select A Route --"*/
                    };
                SelectedRoute = route;
                DistributorRoutes.Add(route);
                //should only get for current distributor
                Region region =null;
                if(config.AppId == Core.VirtualCityApp.Agrimanagr)
                    region =
                        ((Hub) Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId)).
                            Region;
                else if(config.AppId == Core.VirtualCityApp.Ditributr)
                    region =
                        ((Distributor)
                         Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId)).
                            Region;

                Using<IRouteRepository>(c).GetAll().Where(q => q.Region.Id == region.Id)
                             .OrderBy(n => n.Name)
                             .ToList()
                             .ForEach(n => DistributorRoutes.Add(n));

                DistributorOutletCategories.Clear();
                var outletCategory = new OutletCategory(Guid.Empty)
                    {
                        Name = GetLocalText("sl.outlet.edit.category.default") /*"--Please Select an Outlet--"*/
                    };
                SelectedOutletCategory = outletCategory;
                DistributorOutletCategories.Add(outletCategory);
                Using<IOutletCategoryRepository>(c).GetAll()
                                      .OrderBy(n => n.Name)
                                      .ToList()
                                      .ForEach(n => DistributorOutletCategories.Add(n));

                DistributorOutletType.Clear();
                var outletType = new OutletType(Guid.Empty)
                    {
                        Name = GetLocalText("sl.outlet.edit.type.default")
                        /*"--Please Select an Outlet Type--"*/
                    };
                SelectedOutletType = outletType;
                DistributorOutletType.Add(outletType);
                Using<IOutletTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(n => DistributorOutletType.Add(n));

                DistributorProductPricingTier.Clear();
                var ppt = new ProductPricingTier(Guid.Empty)
                    {
                        Name = GetLocalText("sl.outlet.edit.tier.default")
                        /* "--Please Select Product Pricing Tier--"*/
                    };
                SelectedProductPricingTier = ppt;
                DistributorProductPricingTier.Add(ppt);
                Using<IProductPricingTierRepository>(c).GetAll()
                                          .OrderBy(n => n.Name)
                                          .ToList()
                                          .ForEach(n => DistributorProductPricingTier.Add(n));

                VATClasses.Clear();
                var vatClass = new VATClass(Guid.Empty)
                    {
                        VatClass = GetLocalText("sl.outlet.edit.vatclass.default")
                        /*"--NONE--"*/
                    };
                SelectedVATRate = vatClass;
                VATClasses.Add(vatClass);
                Using<IVATClassRepository>(c).GetAll().OrderBy(n => n.VatClass).ToList().ForEach(n => VATClasses.Add(n));

                DiscountGroups.Clear();
                var discountGroup = new DiscountGroup(Guid.Empty)
                    {
                        Name = GetLocalText("sl.outlet.edit.discgp")
                        /*"--NONE--"*/
                    };
                SelectedDiscountGroup = discountGroup;
                DiscountGroups.Add(discountGroup);
                Using<IDiscountGroupRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(n => DiscountGroups.Add(n));
                ShippingAddresses.Clear();
                DeletedAddresses.Clear();
            }
        }

        private Outlet outlet = null;
        public void LoadById(Guid id)
        {
            if (id == Guid.Empty)
            {
                InitializeBlank();
            }
            else
            {
                 using (StructureMap.IContainer c = NestedContainer)
            {

                outlet = Using<ICostCentreRepository>(c).GetById(id) as Outlet;
                if (outlet == null)
                {
                    InitializeBlank();
                }
                else
                {
                        PageTitle = GetLocalText("sl.outlet.edit.editing.title"); // "Edit Outlet";
                        Id = outlet.Id;
                        Name = outlet.Name;
                        OutletCode = outlet.CostCentreCode;
                        Latitude = outlet.Latitude;
                        Longitude = outlet.Longitude;
                        IsApproved = outlet._Status ==EntityStatus.Active;
                        CanApprove = !IsApproved;

                        //load all items to combos
                        DistributorRoutes.Clear();
                        var route = new Route(Guid.Empty)
                            {
                                Name = GetLocalText("sl.outlet.edit.route.default")
                                /* "--Please Select A Route --"*/
                            };
                        DistributorRoutes.Add(route);
                        Using<IRouteRepository>(c).GetAll()
                                     .ToList()
                                     .OrderBy(n => n.Name)
                                     .ToList()
                                     .ForEach(n => DistributorRoutes.Add(n));

                        try
                        {
                            SelectedRoute = DistributorRoutes.FirstOrDefault(n => n.Id == outlet.Route.Id);
                        }
                        catch
                        {
//route deactivated!
                            SelectedRoute = route;
                        }

                        DistributorOutletCategories.Clear();
                        Using<IOutletCategoryRepository>(c).GetAll()
                                              .OrderBy(n => n.Name)
                                              .ToList()
                                              .ForEach(n => DistributorOutletCategories.Add(n));
                        SelectedOutletCategory =
                            DistributorOutletCategories.FirstOrDefault(n => n.Id == outlet.OutletCategory.Id);

                        DistributorOutletType.Clear();
                        Using<IOutletTypeRepository>(c).GetAll()
                                          .OrderBy(n => n.Name)
                                          .ToList()
                                          .ForEach(n => DistributorOutletType.Add(n));
                        SelectedOutletType = DistributorOutletType.FirstOrDefault(n => n.Id == outlet.OutletType.Id);

                        DistributorProductPricingTier.Clear();
                        Using<IProductPricingTierRepository>(c).GetAll()
                                                  .OrderBy(n => n.Name)
                                                  .ToList()
                                                  .ForEach(n => DistributorProductPricingTier.Add(n));
                        var ppt = new ProductPricingTier(Guid.Empty)
                            {
                                Name = GetLocalText("sl.outlet.edit.tier.default")
                                /* "--Please Select Product Pricing Tier--"*/
                            };
                        DistributorProductPricingTier.Add(ppt);
                        try
                        {
                            SelectedProductPricingTier =
                                DistributorProductPricingTier.FirstOrDefault(
                                    n => n.Id == outlet.OutletProductPricingTier.Id);
                        }
                        catch
                        {
                            SelectedProductPricingTier = ppt;
                        }

                        VATClasses.Clear();
                        var vatClass = new VATClass(Guid.Empty)
                            {
                                VatClass = GetLocalText("sl.outlet.edit.vatclass.default") /*"--NONE--"*/
                            };
                        VATClasses.Add(vatClass);
                        Using<IVATClassRepository>(c).GetAll().OrderBy(n => n.VatClass).ToList().ForEach(n => VATClasses.Add(n));
                        try
                        {
                            SelectedVATRate = VATClasses.First(n => n.Id == outlet.VatClass.Id);
                        }
                        catch
                        {
                            SelectedVATRate = vatClass;
                        }

                        DiscountGroups.Clear();
                        var discountGroup = new DiscountGroup(Guid.Empty)
                            {
                                Name = GetLocalText("sl.outlet.edit.discgp") /*"--NONE--"*/
                            };
                        SelectedDiscountGroup = discountGroup;
                        DiscountGroups.Add(discountGroup);
                        Using<IDiscountGroupRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(n => DiscountGroups.Add(n));
                        try
                        {
                            SelectedDiscountGroup = DiscountGroups.First(n => n.Id == outlet.DiscountGroup.Id);
                        }
                        catch
                        {
                            SelectedDiscountGroup = discountGroup;
                        }
                       
                       LoadShipToAddress();

                }
                }
            }
        }
        private void LoadShipToAddress()
        {
           ShippingAddresses.Clear();
            DeletedAddresses.Clear();
             if(outlet !=null)
             {
                 var addresses = outlet.ShipToAddresses.Where(n => n._Status != EntityStatus.Deleted).ToList();
                 if(!string.IsNullOrEmpty(SearchText))
                 {
                     SearchText = SearchText.ToLower();
                     addresses =
                         addresses.Where(
                             p =>
                             p.Code != null && p.Code.ToLower().Contains(SearchText) ||
                             p.Name != null && p.Name.ToLower().Contains(SearchText)).ToList();
                 }
                 
                 if(addresses.Count>10)
                 {
                     PagedList=new PagenatedList<Core.Domain.Master.CostCentreEntities.ShipToAddress>(addresses.AsQueryable(),20,20,addresses.Count);
                     PagedList.ToList().ForEach(n=>ShippingAddresses.Add(MapShippingAddress(n)));
                 }
                 else
                 {
                     addresses.ForEach(n=>ShippingAddresses.Add(MapShippingAddress(n)));
                     
                 }
             }
        }

        private static ShipAddressItem MapShippingAddress(Core.Domain.Master.CostCentreEntities.ShipToAddress address)
        {
            return new ShipAddressItem
                       {
                           Id = address.Id,
                           Name = address.Name,
                           PhysicalAddress = address.PhysicalAddress,
                           PostalAddress = address.PostalAddress,
                           Description = address.Description,
                           AddressLatitude = address.Latitude,
                           AddressLongitude = address.Longitude,
                           EntityStatus = (int) address._Status
                       };
        }

        private void InitializeBlank()
        {
            PageTitle = GetLocalText("sl.outlet.edit.new.title");//"Create New Outlet";
            Id = Guid.Empty;
            Name = String.Empty;
            OutletCode = String.Empty;
            Latitude = String.Empty;
            Longitude = String.Empty;
            CanApprove = false;
            IsApproved = true;
            DoLoad();
        }

        public void SetUp()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                ConfirmNavigatingAway = true;
                CanApprove = false;
                CanManageOutlet = Using<IConfigService>(c).ViewModelParameters.CurrentUserRights.CanManageOutlet;
                BntCancelContent = CanManageOutlet
                                       ? GetLocalText("sl.outlet.edit.cancel") /*"Cancel"*/
                                       : GetLocalText("sl.outlet.edit.back") /*"Back"*/;
            }
        }

        public RelayCommand AddAddressCommand { get; set; }
        public RelayCommand<ShipAddressItem> EditAddressItemCommand { get; set; }
        public RelayCommand<ShipAddressItem> DeleteAddressItemCommand { get; set; }

        void DoAddAddress()
        {
            var addressItem = new ShipAddressItem
                {
                    Id = AddressId,
                    Name = AddressName,
                    Description = AddressDescription,
                    AddressLongitude = AddressLongitude,
                    AddressLatitude = AddressLatitude,
                    PhysicalAddress = PhysicalAddress,
                    PostalAddress = PostalAddress
                };
            var address = ShippingAddresses.FirstOrDefault(n => n.Id == addressItem.Id);
            if (address != null)
            {
                ShippingAddresses.Remove(address);
                ShippingAddresses.Add(addressItem);
            }
            else
            {
                addressItem.Id = Guid.NewGuid();
                ShippingAddresses.Add(addressItem);
            }
            ClearDisplayedAddressItem();
        }

        void DoEditAddress(ShipAddressItem item)
        {
            AddressId = item.Id;
            AddressName = item.Name;
            AddressDescription = item.Description;
            AddressLongitude = item.AddressLongitude;
            AddressLatitude = item.AddressLatitude;
            PhysicalAddress = item.PhysicalAddress;
            PostalAddress = item.PostalAddress;
        }

        List<ShipToAddress> DeletedAddresses = new List<ShipToAddress>(); 
        void DoRemoveAddress(ShipAddressItem item)
        {
            ShippingAddresses.Remove(item);
            var deletedItem = new ShipToAddress
                {
                    MasterId = item.Id,
                    Name = item.Name,
                    PhysicalAddress = item.PhysicalAddress,
                    PostalAddress = item.PostalAddress,
                    Longitude = item.AddressLongitude,
                    Latitude = item.AddressLatitude,
                    Description = item.Description,
                    EntityStatus = (int)EntityStatus.Deleted
                };
                DeletedAddresses.Add(deletedItem);
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

       async void DoSave()
        {
            if (!IsValid())
                return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                ResponseBool response = null;
                var proxy = Using<IDistributorServiceProxy>(c);

                var shippingAddresses = this.ShippingAddresses.Select(address => new ShipToAddress
                                                                                     {
                                                                                         MasterId = address.Id,
                                                                                         Name = address.Name ?? "",
                                                                                         PhysicalAddress =
                                                                                             address.PhysicalAddress ??
                                                                                             "",
                                                                                         PostalAddress =
                                                                                             address.PostalAddress ?? "",
                                                                                         Longitude =
                                                                                             address.AddressLongitude,
                                                                                         Latitude =
                                                                                             address.AddressLatitude,
                                                                                         Description =
                                                                                             address.Description ?? "",
                                                                                     }).ToList();
                shippingAddresses.AddRange(DeletedAddresses);
                if (Id == Guid.Empty)
                {
                    try
                    {
                       
                        OutletItem item = new OutletItem
                                              {
                                                  MasterId = Guid.NewGuid(),
                                                  Name = Name,
                                                  OutletCategoryMasterId = SelectedOutletCategory.Id,
                                                  OutletTypeMasterId = SelectedOutletType.Id,
                                                  RouteMasterId = SelectedRoute.Id,
                                                  StatusId =(int)EntityStatus.Active ,
                                                  CostCentreTypeId = (int) CostCentreType.Outlet,
                                                  DateCreated = DateTime.Now,
                                                  DateLastUpdated = DateTime.Now,
                                                  ParentCostCentreId = Using<IConfigService>(c).Load().CostCentreId,
                                                  OutletProductPricingTierMasterId =
                                                      SelectedProductPricingTier.Id,
                                                  OutletVATClassMasterId =  SelectedVATRate.Id,
                                                  OutletDiscountGroupMasterId = SelectedDiscountGroup.Id,
                                                  outLetCode = OutletCode,
                                                  Latitude = Latitude,
                                                  Longitude = Longitude,
                                                  ShippingAddresses = shippingAddresses
                                              };
                        response = await proxy.OutletAddAsync(item);

                        AuditLogEntry = string.Format("Created New Outlet: {0}; OutletType: {1}; Route: {2}; Tier{3};", Name,
                                                 SelectedOutletType.Name, SelectedRoute.Name,
                                                 SelectedProductPricingTier.Name);
                        Using<IAuditLogWFManager>(c).AuditLogEntry("Outlets Administration", AuditLogEntry);

                    }catch(Exception e)
                    {
                        MessageBox.Show(e.Message, "Distributr", MessageBoxButton.OK);
                    }
                    
                   
                }
                else
                {
                   
                   response= await proxy.OutletUpdateAsync(new OutletItem
                            {
                                MasterId = Id,
                                Name = Name,
                                OutletCategoryMasterId = SelectedOutletCategory.Id,
                                OutletTypeMasterId = SelectedOutletType.Id,
                                RouteMasterId = SelectedRoute.Id,
                                StatusId = IsApproved
                                               ? (int) EntityStatus.Active
                                               : (int) EntityStatus.New,
                                CostCentreTypeId = (int) CostCentreType.Outlet,
                                DateCreated = DateTime.Now,
                                DateLastUpdated = DateTime.Now,
                                ParentCostCentreId = Using<IConfigService>(c).Load().CostCentreId,
                                OutletProductPricingTierMasterId = SelectedProductPricingTier.Id,
                                OutletVATClassMasterId =  SelectedVATRate.Id,
                                OutletDiscountGroupMasterId = SelectedDiscountGroup.Id,
                                outLetCode = OutletCode,
                                Latitude = Latitude,
                                Longitude = Longitude,
                                ShippingAddresses = shippingAddresses
                            });
                    AuditLogEntry = string.Format("Updated Outlet: {0}; OutletType: {1}; Route: {2}; Tier{3};", Name,
                                                  SelectedOutletType.Name, SelectedRoute.Name,
                                                  SelectedProductPricingTier.Name);
                    Using<IAuditLogWFManager>(c).AuditLogEntry("Outlets Administration", AuditLogEntry);
                }
                MessageBox.Show(response.ErrorInfo, "Distributr:", MessageBoxButton.OK);
                if (response.Success)
                {
                    ConfirmNavigatingAway = false;
                    SendNavigationRequestMessage(new Uri("views/administration/outlets/listoutlets.xaml",
                                                         UriKind.Relative));
                }
            }
            AddressName = "";
            PhysicalAddress = "";
            PostalAddress = "";
            AddressLongitude = 0;
            AddressLatitude = 0;
            AddressDescription = "";
        }

        void CancelRequest()
        {
            if (!CanManageOutlet)
            {
                CancelAll();
            }
            else
            {
                if (
                    MessageBox.Show(/*"Unsaved changes will be lost.\nCancel creating/editing outlet anyway?"*/
                    GetLocalText("sl.outlet.edit.cancel.messagebox.propmt"),
                                    GetLocalText("sl.outlet.edit.cancel.messagebox.text")/*"Distributr: Create Outlet"*/
                                    , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    CancelAll();
                }
            }
        }

        public void CancelAll()
        {
            ConfirmNavigatingAway = false;
            SendNavigationRequestMessage(new Uri("/views/administration/outlets/listoutlets.xaml", UriKind.Relative));
            BntCancelContent = GetLocalText("sl.outlet.edit.cancel") /*"Cancel"*/;
        }

        void ClearDisplayedAddressItem()
        {
            AddressId = Guid.Empty;
            AddressName = null;
            AddressDescription = null;
            AddressLongitude = null;
            AddressLatitude = null;
            PhysicalAddress = null;
            PostalAddress = null;
        }

      
        #region Properties
        public ObservableCollection<Route> DistributorRoutes { get; set; }
        public ObservableCollection<OutletCategory> DistributorOutletCategories { get; set; }
        public ObservableCollection<OutletType> DistributorOutletType { get; set; }
        public ObservableCollection<ProductPricingTier> DistributorProductPricingTier { get; set; }
        public ObservableCollection<VATClass> VATClasses { get; set; }
        public ObservableCollection<DiscountGroup> DiscountGroups { get; set; }
        public ObservableCollection<ShipAddressItem> ShippingAddresses { get; set; }
        public RelayCommand LoadShipToAddressCommand { get; set; }

        public const string AuditLogEntryPropertyName = "AuditLogEntry";
        private string _AuditLogEntry = null;
        public string AuditLogEntry
        {
            get
            {
                return _AuditLogEntry;
            }

            set
            {
                if (_AuditLogEntry == value)
                {
                    return;
                }

                var oldValue = _AuditLogEntry;
                _AuditLogEntry = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AuditLogEntryPropertyName);
            }
        }

        public const string IdPropertyPropertyName = "Id";
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

                var oldValue = _id;
                _id = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyPropertyName);

            }
        }

        public const string NamePropertyName = "Name";
        private string _name = "";
        [Required(ErrorMessage = "Name is required")]
        [StringLength(500, ErrorMessage = "Name must be over 1 character", MinimumLength = 1)]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {

                if (_name == value)
                {
                    return;
                }

                var oldValue = _name;
                _name = value;
                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);
            }
        }

        public const string SelectedRoutePropertyName = "SelectedRoute";
        private Route _selectedRotue = null;
        //[CustomValidation(typeof(RouteValidator), "ValidateSelectedRoute")]
        [MasterDataDropDownValidation]
        public Route SelectedRoute
        {
            get
            {
                return _selectedRotue;
            }

            set
            {
                //Validate(value, NamePropertyName);

                if (_selectedRotue == value)
                    return;

                var oldValue = _selectedRotue;
                _selectedRotue = value;
                RaisePropertyChanged(SelectedRoutePropertyName);
            }
        }

        public const string SelectedOutletCategoryPropertyName = "SelectedOutletCategory";
        private OutletCategory _outletCategory = null;
        [MasterDataDropDownValidation]
        public OutletCategory SelectedOutletCategory
        {
            get
            {
                return _outletCategory;
            }

            set
            {
                if (_outletCategory == value)
                    return;

                var oldValue = _outletCategory;
                _outletCategory = value;
                RaisePropertyChanged(SelectedOutletCategoryPropertyName);
            }
        }
        public const string SearchTextPropertyName = "SearchText";
        private string _searchText = "";
        protected string SearchText
        {
            get { return _searchText; }

            set
            {
                if (_searchText == value)
                {
                    return;
                }

                RaisePropertyChanging(SearchTextPropertyName);
                _searchText = value;
                RaisePropertyChanged(SearchTextPropertyName);
            }
        }
        public const string SelectedOutletTypePropertyName = "SelectedOutletType";
        private OutletType _outletType = null;
        [MasterDataDropDownValidation]
        public OutletType SelectedOutletType
        {
            get
            {
                return _outletType;
            }

            set
            {
                if (_outletType == value)
                    return;

                var oldValue = _outletType;
                _outletType = value;
                RaisePropertyChanged(SelectedOutletTypePropertyName);
            }
        }

        public const string SelectedProductPricingTierPropertyName = "SelectedProductPricingTier";
        private ProductPricingTier _selectedProductPricingTier = null;
        [MasterDataDropDownValidation]
        public ProductPricingTier SelectedProductPricingTier
        {
            get
            {
                return _selectedProductPricingTier;
            }

            set
            {
                if (_selectedProductPricingTier == value)
                {
                    return;
                }

                var oldValue = _selectedProductPricingTier;
                _selectedProductPricingTier = value;
                RaisePropertyChanged(SelectedProductPricingTierPropertyName);
            }
        }
         
        public const string SelectedVATRatePropertyName = "SelectedVATRate";
        private VATClass _selectedVATRate = null;
        
        public VATClass  SelectedVATRate
        {
            get
            {
                return _selectedVATRate;
            }

            set
            {
                if (_selectedVATRate == value)
                {
                    return;
                }

                var oldValue = _selectedVATRate;
                _selectedVATRate = value;
                RaisePropertyChanged(SelectedVATRatePropertyName);
            }
        }

        public const string SelectedDiscountGroupPropertyName = "SelectedDiscountGroup";
        private DiscountGroup _discountGroup = null;
        public DiscountGroup SelectedDiscountGroup
        {
            get
            {
                return _discountGroup;
            }

            set
            {
                if (_discountGroup == value)
                {
                    return;
                }

                var oldValue = _discountGroup;
                _discountGroup = value;
                RaisePropertyChanged(SelectedDiscountGroupPropertyName);
            }
        }

        public const string OutletCodePropertyName = "OutletCode";
        private string _outletCode = "";
        [Required(ErrorMessage="Outlet code is required.")]
        public string OutletCode
        {
            get
            {
                return _outletCode;
            }

            set
            {
                if (_outletCode == value)
                {
                    return;
                }

                var oldValue = _outletCode;
                _outletCode = value;
                RaisePropertyChanged(OutletCodePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create New Outlet";
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                if (_pageTitle == value)
                {
                    return;
                }

                var oldValue = _pageTitle;
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == NamePropertyName)
                {
                    if (string.IsNullOrEmpty(_name))
                    {
                        return "Name should not be empty";
                    }
                }
                return null;
            }
        }

        public string Error
        {
            get { return null; }
        }

        private void Validate(object value, string propertyName)
        {
            Validator.ValidateProperty(value,
            new ValidationContext(this, null, null)
            {
                MemberName = propertyName
            });
        }

        public const string LongitudePropertyName = "Longitude";
        private string _longitude = "";
        public string Longitude
        {
            get
            {
                return _longitude;
            }

            set
            {
                if (_longitude == value)
                {
                    return;
                }

                var oldValue = _longitude;
                _longitude = value;
                RaisePropertyChanged(LongitudePropertyName);
            }
        }

        public const string LatitudePropertyName = "Latitude";
        private string _latitude = "";
        public string Latitude
        {
            get
            {
                return _latitude;
            }

            set
            {
                if (_latitude == value)
                {
                    return;
                }

                var oldValue = _latitude;
                _latitude = value;
                RaisePropertyChanged(LatitudePropertyName);
            }
        }

        public const string CanManageOutletPropertyName = "CanManageOutlet";
        private bool _canManageOutlet = false;
        public bool CanManageOutlet
        {
            get
            {
                return _canManageOutlet;
            }

            set
            {
                if (_canManageOutlet == value)
                {
                    return;
                }

                var oldValue = _canManageOutlet;
                _canManageOutlet = value;
                RaisePropertyChanged(CanManageOutletPropertyName);
            }
        }

        public const string BntCancelContentPropertyName = "BntCancelContent";
        private string _btnCancelContent = "Cancel";
        public string BntCancelContent
        {
            get
            {
                return _btnCancelContent;
            }

            set
            {
                if (_btnCancelContent == value)
                {
                    return;
                }

                var oldValue = _btnCancelContent;
                _btnCancelContent = value;
                RaisePropertyChanged(BntCancelContentPropertyName);
            }
        }

        public const string IsApprovedPropertyName = "IsApproved";
        private bool _isApproved = true;
        public bool IsApproved
        {
            get
            {
                return _isApproved;
            }

            set
            {
                if (_isApproved == value)
                {
                    return;
                }

                var oldValue = _isApproved;
                _isApproved = value;
                RaisePropertyChanged(IsApprovedPropertyName);
            }
        }

        public const string CanApprovePropertyName = "CanApprove";
        private bool _canApprove = false;
        public bool CanApprove
        {
            get
            {
                return _canApprove;
            }

            set
            {
                if (_canApprove == value)
                {
                    return;
                }

                var oldValue = _canApprove;
                _canApprove = value;
                RaisePropertyChanged(CanApprovePropertyName);
            }
        }
        #endregion

        #region AddressItem
        public const string AddressNamePropertyName = "AddressName";
        private string _addressName = null;
        public string AddressName
        {
            get
            {
                return _addressName;
            }

            set
            {
                if (_addressName == value)
                {
                    return;
                }

                var oldValue = _addressName;
                _addressName = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressNamePropertyName);
            }
        }

        public const string PhysicalAddressPropertyName = "PhysicalAddress";
        private string _physicalAddress = null;
        public string PhysicalAddress
        {
            get
            {
                return _physicalAddress;
            }

            set
            {
                if (_physicalAddress == value)
                {
                    return;
                }

                var oldValue = _physicalAddress;
                _physicalAddress = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        public const string PostalAddressPropertyName = "PostalAddress";
        private string _postalAddress = null;
        public string PostalAddress
        {
            get
            {
                return _postalAddress;
            }

            set
            {
                if (_postalAddress == value)
                {
                    return;
                }

                var oldValue = _postalAddress;
                _postalAddress = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PostalAddressPropertyName);
            }
        }

        public const string AddressLongitudePropertyName = "AddressLongitude";
        private decimal? _addressLongitude = null;
        public decimal? AddressLongitude
        {
            get
            {
                return _addressLongitude;
            }

            set
            {
                if (_addressLongitude == value)
                {
                    return;
                }

                var oldValue = _addressLongitude;
                _addressLongitude = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressLongitudePropertyName);
            }
        }

        public const string AddressLatitudePropertyName = "AddressLatitude";
        private decimal? _addressLatitude = null;
        public decimal? AddressLatitude
        {
            get
            {
                return _addressLatitude;
            }

            set
            {
                if (_addressLatitude == value)
                {
                    return;
                }

                var oldValue = _addressLatitude;
                _addressLatitude = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressLatitudePropertyName);
            }
        }

        public const string AddressDescriptionPropertyName = "AddressDescription";
        private string _addressDescription = null;
        public string AddressDescription
        {
            get
            {
                return _addressDescription;
            }

            set
            {
                if (_addressDescription == value)
                {
                    return;
                }

                var oldValue = _addressDescription;
                _addressDescription = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressDescriptionPropertyName);
            }
        }

        public const string AddressIdPropertyPropertyName = "AddressId";
        private Guid _addressId = Guid.Empty;
        public Guid AddressId
        {
            get
            {
                return _addressId;
            }

            set
            {
                if (_addressId == value)
                {
                    return;
                }

                var oldValue = _addressId;
                _addressId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressIdPropertyPropertyName);
            }
        }
        #endregion
    }

    public class ShipAddressItem : ViewModelBase
    {
        public const string NamePropertyName = "Name";
        private string _name = null;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                var oldValue = _name;
                _name = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);
            }
        }

        public const string PhysicalAddressPropertyName = "PhysicalAddress";
        private string _physicalAddress = null;
        public string PhysicalAddress
        {
            get
            {
                return _physicalAddress;
            }

            set
            {
                if (_physicalAddress == value)
                {
                    return;
                }

                var oldValue = _physicalAddress;
                _physicalAddress = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        public const string PostalAddressPropertyName = "PostalAddress";
        private string _postalAddress = null;
        public string PostalAddress
        {
            get
            {
                return _postalAddress;
            }

            set
            {
                if (_postalAddress == value)
                {
                    return;
                }

                var oldValue = _postalAddress;
                _postalAddress = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(PostalAddressPropertyName);
            }
        }

        public const string AddressLongitudePropertyName = "Longitude";
        private decimal? _addressLongitude = null;
        public decimal? AddressLongitude
        {
            get
            {
                return _addressLongitude;
            }

            set
            {
                if (_addressLongitude == value)
                {
                    return;
                }

                var oldValue = _addressLongitude;
                _addressLongitude = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressLongitudePropertyName);
            }
        }

        public const string AddressLatitudePropertyName = "AddressLatitude";
        private decimal? _addressLatitude = null;
        public decimal? AddressLatitude
        {
            get
            {
                return _addressLatitude;
            }

            set
            {
                if (_addressLatitude == value)
                {
                    return;
                }

                var oldValue = _addressLatitude;
                _addressLatitude = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(AddressLatitudePropertyName);
            }
        }

        public const string DescriptionPropertyName = "Description";
        private string _description = null;
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                var oldValue = _description;
                _description = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        public const string IdPropertyPropertyName = "Id";
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

                var oldValue = _id;
                _id = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IdPropertyPropertyName);
            }
        }

        public int EntityStatus { get; set; }
    }
}