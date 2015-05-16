using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using Distributr.WPF.Lib.ViewModels.Admin.CommodityProducers;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers
{
    public class EditCommodityProducerModalViewModel : DistributrViewModelBase
    {

        public EditCommodityProducerModalViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AssignSelectedCommand = new RelayCommand(AssignCentre);
            AssignAllCommand = new RelayCommand(AssignAllCentres);
            UnassignSelectedCommand = new RelayCommand(UnassignCentre);
            UnassignAllCommand = new RelayCommand(UnassignAllCentres);

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
        public bool DialogResult { get; set; }

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

        public event EventHandler CloseDialog = (s, e) => { };

        #endregion

        #region methods

        public void Load(CommodityProducer commodityProducer)
        {
            using (var c = NestedContainer)
            {
                if (commodityProducer.Id == Guid.Empty)
                {
                    CommodityProducer = new CommodityProducer(Guid.NewGuid()){CommoditySupplier = commodityProducer.CommoditySupplier};
                    CommodityProducer.CommodityProducerCentres = new List<Centre>();
                    PageTitle = "Add Farm to Account "+ CommodityProducer.CommoditySupplier.Name;
                }
                else
                {
                    CommodityProducer = commodityProducer.Clone<CommodityProducer>();
                    PageTitle = "Edit Farm for Account " + CommodityProducer.CommoditySupplier.Name;
                }
                Setup();
            }
        }

        private void Setup()
        {
            LoadCentresLists();
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
                DialogResult = false;
                CloseDialog(this, null);
            }
            
        }

        private void Save()
        {
            CommodityProducer.CommodityProducerCentres.Clear();
            CommodityProducer.CommodityProducerCentres.AddRange(AssignedCentresList.Select(n => n.Centre));
            if (!IsValid() || !IsValid(CommodityProducer)) return;
            DialogResult = true;
            CloseDialog(this, null);
        }

        #endregion
    }
}
