using System;
using System.Windows;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Service.Utility;
using Distributr.WPF.Lib.Services.Service.Utility;
using GalaSoft.MvvmLight;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using Distributr.Core.ClientApp;

namespace Distributr.WPF.Lib.ViewModels.Transactional.CN
{
    public class EditCNViewModel : DistributrViewModelBase
    {
        CreditNote note = null;
        public ObservableCollection<EditCNItemsViewModel> LineItems { set; get; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        public RelayCommand LoadInvoiceCommand { get; set; }

        public EditCNViewModel(
        
            )
        {

            LoadInvoiceCommand = new RelayCommand(LoadInvoice);
            SaveCommand = new RelayCommand(SaveCommandCN);
            ConfirmCommand = new RelayCommand(ConfirmCommandCN);
            CancelCommand = new RelayCommand(ClearViewModel);
            LineItems = new ObservableCollection<EditCNItemsViewModel>();
        }

        void ClearViewModel()
        {
            LineItems.Clear();
            InvoiceNo = Guid.Empty;
            InvoiceRef = null;
            TotalGross = 0;
            InvoiceId = Guid.Empty;
            Value = 0;
            Description = null;
        }

        #region Properties
        /// <summary>
        /// The <see cref="InvoiceNo" /> property's name.
        /// </summary>
        public const string InvoiceNoPropertyName = "InvoiceNo";
        private Guid _InvoiceNo = Guid.Empty;
        /// <summary>
        /// Gets the InvoiceNo property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid InvoiceNo
        {
            get
            {
                return _InvoiceNo;
            }

            set
            {
                if (_InvoiceNo == value)
                {
                    return;
                }

                var oldValue = _InvoiceNo;
                _InvoiceNo = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceNoPropertyName);
            }
        }

        #endregion

        public void SaveCommandCN()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                note = new CreditNote(Guid.NewGuid());
                note.DocumentDateIssued = DateTime.Now;
                note.DocumentIssuerCostCentre = Using<ICostCentreRepository>(c).GetById(Using<IConfigService>(c).Load().CostCentreId);
                note.DocumentIssuerCostCentreApplicationId = Using<IConfigService>(c).Load().CostCentreApplicationId;
                note.DocumentIssuerUser = Using<IUserRepository>(c).GetById(Using<IConfigService>(c).ViewModelParameters.CurrentUserId);
                note.DocumentRecipientCostCentre = Using<ICostCentreRepository>(c).GetById( Using<IConfigService>(c).Load().CostCentreId);
                note.DocumentReference = "";
                note.InvoiceId = InvoiceNo;
                note.DocumentType = DocumentType.InventoryTransferNote;

