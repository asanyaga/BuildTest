using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Utility.Validation;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CommodityReception
{
   
    public class WeighReceiveDeliveryViewModelPopUp : DistributrViewModelBase
    {
        public ObservableCollection<WeighReceiveItem> LineItem { get; set; }
        public ObservableCollection<ContainerLookUp> ContainerLookUpList { get; set; }
        public ObservableCollection<ContainerType> ContainerTypeLookUpList { get; set; }
        public ObservableCollection<Commodity> CommodityLookUpList { get; set; }
        public ObservableCollection<CommodityGrade> GradeLookUpList { get; set; }
       
        public RelayCommand<TextCompositionEventArgs> ValidNumericInputCommand { get; set; }
        public RelayCommand SelectedCommodityChangedCommand { get; set; }
        public RelayCommand SelectedGradeChangedCommand { get; set; }
        public RelayCommand WeighCommand { get; set; }
        public RelayCommand AddWeightCommand { get; set; }
        public RelayCommand SaveWeightCommand { get; set; }
        public RelayCommand CompleteCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand ItemSelectionChangedCommand { get; set; }
        private bool _commodityReweighed = false;
        private bool _userChangedGrossWeightValue { get; set; }
          
        public event EventHandler RequestClose = (s, e) => { };

       // public SerialPortUtil _weighScaleDevice = null;


        public WeighReceiveDeliveryViewModelPopUp()
        {
            LineItem = new ObservableCollection<WeighReceiveItem>();
            WeighCommand = new RelayCommand(Weigh);
            AddWeightCommand = new RelayCommand(AddWeight);
            SaveWeightCommand = new RelayCommand(SaveWeight);
            CompleteCommand = new RelayCommand(Complete);
            CancelCommand = new RelayCommand(Cancel);
            ContainerLookUpList = new ObservableCollection<ContainerLookUp>();
            GradeLookUpList = new ObservableCollection<CommodityGrade>();
            CommodityLookUpList=new ObservableCollection<Commodity>();
            ContainerTypeLookUpList = new ObservableCollection<ContainerType>();
            ValidNumericInputCommand = new RelayCommand<TextCompositionEventArgs>(ValidNumericInput);
      
            SelectedCommodityChangedCommand = new RelayCommand(SelectedCommodityChanged);
            SelectedGradeChangedCommand = new RelayCommand(SelectedGradeChanged);
            ItemSelectionChangedCommand = new RelayCommand(ItemSelectionChanged);
        }

        private void ItemSelectionChanged()
        {
           CalculateSelectedDeliveredWeight();
        }

        private void SelectedGradeChanged()
        {
            GradeTo = null;
            foreach (var containerLookUp in ContainerLookUpList.ToList())
            {
                containerLookUp.IsCheckable = false;
                containerLookUp.IsChecked = false;
            }
            if(Grade!=null && Grade.Id!=Guid.Empty)
            {
                int counter = 1;
                foreach (var containerLookUp in ContainerLookUpList.Where(s => s.CommodityGrade.Id == Grade.Id).ToList())
                {
                    containerLookUp.IsCheckable = true;
                    containerLookUp.IsChecked = true;
                    if(counter==ItemToWeigh)
                        break;
                    counter++;
                }
               
            }
            var ordereditem = ContainerLookUpList.OrderByDescending(s => s.IsChecked).ToList();
            ContainerLookUpList.Clear();
            ordereditem.ForEach(s=>ContainerLookUpList.Add(s));

            CalculateSelectedDeliveredWeight();

        }
        private void CalculateSelectedDeliveredWeight()
        {
            var itemWeight = ContainerLookUpList.Where(s => s.IsChecked && s.IsCheckable).Sum(s => s.Weight);
            var containerWeight =
                ContainerLookUpList.Where(s => s.IsChecked && s.IsCheckable).Sum(s => s.ContainerType.TareWeight);
            decimal deliveredweight = ContainerLookUpList.Where(s => s.IsChecked && s.IsCheckable).Sum(s => s.Weight)
                 + ContainerLookUpList.Where(s => s.IsChecked && s.IsCheckable).Sum(s => s.ContainerType.TareWeight);
            GrossWeight = deliveredweight;
            DeliveredWeight = deliveredweight;
        }


        private void SelectedCommodityChanged()
        {
            LoadGrade(); 
        }

        private void ValidNumericInput(TextCompositionEventArgs e)
        {
            _userChangedGrossWeightValue = true;
            e.Handled = !IsTextAllowed(e.Text);
        }

        public static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }


        public void SetUp(Guid documentId)
        {
            ClearViewModel();
            DocumentId = documentId;
            LoadDelivery();
        
            using (var c=NestedContainer)
            {
                bool enforceReWeigh;
                var settingsRepository = Using<ISettingsRepository>(c);
                var value = settingsRepository.GetByKey(SettingsKeys.EnforceWeighingOnReception);
                if (value != null)
                {
                    var enforce=value.Value;
                   
                    bool.TryParse(enforce,out enforceReWeigh);
                    EnforceWeighingOnReception =enforceReWeigh;
                }
                var allowManualEntryOfWeight = false;
                var setting = settingsRepository.GetByKey(SettingsKeys.AllowManualEntryOfWeight);
                if (setting != null && !string.IsNullOrEmpty(setting.Value))
                    Boolean.TryParse(setting.Value, out allowManualEntryOfWeight);
                MakeWeightextBoxReadOnly = !allowManualEntryOfWeight;

                ShowVehiclMileageTracker = EnforceVehicleMileageAndTime ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool EnforceVehicleMileageAndTime
        {
            get
            {
                using (var c =NestedContainer)
                {
                    var showvehiclemileagepanel = false;
                    var showvehiclemileage = Using<ISettingsRepository>(c).GetByKey(SettingsKeys.EnforceVehicleMileageAndTime);
                    if (showvehiclemileage != null && !string.IsNullOrEmpty(showvehiclemileage.Value))
                        Boolean.TryParse(showvehiclemileage.Value, out showvehiclemileagepanel);

                    return showvehiclemileagepanel;
                }
               
            }
        }

        private void LoadDelivery()
        {
            using(var c = NestedContainer)
            {
                var note = Using<ICommodityDeliveryRepository>(c).GetById(DocumentId) as CommodityDeliveryNote;
               
                DeliveryNo = note.DocumentReference;
                DeliveryBy = note.DriverName;
                VehicleNo = note.VehiclRegNo;
                NoOfContainer = note.LineItems.Where(s => s.LineItemStatus == SourcingLineItemStatus.Confirmed).GroupBy(s => s.ContainerNo).Count();
                ContainerLookUpList.Clear();
                
                foreach(var item in note.LineItems.Where(s=>s.LineItemStatus==SourcingLineItemStatus.Confirmed))
                {
                    var containerItem = new ContainerLookUp
                    {
                        ContainerNo = item.ContainerNo,
                        Weight = item.Weight,
                        Id = item.ContainerNo,
                        Commodity = item.Commodity,
                        ContainerType = item.ContainerType,
                        CommodityGrade = item.CommodityGrade,
                        IsChecked = false,
                        TareWeight = item.ContainerType.TareWeight,
                        GrossWeight = item.ContainerType.TareWeight + item.Weight
                    };
                    ContainerLookUpList.Add(containerItem);
                }
               
            }
            LoadCommodity();
            LoadContainerType();
        }

        private void LoadCommodity()
        {
            CommodityLookUpList.Clear();
            CommodityLookUpList.Add(new Commodity(Guid.Empty)
            {
                Name = "--Select Commodity--",
            });
            using (var c = NestedContainer)
            {
                foreach (var item in Using<ICommodityRepository>(c).GetAll().OrderBy(s=>s.Name))
                {
                    CommodityLookUpList.Add(item);
                }
            }
            SelectedCommodity = CommodityLookUpList.FirstOrDefault();
        }
        private void LoadContainerType()
        {
            ContainerTypeLookUpList.Clear();
            ContainerTypeLookUpList.Add(new ContainerType( Guid.Empty)
            {
                Name = "--Select ContainType--",
            });
            using (var c = NestedContainer)
            {
                foreach (var item in  Using<IContainerTypeRepository>(c).GetAll())
                {
                    ContainerTypeLookUpList.Add(item);
                }
            }
           selectedCT = ContainerTypeLookUpList.FirstOrDefault();
        }
        private void LoadGrade()
        {
            GradeLookUpList.Clear();
            GradeLookUpList.Add(new CommodityGrade(Guid.Empty)
            {
                Name = "--Select Grade--",
            });
            using (var c = NestedContainer)
            {
                if (SelectedCommodity != null && SelectedCommodity.Id != Guid.Empty)
                {
                    foreach (var item in Using<ICommodityRepository>(c).GetAllGradeByCommodityId(SelectedCommodity.Id).OrderBy(s=>s.Name))
                    {
                        GradeLookUpList.Add(item);
                    }
                }
            }
            //Grade = GradeLookUpList.FirstOrDefault();
        }

        private void Cancel()
        {
            RequestClose(this, EventArgs.Empty);
        }

        private void Complete()
        {
            //Mark as Re weighed
            MessageBox.Show("Tested");

        }

        private void SaveWeight()
        {
            if (ContainerLookUpList.Any() && EnforceWeighingOnReception)
            {
                MessageBox.Show("You have to Weigh all items");
                return;
                
            }

            //confirm mileage
            if (EnforceVehicleMileageAndTime && (VehicleMileage<=0))
            {
                MessageBox.Show("You MUST enter vehicle current mileage and time");
                return;
            }

            //var itemId = LineItem.Select(s => s.Id);
            using (var c = NestedContainer)
            {
                var note = Using<ICommodityDeliveryRepository>(c).GetById(DocumentId) as CommodityDeliveryNote;
                if (note != null)
                {
                    if (ContainerLookUpList.Any())
                    {
                        AutoAddWeight();
                    }
                  
                   
                    var workflowdelivery = Using<ICommodityDeliveryWFManager>(c);
                    var configService = Using<IConfigService>(c);
                    var config = configService.Load();
                    var issuerCostCentre = Using<ICostCentreRepository>(c).GetById(config.CostCentreId);
                    var user = Using<IUserRepository>(c).GetById(configService.ViewModelParameters.CurrentUserId);
                    //todo 
                    var weighedItem = note.LineItems.ToList();
                    note.DocumentIssuerCostCentreApplicationId = config.CostCentreApplicationId;
                    if (weighedItem.Any())
                    {
                        foreach (var item in weighedItem)
                        {
                            note.MarkAsWeighedLineItem(item);
                        }

                        workflowdelivery.SubmitChanges(note);
                    }
                    ReceivedDeliveryNote receivedDeliveryNote = null;
                    if (LineItem.Any())
                    {
                        var factory = Using<IReceivedDeliveryNoteFactory>(c);

                        receivedDeliveryNote = factory.Create(issuerCostCentre, note.DocumentIssuerCostCentre,
                                                              config.CostCentreApplicationId, user,
                                                              "RD_" + note.DocumentReference, note.Id, DateTime.Now,
                                                              DateTime.Now, GetvehicleArrivalTime(),
                                                              note.VehicleDepartureTime, VehicleMileage < 0 ? 0 : VehicleMileage,
                                                              note.VehicleDepartureMileage, note.Description);
                        //receivedDeliveryNote.DocumentParentId = note.Id;
                        receivedDeliveryNote.CentreId = note.CentreId;
                        receivedDeliveryNote.RouteId = note.RouteId;
                        foreach (var item in LineItem)
                        {
                            var lineitem = factory.CreateLineItem(Guid.Empty,
                                                                  item.Grade != null ? item.GradeTo.Id : Guid.Empty,
                                                                  item.BatchNumber,
                                                                  item.Weight, item.DeliveredWeight,
                                                                  item.Description);
                            //lineitem.ContainerType = item.ContainerType;
                            lineitem.Commodity = item.Commodity;
                            receivedDeliveryNote.AddLineItem(lineitem);
                        }
                        receivedDeliveryNote.Confirm();
                        var receivedworkflow = Using<IReceivedDeliveryWorkflow>(c);
                        receivedworkflow.SubmitChanges(receivedDeliveryNote);
                    }
                    note.MarkAsReceived();
                    workflowdelivery.SubmitChanges(note);
                    MessageBox.Show("Delivery No. " + note.DocumentReference + " saved successfully");
                    
                    
                    
                }
                RequestClose(this, EventArgs.Empty);
            }



            
        }
        private DateTime GetvehicleArrivalTime()
        {
            return new DateTime(VehicleArrivalDate.Year, VehicleArrivalDate.Month, VehicleArrivalDate.Day,
                                VehicleArrivalTime.Hour, VehicleArrivalTime.Minute, 00);
        }


        private void AutoAddWeight()
        {
            foreach (var group in ContainerLookUpList.GroupBy(p => new { p.Commodity ,p.CommodityGrade}))
            {
                var item = new WeighReceiveItem();

                item.BatchNumber = GetBatchNumber();
                item.DeliveredWeight = group.Sum(s => s.Weight);
                item.Grade = group.Key.CommodityGrade;
                item.GradeTo = group.Key.CommodityGrade;
                item.Commodity = group.Key.Commodity;
                item.Weight = group.Sum(s=>s.Weight);
                LineItem.Add(item);
            }
            
        }

        private string GetBatchNumber()
        {
            return DeliveryNo+"_"+(LineItem.Count() + 1).ToString();
        }

        private void AddWeight()
        {
            
            var vri = ValiadateAddWeight();
            if (!vri.IsValid)
            {
                string message = vri.Results.Aggregate("Invalid ..........\n", (current, rs) => current + ("\t" + rs.ErrorMessage + "\n"));
                MessageBox.Show(message, "Weigh and Receive Delivery");
                return;
            }

            string warningMessage = "";
            decimal totaltareweight =ContainerLookUpList.Where(s => s.IsChecked && s.IsCheckable).Sum(s => s.ContainerType.TareWeight);
            if(!_commodityReweighed)
                Weight = GrossWeight - totaltareweight;
            else
                Weight = GrossWeight - NoOfContainer*selectedCT.TareWeight;
            var item = new WeighReceiveItem();
            item.BatchNumber = GetBatchNumber();
            item.Commodity = SelectedCommodity;
            item.GradeTo = GradeTo;
            item.DeliveredWeight = DeliveredWeight;
            item.Grade = Grade;
            item.Weight = Weight;
            item.TareWeight = selectedCT.TareWeight;
            if (_commodityReweighed)
            {
                if (Weight <= 0)
                {
                    MessageBox.Show("Item with zero weight cannot be added", "Agrimanagr Warning", MessageBoxButton.OK);
                    return;
                }

                ValidateWeights(warningMessage, item);
            }
            else
            {
                ValidateWeights(warningMessage, item);
               // item.Weight = Weight;
            }
           
            LineItem.Add(item);
            _commodityReweighed = false;
            ClearWeighDetails();

            RemainingWeight = ContainerLookUpList.Where(l => !l.IsChecked).Sum(l => l.Weight);
        }
       

        private void ValidateWeights(string warningMessage, WeighReceiveItem item)
        {


            if (item.DeliveredWeight < Weight)
                warningMessage = "Received weight is more than Delivered Weight.";
            else if (item.DeliveredWeight > Weight)
                warningMessage = "Received weight is less than Delivered weight.";

            if (!string.IsNullOrEmpty(warningMessage) && item.DeliveredWeight != Weight)
            {
                var replace = MessageBox.Show(warningMessage + "\ndo you want to replace delivered weight?",
                                              "Agrimanagr Warning", MessageBoxButton.YesNo);
                item.Weight = replace == MessageBoxResult.Yes ? Weight : item.DeliveredWeight;
            }

            if (string.IsNullOrEmpty(warningMessage))
                item.Weight = item.DeliveredWeight;
        }

        private void ClearViewModel()
        {
            ClearWeighDetails();
            DeliveryNo = "";
            DeliveryBy = "";
            NoOfContainer = 0;
            VehicleNo = "";
            VehicleArrivalDate = DateTime.Today;
            VehicleArrivalTime = DateTime.Now.ToLocalTime();
            VehicleMileage = 0m;
            LineItem.Clear();
            ShowVehiclMileageTracker=Visibility.Collapsed;
            
        }
        private void GetVehicleCurrentMileage()
        {
            if(string.IsNullOrEmpty(VehicleNo))return;
            using (var c =NestedContainer)
            {
              var doc= Using<CokeDataContext>(c).tblSourcingDocument.Where(
                    p =>
                    !string.IsNullOrEmpty(p.VehicleRegNo) &&
                    p.VehicleRegNo.Equals(VehicleNo, StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(
                        p => p.IM_DateLastUpdated).FirstOrDefault(p => p.Id == DocumentId);
              if (doc != null)
                  VehicleMileage = doc.VehicleDepartureMileage.HasValue? doc.VehicleDepartureMileage.Value: 0m;
            }
        }


        private void ClearWeighDetails()
        {
            foreach (var containerLookUp in ContainerLookUpList.Where(s=>s.IsChecked==true).ToList())
            {
                ContainerLookUpList.Remove(containerLookUp);
            }
           
           // ContainerNoLookUp = ContainerLookUpList.FirstOrDefault();
            //ContainerType = ContainerTypeLookUpList.FirstOrDefault();
            Grade = GradeLookUpList.FirstOrDefault();
            SelectedCommodity = CommodityLookUpList.FirstOrDefault();
            Weight = 0;
            GrossWeight = 0;
        }

        private ValidationResultInfo ValiadateAddWeight()
        {
           var vri = new ValidationResultInfo();
           if (SelectedCommodity == null || SelectedCommodity.Id == Guid.Empty)
           {
               vri.Results.Add(new ValidationResult("Commodity  is required"));
           }
            if (Grade == null || Grade.Id == Guid.Empty)
            {
                vri.Results.Add(new ValidationResult("Grade  is required"));
            }
            if (GradeTo == null || GradeTo.Id == Guid.Empty)
            {
                vri.Results.Add(new ValidationResult("Grade To  is required"));
            }
            if (ContainerLookUpList == null || !ContainerLookUpList.Any(s => s.IsChecked))
            {
                vri.Results.Add(new ValidationResult("Select items to weigh"));
            }
           
            if (EnforceWeighingOnReception && !_commodityReweighed)
                vri.Results.Add(new ValidationResult("You must weigh commodity before adding"));
            return vri;
        }

        private void Weigh()

        {
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
                _commodityReweighed = true;

            }
            else if (SerialPortHelper.Init(out msg, scale.WeighScaleType, scale.Port,
                scale.BaudRate, scale.DataBits))
            {
                GrossWeight = SerialPortHelper.Read();
                _commodityReweighed = true;
            }
            else
            {
                MessageBox.Show(msg, "Agrimanagr Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

       

        //private void InitDevice()
        //{
        //    if (SelectedWeighScale != null)
        //    {
        //        _weighScaleDevice = new SerialPortUtil();
        //        if (!_weighScaleDevice.IsDeviceReady)
        //        {
        //            var item = SelectedWeighScale;
        //            _weighScaleDevice.Init(item.WeighScaleType,item.Port, item.BaudRate, item.DataBits);
        //        }
        //    }

        //}
        #region properties

        public const string RemainingWeightPropertyName = "RemainingWeight";

        private decimal _remainingWeight = 0;

        public decimal RemainingWeight
        {
            get
            {
                return _remainingWeight;
            }

            set
            {
                if (_remainingWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(RemainingWeightPropertyName);
                _remainingWeight = value;
                RaisePropertyChanged(RemainingWeightPropertyName);
            }
        }

        public const string DeliveryNoPropertyName = "DeliveryNo";
        private string _deliveryno = "";
        public string DeliveryNo
        {
            get
            {
                return _deliveryno;
            }

            set
            {
                if (_deliveryno == value)
                {
                    return;
                }

                RaisePropertyChanging(DeliveryNoPropertyName);
                _deliveryno = value;
                RaisePropertyChanged(DeliveryNoPropertyName);
            }
        }

        public const string DocumentIdPropertyName = "DocumentId";
        private Guid _document = Guid.Empty;
        public Guid DocumentId
        {
            get
            {
                return _document;
            }

            set
            {
                if (_document == value)
                {
                    return;
                }

                RaisePropertyChanging(DocumentIdPropertyName);
                _document = value;
                RaisePropertyChanged(DocumentIdPropertyName);
            }
        }


        public const string selectedCTPropertyName = "selectedCT";

        private ContainerType _selectedCtType = null;
        public ContainerType selectedCT
        {
            get
            {
                return _selectedCtType;
            }

            set
            {
                if (_selectedCtType == value)
                {
                    return;
                }

                RaisePropertyChanging(selectedCTPropertyName);
                _selectedCtType = value;
                RaisePropertyChanged(selectedCTPropertyName);
            }
        }

        public const string SelectedCommodityPropertyName = "SelectedCommodity";
        private Commodity _SelectedCommodity;
        public Commodity SelectedCommodity
        {
            get
            {
                return _SelectedCommodity;
            }

            set
            {
                if (_SelectedCommodity == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityPropertyName);
                _SelectedCommodity = value;
                RaisePropertyChanged(SelectedCommodityPropertyName);
                

            }
        }
        public const string GradeToPropertyName = "GradeTo";
        private CommodityGrade _gradeto = null;
        public CommodityGrade GradeTo
        {
            get
            {
                return _gradeto;
            }

            set
            {
                if (_gradeto == value)
                {
                    return;
                }

                RaisePropertyChanging(GradeToPropertyName);
                _gradeto = value;
                RaisePropertyChanged(GradeToPropertyName);
            }
        }

        public const string ItemToWeighPropertyName = "ItemToWeigh";
        private int _itemToWeigh = 20;
        public int ItemToWeigh
        {
            get
            {
                return _itemToWeigh;
            }

            set
            {
                if (_itemToWeigh == value)
                {
                    return;
                }

                RaisePropertyChanging(ItemToWeighPropertyName);
                _itemToWeigh = value;
                RaisePropertyChanged(ItemToWeighPropertyName);
            }
        }
        public const string GradePropertyName = "Grade";
        private CommodityGrade _commodity = null;
        public CommodityGrade Grade
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

                RaisePropertyChanging(GradePropertyName);
                _commodity = value;
                RaisePropertyChanged(GradePropertyName);
            }
        }

        public const string GrossWeightPropertyName = "GrossWeight";
        private decimal _grossWeight = 0;
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
                  //RecalculateWeight();
            }
        }
        public const string DeliveredWeightPropertyName = "DeliveredWeight";
        private decimal _DeliveredWeight = 0;
        public decimal DeliveredWeight
        {
            get
            {
                return _DeliveredWeight;
            }

            set
            {
                if (_DeliveredWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(DeliveredWeightPropertyName);
                _DeliveredWeight = value;
                RaisePropertyChanged(DeliveredWeightPropertyName);
                
            }
        }

        public const string WeightPropertyName = "Weight";
        private decimal _weight = 0;
        public decimal Weight
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

        /// <summary>
        /// The <see cref="DateReceived" /> property's name.
        /// </summary>
        public const string DateReceivedPropertyName = "DateReceived";

        private DateTime _dateReceived = DateTime.Now;

        /// <summary>
        /// Sets and gets the DateReceived property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime DateReceived
        {
            get
            {
                return _dateReceived;
            }

            set
            {
                if (_dateReceived == value)
                {
                    return;
                }

                RaisePropertyChanging(DateReceivedPropertyName);
                _dateReceived = value;
                RaisePropertyChanged(DateReceivedPropertyName);
            }
        }

      

        public const string DeliveryByPropertyName = "DeliveryBy";
        private string _deliveryByno = "";
        public string DeliveryBy
        {
            get
            {
                return _deliveryByno;
            }

            set
            {
                if (_deliveryByno == value)
                {
                    return;
                }

                RaisePropertyChanging(DeliveryByPropertyName);
                _deliveryByno = value;
                RaisePropertyChanged(DeliveryByPropertyName);
            }
        }
        public const string VehicleArrivalDatePropertyName = "VehicleArrivalDate";
        private DateTime _vehicleArrivaldate = DateTime.Today;
        public DateTime VehicleArrivalDate
        {
            get
            {
                return _vehicleArrivaldate;
            }

            set
            {
                if (_vehicleArrivaldate == value)
                {
                    return;
                }

                RaisePropertyChanging(VehicleArrivalDatePropertyName);
                _vehicleArrivaldate = value;
                RaisePropertyChanged(VehicleArrivalDatePropertyName);
            }
        }

        public const string VehicleArrivalTimePropertyName = "VehicleArrivalTime";
        private DateTime _vehicleArrivalTime = DateTime.Now.ToLocalTime();
        public DateTime VehicleArrivalTime
        {
            get
            {
                return _vehicleArrivalTime;
            }

            set
            {
                if (_vehicleArrivalTime == value)
                {
                    return;
                }

                RaisePropertyChanging(VehicleArrivalTimePropertyName);
                _vehicleArrivalTime = value;
                RaisePropertyChanged(VehicleArrivalTimePropertyName);
                if(_vehicleArrivalTime !=null && _vehicleArrivalTime.ToLocalTime()>DateTime.Now.ToLocalTime())
                    _vehicleArrivalTime = DateTime.Now.ToLocalTime();
            }
        }


        public const string ShowVehiclMileageTrackerPropertyName = "ShowVehiclMileageTracker";
        private Visibility _vshowvehicleMileagepanel = Visibility.Collapsed;
        public Visibility ShowVehiclMileageTracker
        {
            get
            {
                return _vshowvehicleMileagepanel;
            }

            set
            {
                if (_vshowvehicleMileagepanel == value)
                {
                    return;
                }

                RaisePropertyChanging(ShowVehiclMileageTrackerPropertyName);
                _vshowvehicleMileagepanel = value;
                RaisePropertyChanged(ShowVehiclMileageTrackerPropertyName);
            }
        }

        public const string VehicleMileagePropertyName = "VehicleMileage";
        private decimal _vehicleMileage = 0m;
        public decimal VehicleMileage
        {
            get
            {
                return _vehicleMileage;
            }

            set
            {
                if (_vehicleMileage == value)
                {
                    return;
                }

                RaisePropertyChanging(VehicleMileagePropertyName);
                _vehicleMileage = value;
                RaisePropertyChanged(VehicleMileagePropertyName);
            }
        }

        

        
        public const string EnforceWeighingOnDeliveryPropertyName = "EnforceWeighingOnReception";
        private bool _enforceWeighingOnReception;
        public bool EnforceWeighingOnReception
        {
            get
            {
                return _enforceWeighingOnReception;
            }

            set
            {
                if (_enforceWeighingOnReception == value)
                {
                    return;
                }

                RaisePropertyChanging(EnforceWeighingOnDeliveryPropertyName);
                _enforceWeighingOnReception = value;
                RaisePropertyChanged(EnforceWeighingOnDeliveryPropertyName);
            }
        }
        public const string MakeWeightextBoxReadOnlyPropertyName = "MakeWeightextBoxReadOnly";
        private bool _makeWeightextBoxReadOnly = true;
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

        public const string VehicleNoPropertyName = "VehicleNo";
        private string _vehicleNo = "";
        public string VehicleNo
        {
            get
            {
                return _vehicleNo;
            }

            set
            {
                if (_vehicleNo == value)
                {
                    return;
                }

                RaisePropertyChanging(VehicleNoPropertyName);
                _vehicleNo = value;
                RaisePropertyChanged(VehicleNoPropertyName);
            }
        }

        public const string NoOfContainerPropertyName = "NoOfContainer";
        private decimal _noOfContainer = 0;
        public decimal NoOfContainer
        {
            get
            {
                return _noOfContainer;
            }

            set
            {
                if (_noOfContainer == value)
                {
                    return;
                }

                RaisePropertyChanging(NoOfContainerPropertyName);
                _noOfContainer = value;
                RaisePropertyChanged(NoOfContainerPropertyName);
            }
        }
        #endregion
    }
    public class WeighReceiveItem : ViewModelBase
    {
        
        public WeighReceiveItem()
        {
        }

        
        //public const string ContainerNoLookUpPropertyName = "ContainerNoLookUp";
        //private ContainerLookUp _containerNo = null;
        //public ContainerLookUp ContainerNoLookUp
        //{
        //    get
        //    {
        //        return _containerNo;
        //    }

        //    set
        //    {
        //        if (_containerNo == value)
        //        {
        //            return;
        //        }

        //        RaisePropertyChanging(ContainerNoLookUpPropertyName);
        //        _containerNo = value;
        //        RaisePropertyChanged(ContainerNoLookUpPropertyName);
        //    }
        //}

        
        //public const string ContainerTypePropertyName = "ContainerType";
        //private ContainerType _containerType ;
        //public ContainerType ContainerType
        //{
        //    get
        //    {
        //        return _containerType;
        //    }

        //    set
        //    {
        //        if (_containerType == value)
        //        {
        //            return;
        //        }

        //        RaisePropertyChanging(ContainerTypePropertyName);
        //        _containerType = value;
        //        RaisePropertyChanged(ContainerTypePropertyName);
        //    }
        //}

        
        public const string GradePropertyName = "Grade";
        private CommodityGrade _grade = null;
        public CommodityGrade Grade
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

        public const string GradeToPropertyName = "GradeTo";
        private CommodityGrade _gradeto = null;
        public CommodityGrade GradeTo
        {
            get
            {
                return _gradeto;
            }

            set
            {
                if (_gradeto == value)
                {
                    return;
                }

                RaisePropertyChanging(GradeToPropertyName);
                _gradeto = value;
                RaisePropertyChanged(GradeToPropertyName);
            }
        }

        public const string ItemToWeighPropertyName = "ItemToWeigh";
        private int _itemToWeigh = 20;
        public int ItemToWeigh
        {
            get
            {
                return _itemToWeigh;
            }

            set
            {
                if (_itemToWeigh == value)
                {
                    return;
                }

                RaisePropertyChanging(ItemToWeighPropertyName);
                _itemToWeigh = value;
                RaisePropertyChanged(ItemToWeighPropertyName);
            }
        }


        public const string CommodityPropertyName = "Commodity";
        private Commodity _Commodity = null;
        public Commodity Commodity
        {
            get
            {
                return _Commodity;
            }

            set
            {
                if (_Commodity == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityPropertyName);
                _Commodity = value;
                RaisePropertyChanged(CommodityPropertyName);
            }
        }

       
        public const string WeightPropertyName = "Weight";
        private decimal _weight = 0;
        public decimal Weight
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

        public const string BatchNumberPropertyName = "BatchNumber";
        private string _BatchNumber = "";
        public string BatchNumber
        {
            get
            {
                return _BatchNumber;
            }

            set
            {
                if (_BatchNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(BatchNumberPropertyName);
                _BatchNumber = value;
                RaisePropertyChanged(BatchNumberPropertyName);
            }
        }

        public const string IdPropertyName = "Id";
        private Guid _itemId = Guid.Empty;
        public Guid Id
        {
            get
            {
                return _itemId;
            }

            set
            {
                if (_itemId == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _itemId = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        
        public const string DeliveredWeightPropertyName = "DeliveredWeight";
        private decimal _deliveredWeight = 0;
        public decimal DeliveredWeight
        {
            get
            {
                return _deliveredWeight;
            }

            set
            {
                if (_deliveredWeight == value)
                {
                    return;
                }

                RaisePropertyChanging(DeliveredWeightPropertyName);
                _deliveredWeight = value;
                RaisePropertyChanged(DeliveredWeightPropertyName);
            }
        }

        public const string DescriptionPropertyName = "Description";
        private string _Description = "";
        public string Description
        {
            get
            {
                return _Description;
            }

            set
            {
                if (_Description == value)
                {
                    return;
                }

                RaisePropertyChanging(DescriptionPropertyName);
                _Description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        public const string TareWeightPropertyName = "TareWeight";

        private decimal _tareWeight = 0;

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
    }

    public class ContainerLookUp:ViewModelBase
    {
       // public string Id { get; set; }
       // public string ContainerNo { get; set;    }
        //public decimal Weight { get; set; }
       // public bool IsWeighed { get; set; }
        public Commodity Commodity { get; set; }
        public ContainerType ContainerType { get; set; }
        public CommodityGrade CommodityGrade { get; set; }
        //public bool IsChecked { get; set; }
        //public bool IsCheckable { get; set; }

        public const string IdPropertyName = "Id";
        private string _itemId = "";
        public string Id
        {
            get
            {
                return _itemId;
            }

            set
            {
                if (_itemId == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _itemId = value;
                RaisePropertyChanged(IdPropertyName);
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

                RaisePropertyChanging(ContainerNoPropertyName);
                _containerNo = value;
                RaisePropertyChanged(ContainerNoPropertyName);
            }
        }


        public const string WeightPropertyName = "Weight";
        private decimal _Weight = 0;
        public decimal Weight
        {
            get
            {
                return _Weight;
            }

            set
            {
                if (_Weight == value)
                {
                    return;
                }

                RaisePropertyChanging(WeightPropertyName);
                _Weight = value;
                RaisePropertyChanged(WeightPropertyName);
            }
        }

        public const string IsWeighedPropertyName = "IsWeighed";
        private bool _IsWeighed = false;
        public bool IsWeighed
        {
            get
            {
                return _IsWeighed;
            }

            set
            {
                if (_IsWeighed == value)
                {
                    return;
                }

                RaisePropertyChanging(IsWeighedPropertyName);
                _IsWeighed = value;
                RaisePropertyChanged(IsWeighedPropertyName);
            }
        }

        public const string IsCheckedPropertyName = "IsChecked";
        private bool _IsChecked = false;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }

            set
            {
                if (_IsChecked == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckedPropertyName);
                _IsChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }

        public const string IsCheckablePropertyName = "IsCheckable";
        private bool _IsCheckable = false;
        public bool IsCheckable
        {
            get
            {
                return _IsCheckable;
            }

            set
            {
                if (_IsCheckable == value)
                {
                    return;
                }

                RaisePropertyChanging(IsCheckablePropertyName);
                _IsCheckable = value;
                RaisePropertyChanged(IsCheckablePropertyName);
            }
        }

        public const string GrossWeightPropertyName = "GrossWeight";

        private decimal _grossWeightDecimal = 0;

        public decimal GrossWeight
        {
            get
            {
                return _grossWeightDecimal;
            }

            set
            {
                if (_grossWeightDecimal == value)
                {
                    return;
                }

                RaisePropertyChanging(GrossWeightPropertyName);
                _grossWeightDecimal = value;
                RaisePropertyChanged(GrossWeightPropertyName);
            }
        }

        public const string TareWeightPropertyName = "TareWeight";

        private decimal _tareWeightDecimal = 0;

        public decimal TareWeight
        {
            get
            {
                return _tareWeightDecimal;
            }

            set
            {
                if (_tareWeightDecimal == value)
                {
                    return;
                }

                RaisePropertyChanging(TareWeightPropertyName);
                _tareWeightDecimal = value;
                RaisePropertyChanged(TareWeightPropertyName);
            }
        }
    }
}