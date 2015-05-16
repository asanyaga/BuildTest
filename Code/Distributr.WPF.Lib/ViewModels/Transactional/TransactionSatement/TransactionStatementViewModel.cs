using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Windows.Controls;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Windows.Input;
using StructureMap;

namespace Distributr.WPF.Lib.ViewModels.Transactional.TransactionSatement
{
    public class TransactionStatementViewModel : DistributrViewModelBase
    {
       
        public TransactionStatementViewModel()
        {
           

            LoadCommand = new RelayCommand(LoadData);
            PrintCommand = new RelayCommand<Panel>(RunPrintCommand);
            InvoiceLineIItems = new ObservableCollection<TSLineItem>();
            CreditNoteLineItems = new ObservableCollection<TSLineItem>();
            ReceiptLineItems = new ObservableCollection<TSReceiptLineItem>();
        }

        #region Properties
        #region CommandsNCollections
        public ObservableCollection<TSLineItem> InvoiceLineIItems { get; set; }
        public ObservableCollection<TSReceiptLineItem> ReceiptLineItems { get; set; }
        public ObservableCollection<TSLineItem> CreditNoteLineItems { get; set; }

        public RelayCommand LoadCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region mvvminpc
        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId = Guid.Empty;
        public Guid OrderId
        {
            get
            {
                return _orderId;
            }

            set
            {
                if (_orderId == value)
                {
                    return;
                }

                var oldValue = _orderId;
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        public const string OrderReferencePropertyName = "OrderReference";
        private string _orderReference = "";
        public string OrderReference
        {
            get
            {
                return _orderReference;
            }

            set
            {
                if (_orderReference == value)
                {
                    return;
                }

                var oldValue = _orderReference;
                _orderReference = value;
                RaisePropertyChanged(OrderReferencePropertyName);
            }
        }

        public const string PageTitlePropertyName = "PageTitle";
        private string _pageTitle = "";
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

                var oldValue = _pageTitle;
                _pageTitle = value;
                RaisePropertyChanged(PageTitlePropertyName);
            }
        }

        public const string InvoiceReferencePropertyName = "InvoiceReference";
        private string _invoiceReference = "";
        public string InvoiceReference
        {
            get
            {
                return _invoiceReference;
            }

            set
            {
                if (_invoiceReference == value)
                {
                    return;
                }

                var oldValue = _invoiceReference;
                _invoiceReference = value;
                RaisePropertyChanged(InvoiceReferencePropertyName);
            }
        }

        public const string InvoiceDatePropertyName = "InvoiceDate";
        private string _invoiceDate = "";
        public string InvoiceDate
        {
            get
            {
                return _invoiceDate;
            }

            set
            {
                if (_invoiceDate == value)
                {
                    return;
                }

                var oldValue = _invoiceDate;
                _invoiceDate = value;
                RaisePropertyChanged(InvoiceDatePropertyName);
            }
        }

        public const string InvoiceIdPropertyName = "InvoiceId";
        private Guid _invoiceId = Guid.Empty;
        public Guid InvoiceId
        {
            get
            {
                return _invoiceId;
            }

            set
            {
                if (_invoiceId == value)
                {
                    return;
                }

                var oldValue = _invoiceId;
                _invoiceId = value;
                RaisePropertyChanged(InvoiceIdPropertyName);
            }
        }

        public const string TotalNetPropertyName = "TotalNet";
        private decimal _totalNet = 0m;
        public decimal TotalNet
        {
            get
            {
                return _totalNet;
            }

            set
            {
                if (_totalNet == value)
                {
                    return;
                }

                var oldValue = _totalNet;
                _totalNet = value;
                RaisePropertyChanged(TotalNetPropertyName);
            }
        }

