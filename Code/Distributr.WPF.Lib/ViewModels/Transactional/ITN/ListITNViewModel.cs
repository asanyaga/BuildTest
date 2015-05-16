using System;
using System.Windows;
using System.Windows.Controls;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Resources.Util;
using System.Collections.ObjectModel;
using Distributr.WPF.Lib.ViewModels.Printerutilis;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ITN
{
    public class ListITNViewModel : DistributrViewModelBase
    {
       
        public ObservableCollection<ListITNItemViewModel> ItnList { get; set; }
        public RelayCommand LoadItNs { get; set; }
        public RelayCommand LoadWareHouses { get; set; }
        public ObservableCollection<Warehouse> SalesMen { get; set; }

        public RelayCommand<object> PrintReportCommand { get; set; }
       

        public ListITNViewModel()
        {
            
            


            LoadItNs = new RelayCommand(RunLoadITNs);
            LoadWareHouses = new RelayCommand(LoadSalesMen);
            ItnList = new ObservableCollection<ListITNItemViewModel>();
            SalesMen = new ObservableCollection<Warehouse>();
            PrintReportCommand = new RelayCommand<object>(PrintReport);
        }

        private void PrintReport(object param)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == false)
                return;
            string documentTitle = string.Format("Inventory Transfers Report-{0}", DateTime.Now.ToLocalTime());
           
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            CustomDataGridDocumentPaginator paginator = new CustomDataGridDocumentPaginator(param as DataGrid, documentTitle, pageSize, new Thickness(30, 20, 30, 20));
            printDialog.PrintDocument(paginator, "Grid");
        }

        void RunLoadITNs()
        {
            using (var container = NestedContainer)
            {

              
                var _inventoryTransferNoteService =
                    Using<IInventoryTransferNoteRepository>(container);
                if (EndDate.ToShortDateString() == DateTime.Now.ToShortDateString())
                    EndDate = DateTime.Now;
                var endDate = EndDate;
                EndDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
              
                var itns =
                    _inventoryTransferNoteService.GetAll().OfType<InventoryTransferNote>().Where(
                        n => n.DocumentDateIssued >=StartDate && n.DocumentDateIssued <= EndDate).ToList();
                if (SelectedSaleMan != null)
                    if (SelectedSaleMan.Id != Guid.Empty)
                        itns = itns.Where(n => n.DocumentRecipientCostCentre.Id == SelectedSaleMan.Id).ToList();
                ItnList.Clear();
                itns.ForEach(n => n.LineItems.ForEach(x => ItnList.Add(new ListITNItemViewModel
                                                                            {
                                                                                CreatedBy =
                                                                                    n.DocumentIssuerUser == null
                                                                                        ? "--"
                                                                                        : n.DocumentIssuerUser.Username,
                                                                                DocumentRef = n.DocumentReference,
                                                                                DateTransferred =
                                                                                    n.DocumentDateIssued.ToString(
                                                                                        "dd-MMM-yyyy"),
                                                                                DateIssued = n.DocumentDateIssued,
                                                                                ProductDescription =
                                                                                    x.Product.Description.ToString(),
                                                                                SalesMan =
                                                                                    n.DocumentRecipientCostCentre.Name,
                                                                                IssuedQuantity = x.Qty
                                                                            })));
            }
        }

        public void LoadSalesMen()
        {
            using (var container = NestedContainer)
            {

                var _costCentreService = Using<ICostCentreRepository>(container);
                
                SelectedSaleMan = null;
                SalesMen.Clear();
                var salesman =
                    new Distributor(Guid.Empty)
                        {
                            Name =
                                "--Please select " + GetLocalText("sl.viewInventoryTransfers.salesmen_lbl") +
                                "--"
                        } as Warehouse; //a salesman
                SalesMen.Add(salesman);
                _costCentreService.GetAll().OfType<DistributorSalesman>()
                    .OrderBy(n => n.Name)
                    .ToList()
                    .ForEach(n => SalesMen.Add(n));
                SelectedSaleMan = salesman;
            }
        }

        #region Properties
        public const string EndDatePropertyName = "EndDate";
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                if (_endDate == value)
                {
                    return;
                }

                var oldValue = _endDate;
                _endDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        public const string StartDatePropertyName = "StartDate";
        private DateTime _startDate = DateTime.Now.AddDays(-7);
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                if (_startDate == value)
                {
                    return;
                }

                var oldValue = _startDate;
                _startDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        public const string SelectedSaleManPropertyName = "SelectedSaleMan";
        private Warehouse _SelectedSaleMan;
        public Warehouse SelectedSaleMan
        {
            get
            {
                return _SelectedSaleMan;
            }

            set
            {
                if (_SelectedSaleMan == value)
                {
                    return;
                }

                var oldValue = _SelectedSaleMan;
                _SelectedSaleMan = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedSaleManPropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(SelectedSaleManPropertyName, oldValue, value, true);
            }
        }
        #endregion
    }

    #region Helper Classes
    public class ListITNItemViewModel : ViewModelBase
    {
        public const string DocumentRefPropertyName = "DocumentRef";
        private string _documentRef = "";
        public string DocumentRef
        {
            get
            {
                return _documentRef;
            }

            set
            {
                if (_documentRef == value)
                    return;
                var oldValue = _documentRef;
                _documentRef = value;
                RaisePropertyChanged(DocumentRefPropertyName);
            }
        }

        public const string DateTransferredPropertyName = "DateTransferred";
        private string _DateTransferred = null;
        public string DateTransferred
        {
            get
            {
                return _DateTransferred;
            }

            set
            {
                if (_DateTransferred == value)
                {
                    return;
                }

                var oldValue = _DateTransferred;
                _DateTransferred = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(DateTransferredPropertyName);
            }
        }

        public const string ProductDescriptionPropertyName = "ProductDescription";
        private string _ProductDescription = null;
        public string ProductDescription
        {
            get
            {
                return _ProductDescription;
            }

            set
            {
                if (_ProductDescription == value)
                {
                    return;
                }

                var oldValue = _ProductDescription;
                _ProductDescription = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductDescriptionPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DateIssued" /> property's name.
        /// </summary>
        public const string DateIssuedPropertyName = "DateIssued";
        private DateTime _DateIssued = DateTime.Now;
        /// <summary>
        /// Gets the DateIssued property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime DateIssued
        {
            get
            {
                return _DateIssued;
            }

            set
            {
                if (_DateIssued == value)
                {
                    return;
                }

                var oldValue = _DateIssued;
                _DateIssued = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(DateIssuedPropertyName);
            }
        }

        public const string CreatedByPropertyName = "CreatedBy";
        private string _createdBy = "";
        public string CreatedBy
        {
            get
            {
                return _createdBy;
            }

            set
            {
                if (_createdBy == value)
                    return;
                var oldValue = _createdBy;
                _createdBy = value;
                RaisePropertyChanged(CreatedByPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IssuedQuantity" /> property's name.
        /// </summary>
        public const string IssuedQuantityPropertyName = "IssuedQuantity";
        private decimal _IssuedQuantity = 0;
        /// <summary>
        /// Gets the IssuedQuantity property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal IssuedQuantity
        {
            get
            {
                return _IssuedQuantity;
            }

            set
            {
                if (_IssuedQuantity == value)
                {
                    return;
                }

                var oldValue = _IssuedQuantity;
                _IssuedQuantity = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IssuedQuantityPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SalesMan" /> property's name.
        /// </summary>
        public const string SalesManPropertyName = "SalesMan";
        private string _SalesMan = null;
        /// <summary>
        /// Gets the SalesMan property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string SalesMan
        {
            get
            {
                return _SalesMan;
            }

            set
            {
                if (_SalesMan == value)
                {
                    return;
                }

                var oldValue = _SalesMan;
                _SalesMan = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SalesManPropertyName);
            }
        }
    }

    public class ListWarehouseViewModel : ViewModelBase
    {
        /// <summary>
        /// The <see cref="id" /> property's name.
        /// </summary>
        public const string idPropertyName = "id";
        private Guid _id = Guid.Empty;
        /// <summary>
        /// Gets the id property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid id
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

                var oldValue = _id;
                _id = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(idPropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(idPropertyName, oldValue, value, true);
            }
        }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _Name;

        /// <summary>
        /// Gets the Name property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (_Name == value)
                {
                    return;
                }

                var oldValue = _Name;
                _Name = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(NamePropertyName);

                // Update bindings and broadcast change using GalaSoft.MvvmLight.Messenging
                RaisePropertyChanged(NamePropertyName, oldValue, value, true);
            }
        }
    }
    #endregion
}
