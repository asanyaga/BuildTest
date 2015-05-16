using System;
using System.Linq;
using System.Windows;
using Distributr.Core.ClientApp.CommandResults;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Admin.CommoditySuppliers
{
    public class SupplierToOutletMappingViewModel : DistributrViewModelBase
    {
        public SupplierToOutletMappingViewModel()
        {
            SelectOutletCommand = new RelayCommand(SelectOutlet);
            SaveCommand= new RelayCommand(Save);
        }

        private async void Save()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if(ChoosenOutlet==null || ChoosenOutlet.Id==Guid.Empty)
                {
                    MessageBox.Show("Select Outlet", "Distributr: Manage Commodity Supplier", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                    return;
                    ;
                }
                
                IDistributorServiceProxy proxy = Using<IDistributorServiceProxy>(c);
                ICostCentreRepository costCentreRepository = Using<ICostCentreRepository>(c);
                ResponseBool response = null;
                var mappping = new CostCentreMapping { Id = ChoosenSupplier.Id, MappToId = ChoosenOutlet.Id };
                response = await proxy.SupplierMappingSaveAsync(mappping);
                if (response.Success)
                    costCentreRepository.SaveMapping(mappping);
                MessageBox.Show(response.ErrorInfo, "Distributr: Manage Commodity Supplier", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                if(response.Success)
                {
                    RequestClose(null, null);
                }
            }
        }

        private void SelectOutlet()
        {
            ChoosenOutlet = null;
            using (var container = NestedContainer)
            {
                var selected = Using<IAgriItemsLookUp>(container).SelectOutletToMapToSupplier();

                ChoosenOutlet = selected;
                if (selected == null)
                {
                    ChoosenOutlet = new Outlet(Guid.Empty) { Name = "--Select Outlet---" };
                }
            }
        }


        public RelayCommand SelectOutletCommand { set; get; }
        public RelayCommand SaveCommand { set; get; }

       
        public const string ChoosenSupplierPropertyName = "ChoosenSupplier";
        private CommoditySupplier _choosensupplier = null;
        public CommoditySupplier ChoosenSupplier
        {
            get
            {
                return _choosensupplier;
            }

            set
            {
                if (_choosensupplier == value)
                {
                    return;
                }

                RaisePropertyChanging(ChoosenSupplierPropertyName);
                _choosensupplier = value;
                RaisePropertyChanged(ChoosenSupplierPropertyName);
            }
        }


       
        public const string ChoosenOutletPropertyName = "ChoosenOutlet";
        private Outlet _choosenOutlet = null;
        public Outlet ChoosenOutlet
        {
            get
            {
                return _choosenOutlet;
            }

            set
            {
                if (_choosenOutlet == value)
                {
                    return;
                }

                RaisePropertyChanging(ChoosenOutletPropertyName);
                _choosenOutlet = value;
                RaisePropertyChanged(ChoosenOutletPropertyName);
            }
        }

        public event EventHandler RequestClose = (s, e) => { };

        public void Setup(CommoditySupplier supplier)
        {
            ChoosenSupplier = supplier;
            using (var container = NestedContainer)
            {
                var ctx = Using<CokeDataContext>(container);
                var mapping = ctx.tblCostCentreMapping.FirstOrDefault(s=>s.Id== supplier.Id);
                if(mapping!=null)
                {
                    ChoosenOutlet = Using<ICostCentreRepository>(container).GetById(mapping.MapToCostCentreId) as Outlet;
                    
                }
                else
                {
                    ChoosenOutlet=new Outlet(Guid.Empty){Name = "----Select Outlet----"};
                }
            }

        }
    }
}