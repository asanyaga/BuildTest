using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Factory.SourcingDocuments;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Messages;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Warehousing
{
    public class WarehouseExitViewModel : DistributrViewModelBase
    {

       

        public WarehouseExitViewModel()
        {
            WarehouseExitLoadPageCommand=new RelayCommand(LoadPage);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            WeighCommand = new RelayCommand(Weigh);
        }

        #region Members
        public RelayCommand WarehouseExitLoadPageCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand WeighCommand { get; set; }
        #endregion

        #region Methods

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

                Weight = SerialPortHelper.Read();


            }
            else if (SerialPortHelper.Init(out msg, scale.WeighScaleType, scale.Port,
                scale.BaudRate, scale.DataBits))
            {
                Weight = SerialPortHelper.Read();

            }
            else
            {
                AddLogEntry("Warehousing Weigh", "Error- " + msg);
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


        

        public void SetDocumentId(WarehouseEntryUpdateMessage messageFrom)
        {
            DocumentId = messageFrom.DocumentId;
           
            LoadExit();
            
        }

        private void LoadExit()
        {
            ClearViewModel();
            using (StructureMap.IContainer c = NestedContainer)
            {
                var document =Using<ICommodityWarehouseStorageRepository>(c).GetById(DocumentId) as CommodityWarehouseStorageNote;
                if (document != null)
                {
                    DocumentReference = document.DocumentReference;
                    var lineItems= document.LineItems.FirstOrDefault();

                    if (lineItems != null)
                    {
                        SelectedGrade = lineItems.CommodityGrade;
                        SelectedCommodity = lineItems.Commodity;
                        SelectedFarmer = Using<ICommodityOwnerRepository>(c).GetById(document.CommodityOwnerId);
                        SelectedFarmerName = SelectedFarmer.FullName;
                        DriverName = document.DriverName;
                        RegistrationNumber = document.VehiclRegNo;
                        Notes = lineItems.Note;
                    }
                    
                }
            }
        }

        private void ClearViewModel()
        {
            SelectedCommodity = null;
            SelectedFarmer = null;
            SelectedGrade = null;
            Notes = null;
            RegistrationNumber = null;
            Weight = 0m;
            DriverName = null;
           // MakeWeightextBoxReadOnly = false;

        }

        private void Save()
        {
            string msg = "";
            if(Validate())
            {
                using (StructureMap.IContainer c = NestedContainer)
                {

                    var warehouseWfManager = Using<ICommodityWarehouseStorageWFManager>(c);

                    var document =Using<ICommodityWarehouseStorageRepository>(c).GetById(DocumentId) as CommodityWarehouseStorageNote;
                    if (document != null)
                    {
                        var lineItems = document.LineItems.FirstOrDefault();
                    
                        if (lineItems != null)
                        {
                            lineItems.Weight = Weight;
                        }

                        document.UpdateLineItem(lineItems);
                        document.Approve();
                        warehouseWfManager.SubmitChanges(document);
                    }

                    AddLogEntry("Warehouse Exit", "Created warehouse exit note " + DocumentReference);

                    msg = "Warehouse Exit successfully made . Transaction number: " +
                          DocumentReference;
                   if(MessageBox.Show(msg, "Agrimanagr: Warehouse " + DocumentReference, MessageBoxButton.OK,
                                    MessageBoxImage.Information) == MessageBoxResult.OK)
                   {
                       string url = "/views/warehousing/WarehouseEntryListingPage.xaml";
                       NavigateCommand.Execute(url); 
                   }
                }
            }
        }


        private void LoadPage()
        {
            GetSettings();
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

        private bool Validate()
        {
            var isValid = true;
            if (Weight < 0)
            {
                MessageBox.Show("Net weight cannot be negative", "Agrimanagr : Warehouse Error", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                isValid = false;
            }
            return isValid;


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
                       (_defaultGrade = new CommodityGrade(Guid.Empty) { Name = "--Select grade--" });
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


        public const string IsEditPropertyName = "IsEdit";
        private bool _isEdit = false;
        public bool IsEdit
        {
            get
            {
                return _isEdit;
            }

            set
            {
                if (_isEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(IsEditPropertyName);
                _isEdit = value;
                RaisePropertyChanged(IsEditPropertyName);
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
