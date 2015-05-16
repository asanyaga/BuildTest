using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers
{
    public class EditCommodityOwnerModalViewModel: DistributrViewModelBase
    {
        public EditCommodityOwnerModalViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            CommodityOwnerTypesList = new ObservableCollection<CommodityOwnerType>();
            MaritalStatusList = new ObservableCollection<MaritalStatas>();

        }

        public ObservableCollection<CommodityOwnerType> CommodityOwnerTypesList { get; set; }
        public ObservableCollection<MaritalStatas> MaritalStatusList { get; set; }

        #region properties
        public event EventHandler CloseDialog = (s, e) => { };
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public bool DialogResult { get; set; }

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
                //if (_selectedMaritalStatud == value)
                //{
                //    return;
                //}

                RaisePropertyChanging(SelectedMaritalStatusPropertyName);
                _selectedMaritalStatud = value;
                RaisePropertyChanged(SelectedMaritalStatusPropertyName);
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
        private string _pageTitle = "Create Farmer";
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

        public void Load(CommodityOwner commodityOwner)
        {
            using (var c = NestedContainer)
            {
                if (commodityOwner.Id == Guid.Empty)
                {
                    CommodityOwner = new CommodityOwner(Guid.NewGuid()){CommoditySupplier = commodityOwner.CommoditySupplier};
                    PageTitle = "Add Farmer to Account " + CommodityOwner.CommoditySupplier.Name;
                }
                else
                {
                    CommodityOwner = commodityOwner.Clone<CommodityOwner>();
                    PageTitle = "Edit Farmer for Account " + CommodityOwner.CommoditySupplier.Name;
                }
                Setup();
            }
        }

        private void Setup()
        {
            LoadCommodityOwnerTypes();
            LoadMaritalStatuses();
        }

        private void LoadCommodityOwnerTypes()
        {
            using (var c = NestedContainer)
            {
                CommodityOwnerTypesList.Clear();
                CommodityOwnerTypesList.Add(DefaultCommodityOwnerType);
                Using<ICommodityOwnerTypeRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(
                    n => CommodityOwnerTypesList.Add(n as CommodityOwnerType));
                SelectedCommodityOwnerType = DefaultCommodityOwnerType;

                if (CommodityOwner.CommodityOwnerType != null)
                    SelectedCommodityOwnerType = CommodityOwnerTypesList.FirstOrDefault(n => n.Id == CommodityOwner.CommodityOwnerType.Id);
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

            SelectedMaritalStatus = MaritalStatusList.FirstOrDefault(n => n == CommodityOwner.MaritalStatus);
        }

        private void Save()
        {
            CommodityOwner.CommodityOwnerType = SelectedCommodityOwnerType;
            CommodityOwner.MaritalStatus = SelectedMaritalStatus;
            CommodityOwner.DateOfBirth = DateOfBirth;
            if(DateOfBirth.Year < 18)
            {
                MessageBox.Show("Farmer must be over 18 years old.", "Agrimanagr: Farmer Management",
                                MessageBoxButton.OK);
                return;
            }
            CommodityOwner._SetStatus(EntityStatus.Active);

                if (!IsValid()|| !IsValid(CommodityOwner)) return;
                DialogResult = true;
                CloseDialog(this, null);
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit commodity owner details", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                DialogResult = false;
                CloseDialog(this, null);
            }
        }

        #endregion
    }
}
