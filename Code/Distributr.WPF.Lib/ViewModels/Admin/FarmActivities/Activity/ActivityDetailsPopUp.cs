using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Domain.Transactional.ActivityDocumentEntities;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.WPF.Lib.Messages;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.FarmActivities.Activity
{
    public class ActivityDetailsPopUp:DistributrViewModelBase
    {
        
        public ActivityDetailsPopUp()
        {
            LoadCommand=new RelayCommand(Load);
            BackCommand = new RelayCommand(Back);
            InfectionLineItemsList = new ObservableCollection<VmInfectionLineItem>();
            InputLineItemsList = new ObservableCollection<VmInputLineItem>();
            ProduceLineItemsList = new ObservableCollection<VmProduceLineItem>();
            ServiceLineItemsList = new ObservableCollection<VmServiceLineItem>();

        }

        private void Back()
        {
            SendNavigationRequestMessage(
                    new Uri("views/Activities/ActivityListing.xaml", UriKind.Relative));
        }

        #region Class Members
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public ObservableCollection<VmInfectionLineItem> InfectionLineItemsList { get; set; }
        public ObservableCollection<VmInputLineItem> InputLineItemsList { get; set; }
        public ObservableCollection<VmProduceLineItem> ProduceLineItemsList { get; set; }
        public ObservableCollection<VmServiceLineItem> ServiceLineItemsList { get; set; }
        #endregion

        #region Methods
        public void Load()
        {
            using(var c=NestedContainer )
            {
                ClearViewModel();
                var activity = Using<IActivityRepository>(c).GetById(ActivityId);
                DocumentRef = activity.DocumentReference;
                Hub = activity.Hub.Name;
                FieldClerk = activity.FieldClerk.Name;
                Supplier = activity.Supplier.Name;
                Producer = activity.Producer.Name;
                Route = activity.Route.Name;
                Center = activity.Centre.Name;
                ActivityDate = activity.ActivityDate.ToString("dd/MM/yyyy");
                Season = activity.Season.Name;
                ActivityReference = activity.ActivityReference;

                LoadDetails(activity);
               
            }
        }

        private void ClearViewModel()
        {
            InfectionLineItemsList.Clear();
            InputLineItemsList.Clear();
            ProduceLineItemsList.Clear();
            ServiceLineItemsList.Clear();
            IsInputVisible = "Hidden";
            IsInfectionVisible = "Hidden";
            IsProduceVisible = "Hidden";
            IsServiceVisible = "Hidden";

            IsInfectionExpanded = false;
            IsInputExpanded = false;
            IsProduceExpanded = false;
            IsServiceExpanded = false;


        }

        private void LoadDetails(ActivityDocument activity)
        {
            if(activity.ActivityType.IsInfectionsRequired)
            {
                IsInfectionVisible = activity.ActivityType.IsInfectionsRequired ? "Visible" : "Hidden";
                IsInfectionExpanded = activity.ActivityType.IsInfectionsRequired;
                LoadInfections(activity);
            }
            if(activity.ActivityType.IsInputRequired)
            {
                IsInputVisible = activity.ActivityType.IsInputRequired ? "Visible" : "Hidden";
                IsInputExpanded = activity.ActivityType.IsInputRequired;
                LoadInputs(activity);
            }
            if(activity.ActivityType.IsProduceRequired)
            {
                IsProduceVisible = activity.ActivityType.IsProduceRequired ? "Visible" : "Hidden";
                IsProduceExpanded = activity.ActivityType.IsProduceRequired;
                LoadProduce(activity);
            }
            if(activity.ActivityType.IsServicesRequired)
            {
                IsServiceVisible = activity.ActivityType.IsServicesRequired ? "Visible" : "Hidden";
                IsServiceExpanded = activity.ActivityType.IsServicesRequired;
                LoadServices(activity);
            }
        }


        private void LoadServices(ActivityDocument activity)
        {
            ServiceLineItemsList.Clear();
            using(var c=NestedContainer)
             {
                if (activity.ServiceItems != null)
                    foreach (var item in activity.ServiceItems)
                    {
                        var commodityProducerService =item.Service!=null? Using<IServiceRepository>(c).GetById(item.Service.Id):null;
                        var serviceProvider = item.ServiceProvider != null? Using<IServiceProviderRepository>(c).GetById(item.ServiceProvider.Id): null;
                        var shift = item.Shift != null ? Using<IShiftRepository>(c).GetById(item.Shift.Id) : null;
                        ServiceLineItemsList.Add(new VmServiceLineItem
                        {
                            CommodityProducerService =commodityProducerService!=null?commodityProducerService.Name:"",
                            ServiceProvider=serviceProvider!=null?serviceProvider.Name:"",
                            Shift=shift!=null?shift.Name:""

                        });
                    }
             }
           
        }

        private void LoadInputs(ActivityDocument activity)
        {
            InputLineItemsList.Clear();
            using (var c = NestedContainer)
            {
                if (activity.InputLineItems != null)
                    foreach (var item in activity.InputLineItems)
                    {
                        var product = item.Product != null? Using<IProductRepository>(c).GetById(item.Product.Id): null;
                       
                        InputLineItemsList.Add(new VmInputLineItem
                        {
                             
                            Product =product != null ? product.Description : "",
                            Quantity=item.Quantity.ToString(),
                            ManufacturedDate=item.ManufacturedDate.HasValue?item.ManufacturedDate.Value.ToString("dd/MM/yyyy"):"",
                            ExpiryDate=item.ExpiryDate.HasValue?item.ExpiryDate.Value.ToString("dd/MM/yyyy"):"",
                            SerialNumber=item.SerialNo

                        });
                    }
            }
        }

        private void LoadProduce(ActivityDocument activity)
        {
            ProduceLineItemsList.Clear();
            using (var c = NestedContainer)
            {
                if (activity.ProduceItems != null)
                    foreach (var item in activity.ProduceItems)
                    {
                        var serviceProvider = item.ServiceProvider != null ? Using<IServiceProviderRepository>(c).GetById(item.ServiceProvider.Id) : null;
                        var commodity = item.Commodity != null? Using<ICommodityRepository>(c).GetById(item.Commodity.Id):null;
                        //var grade=item.Grade!=null?Using<ICommodityGradeRepository>(c)
                        ProduceLineItemsList.Add(new VmProduceLineItem
                        {
                            ServiceProvider=serviceProvider!=null?serviceProvider.Name:"",
                            Commodity=commodity!=null?commodity.Name:"",
                            Weight=item.Weight.ToString(),

                        });
                    }
            }
        }

        private void LoadInfections(ActivityDocument activity)
        {
            InfectionLineItemsList.Clear();
            using(var c=NestedContainer)
            {
                if(activity.InfectionLineItems!=null)
                    foreach(var item in activity.InfectionLineItems)
                    {
                        var infection = item.Infection != null? Using<IInfectionRepository>(c).GetById(item.Infection.Id): null;

                        InfectionLineItemsList.Add(new VmInfectionLineItem
                            {
                                Infection=infection!=null?infection.Name:"",
                                Rate=item.Rate.ToString(),
                            });
                    }
            }
        }

        #endregion

        #region Properties


        public const string IsInputVisiblePropertyName = "IsInputVisible";
        private string _isInputVisible = "Hidden";
        public string IsInputVisible
        {
            get
            {
                return _isInputVisible;
            }

            set
            {
                if (_isInputVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsInputVisiblePropertyName);
                _isInputVisible = value;
                RaisePropertyChanged(IsInputVisiblePropertyName);
            }
        }

        
        public const string IsProduceVisiblePropertyName = "IsProduceVisible";
        private string _isProduceVisible = "Hidden";
        public string IsProduceVisible
        {
            get
            {
                return _isProduceVisible;
            }

            set
            {
                if (_isProduceVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsProduceVisiblePropertyName);
                _isProduceVisible = value;
                RaisePropertyChanged(IsProduceVisiblePropertyName);
            }
        }

        
        public const string IsServiceVisiblePropertyName = "IsServiceVisible";
        private string _isServiceVisible = "Hidden";
        public string IsServiceVisible
        {
            get
            {
                return _isServiceVisible;
            }

            set
            {
                if (_isServiceVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsServiceVisiblePropertyName);
                _isServiceVisible = value;
                RaisePropertyChanged(IsServiceVisiblePropertyName);
            }
        }

        
        public const string IsInfectionVisiblePropertyName = "IsInfectionVisible";
        private string _isInfectionVisible = "Hidden";
        public string IsInfectionVisible
        {
            get
            {
                return _isInfectionVisible;
            }

            set
            {
                if (_isInfectionVisible == value)
                {
                    return;
                }

                RaisePropertyChanging(IsInfectionVisiblePropertyName);
                _isInfectionVisible = value;
                RaisePropertyChanged(IsInfectionVisiblePropertyName);
            }
        }

        
        public const string IsInfectionExpandedPropertyName = "IsInfectionExpanded";
        private bool _isInfectionExpanded = false;
        public bool IsInfectionExpanded
        {
            get
            {
                return _isInfectionExpanded;
            }

            set
            {
                if (_isInfectionExpanded == value)
                {
                    return;
                }

                RaisePropertyChanging(IsInfectionExpandedPropertyName);
                _isInfectionExpanded = value;
                RaisePropertyChanged(IsInfectionExpandedPropertyName);
            }
        }

        
        public const string IsInputExpandedPropertyName = "IsInputExpanded";
        private bool _isInputExpanded = false;
        public bool IsInputExpanded
        {
            get
            {
                return _isInputExpanded;
            }

            set
            {
                if (_isInputExpanded == value)
                {
                    return;
                }

                RaisePropertyChanging(IsInputExpandedPropertyName);
                _isInputExpanded = value;
                RaisePropertyChanged(IsInputExpandedPropertyName);
            }
        }

        
        public const string IsProduceExpandedPropertyName = "IsProduceExpanded";
        private bool _isProduceExpanded = false;
        public bool IsProduceExpanded
        {
            get
            {
                return _isProduceExpanded;
            }

            set
            {
                if (_isProduceExpanded == value)
                {
                    return;
                }

                RaisePropertyChanging(IsProduceExpandedPropertyName);
                _isProduceExpanded = value;
                RaisePropertyChanged(IsProduceExpandedPropertyName);
            }
        }

        
        public const string IsServiceExpandedPropertyName = "IsServiceExpanded";
        private bool _isServiceExpanded = false;
        public bool IsServiceExpanded
        {
            get
            {
                return _isServiceExpanded;
            }

            set
            {
                if (_isServiceExpanded == value)
                {
                    return;
                }

                RaisePropertyChanging(IsServiceExpandedPropertyName);
                _isServiceExpanded = value;
                RaisePropertyChanged(IsServiceExpandedPropertyName);
            }
        }


        public const string ActivityIdPropertyName = "ActivityId";
        private Guid _activityId = Guid.Empty;
        public Guid ActivityId
        {
            get
            {
                return _activityId;
            }

            set
            {
                if (_activityId == value)
                {
                    return;
                }

                RaisePropertyChanging(ActivityIdPropertyName);
                _activityId = value;
                RaisePropertyChanged(ActivityIdPropertyName);
            }
        }

        
        public const string ActivityTypePropertyName = "ActivityType";
        private ActivityType _activityType = null;
        public ActivityType ActivityType
        {
            get
            {
                return _activityType;
            }

            set
            {
                if (_activityType == value)
                {
                    return;
                }

                RaisePropertyChanging(ActivityTypePropertyName);
                _activityType = value;
                RaisePropertyChanged(ActivityTypePropertyName);
            }
        }

        public const string DocumentRefPropertyName = "DocumentRef";
        private string _documentRef = "";
        public string DocumentRef
        {
            get
            {
                return _documentRef;
            }

            set
            {
                if (_documentRef == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentRefPropertyName);
                _documentRef = value;
                RaisePropertyChanged(DocumentRefPropertyName);
            }
        }

        
        public const string HubPropertyName = "Hub";
        private string _hub = "";
        public string Hub
        {
            get
            {
                return _hub;
            }

            set
            {
                if (_hub == value)
                {
                    return;
                }

                RaisePropertyChanging(HubPropertyName);
                _hub = value;
                RaisePropertyChanged(HubPropertyName);
            }
        }

        
        public const string FieldClerkPropertyName = "FieldClerk";
        private string _fieldClerk = "";
        public string FieldClerk
        {
            get
            {
                return _fieldClerk;
            }

            set
            {
                if (_fieldClerk == value)
                {
                    return;
                }

                RaisePropertyChanging(FieldClerkPropertyName);
                _fieldClerk = value;
                RaisePropertyChanged(FieldClerkPropertyName);
            }
        }

        
        public const string SupplierPropertyName = "Supplier";
        private string _supplier = "";
        public string Supplier
        {
            get
            {
                return _supplier;
            }

            set
            {
                if (_supplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierPropertyName);
                _supplier = value;
                RaisePropertyChanged(SupplierPropertyName);
            }
        }

        
        public const string ProducerPropertyName = "Producer";
        private string _producer = "";
        public string Producer
        {
            get
            {
                return _producer;
            }

            set
            {
                if (_producer == value)
                {
                    return;
                }

                RaisePropertyChanging(ProducerPropertyName);
                _producer = value;
                RaisePropertyChanged(ProducerPropertyName);
            }
        }

        
        public const string RoutePropertyName = "Route";
        private string _route = "";
        public string Route
        {
            get
            {
                return _route;
            }

            set
            {
                if (_route == value)
                {
                    return;
                }

                RaisePropertyChanging(RoutePropertyName);
                _route = value;
                RaisePropertyChanged(RoutePropertyName);
            }
        }

        
        public const string CenterPropertyName = "Center";
        private string _center = "";
        public string Center
        {
            get
            {
                return _center;
            }

            set
            {
                if (_center == value)
                {
                    return;
                }

                RaisePropertyChanging(CenterPropertyName);
                _center = value;
                RaisePropertyChanged(CenterPropertyName);
            }
        }

        
        public const string ActivityDatePropertyName = "ActivityDate";
        private string _activityDate = "";
        public string ActivityDate
        {
            get
            {
                return _activityDate;
            }

            set
            {
                if (_activityDate == value)
                {
                    return;
                }

                RaisePropertyChanging(ActivityDatePropertyName);
                _activityDate = value;
                RaisePropertyChanged(ActivityDatePropertyName);
            }
        }

        
        public const string DocumentIssuerCostCenterPropertyName = "DocumentIssuerCostCenter";
        private string _documentIssuerCostCenter = "";
        public string DocumentIssuerCostCenter
        {
            get
            {
                return _documentIssuerCostCenter;
            }

            set
            {
                if (_documentIssuerCostCenter == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIssuerCostCenterPropertyName);
                _documentIssuerCostCenter = value;
                RaisePropertyChanged(DocumentIssuerCostCenterPropertyName);
            }
        }

        
        public const string SeasonPropertyName = "Season";
        private string _season = "";
        public string Season
        {
            get
            {
                return _season;
            }

            set
            {
                if (_season == value)
                {
                    return;
                }

                RaisePropertyChanging(SeasonPropertyName);
                _season = value;
                RaisePropertyChanged(SeasonPropertyName);
            }
        }

        
        public const string ActivityReferencePropertyName = "ActivityReference";
        private string _activityReference = "";
        public string ActivityReference
        {
            get
            {
                return _activityReference;
            }

            set
            {
                if (_activityReference == value)
                {
                    return;
                }

                RaisePropertyChanging(ActivityReferencePropertyName);
                _activityReference = value;
                RaisePropertyChanged(ActivityReferencePropertyName);
            }
        }

        
        public const string ProductPropertyName = "Product";
        private string _product = "";
        public string Product
        {
            get
            {
                return _product;
            }

            set
            {
                if (_product == value)
                {
                    return;
                }

                RaisePropertyChanging(ProductPropertyName);
                _product = value;
                RaisePropertyChanged(ProductPropertyName);
            }
        }

        
        public const string QuantityPropertyName = "Quantity";
        private string _quantity = "";
        public string Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                if (_quantity == value)
                {
                    return;
                }

                RaisePropertyChanging(QuantityPropertyName);
                _quantity = value;
                RaisePropertyChanged(QuantityPropertyName);
            }
        }

        
        public const string ManufacturedDatePropertyName = "ManufacturedDate";
        private string _manufacturedDate = "";
        public string ManufacturedDate
        {
            get
            {
                return _manufacturedDate;
            }

            set
            {
                if (_manufacturedDate == value)
                {
                    return;
                }

                RaisePropertyChanging(ManufacturedDatePropertyName);
                _manufacturedDate = value;
                RaisePropertyChanged(ManufacturedDatePropertyName);
            }
        }

        
        public const string ExpiryDatePropertyName = "ExpiryDate";
        private string _expiryDate = "";
        public string ExpiryDate
        {
            get
            {
                return _expiryDate;
            }

            set
            {
                if (_expiryDate == value)
                {
                    return;
                }

                RaisePropertyChanging(ExpiryDatePropertyName);
                _expiryDate = value;
                RaisePropertyChanged(ExpiryDatePropertyName);
            }
        }

        
        public const string SerialNumberPropertyName = "SerialNumber";
        private string _serialNumber = "";
        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }

            set
            {
                if (_serialNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(SerialNumberPropertyName);
                _serialNumber = value;
                RaisePropertyChanged(SerialNumberPropertyName);
            }
        }

        
        public const string CommodityProducerServicePropertyName = "CommodityProducerService";
        private string _commodityProducerService = "";
        public string CommodityProducerService
        {
            get
            {
                return _commodityProducerService;
            }

            set
            {
                if (_commodityProducerService == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityProducerServicePropertyName);
                _commodityProducerService = value;
                RaisePropertyChanged(CommodityProducerServicePropertyName);
            }
        }

        
        public const string ServiceProviderPropertyName = "ServiceProvider";
        private string _serviceProvider = "";
        public string ServiceProvider
        {
            get
            {
                return _serviceProvider;
            }

            set
            {
                if (_serviceProvider == value)
                {
                    return;
                }

                RaisePropertyChanging(ServiceProviderPropertyName);
                _serviceProvider = value;
                RaisePropertyChanged(ServiceProviderPropertyName);
            }
        }

        
        public const string ShiftPropertyName = "Shift";
        private string _shift = "";
        public string Shift
        {
            get
            {
                return _shift;
            }

            set
            {
                if (_shift == value)
                {
                    return;
                }

                RaisePropertyChanging(ShiftPropertyName);
                _shift = value;
                RaisePropertyChanged(ShiftPropertyName);
            }
        }

        
        public const string CommodityPropertyName = "Commodity";
        private string _commodity = "";
        public string Commodity
        {
            get
            {
                return _commodity;
            }

            set
            {
                if (_commodity == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityPropertyName);
                _commodity = value;
                RaisePropertyChanged(CommodityPropertyName);
            }
        }

        
        public const string GradePropertyName = "Grade";
        private string _grade = "";
        public string Grade
        {
            get
            {
                return _grade;
            }

            set
            {
                if (_grade == value)
                {
                    return;
                }

                RaisePropertyChanging(GradePropertyName);
                _grade = value;
                RaisePropertyChanged(GradePropertyName);
            }
        }

        
        public const string WeightPropertyName = "Weight";
        private string _weight = "";
        public string Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                if (_weight == value)
                {
                    return;
                }

                RaisePropertyChanging(WeightPropertyName);
                _weight = value;
                RaisePropertyChanged(WeightPropertyName);
            }
        }

        
        public const string InfectionPropertyName = "Infection";
        private string  _infection = "";
        public string  Infection
        {
            get
            {
                return _infection;
            }

            set
            {
                if (_infection == value)
                {
                    return;
                }

                RaisePropertyChanging(InfectionPropertyName);
                _infection = value;
                RaisePropertyChanged(InfectionPropertyName);
            }
        }

        
        public const string RatePropertyName = "Rate";
        private string _rate = "";
        public string Rate
        {
            get
            {
                return _rate;
            }

            set
            {
                if (_rate == value)
                {
                    return;
                }

                RaisePropertyChanging(RatePropertyName);
                _rate = value;
                RaisePropertyChanged(RatePropertyName);
            }
        }

        #endregion

        public void SetActivity(DetailsPopUpMessage obj)
        {
            ActivityId = obj.ActivityId;
        }
    }
    public class VmActivityDetail
    {
        public string DocumentRef { get; set; }
        public string Hub { get; set; }
        public string FieldClerk { get; set; }
        public string Supplier { get; set; }
        public string Producer { get; set; }
        public string Route { get; set; }
        public string Center { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDate { get; set; }
        public string DocumentIssureCostCentreApplicationId { get; set; }
        public string Season { get; set; }
        public string ActivityReference { get; set; }

        public string Product { get; set; }
        public string Quantity { get; set; }
        public string ManufacturedDate { get; set; }
        public string ExpiryDate { get; set; }
        public string SerialNumber { get; set; }

        public string CommodityProducerService { get; set; }
        public string ServiceProvider { get; set; }
        public string Shift { get; set; }

        public string Commodity { get; set; }
        public string Grade { get; set; }
        public string Weight { get; set; }

        public string Infection { get; set; }
        public string Rate { get; set; }
        

    }
    public class VmInfectionLineItem
    {
        public string Infection { get; set; }
        public string Rate { get; set; }
    }
    public class VmInputLineItem
    {
        public string Product { get; set; }
        public string Quantity { get; set; }
        public string ManufacturedDate { get; set; }
        public string ExpiryDate { get; set; }
        public string SerialNumber { get; set; }
    }
    public class VmProduceLineItem
    {
        public string Commodity { get; set; }
        public string Grade { get; set; }
        public string Weight { get; set; }
        public string ServiceProvider { get; set; }
    }
    public class VmServiceLineItem
    {
        public string CommodityProducerService { get; set; }
        public string ServiceProvider { get; set; }
        public string Shift { get; set; }
    }
}