        public const string TotalVatPropertyName = "TotalVat";
        private decimal _totalvat = 0m;
        public decimal TotalVat
        {
            get
            {
                return _totalvat;
            }

            set
            {
                if (_totalvat == value)
                {
                    return;
                }

                var oldValue = _totalvat;
                _totalvat = value;
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private decimal _totalGross = 0m;
        public decimal TotalGross
        {
            get
            {
                return _totalGross;
            }

            set
            {
                if (_totalGross == value)
                {
                    return;
                }

                var oldValue = _totalGross;
                _totalGross = value;
                RaisePropertyChanged(TotalGrossPropertyName);
            }
        }

        public const string TotalAmountPaidPropertyName = "TotalAmountPaid";
        private decimal _totalAmountPaid = 0m;
        public decimal TotalAmountPaid
        {
            get
            {
                return _totalAmountPaid;
            }

            set
            {
                if (_totalAmountPaid == value)
                {
                    return;
                }

                var oldValue = _totalAmountPaid;
                _totalAmountPaid = value;
                RaisePropertyChanged(TotalAmountPaidPropertyName);
            }
        }

        public const string InvoiceBalancePropertyName = "InvoiceBalance";
        private decimal _invoiceBalance = 0m;
        public decimal InvoiceBalance
        {
            get
            {
                return _invoiceBalance;
            }

            set
            {
                if (_invoiceBalance == value)
                {
                    return;
                }

                var oldValue = _invoiceBalance;
                _invoiceBalance = value;
                RaisePropertyChanged(InvoiceBalancePropertyName);
            }
        }

        public const string TotalDeductionsPropertyName = "TotalDeductions";
        private decimal _TotalDeductions = 0m;
        public decimal TotalDeductions
        {
            get
            {
                return _TotalDeductions;
            }

            set
            {
                if (_TotalDeductions == value)
                {
                    return;
                }

                var oldValue = _TotalDeductions;
                _TotalDeductions = value;
                RaisePropertyChanged(TotalDeductionsPropertyName);
            }
        }

        public const string InvoiceSubBalancePropertyName = "InvoiceSubBalance";
        private decimal _invoiceSubBalance = 0m;
        public decimal InvoiceSubBalance
        {
            get
            {
                return _invoiceSubBalance;
            }

            set
            {
                if (_invoiceSubBalance == value)
                {
                    return;
                }

                var oldValue = _invoiceSubBalance;
                _invoiceSubBalance = value;
                RaisePropertyChanged(InvoiceSubBalancePropertyName);
            }
        }
        #endregion

        #endregion

        #region Methods
        public void LoadData()
        {
            using (IContainer container = NestedContainer)
            {
                var invoiceService = Using<IInvoiceRepository>(container);
                var creditNoteService = Using<ICreditNoteRepository>(container);
                IReceiptRepository receiptService = Using<IReceiptRepository>(container);
                var orderService = Using<IOrderRepository>(container);
                var theInvoice = invoiceService.GetInvoiceByOrderId(OrderId);
                var theCreditNote = creditNoteService.GetAll().OfType<CreditNote>().Where(n => n.InvoiceId == theInvoice.Id).ToList();
                var theReceipts = receiptService.GetByInvoiceId(theInvoice.Id)
                    .Where(n => n.Total > 0).ToList();

                InvoiceId = theInvoice.Id;
                InvoiceReference = theInvoice.DocumentReference;
                InvoiceDate = theInvoice.DocumentDateIssued.ToString("dd-MMM-yyyy");
                OrderReference = "Order Ref: " + orderService.GetById(OrderId).DocumentReference;

                LoadInvoiceLineItems(theInvoice);

                LoadCreditNoteLineItems(theCreditNote);
                InvoiceSubBalance = theInvoice.TotalGross - TotalDeductions;

                LoadReceiptLineItems(theReceipts);
                InvoiceBalance = InvoiceSubBalance - TotalAmountPaid;
            }
        }

        void LoadInvoiceLineItems(Invoice invoice)
        {
            InvoiceLineIItems.Clear();
                var items = invoice.LineItems.Select((n, i) => new TSLineItem
            {
                SequenceId = i + 1,
                DocumentId = invoice.Id,
                DocumentReference = i > 0 ? "" : invoice.DocumentReference,
                Description = n.Product.Description,
                Qty = n.Qty,
                UnitPrice = n.Value,
                UnitVat = n.LineItemVatValue,
                Amount = n.LineItemTotal
            }).ToList();

            items.ForEach(InvoiceLineIItems.Add);
            TotalNet = invoice.TotalNet;
            TotalVat = invoice.TotalVat;
            TotalGross = invoice.TotalGross;
        }

        void LoadCreditNoteLineItems(List<CreditNote> creditNotes)
        {
            CreditNoteLineItems.Clear();
            var tempList = new List<TSLineItem>();
            foreach (var cn in creditNotes)
            {
                var li = cn.LineItems.Select((n, i) => new TSLineItem
                {
                    DocumentId = cn.Id,
                    DocumentReference = i > 0 ? "" : cn.DocumentReference,
                    Description = n.Product.Description,
                    Qty = n.Qty,
                    UnitPrice = n.Value,
                    UnitVat = n.LineItemVatValue,
                    Amount = n.LineItemTotal
                });
                li.ToList().ForEach(tempList.Add);
            }

            var items = tempList.Select((n, i) => new TSLineItem
            {
                SequenceId = i + 1,
                DocumentId = n.DocumentId,
                DocumentReference = n.DocumentReference,
                Description = n.Description,
                Qty = n.Qty,
                UnitPrice = n.UnitPrice,
                UnitVat = n.UnitVat,
                Amount = n.Amount
            }).ToList();
            items.ForEach(CreditNoteLineItems.Add);
            TotalDeductions = creditNotes.Sum(n => n.Total);
        }

        void LoadReceiptLineItems(List<Receipt> receipts)
        {
            ReceiptLineItems.Clear();
            var items = receipts.Select((n, i) => new TSReceiptLineItem
            {
                SequenceId = i + 1,
                ReceiptId = n.Id,
                ReceiptReference = n.DocumentReference,
                ReceiptDate = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                Amount = n.Total
            }).ToList();
            items.ForEach(ReceiptLineItems.Add);
            TotalAmountPaid = receipts.Sum(i => i.Total);
        }

        void RunPrintCommand(Panel p)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                //p.Width = e.PrintableArea.Width;
                //p.Height = e.PrintableArea.Height;
                //e.PageVisual = p;
                e.HasMorePages = false;
            };

            pd.Print();
        }

