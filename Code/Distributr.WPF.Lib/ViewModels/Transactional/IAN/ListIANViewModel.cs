using System;
using System.Windows;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Collections.Generic;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Transactional.IAN
{
    public class ListIANViewModel:DistributrViewModelBase
    {
        public ObservableCollection<ListIANItemViewModel> IANList { get; set; }
        public ObservableCollection<ListIANItemViewModel> StockTakeList { get; set; }
        public RelayCommand LoadIANs { get; set; }

        public ListIANViewModel()
        {
            LoadIANs = new RelayCommand(RunLoadIANs);
            StockTakeList = new ObservableCollection<ListIANItemViewModel>();
            IANList = new ObservableCollection<ListIANItemViewModel>();
        }

        public void RunLoadIANs()
        {
            try
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    List<InventoryAdjustmentNote> itns = null;
                    Guid disDocid = Guid.Empty;
                    if (Guid.TryParse(Id, out disDocid))
                    {
                        Id = "";
                        itns =
                 Using<IInventoryAdjustmentNoteRepository>(c).GetAll().OfType<InventoryAdjustmentNote>() .Where(n =>
                                                                           n.InventoryAdjustmentNoteType !=
                                                                           InventoryAdjustmentNoteType.StockTake
                                                                           && n.Id == disDocid)
                                                           .OrderByDescending(o => o.DocumentDateIssued).ToList();
                    }
                    else
                    {
                        itns =
                           Using<IInventoryAdjustmentNoteRepository>(c).GetAll().OfType<InventoryAdjustmentNote>().Where(
                                n =>
                                n.InventoryAdjustmentNoteType != InventoryAdjustmentNoteType.StockTake
                                && n.DocumentDateIssued >= StartDate && n.DocumentDateIssued <= EndDate.AddDays(1))
                                                           .OrderByDescending(o => o.DocumentDateIssued).ToList();
                    }
                    IANList.Clear();
                    itns.ForEach(n => n.LineItem.ForEach(x => IANList.Add(new ListIANItemViewModel
                        {
                            CreatedBy = n.DocumentIssuerUser == null ? "--" : n.DocumentIssuerUser.Username,
                            DocumentRef = n.DocumentReference,
                            DateAdjusted = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                            ProductDescription = x.Product.Description.ToString(),
                            ExpectedQuantity = x.Qty,
                            ActualQuantity = x.Actual,
                            Reason = x.Description,
                        })));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LoadStockTakeReport()
        {
            StockTakeList.Clear();
            try
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    List<InventoryAdjustmentNote> itns = null;
                    Guid disDocid = Guid.Empty;
                    if (Guid.TryParse(Id, out disDocid))
                    {
                        Id = "";
                        itns =
                 Using<IInventoryAdjustmentNoteRepository>(c).GetAll().OfType<InventoryAdjustmentNote>().Where(n =>
                                                                           n.InventoryAdjustmentNoteType ==
                                                                           InventoryAdjustmentNoteType.StockTake
                                                                           && n.Id == disDocid)
                                                           .OrderByDescending(o => o.DocumentDateIssued).ToList();
                    }
                    else
                    {
                        itns =
                           Using<IInventoryAdjustmentNoteRepository>(c).GetAll().OfType<InventoryAdjustmentNote>().Where(
                                n =>
                                n.InventoryAdjustmentNoteType == InventoryAdjustmentNoteType.StockTake
                                && n.DocumentDateIssued >= StartDate && n.DocumentDateIssued <= EndDate.AddDays(1))
                                                           .OrderByDescending(o => o.DocumentDateIssued).ToList();
                    }
                    itns.ForEach(n => n.LineItem.ForEach(x => StockTakeList.Add(new ListIANItemViewModel
                        {
                            CreatedBy = n.DocumentIssuerUser == null ? "--" : n.DocumentIssuerUser.Username,
                            DocumentRef = n.DocumentReference,
                            DateAdjusted = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                            ProductDescription = x.Product.Description.ToString(),
                            ExpectedQuantity = x.Qty,
                            ActualQuantity = x.Actual,
                            Reason = x.Description,
                        })));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Properties
        /// <summary>
        /// The <see cref="EndDate" /> property's name.
        /// </summary>
        public const string EndDatePropertyName = "EndDate";
        private DateTime _EndDate = DateTime.Now;
        /// <summary>
        /// Gets the EndDate property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }

            set
            {
                if (_EndDate == value)
                {
                    return;
                }

                var oldValue = _EndDate;
                _EndDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="StartDate" /> property's name.
        /// </summary>
        public const string StartDatePropertyName = "StartDate";
        private DateTime _StartDate = DateTime.Now.AddDays(-7);
        /// <summary>
        /// Gets the StartDate property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }

            set
            {
                if (_StartDate == value)
                {
                    return;
                }

                var oldValue = _StartDate;
                _StartDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        public const string IdPropertyName = "Id";

        private string _id = "";

        /// <summary>
        /// Gets the Id property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Id
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
                RaisePropertyChanged(IdPropertyName);

            
            }
        }
        #endregion

    }

    public class ListIANItemViewModel : ViewModelBase
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

        /// <summary>
        /// The <see cref="DateAdjusted" /> property's name.
        /// </summary>
        public const string DateAdjustedPropertyName = "DateAdjusted";
        private string _DateAdjusted = null;
        /// <summary>
        /// Gets the DateAdjusted property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string DateAdjusted
        {
            get
            {
                return _DateAdjusted;
            }

            set
            {
                if (_DateAdjusted == value)
                {
                    return;
                }

                var oldValue = _DateAdjusted;
                _DateAdjusted = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(DateAdjustedPropertyName);
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
        /// The <see cref="ExpectedQuantity" /> property's name.
        /// </summary>
        public const string ExpectedQuantityPropertyName = "ExpectedQuantity";
        private decimal _ExpectedQuantity = 0;
        /// <summary>
        /// Gets the ExpectedQuantity property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal ExpectedQuantity
        {
            get
            {
                return _ExpectedQuantity;
            }

            set
            {
                if (_ExpectedQuantity == value)
                {
                    return;
                }

                var oldValue = _ExpectedQuantity;
                _ExpectedQuantity = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ExpectedQuantityPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ActualQuantity" /> property's name.
        /// </summary>
        public const string ActualQuantityPropertyName = "ActualQuantity";
        private decimal _ActualQuantity = 0;
        /// <summary>
        /// Gets the ActualQuantity property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal ActualQuantity
        {
            get
            {
                return _ActualQuantity;
            }

            set
            {
                if (_ActualQuantity == value)
                {
                    return;
                }

                var oldValue = _ActualQuantity;
                _ActualQuantity = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ActualQuantityPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Reason" /> property's name.
        /// </summary>
        public const string ReasonPropertyName = "Reason";

        private string _Reason = "";

        /// <summary>
        /// Gets the Reason property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Reason
        {
            get
            {
                return _Reason;
            }

            set
            {
                if (_Reason == value)
                {
                    return;
                }

                var oldValue = _Reason;
                _Reason = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(ReasonPropertyName);

             
            }
        }
    }
}
