using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.EquipmentRepository;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.Containers
{
    public class EditContainerTypeViewModel : DistributrViewModelBase
    {
        bool _loadcommodityGradesList = true;
        public EditContainerTypeViewModel()
        {
            CommodityGradesList = new ObservableCollection<CommodityGrade>();
            ContainerUseTypeList = new ObservableCollection<ContainerUseType>();
            CommoditiesList = new ObservableCollection<Commodity>();
        }

        #region properties

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        
        private RelayCommand _selectedCommodityChangedCommand;
        public RelayCommand SelectedCommodityChangedCommand
        {
            get
            {
                return _selectedCommodityChangedCommand ??
                       (_selectedCommodityChangedCommand = new RelayCommand(LoadCommodityGradesList));
            }
        }

        public ObservableCollection<CommodityGrade> CommodityGradesList { get; set; }
        public ObservableCollection<Commodity> CommoditiesList { get; set; }
        public ObservableCollection<ContainerUseType> ContainerUseTypeList { get; set; }

        public const string ContainerTypePropertyName = "ContainerType";
        private ContainerType _containerType = null;

        public ContainerType ContainerType
        {
            get { return _containerType; }

            set
            {
                if (_containerType == value)
                {
                    return;
                }

                RaisePropertyChanging(ContainerTypePropertyName);
                _containerType = value;
                RaisePropertyChanged(ContainerTypePropertyName);
            }
        }

        public const string SelectedCommodityPropertyName = "SelectedCommodity";
        private Commodity _selectedCommodity = null;

        public Commodity SelectedCommodity
        {
            get { return _selectedCommodity; }

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

        public const string SelectedContainerUseTypePropertyName = "SelectedContainerUseType";
        private ContainerUseType _selectedContainerUseType = ContainerUseType.Unknown;

        public ContainerUseType SelectedContainerUseType
        {
            get { return _selectedContainerUseType; }

            set
            {
                if (_selectedContainerUseType == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedContainerUseTypePropertyName);
                _selectedContainerUseType = value;
                RaisePropertyChanged(SelectedContainerUseTypePropertyName);
            }
        }

        public const string SelectedCommodityGradePropertyName = "SelectedCommodityGrade";
        private CommodityGrade _selectedCommodityGrade = null;

        public CommodityGrade SelectedCommodityGrade
        {
            get { return _selectedCommodityGrade; }

            set
            {
                if (_selectedCommodityGrade == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedCommodityGradePropertyName);
                _selectedCommodityGrade = value;
                RaisePropertyChanged(SelectedCommodityGradePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Container";

        public string PageTitle
        {
            get { return _pageTitle; }

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

        private Commodity _defaultCommodity;

        private Commodity DefaultCommodity
        {
            get
            {
                return _defaultCommodity ??
                       (_defaultCommodity = new Commodity(Guid.Empty) {Name = "--Select commodity--"});
            }
        }

        private CommodityGrade _defaultCommodityGrade;

        private CommodityGrade DefaultCommodityGrade
        {
            get
            {
                return _defaultCommodityGrade ??
                       (_defaultCommodityGrade = new CommodityGrade(Guid.Empty) {Name = "--Select commodity grade--"});
            }
        }

        public const string CanEditPropertyName = "CanEdit";
private bool _canEdit = false;

        public bool CanEdit
        {
            get { return _canEdit; }

            set
            {
                if (_canEdit == value)
                {
                    return;
                }

                RaisePropertyChanging(CanEditPropertyName);
                _canEdit = value;
                RaisePropertyChanged(CanEditPropertyName);
            }
        }

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            _loadcommodityGradesList = true;
            Guid id = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (id == Guid.Empty)
                {
                    PageTitle = "Create Container Type";
                    ContainerType = new ContainerType(Guid.NewGuid());
                }
                else
                {
                    if (!CanEdit)
                        PageTitle = "View Container Type Details";
                    var cont = Using<IContainerTypeRepository>(c).GetById(id);
                    ContainerType = cont.Clone<ContainerType>();
                }
            }
            Setup();
            if (ContainerType._Status != EntityStatus.New)
            {
                _loadcommodityGradesList = false;
                if (ContainerType.CommodityGrade != null)
                {
                    SelectedCommodityGrade =
                        CommodityGradesList.FirstOrDefault(n => n.Id == ContainerType.CommodityGrade.Id);
                    SelectedCommodity =
                        CommoditiesList.FirstOrDefault(
                            n => n.CommodityGrades.Any(p => p.Id == SelectedCommodityGrade.Id));
                }
            }
        }

        private void Setup()
        {
            LoadCommoditiesList();
            LoadCommodityGradesList();
            LoadContainerUseTypeList();
        }

        private void LoadCommoditiesList()
        {
            CommoditiesList.Clear();
            CommoditiesList.Add(DefaultCommodity);
            SelectedCommodity = DefaultCommodity;
            using (var c = NestedContainer)
            {
                Using<ICommodityRepository>(c).GetAll().OrderBy(n => n.Name).ToList().ForEach(CommoditiesList.Add);
            }
        }

        private void LoadCommodityGradesList()
        {
            if (!_loadcommodityGradesList) return;
            CommodityGradesList.Clear();
            CommodityGradesList.Add(DefaultCommodityGrade);
            SelectedCommodityGrade = DefaultCommodityGrade;
            if (SelectedCommodity != null && SelectedCommodity.Id != Guid.Empty)
            {
                SelectedCommodity.CommodityGrades.OrderBy(n => n.Name).ToList().ForEach(CommodityGradesList.Add);
            }
        }

        private void LoadContainerUseTypeList()
        {
            Type _enumType = typeof (ContainerUseType);
            ContainerUseTypeList.Clear();
            FieldInfo[] infos;
            infos = _enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in infos)
                ContainerUseTypeList.Add((ContainerUseType) Enum.Parse(_enumType, fi.Name, true));
            SelectedContainerUseType = ContainerUseType.Unknown;

        }

        #endregion

    }

}
