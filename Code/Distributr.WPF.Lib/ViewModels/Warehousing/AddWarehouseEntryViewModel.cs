using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class AddWarehouseEntryViewModel:DistributrViewModelBase
    {
        public AddWarehouseEntryViewModel()
        {
          
            WeighCommand = new RelayCommand(Weigh);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AddWarehouseEntryLoadPageCommand = new RelayCommand(LoadPage);
            AccountSelectionChangedCommand=new RelayCommand(SelectAccount);
            FarmerSelectionChangedCommand = new RelayCommand(SelectFarmer);
            CommodityChangeCommand = new RelayCommand(SelectCommodity);
            SelectedCommodityChangedCommand=new RelayCommand(LoadGrades);
            GradeChangeCommand=new RelayCommand(SelectGrade);



            GradeList=new ObservableCollection<CommodityGrade>();
            FarmersList=new ObservableCollection<CommodityOwner>();
            CommodityList=new ObservableCollection<Commodity>();
            LineItems = new ObservableCollection<WarehouseLineItemViewModel>();
        }

     

      

        #region Members

        public RelayCommand WeighCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AddWarehouseEntryLoadPageCommand { get; set; }

        public RelayCommand AccountSelectionChangedCommand { get; set; }
        public RelayCommand FarmerSelectionChangedCommand { get; set; }
        public RelayCommand CommodityChangeCommand { get; set; }
        public RelayCommand SelectedCommodityChangedCommand { get; set; }
        public RelayCommand GradeChangeCommand { get; set; }


        public ObservableCollection<CommodityGrade> GradeList { get; set; }
        public ObservableCollection<CommodityOwner> FarmersList { get; set; }
        public ObservableCollection<Commodity> CommodityList { get; set; }
        public ObservableCollection<WarehouseLineItemViewModel> LineItems { get; set; }
       

        #endregion

        #region Methods

        private void LoadPage()
        {
            ClearViewModel();
            DocumentId = Guid.NewGuid();
            using (StructureMap.IContainer cont = NestedContainer)
            {
                IConfigService configService = Using<IConfigService>(cont);
                var _costCentreRepo = Using<ICostCentreRepository>(cont);
                DocumentIssuerUser = Using<IUserRepository>(cont).GetById(configService.ViewModelParameters.CurrentUserId);

                DocumentIssuerCostCentre = _costCentreRepo.GetById(GetConfigParams().CostCentreId);
                
              }
            //DefaultValues();
           
            GetSettings();

            LoadCommodities();
            //LoadFarmers();
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
            }
        }

        private void DefaultValues()
        {
            if(System.Diagnostics.Debugger.IsAttached)
            {
                Notes = "Kenyan";
                Weight = 45;
                DriverName = "James";
                RegistrationNumber = "KAV 754B";
            }
        }

        private void Weigh()
        {
            ClearViewModel();
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

                Weight = SerialPortHelper.Read();
                using (StructureMap.IContainer c = NestedContainer)
                {
                    var setting =
                        c.GetInstance<ISettingsRepository>().GetByKey(SettingsKeys.EnforceTransactionalWeightLimit);
                    var val = setting.Value;
                    if (val.Trim().StartsWith("True"))
                    {
                        StringCollection information = new StringCollection();
                        foreach (Match match in Regex.Matches(val, @"\(([^)]*)\)"))
                        {
                            information.Add(match.Value);
                        }
                        var minValue = information[0].Trim();
                        minValue = minValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        string maxValue = information[1].Trim();
                        maxValue = maxValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        double mn = double.Parse(minValue);
                        double mx = double.Parse(maxValue);

                        string errLessThan = string.Format(
                            "Weight is Less than the set Limit. Minimum Weight is {0}", mn);
                        string errGreaterThan = string.Format(
                            "Weight is Greater than the set Limit. Maximum Weight is[{0}]", mx);
                        if (Weight < (decimal)mn)
                        {
                            MessageBox.Show(errLessThan, "Agrimanager", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else if (Weight > (decimal)mx)
                        {
                            MessageBox.Show(errGreaterThan, "Agrimanager", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                    }
                }

            }
            else if (SerialPortHelper.Init(out msg, scale.WeighScaleType, scale.Port,
                scale.BaudRate, scale.DataBits))
            {
                Weight = SerialPortHelper.Read();
                using (StructureMap.IContainer c = NestedContainer)
                {
                    var setting =
                        c.GetInstance<ISettingsRepository>().GetByKey(SettingsKeys.EnforceTransactionalWeightLimit);
                    var val = setting.Value;
                    if (val.Trim().StartsWith("True"))
                    {
                        StringCollection information = new StringCollection();
                        foreach (Match match in Regex.Matches(val, @"\(([^)]*)\)"))
                        {
                            information.Add(match.Value);
                        }
                        var minValue = information[0].Trim();
                        minValue = minValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        string maxValue = information[1].Trim();
                        maxValue = maxValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        double mn = double.Parse(minValue);
                        double mx = double.Parse(maxValue);

                        string errLessThan = string.Format(
                            "Weight is Less than the set Limit. Minimum Weight is {0}", mn);
                        string errGreaterThan = string.Format(
                            "Weight is Greater than the set Limit. Maximum Weight is[{0}]", mx);
                        if (Weight < (decimal) mn)
                        {
                            MessageBox.Show(errLessThan, "Agrimanager", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else if (Weight > (decimal) mx)
                        {
                            MessageBox.Show(errGreaterThan, "Agrimanager", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                    }
                }
            }
            else
            {
                AddLogEntry("Warehousing Weigh","Error- "+msg);
                MessageBox.Show("The weight could not be recorded,check the audit log and make sure weighing scale is setup properly", "Agrimanagr Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }

        private void Cancel()
        {
            if (
                MessageBox.Show("Are you sure you want to leave this page?\nUnsaved changes will be lost",
                                "Agrimanagr: Warehouse Entry", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) ==
                MessageBoxResult.Yes)
            {
                string url = "/views/warehousing/WarehouseEntryListingPage.xaml";
                NavigateCommand.Execute(url);
            }
        }


        string GetDocumentReference(string docRef)
        {
            using (StructureMap.IContainer cont = NestedContainer)
            {
                docRef = Using<IGetDocumentReference>(cont)
                    .GetDocReference(docRef, DocumentIssuerCostCentre.Id,
                                     DocumentRecipientCostCentre.Id);
            }
            return docRef;
        }

        private void Save()
        {
            LineItems.Clear();
            string msg = "";

           
            try
            {


                using (StructureMap.IContainer c = NestedContainer)
                {
                    var setting =
                        c.GetInstance<ISettingsRepository>().GetByKey(SettingsKeys.EnforceTransactionalWeightLimit);
                    var val = setting.Value;
                    if (val.Trim().StartsWith("True"))
                    {
                        StringCollection information = new StringCollection();
                        foreach (Match match in Regex.Matches(val, @"\(([^)]*)\)"))
                        {
                            information.Add(match.Value);
                        }
                        var minValue = information[0].Trim();
                        minValue = minValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        string maxValue = information[1].Trim();
                        maxValue = maxValue.Replace("(", string.Empty).Replace(")", string.Empty);
                        double mn = double.Parse(minValue);
                        double mx = double.Parse(maxValue);

                        string errLessThan = string.Format(
                            "Weight is Less than the set Limit. Minimum Weight is {0}", mn);
                        string errGreaterThan = string.Format(
                            "Weight is Greater than the set Limit. Maximum Weight is[{0}]", mx);
                        if (Weight < (decimal)mn)
                        {
                            MessageBox.Show(errLessThan, "Agrimanager", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else if (Weight > (decimal)mx)
                        {
                            MessageBox.Show(errGreaterThan, "Agrimanager", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                    }
                }



                var warehouseLineItem = new WarehouseLineItemViewModel()
                {
                    Id = Guid.NewGuid(),
                    GrossWeight = Weight,
                    Commodity = SelectedCommodity,
                    CommodityGrade = SelectedGrade,
                    Farmer = SelectedFarmer,
                    Description = Notes,
                    Note = Notes,
                };

                LineItems.Add(warehouseLineItem);
                if (Validate())
                {
                    using (StructureMap.IContainer c = NestedContainer)
                    {
                        var configService = Using<IConfigService>(c);
                        Config config = configService.Load();
                        Guid costCentreApplicationid = config.CostCentreApplicationId;

                        DocumentRecipientCostCentre = Using<ICostCentreRepository>(c).GetById(SelectedFarmer.CommoditySupplier.Id);

                        DocumentReference = GetDocumentReference("WarehouseAddEntryNote");

                        DateTime now = DateTime.Now;
                        var warehouseWfManager = Using<ICommodityWarehouseStorageWFManager>(c);
                        var warehouseFactory = Using<ICommodityWarehouseStorageFactory>(c);


                        var warehouseNote = warehouseFactory.Create(DocumentIssuerCostCentre, costCentreApplicationid,
                                                                    DocumentRecipientCostCentre, DocumentIssuerUser,
                                                                    DocumentReference,Guid.Empty,SelectedFarmer.Id, DateTime.Now,
                                                                    DateTime.Now, DriverName, RegistrationNumber,null,
                                                                    null, null, null, Notes, Notes);


                       
                        foreach (var item in LineItems)
                        {
                            var lineItem = warehouseFactory.CreateLineItem(item.Id, item.Commodity.Id,
                                                                           item.CommodityGrade.Id, Guid.Empty, null,
                                                                           Weight, item.Description);
                            warehouseNote.AddLineItem(lineItem);
                        }
                        warehouseNote.Confirm();
                        warehouseWfManager.SubmitChanges(warehouseNote);


                        AddLogEntry("Warehouse Entry", "Created warehouse entry note " + DocumentReference);
                       
                        msg = "Warehouse Entry successfully made . Transaction number: " +
                              DocumentReference;
                       if( MessageBox.Show(msg, "Agrimanagr: Warehouse " + DocumentReference, MessageBoxButton.OK,
                                        MessageBoxImage.Information)==MessageBoxResult.OK)
                       {
                           string url = "/views/warehousing/WarehouseEntryListingPage.xaml";
                           NavigateCommand.Execute(url);
                       }
                    }
                }
            }
            catch(Exception e)
            {
                
               msg = "Error occured while saving transaction.\n" + e.Message +
                     (e.InnerException == null ? "" : e.InnerException.Message);
               MessageBox.Show(msg, "Agrimanagr: Warehouse Entry", MessageBoxButton.OK, MessageBoxImage.Error);
               
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
                isValid = false;
            }
            if (Weight <= 0)
            {
                MessageBox.Show("Net weight must be greater than 0", "Agrimanagr : Warehouse Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid = false;
            }
            if (SelectedCommodity.Name =="--Select Commodity---")
            {
                MessageBox.Show("Please select commodity", "Agrimanagr : Warehouse Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid = false;
            }
            if (SelectedGrade.Name == "--Select Grade---" && GradeList.Any())
            {
                LoadGrades();
                MessageBox.Show("Please select grade", "Agrimanagr : Warehouse Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid = false;
            }
            if (SelectedFarmer.FirstName == "--Select Farmer---")
            {
                MessageBox.Show("Please select farmer", "Agrimanagr : Warehouse Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid = false;
            }




            return isValid;
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

                }

            }
        }

       

        private void LoadFarmers(Guid supplierId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {

                var commodityOwner = Using<ICommodityOwnerRepository>(c).GetBySupplier(supplierId).OrderBy(n => n.FullName).ThenBy(n => n.Code).ToList();
                commodityOwner.ForEach(n => { if (FarmersList.Select(q => q.Id).All(p => p != n.Id)) FarmersList.Add(n); });

                if (FarmersList.Count(p => p.Id != Guid.Empty) == 1)
                {
                    SelectedFarmer = FarmersList.FirstOrDefault(p => p.Id != Guid.Empty);

                }

            }
        }

        private void LoadGrades()
        {
            if (SelectedCommodity == null || SelectedCommodity.Id == Guid.Empty)
                return;
            GradeList.Clear();
            GradeList.Add(DefaultGrade);
            var grades = SelectedCommodity.CommodityGrades.Where(p => p._Status == EntityStatus.Active)
                 .OrderBy(n => n.Name)
                 .ThenBy(n => n.Code)
                 .ToList();
            grades.ForEach(n => { if (GradeList.Select(q => q.Id).All(p => p != n.Id)) GradeList.Add(n); });
            if (GradeList.Count(n => n.Id != Guid.Empty) == 0)
            {
               

                GradeList.Add(DefaultGrade);
                SelectedGrade = DefaultGrade;
            }
            else if (GradeList.Count(n => n.Id != Guid.Empty) == 1)
            {
                SelectedGrade = GradeList.FirstOrDefault(n => n.Id != Guid.Empty);
            }
            else
            {
                SelectedGrade = DefaultGrade;
            }
        }


        private void SelectGrade()
        {
            if (SelectedCommodity.Name == "--Select Commodity---")
            {
                SelectedGrade = new CommodityGrade(Guid.Empty) { Name = "--Select Grade---" };
                MessageBox.Show("Please Select the Commodity first", "Agrimanagr", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectGrade(SelectedCommodity.Id);

                SelectedGrade = selected;
                if (selected == null)
                {
                    SelectedGrade = new CommodityGrade(Guid.Empty) { Name = "--Select Grade---" };
                }
            }
        }

        private void SelectAccount()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectCommoditySupplier();

                SelectedAccount = selected;
                if (selected == null)
                {
                    SelectedAccount = new CommoditySupplier(Guid.Empty) { AccountName = "--Select Account---" };
                   
                    FarmerVisibility = "Collapsed";
                }
                else
                {

                    SelectedAccountName = SelectedAccount.Name;

                    if(SelectedAccount.CommoditySupplierType==CommoditySupplierType.Cooperative)
                    {
                        if (SelectedFarmer!=null && SelectedFarmer.Id != SelectedAccount.Id)
                        {
                            SelectedFarmer = new CommodityOwner(Guid.Empty) { FirstName = "--Select Farmer---" };
                            SelectedFarmerName = "--Select Farmer---";
                        }
                        FarmerVisibility = "Visible";
                    }
                    else
                    {
                        SelectedFarmer = Using<ICommodityOwnerRepository>(container).GetBySupplier(SelectedAccount.Id).FirstOrDefault();
                        FarmerVisibility = "Collapsed";
                    }
                    
                }
            }
        }

        private void SelectFarmer()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectFarmersBySupplier(SelectedAccount.Id);

                SelectedFarmer = selected;
                if (selected == null)
                {
                    SelectedFarmer = new CommodityOwner(Guid.Empty) { FirstName = "--Select Farmer---" };
                    SelectedFarmerName = "--Select Farmer---";
                }
                else
                {
                    
                    SelectedFarmerName = SelectedFarmer.FullName;
                }
            }
        }

        private void SelectCommodity()
        {
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectCommodity();

                SelectedCommodity = selected;
                if (selected == null)
                {
                    SelectedCommodity = new Commodity(Guid.Empty) { Name = "--Select Commodity---" };
                }
                LoadGrades();
            }
        }

        void ClearViewModel()
        {

            DocumentReference = string.Empty;
            Notes = "";
            SelectedAccountName = "--Select Account---"; ;
            SelectedFarmerName = "--Select Farmer---";
            DocumentId = Guid.Empty;
            SelectedAccount = new CommoditySupplier(Guid.Empty) { AccountName = "--Select Account---" };
            SelectedFarmer = new CommodityOwner(Guid.Empty) { FirstName = "--Select Farmer---" };
            DocumentIssuerUser = null;
            DocumentParentId = Guid.Empty;
            SelectedCommodity = new Commodity(Guid.Empty) { Name = "--Select Commodity---" }; ;
            SelectedGrade = new CommodityGrade(Guid.Empty) { Name = "--Select Grade---" };
            DocumentIssuerCostCentre = null;
            DocumentRecipientCostCentre = null;
            DriverName = "";
            RegistrationNumber = "";
            Weight = decimal.Zero;
            LineItems.Clear();
            FarmersList.Clear();
            CommodityList.Clear();
            GradeList.Clear();
            FarmerVisibility = "Collapsed";
            // MakeWeightextBoxReadOnly = false;
        }

        #endregion

        #region Properties

        public const string SelectedFarmerNamePropertyName = "SelectedFarmerName";
        private string _selectedFarmerName = "--Select Farmer---";
        public string SelectedFarmerName
        {
            get
            {
                return _selectedFarmerName;
            }

            set
            {
                if (_selectedFarmerName == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedFarmerNamePropertyName);
                _selectedFarmerName = value;
                RaisePropertyChanged(SelectedFarmerNamePropertyName);
            }
        }

        public const string SelectedAccountNamePropertyName = "SelectedAccountName";
        private string _selectedAccountName = "--Select Account---";
        public string SelectedAccountName
        {
            get
            {
                return _selectedAccountName;
            }

            set
            {
                if (_selectedAccountName == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedAccountNamePropertyName);
                _selectedAccountName = value;
                RaisePropertyChanged(SelectedAccountNamePropertyName);
            }
        }


        public const string FarmerVisibilityPropertyName = "FarmerVisibility";
        private string _farmerVisibility = "Collapsed";
        public string FarmerVisibility
        {
            get
            {
                return _farmerVisibility;
            }

            set
            {
                if (_farmerVisibility == value)
                {
                    return;
                }

                RaisePropertyChanging(FarmerVisibilityPropertyName);
                _farmerVisibility = value;
                RaisePropertyChanged(FarmerVisibilityPropertyName);
            }
        }

        public const string SelectedFarmerPropertyName = "SelectedFarmer";
        private CommodityOwner _selectedFarmer = new CommodityOwner(Guid.Empty) { FirstName = "--Select Farmer---" };
        public CommodityOwner SelectedFarmer
        {
            get
            {
                return _selectedFarmer;
            }

            set
            {
                if (_selectedFarmer == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedFarmerPropertyName);
                _selectedFarmer = value;
                RaisePropertyChanged(SelectedFarmerPropertyName);
            }
        }

        public const string SelectedAccountPropertyName = "SelectedAccount";
        private CommoditySupplier _selectedAccount = new CommoditySupplier(Guid.Empty) { AccountName = "--Select Account---" };
        public CommoditySupplier SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }

            set
            {
                if (_selectedAccount == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedAccountPropertyName);
                _selectedAccount = value;
                RaisePropertyChanged(SelectedAccountPropertyName);
            }
        }


        public const string SelectedCommodityPropertyName = "SelectedCommodity";
        private Commodity _selectedCommodity = new Commodity(Guid.Empty) { Name = "--Select Commodity---" };
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
            }
        }

        public const string SelectedGradePropertyName = "SelectedGrade";
        private CommodityGrade _selectedGrade = new CommodityGrade(Guid.Empty) { Name = "--Select Grade---" };
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
            }
        }

        public const string RegistrationNumberPropertyName = "RegistrationNumber";
        private string _registrationNumber = "";
        public string RegistrationNumber
        {
            get
            {
                return _registrationNumber;
            }

            set
            {
                if (_registrationNumber == value)
                {
                    return;
                }

                RaisePropertyChanging(RegistrationNumberPropertyName);
                _registrationNumber = value;
                RaisePropertyChanged(RegistrationNumberPropertyName);
            }
        }

        public const string DriverNamePropertyName = "DriverName";
        private string _driverName = "";
        public string DriverName
        {
            get
            {
                return _driverName;
            }

            set
            {
                if (_driverName == value)
                {
                    return;
                }

                RaisePropertyChanging(DriverNamePropertyName);
                _driverName = value;
                RaisePropertyChanged(DriverNamePropertyName);
            }
        }

        public const string NotesPropertyName = "Notes";
        private string _notes = "";
        public string Notes
        {
            get
            {
                return _notes;
            }

            set
            {
                if (_notes == value)
                {
                    return;
                }

                RaisePropertyChanging(NotesPropertyName);
                _notes = value;
                RaisePropertyChanged(NotesPropertyName);
            }
        }


        public const string WeightPropertyName = "Weight";
        private decimal _weight = 0m;
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

        CommodityGrade _defaultGrade = null;
        private CommodityGrade DefaultGrade
        {
            get
            {
                return _defaultGrade ??
                       (_defaultGrade = new CommodityGrade(Guid.Empty) { Name = "--Select Grade---" });
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

        #endregion

        
    }
}