        void lodli()
        {
            //var tempList = new List<TSReceiptLineItem>();
            //foreach (var rct in receipts)
            //{
            //    var li = rct.LineItems.Select((n, i) => new TSReceiptLineItem
            //                                                {
            //                                                    ReceiptReference = i > 0 ? "" : rct.DocumentReference,
            //                                                    ReceiptDate = i > 0? "" : rct.DocumentDateIssued.ToString("dd-MMM-yyyy"),
            //                                                    ReceiptId = rct.Id,
            //                                                    Description = n.Description,
            //                                                    PaymentType = n.PaymentType.ToString(),
            //                                                    PaymentTypeReference = n.PaymentTypeRef,
            //                                                    Amount = n.Value,
            //                                                    Bank = n.bank.Name,
            //                                                    BankBranch = n.bankBranch.Name
            //                                                });
            //    li.ToList().ForEach(tempList.Add);
            //}

            //ReceiptLineItems = tempList.Select((n, i) => new TSReceiptLineItem
            //                                                 {
            //                                                     SequenceId = i + 1,
            //                                                     ReceiptId = n.ReceiptId,
            //                                                     ReceiptReference = n.ReceiptReference,
            //                                                     ReceiptDate = n.ReceiptDate,
            //                                                     Description = n.Description,
            //                                                     PaymentType = n.PaymentType,
            //                                                     PaymentTypeReference = n.PaymentTypeReference,
            //                                                     Bank = n.Bank,
            //                                                     BankBranch = n.BankBranch,
            //                                                     Amount = n.Amount
            //                                                 }).ToList();
        }
        #endregion
    }

    #region HelperClasses
    public class TSReceiptLineItem
    {
        public Guid ReceiptId { get; set; }
        public string ReceiptReference { get; set; }
        public int SequenceId { get; set; }
        public string Description { get; set; }
        public string PaymentType { get; set; }
        public string PaymentTypeReference { get; set; }
        public decimal Amount { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public string ReceiptDate { get; set; }
    }

    public class TSLineItem
    {
        public Guid DocumentId { get; set; }
        public string DocumentReference { get; set; }
        public int SequenceId { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UnitVat { get; set; }
        public decimal Amount { get; set; }
    }
    #endregion
}