using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Repository.MasterData.CostCentreRepositories;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.SuppliersRepositories;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Distributr.WPF.Lib.UI.Pages;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Master.SuppliersEntities;

namespace Distributr.WPF.Lib.ViewModels.Admin.SalesmanSupplier
{
    public class SalesmanSupplierViewModel : DistributrViewModelBase
    {
        
        public SalesmanSupplierViewModel()
        {
            SalemanSupplierItems = new ObservableCollection<SalesmanSupplierViewModel>();

            LoadSalesmanSupplierItemsCommand = new RelayCommand(LoadSalesmanSupplierItems);
            SalesmanDropDownOpenedCommand = new RelayCommand(SalesmanDropDownOpened);
            SuppliersList = new ObservableCollection<Distributr.Core.Domain.Master.SuppliersEntities.Supplier>();
            SaveCommand = new RelayCommand(Save);
        }

       

        #region COMMANDS AND COLLECTION
        public RelayCommand SalesmanDropDownOpenedCommand { get; set; }
        public ObservableCollection<SalesmanSupplierViewModel> SalemanSupplierItems { get; set; }

        public ObservableCollection<Distributr.Core.Domain.Master.SuppliersEntities.Supplier> SuppliersList { get; set; }
        public RelayCommand LoadSalesmanSupplierItemsCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        #endregion
        #region PROPERTIES
        
        public const string SelectedSalesmanPropertyName = "SelectedSalesman";
        private CostCentre _selectedSalesman = null;
        public CostCentre  SelectedSalesman
        {
            get
            {
                return _selectedSalesman;
            }

            set
            {
                if (_selectedSalesman == value)
                {
                    return;
                }

                RaisePropertyChanging(SelectedSalesmanPropertyName);
                _selectedSalesman = value;
                RaisePropertyChanged(SelectedSalesmanPropertyName);
            }
        }


        public const string IdPropertyName = "Id";

        private Guid _id = Guid.Empty;

        public Guid Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        public const string SalesmanPropertyName = "Salesman";

        private CostCentreRef _salesman = null;
        public CostCentreRef Salesman
        {
            get
            {
                return _salesman;
            }

            set
            {
                if (_salesman == value)
                {
                    return;
                }

                RaisePropertyChanging(SalesmanPropertyName);
                _salesman = value;
                RaisePropertyChanged(SalesmanPropertyName);
            }
        }


        public const string IsAssignedPropertyName = "IsAssigned";
        private bool _isAssigned = false;
        public bool IsAssigned
        {
            get
            {
                return _isAssigned;
            }

            set
            {
                if (_isAssigned == value)
                {
                    return;
                }

                RaisePropertyChanging(IsAssignedPropertyName);
                _isAssigned = value;
                RaisePropertyChanged(IsAssignedPropertyName);
            }
        }



        public const string SupplierPropertyName = "Supplier";
        private Distributr.Core.Domain.Master.SuppliersEntities.Supplier _supplier = null;
        public Distributr.Core.Domain.Master.SuppliersEntities.Supplier Supplier
        {
            get
            {
                return _supplier;
            }

            set
            {
                if (_supplier == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierPropertyName);
                _supplier = value;
                RaisePropertyChanged(SupplierPropertyName);
            }
        }

        public const string SupplierNamePropertyName = "SupplierName";

        private string _supplierName = "";

        public string SupplierName
        {
            get
            {
                return _supplierName;
            }

            set
            {
                if (_supplierName == value)
                {
                    return;
                }

                RaisePropertyChanging(SupplierNamePropertyName);
                _supplierName = value;
                RaisePropertyChanged(SupplierNamePropertyName);
            }
        }
    
        #endregion

        #region METHODS
        private void SalesmanDropDownOpened()
        {
            using (var container = NestedContainer)
            {
                SelectedSalesman = Using<IItemsLookUp>(container).SelectDistributrSalesman(); //??default;
            }
           // LoadSalesmanSupplierItems();
            LoadAllSupplier();
        }

