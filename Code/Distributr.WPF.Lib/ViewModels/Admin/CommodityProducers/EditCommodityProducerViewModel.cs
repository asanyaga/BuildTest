using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Workflow.Impl.AuditLogs;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers
{
    public class EditCommodityProducerViewModel : DistributrViewModelBase
    {
        public EditCommodityProducerViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AssignSelectedCommand = new RelayCommand(AssignCentre);
            AssignAllCommand = new RelayCommand(AssignAllCentres);
            UnassignSelectedCommand = new RelayCommand(UnassignCentre);
            UnassignAllCommand = new RelayCommand(UnassignAllCentres);

            CommoditySupplierList = new ObservableCollection<CommoditySupplier>();
            AssignedCentresList = new ObservableCollection<VMCentreItem>();
            UnassignedCentresList = new ObservableCollection<VMCentreItem>();
        }

        #region properties

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand AssignSelectedCommand { get; set; }
        public RelayCommand AssignAllCommand { get; set; }
        public RelayCommand UnassignSelectedCommand { get; set; }
        public RelayCommand UnassignAllCommand { get; set; }

        public ObservableCollection<CommoditySupplier> CommoditySupplierList { get; set; }
        public ObservableCollection<VMCentreItem> AssignedCentresList { get; set; }
        public ObservableCollection<VMCentreItem> UnassignedCentresList { get; set; }

        public const string CommodityProducerPropertyName = "CommodityProducer";
        private CommodityProducer _commodityProducer = null;

        public CommodityProducer CommodityProducer
        {
            get { return _commodityProducer; }

            set
            {
                if (_commodityProducer == value)
                {
                    return;
                }

                RaisePropertyChanging(CommodityProducerPropertyName);
                _commodityProducer = value;
                RaisePropertyChanged(CommodityProducerPropertyName);
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
                        new CommoditySupplier(Guid.Empty) {Name = "--Select commodity supplier--"});
            }
        }
         
        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "Create Commodity Producer";
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

        #endregion

        #region methods

        protected override void LoadPage(Page page)
        {
            Guid oommodityProducerId = PresentationUtility.ParseIdFromUrl(page.NavigationService.CurrentSource);
            using (var c = NestedContainer)
            {
                if (oommodityProducerId == Guid.Empty)
                {
                    CommodityProducer = new CommodityProducer(Guid.NewGuid());
                    CommodityProducer.CommodityProducerCentres = new List<Centre>();
                    PageTitle = "Create Commodity Producer";
                }
                else
                {
                    var commodityProducer = Using<ICommodityProducerRepository>(c).GetById(oommodityProducerId);
                    CommodityProducer = commodityProducer.Clone<CommodityProducer>();
                    PageTitle = "Edit Commodity Producer";
                }
                Setup();
                if (CommodityProducer._Status != EntityStatus.New)
                    SelectedCommoditySupplier =
                        CommoditySupplierList.FirstOrDefault(n => n.Id == CommodityProducer.CommoditySupplier.Id);
            }
        }

        private void Setup()
        {
            LoadCommoditySupplierList();
            LoadCentresLists();
        }

        private void LoadCommoditySupplierList()
        {
            CommoditySupplierList.Clear();
            using (var c = NestedContainer)
            {
                var list =
                    Using<ICommoditySupplierRepository>(c).GetAll().ToList();
                CommoditySupplierList.Add(DefaultCommoditySupplier);
                SelectedCommoditySupplier = DefaultCommoditySupplier;
                list.ForEach(
                    n => CommoditySupplierList.Add(n as CommoditySupplier));
            }
        }

        private void LoadCentresLists()
        {
            AssignedCentresList.Clear();
            UnassignedCentresList.Clear();

            if (CommodityProducer.CommodityProducerCentres != null)
                CommodityProducer.CommodityProducerCentres.OrderBy(n => n.Name).ToList().ForEach(
                    n => AssignedCentresList.Add(new VMCentreItem {Centre = n, IsSelected = false}));

            using (var c = NestedContainer)
            {
                var allCentres = Using<ICentreRepository>(c).GetAll();
                allCentres.Where(n => AssignedCentresList.All(p => p.Centre.Id != n.Id)).OrderBy(
                    n => n.Name).ToList().ForEach
                    (n => UnassignedCentresList.Add(new VMCentreItem {Centre = n, IsSelected = false}));
            }
        }

        private void AssignCentre()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            UnassignedCentresList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach(var item in selected)
            {
                if(AssignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    AssignedCentresList.Add(item);
                    UnassignedCentresList.Remove(item);
                }
            }
        }

        private void AssignAllCentres()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            UnassignedCentresList.ToList().ForEach(selected.Add);
            foreach(var item in selected)
            {
                if(AssignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    AssignedCentresList.Add(item);
                    UnassignedCentresList.Remove(item);
                }
            }
        }

        private void UnassignCentre()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            AssignedCentresList.Where(n => n.IsSelected).ToList().ForEach(selected.Add);
            foreach(var item in selected)
            {
                if(UnassignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    UnassignedCentresList.Add(item);
                    AssignedCentresList.Remove(item);
                }
            }
        }

        private void UnassignAllCentres()
        {
            List<VMCentreItem> selected = new List<VMCentreItem>();
            AssignedCentresList.ToList().ForEach(selected.Add);
            foreach(var item in selected)
            {
                if(UnassignedCentresList.All(n => n.Centre.Id != item.Centre.Id))
                {
                    UnassignedCentresList.Add(item);
                    AssignedCentresList.Remove(item);
                }
            }
        }

        private void Cancel()
        {
            if (MessageBox.Show("Unsaved changes will be lost. Do you want to continue?", "Agrimanagr: Edit commodity producer details", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                SendNavigationRequestMessage(new Uri(@"\views\admin\commodityproducers\listcommodityproducers.xaml",
                                                                            UriKind.Relative));
            }
        }

        private async void Save()
        {
            CommodityProducer.CommodityProducerCentres.Clear();
            CommodityProducer.CommodityProducerCentres.AddRange(AssignedCentresList.Select(n => n.Centre));
            CommodityProducer.CommoditySupplier = SelectedCommoditySupplier;
            if (!IsValid()) return;
            using (StructureMap.IContainer c = NestedContainer)
            {
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ResponseBool response = null;
                response = await proxy.CommodityProducerAddAsync(CommodityProducer);
                string log = string.Format("Created commodity producer: {0}; Code: {1}; In Account {2}", CommodityProducer.Name,
                                             CommodityProducer.Code, CommodityProducer.CommoditySupplier.Name);
                Using<IAuditLogWFManager>(c).AuditLogEntry("Commodity Producer Management", log);

                MessageBox.Show(response.ErrorInfo, "Distributr: Add/ Edit Commodity Producer", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                if(response.Success)
                {
                        SendNavigationRequestMessage(
                            new Uri("views/admin/commodityproducers/listcommodityproducers.xaml", UriKind.Relative));
                }
            }
        }

        #endregion

    }

    #region helpers

    public class VMCentreItem
    {
        public Centre Centre { get; set; }
        public bool IsSelected { get; set; }
    }

    #endregion

}
