using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using log4net;
using System.Windows.Controls;
using Distributr.Core.ClientApp;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityPurchase
{
    public class EditCommodityPurchaseViewModel : DistributrViewModelBase
    {
        
        private bool _isUsingWeighingContainerForStorage = false;
        private bool _enforceNoOfContainers = false;
       
        private ILog _logger = LogManager.GetLogger("App");
       
        public EditCommodityPurchaseViewModel()
        {
            LineItems = new ObservableCollection<CommodyLineItemViewModel>();
            FarmsList = new ObservableCollection<CommodityProducer>();
            CommodityList = new ObservableCollection<Commodity>();
            WeighingContainerList = new ObservableCollection<ContainerType>();
            StorageContainerList = new ObservableCollection<ContainerType>();
            GradeList = new ObservableCollection<CommodityGrade>();
            TransporterList = new ObservableCollection<Transporter>();
            ContainerSummaryList = new ObservableCollection<CommodyLineItemViewModel>();
            FarmCentresList= new ObservableCollection<Centre>();

            AddToContainerCommand = new RelayCommand<CommodyLineItemViewModel>(AddToCotainer);
            AddLineItemCommand = new RelayCommand(AddLineItem);
            WeighCommand = new RelayCommand(Weigh);

          
           
            EditPurchaseLoadedCommand = new RelayCommand(PageLoaded);
            SaveTransactionCommand = new RelayCommand(ConfirmPurchase);
            CommoditySelectionChangedCommand = new RelayCommand(CommoditySelectionChanged);
            DropDownOpenedCommand = new RelayCommand<object>(DropDownOpened);
            CancelPurchaseCommand = new RelayCommand(CancelPurchase);
            RemoveLineItemCommand = new RelayCommand<CommodyLineItemViewModel>(RemoveLineItem);

            RecalCulateWeight = new RelayCommand(RecalcNetWeight);
        }
       
        
        #region Properties

        public ObservableCollection<LocalEquipmentConfig> WeighScales { get; set; }
        public ObservableCollection<CommodyLineItemViewModel> LineItems { get; set; }
        public ObservableCollection<CommodityProducer> FarmsList { get; set; }
        public ObservableCollection<Centre> FarmCentresList { get; set; }
        public ObservableCollection<Commodity> CommodityList { get; set; }
        public ObservableCollection<ContainerType> WeighingContainerList { get; set; }
        public ObservableCollection<ContainerType> StorageContainerList { get; set; }
        public ObservableCollection<CommodityGrade> GradeList { get; set; }
        public ObservableCollection<Transporter> TransporterList { get; set; }
        public ObservableCollection<CommodyLineItemViewModel> ContainerSummaryList { get; set; }

        public RelayCommand<CommodyLineItemViewModel> AddToContainerCommand { get; set; }
        public RelayCommand AddLineItemCommand { get; set; }
        public RelayCommand<CommodyLineItemViewModel> RemoveLineItemCommand { get; set; }
        public RelayCommand WeighCommand { get; set; }
        public RelayCommand InitialiseWeighScalesCommand { get; set; }
        public RelayCommand EditPurchaseLoadedCommand { get; set; }
        public RelayCommand SaveTransactionCommand { get; set; }
        public RelayCommand CommoditySelectionChangedCommand { get; set; }
        public RelayCommand<object> DropDownOpenedCommand { get; set; }
        public RelayCommand CancelPurchaseCommand { get; set; }

        public RelayCommand RecalCulateWeight { get; set; }
        
        public const string DocumentReferencePropertyName = "DocumentReference";
        private string _documentReference = "";
        public string DocumentReference
        {
            get
            {
                return _documentReference;
            }

            set
            {
                if (_documentReference == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentReferencePropertyName);
                _documentReference = value;
                RaisePropertyChanged(DocumentReferencePropertyName);
            }
        }

        public const string ContainerNoPropertyName = "ContainerNo";
        private string _containerNo = "";
        public string ContainerNo
        {
            get
            {
                return _containerNo;
            }

            set
            {
                if (_containerNo == value)
                {
                    return;
                }
                _containerNo = value;
                RaisePropertyChanged(ContainerNoPropertyName);
            }
        }


        public const string FarmerIdPropertyName = "FarmerId";
        private Guid _farmerId = Guid.Empty;
        public Guid FarmerId
        {
            get
            {
                return _farmerId;
            }

            set
            {
                if (_farmerId == value)
                {
                    return;
                }

                _farmerId = value;
                RaisePropertyChanged(FarmerIdPropertyName);
            }
        }

        public const string DocumentIdPropertyName = "DocumentId";
        private Guid _documentId = Guid.Empty;
        public Guid DocumentId
        {
            get
            {
                return _documentId;
            }

            set
            {
                if (_documentId == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIdPropertyName);
                _documentId = value;
                RaisePropertyChanged(DocumentIdPropertyName);
            }
        }

        public const string FarmerPropertyName = "Farmer";
        private CommodityOwner _farmer = null;
        public CommodityOwner Farmer
        {
            get
            {
                return _farmer;
            }

            set
            {
                if (_farmer == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmerPropertyName);
                _farmer = value;
                RaisePropertyChanged(FarmerPropertyName);
            }
        }

        public const string SelectedFarmCentrePropertyName = "SelectedFarmCentre";
        private Centre _selectedFarmcentre = null;
        [MasterDataDropDownValidation]
        public Centre SelectedFarmCentre
        {
            get
            {
                return _selectedFarmcentre;
            }

            set
            {
                if (_selectedFarmcentre == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedFarmCentrePropertyName);
                _selectedFarmcentre = value;
                RaisePropertyChanged(SelectedFarmCentrePropertyName);
            }
        }
        public const string SelectedFarmPropertyName = "SelectedFarm";
        private CommodityProducer _selectedFarm = null;
        [MasterDataDropDownValidation]
        public CommodityProducer SelectedFarm
        {
            get
            {
                return _selectedFarm;
            }

            set
            {
                if (_selectedFarm == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedFarmPropertyName);
                _selectedFarm = value;
                RaisePropertyChanged(SelectedFarmPropertyName);
            }
        }
        

        public const string MakeWeightextBoxReadOnlyPropertyName = "MakeWeightextBoxReadOnly";
        private bool _makeWeightextBoxReadOnly =true;
        public bool MakeWeightextBoxReadOnly
        {
            get
            {
                return _makeWeightextBoxReadOnly;
            }

            set
            {
                if (_makeWeightextBoxReadOnly == value)
                {
                    return;
                }

                RaisePropertyChanging(MakeWeightextBoxReadOnlyPropertyName);
                _makeWeightextBoxReadOnly = value;
                RaisePropertyChanged(MakeWeightextBoxReadOnlyPropertyName);
            }
        }

        public const string CommoditySupplierPropertyName = "CommoditySupplier";
        private CommoditySupplier _commoditySupplier = null;
        public CommoditySupplier CommoditySupplier
        {
            get
            {
                return _commoditySupplier;
            }

            set
            {
                if (_commoditySupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(CommoditySupplierPropertyName);
                _commoditySupplier = value;
                RaisePropertyChanged(CommoditySupplierPropertyName);
            }
        }

        public const string DeliveredByPropertyName = "DeliveredBy";
        private string _deliveredBy = "";
        public string DeliveredBy
        {
            get
            {
                return _deliveredBy;
            }

            set
            {
                if (_deliveredBy == value)
                {
                    return;
                }

                RaisePropertyChanging(DeliveredByPropertyName);
                _deliveredBy = value;
                RaisePropertyChanged(DeliveredByPropertyName);
            }
        }

        public const string RecipientCostCentrePropertyName = "RecipientCostCentre";
        private CostCentre _reciptientCostCentre = null;
        public CostCentre RecipientCostCentre
        {
            get
            {
                return _reciptientCostCentre;
            }

            set
            {
                if (_reciptientCostCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(RecipientCostCentrePropertyName);
                _reciptientCostCentre = value;
                RaisePropertyChanged(RecipientCostCentrePropertyName);
            }
        }

        public const string OnBehalfOfCostCentrePropertyName = "OnBehalfOfCostCentre";
        private CostCentre _onBehalfOfCostCentre = null;
        public CostCentre OnBehalfOfCostCentre
        {
            get
            {
                return _onBehalfOfCostCentre;
            }

            set
            {
                if (_onBehalfOfCostCentre == value)
                {
                    return;
                }

                RaisePropertyChanging(OnBehalfOfCostCentrePropertyName);
                _onBehalfOfCostCentre = value;
                RaisePropertyChanged(OnBehalfOfCostCentrePropertyName);
            }
        }

        public const string DocumentIssuerUserPropertyName = "DocumentIssuerUser";
        private User _documentIssuerUser = null;
        public User DocumentIssuerUser
        {
            get
            {
                return _documentIssuerUser;
            }

            set
            {
                if (_documentIssuerUser == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIssuerUserPropertyName);
                _documentIssuerUser = value;
                RaisePropertyChanged(DocumentIssuerUserPropertyName);
            }
        }

        public const string DocumentParentIdPropertyName = "DocumentParentId";
        private Guid _documentParentId = Guid.Empty;
        public Guid DocumentParentId
        {
            get
            {
                return _documentParentId;
            }

            set
            {
                if (_documentParentId == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentParentIdPropertyName);
                _documentParentId = value;
                RaisePropertyChanged(DocumentParentIdPropertyName);
            }
        }

        public const string SelectedCommodityPropertyName = "SelectedCommodity";
        private Commodity _selectedCommodity = null;
        [MasterDataDropDownValidation]
        public Commodity SelectedCommodity
        {
            get
            {
                return _selectedCommodity;
            }

            set
            {
                if (_selectedCommodity == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityPropertyName);
                _selectedCommodity = value;
                RaisePropertyChanged(SelectedCommodityPropertyName);
               // CommoditySelectionChanged();
            }
        }

        public const string SelectedGradePropertyName = "SelectedGrade";
        private CommodityGrade _selectedGrade = null;
        [MasterDataDropDownValidation]
        public CommodityGrade SelectedGrade
        {
            get
            {
                return _selectedGrade;
            }

            set
            {
                if (_selectedGrade == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedGradePropertyName);
                _selectedGrade = value;
                RaisePropertyChanged(SelectedGradePropertyName);
                if (SelectedGrade !=null &&SelectedGrade.Id != Guid.Empty)
                {
                    LoadWeighingContainers();
                    LoadStorageContainers();
                }
                
            }
        }

        public const string SelectedWeighingContainerPropertyName = "SelectedWeighingContainer";
        private ContainerType _selectedWeighingContainer = null;
        [MasterDataDropDownValidation]
        public ContainerType SelectedWeighingContainer
        {
            get
            {
                return _selectedWeighingContainer;
            }

            set
            {
                if (_selectedWeighingContainer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedWeighingContainerPropertyName);
                _selectedWeighingContainer = value;
                RaisePropertyChanged(SelectedWeighingContainerPropertyName);
                if (SelectedWeighingContainer != null && SelectedWeighingContainer.Id != Guid.Empty)
                {
                  TareWeight = SelectedWeighingContainer.TareWeight * NoOfContainers;
                  RecalcNetWeight();
                }
            }
        }

        public const string SelectedStorageContainerPropertyName = "SelectedStorageContainer";
        private ContainerType _selectedStorageContainer = null;

        public ContainerType SelectedStorageContainer
        {
            get
            {
                return _selectedStorageContainer;
            }

            set
            {
                if (_selectedStorageContainer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedStorageContainerPropertyName);
                _selectedStorageContainer = value;
                RaisePropertyChanged(SelectedStorageContainerPropertyName);
            }
        }
        public const string NoOfContainersPropertyName = "NoOfContainers";
        private decimal _noOfContainers = 1;
        public decimal NoOfContainers
        {
            get
            {
                return _noOfContainers;
            }

            set
            {
                if (_noOfContainers == value)
                {
                    return;
                }

                RaisePropertyChanging(NoOfContainersPropertyName);
                _noOfContainers = value;
                RaisePropertyChanged(NoOfContainersPropertyName);
                if (_noOfContainers > 0m && SelectedWeighingContainer !=null)
                {
                    TareWeight = ((decimal)SelectedWeighingContainer.TareWeight * NoOfContainers);
                    RecalcNetWeight();
                }
               
            }
        }

        public const string SelectedTransporterPropertyName = "SelectedTransporter";
        private Transporter _selectedTranporter = null;
        public Transporter SelectedTransporter
        {
            get
            {
                return _selectedTranporter;
            }

            set
            {
                if (_selectedTranporter == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedTransporterPropertyName);
                _selectedTranporter = value;
                RaisePropertyChanged(SelectedTransporterPropertyName);
            }
        }

        public const string GrossWeightPropertyName = "GrossWeight";
        private decimal _grossWeight;
        public decimal GrossWeight
        {
            get
            {
                return _grossWeight;
            }

            set
            {
                if (_grossWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(GrossWeightPropertyName);
                _grossWeight = value;
                RaisePropertyChanged(GrossWeightPropertyName);
                //RecalcNetWeight();
            }
        }

        public const string TareWeightPropertyName = "TareWeight";
        private decimal _tareWeight = 0m;
        public decimal TareWeight
        {
            get
            {
                return _tareWeight;
            }

            set
            {
                if (_tareWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(TareWeightPropertyName);
                _tareWeight = value;
                RaisePropertyChanged(TareWeightPropertyName);
            }
        }

        public const string NetWeightPropertyName = "NetWeight";
        private decimal _netWeight = 0m;
        public decimal NetWeight
        {
            get
            {
                return _netWeight;
            }

            set
            {
                if (_netWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(NetWeightPropertyName);
                _netWeight = value;
                RaisePropertyChanged(NetWeightPropertyName);
            }
        }

        public const string TotalWeightPropertyName = "TotalWeight";
        private decimal _totalWeight = 0m;
        public decimal TotalWeight
        {
            get
            {
                return _totalWeight;
            }

            set
            {
                if (_totalWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(TotalWeightPropertyName);
                _totalWeight = value;
                RaisePropertyChanged(TotalWeightPropertyName);
            }
        }

        public const string DescriptionPropertyName = "Description";
        private string _description = "";
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

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        public const string DocumentIssuerCostCentrePropertyName = "DocumentIssuerCostCentre";
        private CostCentre _myProperty = null;
        public CostCentre DocumentIssuerCostCentre
        {
            get
            {
                return _myProperty;
            }

            set
            {
                if (_myProperty == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIssuerCostCentrePropertyName);
                _myProperty = value;
                RaisePropertyChanged(DocumentIssuerCostCentrePropertyName);
            }
        }

        public const string DocumentRecipientCostCentrePropertyName = "DocumentRecipientCostCentre";
        private CostCentre _docRecipientCC = null;
        public CostCentre DocumentRecipientCostCentre
        {
            get
            {
                return _docRecipientCC;
            }

            set
            {
                if (_docRecipientCC == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentRecipientCostCentrePropertyName);
                _docRecipientCC = value;
                RaisePropertyChanged(DocumentRecipientCostCentrePropertyName);
            }
        }

        public const string StorageContainerVisibilityPropertyName = "StorageContainerVisibility";
        private Visibility _StorageContainerVisibility = Visibility.Visible;
        public Visibility StorageContainerVisibility
        {
            get
            {
                return _StorageContainerVisibility;
            }

            set
            {
                if (_StorageContainerVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(StorageContainerVisibilityPropertyName);
                _StorageContainerVisibility = value;
                RaisePropertyChanged(StorageContainerVisibilityPropertyName);
            }
        }
        public const string ShowDeliveredByVisibilityPropertyName = "ShowDeliveredBy";
        private Visibility _showDeliveredBy = Visibility.Visible;
        public Visibility ShowDeliveredBy
        {
            get
            {
                return _showDeliveredBy;
            }

            set
            {
                if (_showDeliveredBy == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowDeliveredByVisibilityPropertyName);
                _showDeliveredBy = value;
                RaisePropertyChanged(ShowDeliveredByVisibilityPropertyName);
            }
        }

        public const string CommodityVisibilityPropertyName = "CommodityVisibility";
        private Visibility _commodityVisibility = Visibility.Visible;
        public Visibility CommodityVisibility
        {
            get
            {
                return _commodityVisibility;
            }

            set
            {
                if (_commodityVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityVisibilityPropertyName);
                _commodityVisibility = value;
                RaisePropertyChanged(CommodityVisibilityPropertyName);
            }
        }

        public const string CentreVisibilityPropertyName = "CentreVisibility";
        private Visibility _centreVisibility = Visibility.Visible;
        public Visibility CentreVisibility
        {
            get
            {
                return _centreVisibility;
            }

            set
            {
                if (_centreVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(CentreVisibilityPropertyName);
                _centreVisibility = value;
                RaisePropertyChanged(CentreVisibilityPropertyName);
            }
        }
        public const string FarmVisibilityPropertyName = "FarmVisibility";
        private Visibility _farmVisibility = Visibility.Visible;
        public Visibility FarmVisibility
        {
            get
            {
                return _farmVisibility;
            }

            set
            {
                if (_farmVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmVisibilityPropertyName);
                _farmVisibility = value;
                RaisePropertyChanged(FarmVisibilityPropertyName);
            }
        }

        public const string GradeVisibilityPropertyName = "GradeVisibility";
        private Visibility _gradeVisibility = Visibility.Visible;
        public Visibility GradeVisibility
        {
            get
            {
                return _gradeVisibility;
            }

            set
            {
                if (_gradeVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(GradeVisibilityPropertyName);
                _gradeVisibility = value;
                RaisePropertyChanged(GradeVisibilityPropertyName);
            }
        }

        public const string WeighingContainerVisibilityPropertyName = "WeighingContainerVisibility";
        private Visibility _weighingContainerVisibility = Visibility.Visible;
        public Visibility WeighingContainerVisibility
        {
            get
            {
                return _weighingContainerVisibility;
            }

            set
            {
                if (_weighingContainerVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(WeighingContainerVisibilityPropertyName);
                _weighingContainerVisibility = value;
                RaisePropertyChanged(WeighingContainerVisibilityPropertyName);
            }
        }

        public const string ShowNoOfContainersPropertyName = "ShowNoOfContainers";
        private Visibility _showNoOfContainers = Visibility.Visible;
        public Visibility ShowNoOfContainers
        {
            get
            {
                return _showNoOfContainers;
            }

            set
            {
                if (_showNoOfContainers == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowNoOfContainersPropertyName);
                _showNoOfContainers = value;
                RaisePropertyChanged(ShowNoOfContainersPropertyName);
            }
        }
        

        Commodity _defaultCommodity = null;
        private Commodity DefaultCommodity
        {
            get
            {
                return _defaultCommodity ??
                       (_defaultCommodity = new Commodity(Guid.Empty) { Name = "--Select commodity--" });
            }
        }

        CommodityProducer _defaultFarm = null;
        private CommodityProducer DefaultFarm
        {
            get
            {
                return _defaultFarm ??
                       (_defaultFarm = new CommodityProducer(Guid.Empty) { Name = "--Select farm--" });
            }
        }

        CommodityGrade _defaultGrade = null;
        private CommodityGrade DefaultGrade
        {
            get
            {
                return _defaultGrade ??
                       (_defaultGrade = new CommodityGrade(Guid.Empty) { Name = "--Select grade--" });
            }
        }

        ContainerType _defaultStorageContainer = null;
        private ContainerType DefaultStorageContainer
        {
            get
            {
                return _defaultStorageContainer ??
                       (_defaultStorageContainer = new ContainerType(Guid.Empty) { Name = "--Select storage container--" });
            }
        }

        ContainerType _defaultWeighingContainer = null;
        private ContainerType DefaultWeighingContainer
        {
            get
            {
                return _defaultWeighingContainer ??
                       (_defaultWeighingContainer = new ContainerType(Guid.Empty) { Name = "--Select weighing container--" });
            }
        }



        #endregion

        #region Methods

        
        public void SetId(ViewModelMessage obj)
        {
            FarmerId = obj.Id;

        }
        private void RemoveLineItem(CommodyLineItemViewModel item)
        {
            if (LineItems.Any(p => p.Id == item.Id))
            try
            {
               
                if (
                   MessageBox.Show("Are you sure you want to remove " + item.Description,
                                   "Agrimanagr: Commodity Purchase", MessageBoxButton.YesNo,
                                   MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    LineItems.Remove(item);
                    if (ContainerSummaryList.Any(p => p.Id == item.Id))
                        ContainerSummaryList.Remove(item);

               
                }
            }
            catch(Exception e)
            {
            }
           
              
        }
        private void PageLoaded()
        {
            SetUp();
            Load();
        }
        void ClearViewModel()
        {
            
            DocumentReference = string.Empty;
            DeliveredBy = string.Empty;
            DocumentId = Guid.Empty;
            SelectedFarm = null;
            ContainerNo = string.Empty;
            CommoditySupplier = null;
            OnBehalfOfCostCentre = null;
            DocumentIssuerUser = null;
            DocumentParentId = Guid.Empty;
            SelectedCommodity = null;
            SelectedGrade = null;
            SelectedWeighingContainer = null;
            SelectedStorageContainer = null;
            SelectedTransporter = null;
            GrossWeight = 0m;
            NetWeight = 0m;
            TotalWeight = 0m;
            NoOfContainers = 1;
            Description = string.Empty;
            DocumentIssuerCostCentre = null;
            DocumentRecipientCostCentre = null;
            TareWeight = 0m;
            LineItems.Clear();
            FarmsList.Clear();
            FarmCentresList.Clear();
            CommodityList.Clear();
            WeighingContainerList.Clear();
            StorageContainerList.Clear();
            GradeList.Clear();
            TransporterList.Clear();
            ContainerSummaryList.Clear();
            MakeWeightextBoxReadOnly = false;
            _isUsingWeighingContainerForStorage = false;
        }

        void ResetAfterAddLineItem()
        {
            var grade = GradeList.FirstOrDefault(n => n.Id != Guid.Empty);
            SelectedGrade = grade ?? DefaultGrade;
            SelectedStorageContainer = DefaultStorageContainer;
            SelectedWeighingContainer = DefaultWeighingContainer;
            GrossWeight = 0m;
            NetWeight = 0m;
            NoOfContainers = 1;
            ContainerNo = string.Empty;
        }


        void ResetAfterPurchase()
        {
            ResetAfterAddLineItem();
            DocumentReference = GetDocumentReference("PurchaseNote");
            LineItems.Clear();
            ContainerSummaryList.Clear();
            SelectedCommodity = DefaultCommodity;
            SelectedStorageContainer = DefaultStorageContainer;
            SelectedWeighingContainer = DefaultWeighingContainer;
           
        }

        void SetUp()
        {
            ClearViewModel();
            DocumentId = Guid.NewGuid();
            using (StructureMap.IContainer cont = NestedContainer)
            {
                IConfigService configService = Using<IConfigService>(cont);
                var _costCentreRepo = Using<ICostCentreRepository>(cont);
                DocumentIssuerUser = Using<IUserRepository>(cont).GetById(configService.ViewModelParameters.CurrentUserId);
                DocumentIssuerCostCentre = _costCentreRepo.GetById(GetConfigParams().CostCentreId);
                DocumentRecipientCostCentre = DocumentIssuerCostCentre;
                DocumentReference = GetDocumentReference("PurchaseNote");
                GradeVisibility = Visibility.Visible;
                FarmVisibility = Visibility.Visible;
                StorageContainerVisibility = Visibility.Visible;
                
                GetSettings();
            }

            LoadDefaults();
         
            if (WeighConfiguration.Load() == null && MakeWeightextBoxReadOnly)
            {
                MessageBox.Show("No Weigh Scale device configured", "Agrimanagr Warning");
            }
               
        }

        private void GetSettings()
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                var settingsRepository = Using<ISettingsRepository>(cont);
                var allowManualEntryOfWeight = false;
                var setting = settingsRepository.GetByKey(SettingsKeys.AllowManualEntryOfWeight);
                if (setting != null && !string.IsNullOrEmpty(setting.Value))
                    Boolean.TryParse(setting.Value, out allowManualEntryOfWeight);
                MakeWeightextBoxReadOnly = !allowManualEntryOfWeight;

                _isUsingWeighingContainerForStorage = false;
                var weighingContainerForStorage = settingsRepository.GetByKey(SettingsKeys.WeighingContainerForStorage);
                if (weighingContainerForStorage != null && !string.IsNullOrEmpty(weighingContainerForStorage.Value))
                    Boolean.TryParse(weighingContainerForStorage.Value, out _isUsingWeighingContainerForStorage);

               

                var showDeliveredBy = true;
                var showDeliveredByValue = settingsRepository.GetByKey(SettingsKeys.ShowDeliveredBy);
                if (showDeliveredByValue != null && !string.IsNullOrEmpty(showDeliveredByValue.Value))
                    Boolean.TryParse(showDeliveredByValue.Value, out showDeliveredBy);

                var enforceNoofContainers = settingsRepository.GetByKey(SettingsKeys.EnforceNoOfContainers);
                if (enforceNoofContainers != null && !string.IsNullOrEmpty(enforceNoofContainers.Value))
                    Boolean.TryParse(enforceNoofContainers.Value, out _enforceNoOfContainers);
                
                StorageContainerVisibility = _isUsingWeighingContainerForStorage ? Visibility.Collapsed : Visibility.Visible;
                ShowDeliveredBy = showDeliveredBy ? Visibility.Visible : Visibility.Collapsed;
                ShowNoOfContainers = _enforceNoOfContainers ? Visibility.Visible : Visibility.Collapsed;
            }
        }

      
        void LoadDefaults(string entity = "")
        {
            if (entity == "" || entity == "farm")
            {
                FarmsList.Clear();
                FarmsList.Add(DefaultFarm);
                SelectedFarm = DefaultFarm;
            }
            if (entity == "" || entity == "commodity")
            {
                CommodityList.Clear();
                CommodityList.Add(DefaultCommodity);
                SelectedCommodity = DefaultCommodity;
            }
            if (entity == "" || entity == "grade")
            {
                GradeList.Clear();
                GradeList.Add(DefaultGrade);
                SelectedGrade = DefaultGrade;
            }
            if (entity == "" || entity == "weighingcontainer")
            {
                WeighingContainerList.Clear();
                WeighingContainerList.Add(DefaultWeighingContainer);
                SelectedWeighingContainer = DefaultWeighingContainer;
            }
            if (entity == "" || entity == "storagecontainer")
            {
                StorageContainerList.Clear();
                StorageContainerList.Add(DefaultStorageContainer);
                SelectedStorageContainer = DefaultStorageContainer;
            }
           
        }

        public void Load()
        {
            using (var c = NestedContainer)
            {
                Farmer = Using<ICommodityOwnerRepository>(c).GetById(FarmerId);

                CommoditySupplier = Farmer.CommoditySupplier;
                LoadFarms();
                LoadCommodities();
                LoadWeighingContainers();
                LoadStorageContainers();

            }

        }
     
        void LoadFarms()
        {
            FarmVisibility = Visibility.Collapsed;
            CentreVisibility=Visibility.Collapsed;
            if (CommoditySupplier == null || CommoditySupplier.Id == Guid.Empty)
                return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                var farm = Using<ICommodityProducerRepository>(c).GetAll().Where(n => n.CommoditySupplier.Id == CommoditySupplier.Id).ToList();
                farm.OrderBy(n => n.Name).ThenBy(n => n.Code).ToList()
                    .ForEach(n => { if (FarmsList.Select(q => q.Id).All(p => p != n.Id)) FarmsList.Add(n); });
                if (FarmsList.Count(n => n.Id != Guid.Empty) == 1)
                {
                    SelectedFarm = FarmsList.FirstOrDefault(n => n.Id != Guid.Empty);
                    FarmVisibility = Visibility.Collapsed;
                    LoadFarmCentres();
                }
                if (FarmsList.Count(n => n.Id != Guid.Empty)>1)
                {
                    FarmVisibility = Visibility.Visible;
                    FarmsList.Add(DefaultFarm);
                    SelectedFarm = DefaultFarm;
                }
               
            }
        }

        private void LoadCommodities()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {

                var commodity = Using<ICommodityRepository>(c).GetAll().OrderBy(n => n.Name).ThenBy(n => n.Code).ToList();
                commodity.ForEach(n => { if (CommodityList.Select(q => q.Id).All(p => p != n.Id)) CommodityList.Add(n); });

                if (CommodityList.Count(p => p.Id != Guid.Empty) == 1)
                {
                    SelectedCommodity = CommodityList.FirstOrDefault(p => p.Id != Guid.Empty);
                    CommodityVisibility = Visibility.Collapsed;
                }
                else
                {
                    CommodityVisibility = Visibility.Visible;
                }

            }
           LoadGrades();
        }

        

        private void DropDownOpened(object sender)
        {
            var comboBox = sender as ComboBox;
            switch (comboBox.Name)
            {
                case "cmbCommodity":
                    using (var container = NestedContainer)
                    {
                        SelectedCommodity = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as Commodity;
                        LoadGrades();
                    }
                    break;
                case "cmbGrade":
                    using (var container = NestedContainer)
                    {
                        if (SelectedCommodity==null)
                        {
                            MessageBox.Show("Select a commody first");
                            return;
                        }
                        LoadGrades();
                        SelectedGrade = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as CommodityGrade;

                    }
                    break;
                case "cmbWeighingContainer":
                    using (var container = NestedContainer)
                    {
                        SelectedWeighingContainer = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as ContainerType;

                    }
                    break;
                case "cmbStorageContainer":
                    using (var container = NestedContainer)
                    {
                        SelectedStorageContainer = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as ContainerType;

                    }
                    break;
                case "cmbTransporter":
                    using (var container = NestedContainer)
                    {
                        SelectedTransporter = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as Transporter;

                    }
                    break;
                case "cmbFarm":
                    using (var container = NestedContainer)
                    {
                        SelectedFarm = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as CommodityProducer;
                        LoadFarmCentres();
                    }
                    break;
                case "cmbCentre":
                    using (var container = NestedContainer)
                    {
                        SelectedFarmCentre = Using<IAgrimanagrComboPopUp>(container).ShowDlg(sender) as Centre;

                    }
                    break;
                    
            }
            
        }

        private void LoadFarmCentres()
        {
            if (FarmCentresList.Any())
                FarmCentresList.Clear();
            if (SelectedFarm != null)
            {
                foreach (var centre in SelectedFarm.CommodityProducerCentres)
                {
                    FarmCentresList.Add(centre);

                }
                if (FarmCentresList.Count(n => n != null && n.Id != Guid.Empty) == 1)
                {
                    SelectedFarmCentre = FarmCentresList.FirstOrDefault(n => n.Id != Guid.Empty);
                    CentreVisibility = Visibility.Collapsed;
                }
                if (FarmCentresList.Count(n => n != null && n.Id != Guid.Empty) > 1)
                {
                    CentreVisibility = Visibility.Visible;
                    SelectedFarmCentre = FarmCentresList.FirstOrDefault();
                  
                }
                
           }
        }

        private void CommoditySelectionChanged()
        {
            if (SelectedCommodity == null)
                return;
            LoadGrades();
        }

       private void LoadGrades()
        {
            if (SelectedCommodity == null || SelectedCommodity.Id == Guid.Empty)
                return;
            GradeList.Clear();
            GradeList.Add(DefaultGrade);
          var grades= SelectedCommodity.CommodityGrades.Where(p => p._Status == EntityStatus.Active)
               .OrderBy(n => n.Name)
               .ThenBy(n => n.Code)
               .ToList();
           grades.ForEach(n => { if (GradeList.Select(q => q.Id).All(p => p != n.Id)) GradeList.Add(n); });
            if (GradeList.Count(n => n.Id != Guid.Empty) == 0)
            {
                GradeVisibility = Visibility.Visible;

                GradeList.Add(DefaultGrade);
                SelectedGrade = DefaultGrade;
            }
            else if (GradeList.Count(n => n.Id != Guid.Empty) == 1)
            {
                GradeVisibility = Visibility.Collapsed;
                SelectedGrade = GradeList.FirstOrDefault(n => n.Id != Guid.Empty);
            }
            else
            {
                GradeVisibility = Visibility.Visible;
                SelectedGrade = DefaultGrade;
            }
        }

       private void LoadWeighingContainers()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                WeighingContainerList.Clear();
                WeighingContainerList.Add(DefaultWeighingContainer);
                SelectedWeighingContainer = DefaultWeighingContainer;
                IEnumerable<ContainerType> weighContainers =
                    Using<IContainerTypeRepository>(c).GetAll().Where(
                        p =>(p.ContainerUseType == ContainerUseType.WeighingContainer ||
                         p.ContainerUseType == ContainerUseType.Unknown));

                var containerswithNoGrades=weighContainers.ToList().Where(p => p.CommodityGrade == null);
                if(SelectedGrade !=null && SelectedGrade.Id !=Guid.Empty)
                {
                    weighContainers = weighContainers.Where(p => p.CommodityGrade != null && p.CommodityGrade.Id == SelectedGrade.Id );

                }
               weighContainers=weighContainers.Union(containerswithNoGrades);
                weighContainers.OrderBy(n => n.Name).ThenBy(n => n.Code).ToList()
                    .ForEach(n =>
                                 {
                                     if (WeighingContainerList.Select(q => q.Id).All(p => p != n.Id))
                                         WeighingContainerList.Add(n);
                                 });
            }

            if (WeighingContainerList.Count(n => n.Id != Guid.Empty) == 1)
            {
                WeighingContainerVisibility = Visibility.Collapsed;
                SelectedWeighingContainer = WeighingContainerList.FirstOrDefault(n => n.Id != Guid.Empty);
            }
           
        }

        void LoadStorageContainers()
        {
            if (StorageContainerVisibility !=Visibility.Visible)return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                StorageContainerList.Clear();
                StorageContainerList.Add(DefaultStorageContainer);
                SelectedStorageContainer = DefaultStorageContainer;
                IEnumerable<ContainerType> sconts =
                   Using<IContainerTypeRepository>(c).GetAll().Where(
                       p => (p.ContainerUseType == ContainerUseType.StorageContainer ||
                        p.ContainerUseType == ContainerUseType.Unknown));

                var containerswithNoGrades = sconts.ToList().Where(p => p.CommodityGrade == null);

                if (SelectedGrade != null && SelectedGrade.Id != Guid.Empty)
                {
                    sconts = sconts.Where(p => p.CommodityGrade != null && p.CommodityGrade.Id == SelectedGrade.Id);

                }
                sconts = sconts.Union(containerswithNoGrades);
                
                sconts.OrderBy(n => n.Name).ThenBy(n => n.Code).ToList().ForEach(n =>
                                                                                     {
                                                                                         if (
                                                                                             StorageContainerList.Select
                                                                                                 (q => q.Id).All(
                                                                                                     p => p != n.Id))
                                                                                             StorageContainerList.Add(n);
                                                                                     });
            }

            if (StorageContainerList.Count(n => n.Id != Guid.Empty) == 1)
            {
                StorageContainerVisibility = Visibility.Collapsed;
                SelectedStorageContainer = StorageContainerList.FirstOrDefault(n => n.Id != Guid.Empty);
            }
        }

        void CalcTotalWeight()
        {
            TotalWeight = LineItems.Sum(n => (n.NetWeight));
        }

        void RecalcNetWeight()
        {
            var tare = SelectedWeighingContainer == null ? 0m :SelectedWeighingContainer==null?0m: SelectedWeighingContainer.TareWeight;
            TareWeight = ((decimal)tare * NoOfContainers);
           NetWeight = TruncateDecimal((GrossWeight - TareWeight) < 0 ? 0m : (GrossWeight - TareWeight), 1);
        }

        private void PreviewInput(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            //e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
        private void AddLineItem()
        {
            try
            {
               if (!IsValid())
                    return;
              
               var container = _isUsingWeighingContainerForStorage
                                  ? SelectedWeighingContainer
                                  : SelectedStorageContainer;

               if ((container == null && !_isUsingWeighingContainerForStorage) || (container !=null && container.Id == Guid.Empty))
                {
                    MessageBox.Show("Storage Container is required",
                                   "Agrimanagr Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (NetWeight <= 0)
                {
                    MessageBox.Show("A commodity with zero weight cannot be added!",
                                    "Agrimanagr Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (_enforceNoOfContainers && NoOfContainers<1)
                {
                    MessageBox.Show("No of containers is required",
                                    "Agrimanagr Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (SelectedWeighingContainer.Id == Guid.Empty)
                {
                    MessageBox.Show("Select weighing container", "Agrimanagr Info",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                var lineItem = new CommodyLineItemViewModel
                {
                    Id = Guid.NewGuid(),
                    GrossWeight = GrossWeight,
                    NetWeight = NetWeight,
                    ContainerType = container,
                    Commodity = SelectedCommodity,
                    CommodityGrade = SelectedGrade,
                    ContainerNo = ContainerNo,
                    Description = SelectedCommodity.Name,
                    Note = "",
                    NoOfContainers = NoOfContainers,
                    TareWeight = TareWeight

                };

                LineItems.Add(lineItem);
                AddToCotainer(lineItem);

                CalcTotalWeight();
                ResetAfterAddLineItem();

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured while adding line item. " + ex.Message, "Agrimanagr: Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
       

        private bool Validate()
        {
            var isValid = true;
            if (LineItems.Count < 1)
            {
                MessageBox.Show("You must add at least one line item", "Agrimanagr : Purchase Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid= false;
            }
            if (TotalWeight <= 0)
            {
                MessageBox.Show("Net weight must be greater than 0", "Agrimanagr : Purchase Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid= false;
            }
           

            return isValid;
        }

        private void CancelPurchase()
        {
            if (
                MessageBox.Show("Are you sure you want to cancel this transaction?\nUnsaved changes will be lost",
                                "Agrimanagr: Purchase Commodity", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                MessageBoxResult.Yes)
            {
                string url = "/views/CommodityPurchase/ListFarmers.xaml";
                NavigateCommand.Execute(url);
            }
        }

        private void ConfirmPurchase()
       {
           string msg = "";
           try
           {
              if(Validate())
              {
                  var purchaseNote = Purchase();

                  msg = "Purchase successfully made and received. Transaction number: " +
                        DocumentReference;
                  MessageBox.Show(msg, "Agrimanagr: Purchase " + DocumentReference, MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                  using (var c = NestedContainer)
                  {
                      Using<IReceiptDocumentPopUp>(c).ShowReceipt(purchaseNote);

                      ResetAfterPurchase();
                      string url = @"/views/CommodityPurchase/ListFarmers.xaml";
                      NavigateCommand.Execute(url);
                  }
              }
           }
           catch (Exception e)
           {
               msg = "Error occured while saving transaction.\n" + e.Message +
                     (e.InnerException == null ? "" : e.InnerException.Message);
               MessageBox.Show(msg, "Agrimanagr: Purchase Commodity", MessageBoxButton.OK, MessageBoxImage.Error);
               _logger.Info("Commodity Storage: " + msg);

           }
       }
       
        private CommodityPurchaseNote Purchase()
        {
            //1. create purchase note n submit
            //2.create reception note n submit
            using (StructureMap.IContainer c = NestedContainer)
            {
                var configService = Using<IConfigService>(c);
                Config config = configService.Load();
                Guid costCentreApplicationid = config.CostCentreApplicationId;

                DateTime now = DateTime.Now;
                var purchaseWfManager = Using<ICommodityPurchaseWFManager>(c);
                var purchaseNoteFactory = Using<ICommodityPurchaseNoteFactory>(c);

                var commodityReceptionNoteFactory = Using<ICommodityReceptionNoteFactory>(c);
                var commodityReceptionWfManager = Using<ICommodityReceptionWFManager>(c);
                
                var purchaseNote = purchaseNoteFactory.Create(DocumentIssuerCostCentre,
                                                                                costCentreApplicationid,
                                                                                DocumentRecipientCostCentre,
                                                                                DocumentIssuerUser,
                                                                                DocumentReference, Guid.Empty,
                                                                                DeliveredBy, SelectedFarm,
                                                                                CommoditySupplier, Farmer, now, now,
                                                                                Description);
                if(SelectedFarmCentre !=null)
                {
                    if (SelectedFarmCentre.Route != null)
                        purchaseNote.RouteId = SelectedFarmCentre.Route.Id;
                    purchaseNote.CentreId = SelectedFarmCentre.Id;
                }

                int batchNo = 0;
                foreach (var item in LineItems)
                {
                    string containerNo = item.ContainerNo;
                    if (string.IsNullOrWhiteSpace(item.ContainerNo))
                    {
                        batchNo++;
                         containerNo = purchaseNote.DocumentReference + "_" + batchNo;
                    }

                    var lineItem = purchaseNoteFactory.CreateLineItem(item.ParentLineItemId, item.Commodity.Id,
                                                                      item.CommodityGrade.Id, item.ContainerType.Id,
                                                                      containerNo, item.NetWeight, item.Description, item.NoOfContainers, item.TareWeight);
                    purchaseNote.AddLineItem(lineItem);
                }
                purchaseNote.Confirm();
                purchaseWfManager.SubmitChanges(purchaseNote);
                AddLogEntry("Commodity Purchase", "Created commodity purchase note " + DocumentReference);
                //create reception note
                if (purchaseNote == null)
                    throw new Exception("Purchase Note not created");

                var commodityReceptionNote = commodityReceptionNoteFactory
                    .Create(purchaseNote.DocumentIssuerCostCentre,
                            purchaseNote.DocumentIssuerCostCentreApplicationId,
                            purchaseNote.DocumentRecipientCostCentre,
                            purchaseNote.DocumentIssuerUser,
                            GetDocumentReference("ReceptionNote"), purchaseNote.Id, now, now,
                            purchaseNote.VehicleArrivalTime, purchaseNote.VehicleDepartureTime,
                            purchaseNote.VehicleArrivalMileage, purchaseNote.VehicleDepartureMileage,
                            "Commodity Reception Note");

               foreach (var item in purchaseNote.LineItems)
               {
                   var lineitem = commodityReceptionNoteFactory.CreateLineItem(item.Id, item.Commodity.Id,
                                                                               item.CommodityGrade.Id,
                                                                               item.ContainerType.Id, item.ContainerNo,
                                                                               item.Weight, item.Commodity.Name);
                   commodityReceptionNote.AddLineItem(lineitem);

               }
                commodityReceptionNote.Confirm();
                commodityReceptionWfManager.SubmitChanges(commodityReceptionNote);
                AddLogEntry("Commodity Purchase", "Created commodity reception note " + commodityReceptionNote.DocumentReference);

                //we create received delivery note for this purchase and submit to workflow
                if (purchaseNote.LineItems.Any())
                {
                    var factory = Using<IReceivedDeliveryNoteFactory>(c);

                    var receivedDeliveryNote = factory.Create(purchaseNote.DocumentIssuerCostCentre,purchaseNote.DocumentIssuerCostCentre,
                                                              purchaseNote.DocumentIssuerCostCentreApplicationId,
                                                              purchaseNote.DocumentIssuerUser,
                                                              "RD_" + purchaseNote.DocumentReference, purchaseNote.Id,
                                                              DateTime.Now,
                                                              DateTime.Now, null,null,0m,0m, purchaseNote.Description);
                    receivedDeliveryNote.CentreId = purchaseNote.CentreId;
                    receivedDeliveryNote.RouteId = purchaseNote.RouteId;
                    
                    foreach (var item in purchaseNote.LineItems)
                    {
                        var lineitem = factory.CreateLineItem(Guid.Empty,
                                                              item.CommodityGrade != null ? item.CommodityGrade.Id : Guid.Empty,
                                                              item.ContainerNo,item.Weight,item.Weight,item.Description);
                        lineitem.ContainerType = item.ContainerType;
                        lineitem.Commodity = item.Commodity;
                        receivedDeliveryNote.AddLineItem(lineitem);
                    }
                    receivedDeliveryNote.Confirm();
                    var receivedworkflow = Using<IReceivedDeliveryWorkflow>(c);
                    receivedworkflow.SubmitChanges(receivedDeliveryNote);
                }

                return purchaseNote;
            }
             
        }
       

        private void AddToCotainer(CommodyLineItemViewModel item)
        {
            if (!IsValid())
                return;
            ContainerSummaryList.Add(item);
        }

        string GetDocumentReference(string docRef)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                docRef = Using<IGetDocumentReference>(cont)
                    .GetDocReference(docRef, DocumentIssuerCostCentre.Id,
                                     DocumentIssuerCostCentre.Id);
            }
            return docRef;
        }
        #region Weigh utils

        void Weigh()
        {
            if (SelectedWeighingContainer.Id == Guid.Empty)
            {
                MessageBox.Show("Select weighing container", "Agrimanagr Info",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var scale = WeighConfiguration.Load();
            string msg = "";

            if (scale == null)
            {
                MessageBox.Show("No weigh scale device not found ,Please Setup Device first", "Agrimanagr Info",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (SerialPortHelper.IsDeviceReady)
            {

                GrossWeight = SerialPortHelper.Read();
              

            }
            else if (SerialPortHelper.Init(out msg, scale.WeighScaleType, scale.Port,
                scale.BaudRate, scale.DataBits))
            {
                GrossWeight = SerialPortHelper.Read();
             
            }
            else
            {
                MessageBox.Show(msg, "Agrimanagr Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

    
        #endregion


        #endregion



     
    }
}