        private void LoadSalesmanSupplierItems()
        {
            SalemanSupplierItems.Clear();
            if (SelectedSalesman != null)
            {
                SalemanSupplierItems.Add(new SalesmanSupplierViewModel ());
                using (StructureMap.IContainer c = NestedContainer)
                {
                    List<Core.Domain.Master.CostCentreEntities.SalesmanSupplier> assign = Using<SalesmanSupplierRepository >(c)
                        .GetAll().ToList();
                    /*.Where(p => p.DistributorSalesmanRef.Id == SelectedSalesman.Id).*/
                  //SalemanSupplierItems =  assign.Select(p => Map(p)).ToList() ;
                    foreach (var item in assign)
                    {
                        SalemanSupplierItems.Add(Map(item));
                        //var i = SalemanSupplierItems.FirstOrDefault(p => p.Id == item.Id);
                        //if (i != null)
                        //{
                        
                        //}
                    }
                }

            }
            LoadAllSupplier();


        }
        public void LoadAllSupplier()
        {
            
    
            using (var c = NestedContainer)
            {
                //var ctx = Using<CokeDataContext>(c);
                //var suppliers =
                //    ctx.tblSupplier.Where(n => n.IM_Status == (int)EntityStatus.Active).ToList();//.Select(n => n.Name).ToList();
                var suppliers = Using<ISupplierRepository>(c).GetAll().ToList();
                 
                foreach (var supplier in suppliers)
                {
                    var supplierItem = new SalesmanSupplierViewModel();
                    supplierItem.Supplier = supplier;
                    SalemanSupplierItems.Add(supplierItem);
                }
                if (!suppliers.Any()) return;
            }

        }



        private async void Save()
        {
            if (SelectedSalesman == null)
            {
                MessageBox.Show("A Salesman has to be selected first", "Salesman Supplier", MessageBoxButton.OK);
            }
            else
            {
                using (var c = NestedContainer)
                {
                    Core.Domain.Master.CostCentreEntities.SalesmanSupplier sSupplier =
                        Using<ISalesmanSupplierRepository>(c).GetById(Id);
                    SalemanSupplierItems.FirstOrDefault().SelectedSalesman = SelectedSalesman;
                    var assignedSupplier = SalemanSupplierItems.Where(n => n.IsAssigned).ToList();
              
                    foreach (var item in assignedSupplier)
                    {
                        item.Salesman = new CostCentreRef() {Id = SelectedSalesman.Id};

                        if (sSupplier == null)
                        {
                            sSupplier = new Core.Domain.Master.CostCentreEntities.SalesmanSupplier(Id );
                        }
                        sSupplier.DistributorSalesmanRef = item.Salesman;
                        sSupplier.Supplier = item.Supplier;
                        sSupplier._Status = EntityStatus.Active;
                        sSupplier._DateCreated = DateTime.Now;
                        sSupplier._DateLastUpdated = DateTime.Now;
                        var response = await Using<IDistributorServiceProxy>(c).SalesmanSupplierSaveAsync(sSupplier);

                        if (response.Success)
                        {
                            MessageBox.Show("SalesmanSupplier Successfully Added", "Distribtr: SalesmanSupplier ",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            // SendNavigationRequestMessage(new Uri("/Views/Admin/FarmActivities/Infections/ListInfection.xaml", UriKind.Relative));
                        }
                        else
                        {

                            MessageBox.Show(response.ErrorInfo, "Distribtr: Error ",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        }


        private SalesmanSupplierViewModel Map(Core.Domain.Master.CostCentreEntities.SalesmanSupplier item)
        {
            var salesmanSupplier = new SalesmanSupplierViewModel();
            salesmanSupplier.Id = item.Id;
            salesmanSupplier.Supplier = item.Supplier;

            salesmanSupplier.SelectedSalesman.Id = item.DistributorSalesmanRef.Id;
            

                 
            return salesmanSupplier;
        }


        #endregion

    }

 




}