                List<CreditNoteLineItem> ListitnLineitem = new List<CreditNoteLineItem>();
                //foreach (EditCNItemsViewModel item in LineItems)
                //{
                CreditNoteLineItem itnLineitem = new CreditNoteLineItem(Guid.NewGuid())
                    {
                        Description = Description,
                        LineItemSequenceNo = 0,
                        LineItemVatValue = LineItemVatValue,
                        Product = Using<IProductRepository>(c).GetById(ProductId),
                        Qty = Qty,
                        IsNew = true,
                        Value = Value
                    };
                ListitnLineitem.Add(itnLineitem);
                //}
                note._SetLineItems(ListitnLineitem);
            }
        }

        void LoadInvoice()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                Invoice invoice = Using<IInvoiceRepository>(c).GetById(InvoiceNo) as Invoice;
                //var amountpaid = _receiptService.GetByInvoiceId(InvoiceNo).Sum(n=> n.Total);
                if (invoice != null)
                {
                    InvoiceRef = invoice.DocumentReference;
                    TotalGross = invoice.TotalGross;
                }
                //Check if Credit Note Exists
                //List<CreditNote> cn = _creditNoteService.GetAll().Where(n => n.LineItems.Any(li => li.InvoiceId == InvoiceNo)).ToList();
                //List<CreditNote> cn = _creditNoteService.GetAll().Where(n => n.InvoiceId == InvoiceId).ToList();//cn
                List<CreditNote> cn = Using<ICreditNoteRepository>(c).GetCreditNotesByInvoiceId(InvoiceId).ToList(); //cn:
                var cnTotals = new decimal();
                foreach (CreditNote item in cn)
                {
                    item.LineItems.ForEach(x => cnTotals += x.Value);
                }
                if (cnTotals >= TotalGross)
                {
                    MessageBox.Show("Credit Notes on the full invoice amount already issued");
                    ClearViewModel();
                    SendNavigationRequestMessage(new Uri("/CN/ListInvoices", UriKind.Relative));
                }
                IssuedCreditNotes = cnTotals;
            }
        }

        public void ConfirmCommandCN()
        {
            SaveCommandCN();
            if (note != null)
            {
                using (StructureMap.IContainer c = NestedContainer)
                {
                    BasicConfig config = c.GetInstance<IConfigService>().Load();
                 Using<IConfirmCreditNoteWFManager>(c).SubmitChanges(note,config);
                    note = null;
                    ClearViewModel();
                    MessageBox.Show("Credit Note Issued Succesful");
                }
            }
            else
            {
                throw new Exception("Save first ");
            }
        }

        #region Properties

        /// <summary>
        /// The <see cref="IssuedCreditNotes" /> property's name.
        /// </summary>
        public const string IssuedCreditNotesPropertyName = "IssuedCreditNotes";
        private Decimal _IssuedCreditNotes = 0;
        /// <summary>
        /// Gets the IssuedCreditNotes property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Decimal IssuedCreditNotes
        {
            get
            {
                return _IssuedCreditNotes;
            }

            set
            {
                if (_IssuedCreditNotes == value)
                {
                    return;
                }

                var oldValue = _IssuedCreditNotes;
                _IssuedCreditNotes = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(IssuedCreditNotesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InvoiceRef" /> property's name.
        /// </summary>
        public const string InvoiceRefPropertyName = "InvoiceRef";
        private string _InvoiceRef = null;
        /// <summary>
        /// Gets the InvoiceRef property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string InvoiceRef
        {
            get
            {
                return _InvoiceRef;
            }

            set
            {
                if (_InvoiceRef == value)
                {
                    return;
                }

                var oldValue = _InvoiceRef;
                _InvoiceRef = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceRefPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="TotalGross" /> property's name.
        /// </summary>
        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _TotalGross = 0;
        /// <summary>
        /// Gets the TotalGross property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal TotalGross
        {
            get
            {
                return _TotalGross;
            }

            set
            {
                if (_TotalGross == value)
                {
                    return;
                }

                var oldValue = _TotalGross;
                _TotalGross = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="InvoiceId" /> property's name.
        /// </summary>
        public const string InvoiceIdPropertyName = "InvoiceId";
        private Guid _InvoiceId = Guid.Empty;
        /// <summary>
        /// Gets the InvoiceId property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid InvoiceId
        {
            get
            {
                return _InvoiceId;
            }

            set
            {
                if (_InvoiceId == value)
                {
                    return;
                }

                var oldValue = _InvoiceId;
                _InvoiceId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(InvoiceIdPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";
        private decimal _Value = 0;
        /// <summary>
        /// Gets the Value property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public decimal Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (_Value == value)
                {
                    return;
                }

                var oldValue = _Value;
                _Value = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";
        private string _Description = null;
        /// <summary>
        /// Gets the Description property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string Description
        {
            get
            {
                return _Description;
            }

            set
            {
                if (_Description == value)
                {
                    return;
                }

                var oldValue = _Description;
                _Description = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        public const string ProductIdPropertyName = "ProductId";
        private Guid _productId = Guid.Empty;
        public Guid ProductId
        {
            get
            {
                return _productId;
            }

            set
            {
                if (_productId == value)
                {
                    return;
                }

                var oldValue = _productId;
                _productId = value;
                RaisePropertyChanged(ProductIdPropertyName);
            }
        }

        public const string QtyPropertyName = "Qty";
        private int _qty = 0;
        public int Qty
        {
            get
            {
                return _qty;
            }

            set
            {
                if (_qty == value)
                {
                    return;
                }

                var oldValue = _qty;
                _qty = value;
                RaisePropertyChanged(QtyPropertyName);
            }
        }

        public const string LineItemVatValuePropertyName = "LineItemVatValue";
        private decimal _lineItemVatValue = 0m;
        public decimal LineItemVatValue
        {
            get
            {
                return _lineItemVatValue;
            }

            set
            {
                if (_lineItemVatValue == value)
                {
                    return;
                }

                var oldValue = _lineItemVatValue;
                _lineItemVatValue = value;
                RaisePropertyChanged(LineItemVatValuePropertyName);
            }
        }

        #endregion

        public class EditCNItemsViewModel : ViewModelBase
        {
            /// <summary>
            /// The <see cref="Id" /> property's name.
            /// </summary>
            public const string IdPropertyName = "Id";
            private string _Id = null;
            /// <summary>
            /// Gets the Id property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string Id
            {
                get
                {
                    return _Id;
                }

                set
                {
                    if (_Id == value)
                    {
                        return;
                    }

                    var oldValue = _Id;
                    _Id = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(IdPropertyName);
                }
            }

            ///// <summary>
            ///// The <see cref="InvoiceId" /> property's name.
            ///// </summary>
            //public const string InvoiceIdPropertyName = "InvoiceId";
            //private Guid _InvoiceId = Guid.Empty;
            ///// <summary>
            ///// Gets the InvoiceId property.
            //
            ///// Changes to that property's value raise the PropertyChanged event. 
            ///// This property's value is broadcasted by the Messenger's default instance when it changes.
            ///// </summary>
            //public Guid InvoiceId
            //{
            //    get
            //    {
            //        return _InvoiceId;
            //    }

            //    set
            //    {
            //        if (_InvoiceId == value)
            //        {
            //            return;
            //        }

            //        var oldValue = _InvoiceId;
            //        _InvoiceId = value;

            //        // Update bindings, no broadcast
            //        RaisePropertyChanged(InvoiceIdPropertyName);
            //    }
            //}

            ///// <summary>
            ///// The <see cref="Value" /> property's name.
            ///// </summary>
            //public const string ValuePropertyName = "Value";
            //private decimal _Value = 0;
            ///// <summary>
            ///// Gets the Value property.
            //
            ///// Changes to that property's value raise the PropertyChanged event. 
            ///// This property's value is broadcasted by the Messenger's default instance when it changes.
            ///// </summary>
            //public decimal Value
            //{
            //    get
            //    {
            //        return _Value;
            //    }

            //    set
            //    {
            //        if (_Value == value)
            //        {
            //            return;
            //        }

            //        var oldValue = _Value;
            //        _Value = value;

            //        // Update bindings, no broadcast
            //        RaisePropertyChanged(ValuePropertyName);
            //    }
            //}

            ///// <summary>
            ///// The <see cref="Description" /> property's name.
            ///// </summary>
            //public const string DescriptionPropertyName = "Description";
            //private string _Description = null;
            ///// <summary>
            ///// Gets the Description property.
            //
            ///// Changes to that property's value raise the PropertyChanged event. 
            ///// This property's value is broadcasted by the Messenger's default instance when it changes.
            ///// </summary>
            //public string Description
            //{
            //    get
            //    {
            //        return _Description;
            //    }

            //    set
            //    {
            //        if (_Description == value)
            //        {
            //            return;
            //        }

            //        var oldValue = _Description;
            //        _Description = value;

            //        // Update bindings, no broadcast
            //        RaisePropertyChanged(DescriptionPropertyName);
            //    }
            //}

        }

        public class InvoiceViewModel : ViewModelBase
        {
            ///// <summary>
            ///// The <see cref="InvoiceRef" /> property's name.
            ///// </summary>
            //public const string InvoiceRefPropertyName = "InvoiceRef";
            //private string _InvoiceRef = null;
            ///// <summary>
            ///// Gets the InvoiceRef property.
            //
            ///// Changes to that property's value raise the PropertyChanged event. 
            ///// This property's value is broadcasted by the Messenger's default instance when it changes.
            ///// </summary>
            //public string InvoiceRef
            //{
            //    get
            //    {
            //        return _InvoiceRef;
            //    }

            //    set
            //    {
            //        if (_InvoiceRef == value)
            //        {
            //            return;
            //        }

            //        var oldValue = _InvoiceRef;
            //        _InvoiceRef = value;

            //        // Update bindings, no broadcast
            //        RaisePropertyChanged(InvoiceRefPropertyName);
            //    }
            //}

            ///// <summary>
            ///// The <see cref="TotalGross" /> property's name.
            ///// </summary>
            //public const string TotalGrossPropertyName = "TotalGross";
            //private decimal _TotalGross = 0;
            ///// <summary>
            ///// Gets the TotalGross property.
            //
            ///// Changes to that property's value raise the PropertyChanged event. 
            ///// This property's value is broadcasted by the Messenger's default instance when it changes.
            ///// </summary>
            //public decimal TotalGross
            //{
            //    get
            //    {
            //        return _TotalGross;
            //    }

            //    set
            //    {
            //        if (_TotalGross == value)
            //        {
            //            return;
            //        }

            //        var oldValue = _TotalGross;
            //        _TotalGross = value;

            //        // Update bindings, no broadcast
            //        RaisePropertyChanged(TotalGrossPropertyName);
            //    }
            //}
        }
    }
}
