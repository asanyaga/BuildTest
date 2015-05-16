using System;
using System.Windows.Data;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using System.Collections.Generic;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Reports
{
    public class ProductTransactionsViewModel : DistributrViewModelBase
    {
        public RelayCommand LoadDocumentsCommand { get; set; }
        public PagedCollectionView Transactions { get; set; }

        public ProductTransactionsViewModel()
        {
            LoadDocumentsCommand = new RelayCommand(LoadDocuments);
        }

        /// <summary>
        /// The <see cref="ProductId" /> property's name.
        /// </summary>
        public const string ProductIdPropertyName = "ProductId";
        private Guid _ProductId = Guid.Empty;
        /// <summary>
        /// Gets the ProductId property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public Guid ProductId
        {
            get
            {
                return _ProductId;
            }

            set
            {
                if (_ProductId == value)
                {
                    return;
                }

                var oldValue = _ProductId;
                _ProductId = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(ProductIdPropertyName);
            }
        }

        void LoadDocuments()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<ProductTransactionItemsViewModel> docs = null;
                List<InventoryTransferNote> ITN =
                  Using<IInventoryTransferNoteRepository>(c).GetAll().OfType<InventoryTransferNote>()
                                          .Where(n => n.LineItems.Any(x => x.Product.Id == ProductId))
                                          .ToList();
                List<InventoryAdjustmentNote> IAN =
                    Using<IInventoryAdjustmentNoteRepository>(c).GetAll().OfType<InventoryAdjustmentNote>()
                                            .Where(n => n.LineItem.Any(x => x.Product.Id == ProductId))
                                            .ToList();
                List<InventoryReceivedNote> IRN =
                    Using<IInventoryReceivedNoteRepository>(c)
                    .GetAll()
                    .OfType<InventoryReceivedNote>()
                    .ToList()
                    .Where(n => n.LineItems.Any(x => x.Product.Id == ProductId))
                    .ToList();
                List<Order> transactions =
                    Using<IOrderRepository>(c).GetAll().OfType<Order>().Where(n => n.LineItems.Any(x => x.Product.Id == ProductId)).ToList();

                //var transactionitems = transactions
                //    .GroupJoin(inv, t => t.Id, i => i.DocumentId, (t, n) => n
                //        .Where(x => x.Inventory.Product.Id == ProductId)
                //        .Select(z => new ProductTransactionItemsViewModel
                //{
                //    CreatedBy = t.DocumentIssuerUser.Username,
                //    DateIssued = t.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                //    Id = t.Id,
                //    DocumentReference = t.DocumentReference.ToString(),
                //    TransactionType = t.DocumentType
                //})
                //); 

                var itnitems = ITN.Select(n => new ProductTransactionItemsViewModel
                    {
                        CreatedBy = n.DocumentIssuerUser.Username,
                        DateIssued = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                        Id = n.Id,
                        DocumentReference = n.Id.ToString(),
                        TransactionType = n.DocumentType
                    }
                    );

                var ianitems = IAN.Select(n => new ProductTransactionItemsViewModel
                    {
                        CreatedBy = n.DocumentIssuerUser.Username,
                        DateIssued = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                        Id = n.Id,
                        DocumentReference = n.Id.ToString(),
                        TransactionType = n.DocumentType
                    }
                    );

                var irnitems = IRN.Select(n => new ProductTransactionItemsViewModel
                    {
                        CreatedBy = n.DocumentIssuerUser.Username,
                        DateIssued = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                        Id = n.Id,
                        DocumentReference = n.Id.ToString(),
                        TransactionType = n.DocumentType
                    }
                    );

                var transactionitems = transactions.Select(n => new ProductTransactionItemsViewModel
                    {
                        CreatedBy = n.DocumentIssuerUser.Username,
                        DateIssued = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                        Id = n.Id,
                        DocumentReference = n.Id.ToString(),
                        TransactionType = n.DocumentType
                    }
                    ).ToList();
                Transactions = null;
                docs = new List<ProductTransactionItemsViewModel>();
                docs.AddRange(transactionitems);
                docs.AddRange(itnitems);
                docs.AddRange(ianitems);
                docs.AddRange(irnitems);
                Transactions = new PagedCollectionView(docs);
            }
        }

        public class ProductTransactionItemsViewModel : ViewModelBase
        {
            /// <summary>
            /// The <see cref="Id" /> property's name.
            /// </summary>
            public const string IdPropertyName = "Id";
            private Guid _Id = Guid.Empty;
            /// <summary>
            /// Gets the Id property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public Guid Id
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

            /// <summary>
            /// The <see cref="DocumentReference" /> property's name.
            /// </summary>
            public const string DocumentReferencePropertyName = "DocumentReference";
            private string _DocumentReference = null;
            /// <summary>
            /// Gets the DocumentReference property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string DocumentReference
            {
                get
                {
                    return _DocumentReference;
                }

                set
                {
                    if (_DocumentReference == value)
                    {
                        return;
                    }

                    var oldValue = _DocumentReference;
                    _DocumentReference = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(DocumentReferencePropertyName);
                }
            }

            /// <summary>
            /// The <see cref="TransactionType" /> property's name.
            /// </summary>
            public const string TransactionTypePropertyName = "TransactionType";
            private DocumentType _TransactionType = DocumentType.Invoice;
            /// <summary>
            /// Gets the TransactionType property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public DocumentType TransactionType
            {
                get
                {
                    return _TransactionType;
                }

                set
                {
                    if (_TransactionType == value)
                    {
                        return;
                    }

                    var oldValue = _TransactionType;
                    _TransactionType = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(TransactionTypePropertyName);
                }
            }

            /// <summary>
            /// The <see cref="CreatedBy" /> property's name.
            /// </summary>
            public const string CreatedByPropertyName = "CreatedBy";
            private string _CreatedBy = null;
            /// <summary>
            /// Gets the CreatedBy property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string CreatedBy
            {
                get
                {
                    return _CreatedBy;
                }

                set
                {
                    if (_CreatedBy == value)
                    {
                        return;
                    }

                    var oldValue = _CreatedBy;
                    _CreatedBy = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(CreatedByPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="DateIssued" /> property's name.
            /// </summary>
            public const string DateIssuedPropertyName = "DateIssued";
            private string _DateIssued = null;
            /// <summary>
            /// Gets the DateIssued property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string DateIssued
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
        }
    }

}
