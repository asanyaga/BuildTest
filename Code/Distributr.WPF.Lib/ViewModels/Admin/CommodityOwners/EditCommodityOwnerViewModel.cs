using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DataContracts;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommodityOwners
{
    public class EditCommodityOwnerViewModel : DistributrViewModelBase
    {
        public EditCommodityOwnerViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            CommoditySuppliersList = new ObservableCollection<CommoditySupplier>();
            CommodityOwnerTypesList = new ObservableCollection<CommodityOwnerType>();
            MaritalStatusList = new ObservableCollection<MaritalStatas>();

        }

        #region properties

        public ObservableCollection<CommoditySupplier> CommoditySuppliersList { get; set; }
        public ObservableCollection<CommodityOwnerType> CommodityOwnerTypesList { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatusList { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public const string CommodityOwnerPropertyName = "CommodityOwner";
        private CommodityOwner _commodityOwner = null;

        public CommodityOwner CommodityOwner
        {
            get { return _commodityOwner; }

            set
            {
                if (_commodityOwner == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityOwnerPropertyName);
                _commodityOwner = value;
                RaisePropertyChanged(CommodityOwnerPropertyName);
            }
        }
         
        public const string SelectedCommodityOwnerTypePropertyName = "SelectedCommodityOwnerType";
        private CommodityOwnerType _commodityOwnerType = null;
        public CommodityOwnerType SelectedCommodityOwnerType
        {
            get
            {
                return _commodityOwnerType;
            }

            set
            {
                if (_commodityOwnerType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityOwnerTypePropertyName);
                _commodityOwnerType = value;
                RaisePropertyChanged(SelectedCommodityOwnerTypePropertyName);
            }
        }

        public const string SelectedMaritalStatusPropertyName = "SelectedMaritalStatus";
        private MaritalStatas _selectedMaritalStatud = MaritalStatas.Unknown;
        public MaritalStatas SelectedMaritalStatus
        {
            get
            {
                return _selectedMaritalStatud;
            }

            set
            {
                if (_selectedMaritalStatud == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedMaritalStatusPropertyName);
                _selectedMaritalStatud = value;
                RaisePropertyChanged(SelectedMaritalStatusPropertyName);
            }
        }

        public const string SelectedCommoditySupplierPropertyName = "SelectedCommoditySupplier";
        private CommoditySupplier _selectedCommoditySupplier = null;

        public CommoditySupplier SelectedCommoditySupplier
        {
            get { return _selectedCommoditySupplier; }

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

        private CommoditySupplier _defaultCommoditySupplier;

        private CommoditySupplier DefaultCommoditySupplier
        {
            get
            {
                return _defaultCommoditySupplier ??
                       (_defaultCommoditySupplier =
                        new CommoditySupplier(Guid.Empty) {Name = "--Select commodity producer--"});
            }
        }

        private CommodityOwnerType _defaultCommodityOwnerType;

        private CommodityOwnerType DefaultCommodityOwnerType
        {
            get
            {
                return _defaultCommodityOwnerType ??
                       (_defaultCommodityOwnerType =
                        new CommodityOwnerType(Guid.Empty) { Name = "--Select commodity owner type--" });
            }
        }
         
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Commodity Owner";
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

                RaisePropertyChanging(PageTitlePropertyName);
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }
         
        public const string DateOfBirthPropertyName = "DateOfBirth";
        private DateTime  _dob = DateTime.Now;
        public DateTime  DateOfBirth
        {
            get
            {
                return _dob;
            }

            set
            {
                if (_dob == value)
                {
                    return;
                }

                RaisePropertyChanging(DateOfBirthPropertyName);
                _dob = value;
                RaisePropertyChanged(DateOfBirthPropertyName);
            }
        }

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid oommodityProducerId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (oommodityProducerId == Guid.Empty)
                {
                    CommodityOwner = new CommodityOwner(Guid.NewGuid());
                }
                else
                {
                    var commodityProducer = Using<ICommodityOwnerRepository>(c).GetById(oommodityProducerId);
                    CommodityOwner = commodityProducer.Clone<CommodityOwner>();
                    PageTitle = "Edit Commodity Owner";
                    DateOfBirth = CommodityOwner.DateOfBirth;
                }
                Setup();

                if (CommodityOwner._Status == EntityStatus.New) return;
                SelectedCommoditySupplier = CommoditySuppliersList.FirstOrDefault(n => n.Id == CommodityOwner.CommoditySupplier.Id);
                SelectedMaritalStatus = CommodityOwner.MaritalStatus;
                SelectedCommodityOwnerType = CommodityOwnerTypesList.FirstOrDefault(n => n.Id == CommodityOwner.CommodityOwnerType.Id);
            }
        }

        private void Setup()
        {
            
            LoadCommoditySuppliers();
            LoadCommodityOwnerTypes();
            LoadMaritalStatuses();
        }

        private void LoadCommoditySuppliers()
        {
            using (var c = NestedContainer)
            {
                CommoditySuppliersList.Clear();
                CommoditySuppliersList.Add(DefaultCommoditySupplier);
                SelectedCommoditySupplier = DefaultCommoditySupplier;
                Using<ICommoditySupplierRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(
                    n => CommoditySuppliersList.Add(n as CommoditySupplier));
            }
        }

        private void LoadCommodityOwnerTypes()
        {
            using (var c = NestedContainer)
            {
                CommodityOwnerTypesList.Clear();
                CommodityOwnerTypesList.Add(DefaultCommodityOwnerType);
                SelectedCommodityOwnerType = DefaultCommodityOwnerType;
                Using<ICommodityOwnerTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(
                    n => CommodityOwnerTypesList.Add(n as CommodityOwnerType));
            }
        }

        private void LoadMaritalStatuses()
        {
            Type _enumType = typeof(MaritalStatas);
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            MaritalStatusList.Clear();
            foreach (FieldInfo fi in infos)
                MaritalStatusList.Add((MaritalStatas)Enum.Parse(_enumType, fi.Name, true));
            SelectedMaritalStatus = MaritalStatas.Unknown;
        }

        private async void Save()
        {
            CommodityOwner.CommodityOwnerType = SelectedCommodityOwnerType;
            CommodityOwner.CommoditySupplier = SelectedCommoditySupplier;
            CommodityOwner.MaritalStatus = SelectedMaritalStatus;
            CommodityOwner.DateOfBirth = DateOfBirth;
            CommodityOwner._SetStatus(EntityStatus.Active);

            using (var c = NestedContainer)
            {
                if (!IsValid()) return;
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.CommodityOwnerAddAsync(Map(CommodityOwner));
                string log = string.Format("Created commodity owner: {0}; Code: {1}; In Account {2}",
                                           CommodityOwner.FullName,
                                           CommodityOwner.Code, CommodityOwner.CommoditySupplier.Name);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Commodity Owner Management", log);

                MessageBox.Show(response.ErrorInfo, "Distributr: Add/ Edit Commodity Owner", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if (response.Success)
                {
                    SendNavigationRequestMessage(
                        new Uri("views/admin/commodityowners/listcommodityowners.xaml", UriKind.Relative));
                }
            }
        }

        CommodityOwnerItem Map(CommodityOwner co)
        {
            var item = new CommodityOwnerItem
                           {
                               BusinessNumber = co.BusinessNumber,
                               Code = co.Code,
                               CommodityOwnerTypeMasterId = co.CommodityOwnerType.Id,
                               CommoditySupplierMasterId = co.CommoditySupplier.Id,
                               DateCreated = co._DateCreated,
                               DateLastUpdated = co._DateLastUpdated,
                               DateOfBirth = co.DateOfBirth,
                               Description = co.Description,
                               Email = co.Email,
                               FaxNumber = co.FaxNumber,
                               FirstName = co.FirstName,
                               GenderId = (int) co.Gender,
                               IdNo = co.IdNo,
                               LastName = co.LastName,
                               MaritalStatasMasterId = (int) co.MaritalStatus,
                               MasterId = co.Id,
                               OfficeNumber = co.OfficeNumber,
                               PhoneNumber = co.PhoneNumber,
                               PhysicalAddress = co.PhysicalAddress,
                               PinNo = co.PinNo,
                               PostalAddress = co.PostalAddress,
                               StatusId = (int) co._Status,
                               Surname = co.Surname
                           };
            return item;
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit commodity owner details", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(new Uri(@"\views\admin\commodityowners\listcommodityowners.xaml",
                                                                            UriKind.Relative));
            }
        }

        #endregion

    }
}
