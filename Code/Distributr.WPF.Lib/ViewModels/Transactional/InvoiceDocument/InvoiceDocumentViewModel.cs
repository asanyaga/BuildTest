using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using System.Linq;
using System.Collections.Generic;
using Distributr.Core.Utility;
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Distributr.WPF.Lib.ViewModels.Transactional.InvoiceDocument
{
    public class InvoiceDocumentViewModel : DistributrViewModelBase
    {

        public InvoiceDocumentViewModel()
        {

            ReturnToListCommand = new RelayCommand(ReturnToView);
            ViewReceiptCommand = new RelayCommand(RunViewReceiptCommand);
            PrintCommand = new RelayCommand<Panel>(OnPrintCommand);
            InvoiceDocumentLoadedCommand = new RelayCommand(PageLoaded);
            InvoicePaymentInfo = new List<InvoicePaymentInfoLineItem>();
            InvoiceReceipts = new ObservableCollection<Receipt>();
            InvoicePaymentInfo = new List<InvoicePaymentInfoLineItem>();
            InvoiceDeductions = new List<InvoiceDeductionLineItem>();
            InvoiceLineItemsList = new ObservableCollection<InvoiceLineItem>();
        }



        #region Properties

        public ObservableCollection<InvoiceLineItem> InvoiceLineItemsList { get; set; }

        public ObservableCollection<Receipt> InvoiceReceipts { get; set; }
        public List<InvoicePaymentInfoLineItem> InvoicePaymentInfo { get; set; }
        public List<InvoiceDeductionLineItem> InvoiceDeductions { get; set; }
        public RelayCommand ReturnToListCommand { get; set; }
        public RelayCommand ViewReceiptCommand { get; set; }
        public RelayCommand InvoiceDocumentLoadedCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public const string InvoiceLookupIdPropertyName = "InvoiceLookupId";
        private Guid _invoiceLookupId = Guid.Empty;

        public Guid InvoiceLookupId
        {
            get { return _invoiceLookupId; }

            set
            {
                if (_invoiceLookupId == value)
                {
                    return;
                }

                var oldValue = _invoiceLookupId;
                _invoiceLookupId = value;
                RaisePropertyChanged(InvoiceLookupIdPropertyName);
            }
        }

        public const string TotalNetPropertyName = "TotalNet";
        private string _totalNet = "";

        public string TotalNet
        {
            get { return _totalNet; }

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
        private string _totalVat = "";

        public string TotalVat
        {
            get { return _totalVat; }

            set
            {
                if (_totalVat == value)
                {
                    return;
                }

                var oldValue = _totalVat;
                _totalVat = value;
                RaisePropertyChanged(TotalVatPropertyName);
            }
        }

        public const string TotalGrossPropertyName = "TotalGross";
        private string _totalGross = "";

        public string TotalGross
        {
            get { return _totalGross; }

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

        public const string DocumentTitlePropertyName = "DocumentTitle";
        private string _documentTitle = "";

        public string DocumentTitle
        {
            get { return _documentTitle; }

            set
            {
                if (_documentTitle == value)
                {
                    return;
                }

                var oldValue = _documentTitle;
                _documentTitle = value;
                RaisePropertyChanged(DocumentTitlePropertyName);
            }
        }

        public const string InvoiceNoPropertyName = "InvoiceNo";
        private string _invoiceNo = "";

        public string InvoiceNo
        {
            get { return _invoiceNo; }

            set
            {
                if (_invoiceNo == value)
                {
                    return;
                }

                var oldValue = _invoiceNo;
                _invoiceNo = value;
                RaisePropertyChanged(InvoiceNoPropertyName);
            }
        }

        public const string OutletNamePropertyName = "OutletName";
        private string _outletName = "";

        public string OutletName
        {
            get { return _outletName; }

            set
            {
                if (_outletName == value)
                {
                    return;
                }

                var oldValue = _outletName;
                _outletName = value;
                RaisePropertyChanged(OutletNamePropertyName);
            }
        }

        public const string InvoiceDatePropertyName = "InvoiceDate";
        private string _invoiceDate = DateTime.Now.ToString("dd-MMM-yyyy");

        public string InvoiceDate
        {
            get { return _invoiceDate; }

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

        public const string CreditTermsPropertyName = "CreditTerms";
        private string _creditTerms = "";

        public string CreditTerms
        {
            get { return _creditTerms; }

            set
            {
                if (_creditTerms == value)
                {
                    return;
                }

                var oldValue = _creditTerms;
                _creditTerms = value;
                RaisePropertyChanged(CreditTermsPropertyName);
            }
        }

        public const string ChequePayableToPropertyName = "ChequePayableTo";
        private string _chequePayableTo = "";

        public string ChequePayableTo
        {
            get { return _chequePayableTo; }

            set
            {
                if (_chequePayableTo == value)
                {
                    return;
                }

                var oldValue = _chequePayableTo;
                _chequePayableTo = value;
                RaisePropertyChanged(ChequePayableToPropertyName);
            }
        }

        public const string PreparedByUserNamePropertyName = "PreparedByUserName";
        private string _preparedByUserName = "";

        public string PreparedByUserName
        {
            get { return _preparedByUserName; }

            set
            {
                if (_preparedByUserName == value)
                {
                    return;
                }

                var oldValue = _preparedByUserName;
                _preparedByUserName = value;
                RaisePropertyChanged(PreparedByUserNamePropertyName);
            }
        }

        public const string PreparedByJobTitlePropertyName = "PreparedByJobTitle";
        private string _preparedByJobTitle = "";
        public string PreparedByJobTitle
        {
            get { return _preparedByJobTitle; }

            set
            {
                if (_preparedByJobTitle == value)
                {
                    return;
                }

                var oldValue = _preparedByJobTitle;
                _preparedByJobTitle = value;
                RaisePropertyChanged(PreparedByJobTitlePropertyName);
            }
        }

        public const string DatePreparedPropertyName = "DatePrepared";
        private DateTime _datePrepared = DateTime.Now;

        public DateTime DatePrepared
        {
            get { return _datePrepared; }

            set
            {
                if (_datePrepared == value)
                {
                    return;
                }

                var oldValue = _datePrepared;
                _datePrepared = value;
                RaisePropertyChanged(DatePreparedPropertyName);
            }
        }

        public const string ApprovedByUserNamePropertyName = "ApprovedByUserName";
        private string _approvedByUserName = "";

        public string ApprovedByUserName
        {
            get { return _approvedByUserName; }

            set
            {
                if (_approvedByUserName == value)
                {
                    return;
                }

                var oldValue = _approvedByUserName;
                _approvedByUserName = value;
                RaisePropertyChanged(ApprovedByUserNamePropertyName);
            }
        }

        public const string DateApprovedPropertyName = "DateApproved";
        private DateTime _dateApproved = DateTime.Now;

        public DateTime DateApproved
        {
            get { return _dateApproved; }

            set
            {
                if (_dateApproved == value)
                {
                    return;
                }

                var oldValue = _dateApproved;
                _dateApproved = value;
                RaisePropertyChanged(DateApprovedPropertyName);
            }
        }

        //company Info
        public const string CompanyNamePropertyName = "CompanyName";
        private string _companyName = "";

        public string CompanyName
        {
            get { return _companyName; }

            set
            {
                if (_companyName == value)
                {
                    return;
                }

                var oldValue = _companyName;
                _companyName = value;
                RaisePropertyChanged(CompanyNamePropertyName);
            }
        }

        public const string AddressPropertyName = "Address";
        private string _address = "";

        public string Address
        {
            get { return _address; }

            set
            {
                if (_address == value)
                {
                    return;
                }

                var oldValue = _address;
                _address = value;
                RaisePropertyChanged(AddressPropertyName);
            }
        }

        public const string PhysicalAddressPropertyName = "PhysicalAddress";
        private string _physicalAddress = "";

        public string PhysicalAddress
        {
            get { return _physicalAddress; }

            set
            {
                if (_physicalAddress == value)
                {
                    return;
                }

                var oldValue = _physicalAddress;
                _physicalAddress = value;
                RaisePropertyChanged(PhysicalAddressPropertyName);
            }
        }

        public const string TelNoPropertyName = "TelNo";
        private string _telNo = "";

        public string TelNo
        {
            get { return _telNo; }

            set
            {
                if (_telNo == value)
                {
                    return;
                }

                var oldValue = _telNo;
                _telNo = value;
                RaisePropertyChanged(TelNoPropertyName);
            }
        }

        public const string FaxNoPropertyName = "FaxNo";
        private string _faxNo = "";

        public string FaxNo
        {
            get { return _faxNo; }

            set
            {
                if (_faxNo == value)
                {
                    return;
                }

                var oldValue = _faxNo;
                _faxNo = value;
                RaisePropertyChanged(FaxNoPropertyName);
            }
        }

        public const string CellPropertyName = "Cell";
        private string _cell = "";

        public string Cell
        {
            get { return _cell; }

            set
            {
                if (_cell == value)
                {
                    return;
                }

                var oldValue = _cell;
                _cell = value;
                RaisePropertyChanged(CellPropertyName);
            }
        }

        public const string VatNoPropertyName = "VatNo";
        private string _vatNo = "";

        public string VatNo
        {
            get { return _vatNo; }

            set
            {
                if (_vatNo == value)
                {
                    return;
                }

                var oldValue = _vatNo;
                _vatNo = value;
                RaisePropertyChanged(VatNoPropertyName);
            }
        }

        public const string PinNoPropertyName = "PinNo";
        private string _pinNo = "";

        public string PinNo
        {
            get { return _pinNo; }

            set
            {
                if (_pinNo == value)
                {
                    return;
                }

                var oldValue = _pinNo;
                _pinNo = value;
                RaisePropertyChanged(PinNoPropertyName);
            }
        }

        public const string EmailPropertyName = "Email";
        private string _email = "";

        public string Email
        {
            get { return _email; }

            set
            {
                if (_email == value)
                {
                    return;
                }

                var oldValue = _email;
                _email = value;
                RaisePropertyChanged(EmailPropertyName);
            }
        }

        public const string WebSitePropertyName = "WebSite";
        private string _webSite = "";

        public string WebSite
        {
            get { return _webSite; }

            set
            {
                if (_webSite == value)
                {
                    return;
                }

                var oldValue = _webSite;
                _webSite = value;
                RaisePropertyChanged(WebSitePropertyName);
            }
        }

        public const string OrderIdPropertyName = "OrderId";
        private Guid _orderId = Guid.Empty;

        public Guid OrderId
        {
            get { return _orderId; }

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

        public const string InvoiceRecipientCompanyNamePropertyName = "InvoiceRecipientCompanyName";
        private string _invoiceRecipientCompanyName = "";

        public string InvoiceRecipientCompanyName
        {
            get { return _invoiceRecipientCompanyName; }

            set
            {
                if (_invoiceRecipientCompanyName == value)
                {
                    return;
                }

                var oldValue = _invoiceRecipientCompanyName;
                _invoiceRecipientCompanyName = value;
                RaisePropertyChanged(InvoiceRecipientCompanyNamePropertyName);
            }
        }

        public const string SelectedReceiptPropertyName = "SelectedReceipt";
        private Receipt _selectedReceipt = null;

        public Receipt SelectedReceipt
        {
            get { return _selectedReceipt; }

            set
            {
                if (_selectedReceipt == value)
                {
                    return;
                }

                var oldValue = _selectedReceipt;
                _selectedReceipt = value;
                RaisePropertyChanged(SelectedReceiptPropertyName);
            }
        }

        public const string TheInvoicePropertyName = "TheInvoice";
        private Invoice _theInvoice = null;

        public Invoice TheInvoice
        {
            get { return _theInvoice; }

            set
            {
                if (_theInvoice == value)
                {
                    return;
                }

                var oldValue = _theInvoice;
                _theInvoice = value;
                RaisePropertyChanged(TheInvoicePropertyName);
            }
        }

        public const string TotalAmountPaidPropertyName = "TotalAmountPaid";
        private decimal _totalAmountPaid = 0m;

        public decimal TotalAmountPaid
        {
            get { return _totalAmountPaid; }

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
            get { return _invoiceBalance; // < 0 ? 0 : _invoiceBalance;
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
            get { return _TotalDeductions; }

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
            get { return _invoiceSubBalance; }

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

        public const string SaleDiscountPropertyName = "SaleDiscount";
        private decimal _saleDiscount = 0m;

        public decimal SaleDiscount
        {
            get { return _saleDiscount; }

            set
            {
                if (_saleDiscount == value)
                {
                    return;
                }

                var oldValue = _saleDiscount;
                _saleDiscount = value;
                RaisePropertyChanged(SaleDiscountPropertyName);
            }
        }

        public const string TotalProductDiscountPropertyName = "TotalProductDiscount";
        private decimal _totalProducDiscount = 0m;

        public decimal TotalProductDiscount
        {
            get { return _totalProducDiscount; }

            set
            {
                if (_totalProducDiscount == value)
                {
                    return;
                }

                var oldValue = _totalProducDiscount;
                _totalProducDiscount = value;
                RaisePropertyChanged(TotalProductDiscountPropertyName);
            }
        }

        #endregion

        #region Methods

        public void SetId(ViewModelMessage obj)
        {
            OrderId = obj.Id;

        }

        private void PageLoaded()
        {
            Load();
        }

        private void OnPrintCommand(Panel p)
        {

            //PrintDocument pd = new PrintDocument();
            //pd.PrintPage += (s, e) =>
            //                    {
            //                        p.
            //                        e.HasMorePages = false;
            //                    };

            //pd.Print();
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(p,"Print Invoice");
            }
        }

        public void Load()
        {
            LoadInvoiceAndReceipts();
            Distributor distributr = null;
            using (StructureMap.IContainer c = NestedContainer)
            {
                CostCentre cc = Using<ICostCentreRepository>(c).GetById(TheInvoice.DocumentIssuerCostCentre.Id);
                CostCentre cc2 = Using<ICostCentreRepository>(c).GetById(TheInvoice.DocumentRecipientCostCentre.Id);

                var pricingService = Using<IDiscountProWorkflow>(c);

                if (cc is Distributor)
                {
                    distributr = cc as Distributor;
                }
                if (cc2 is Distributor)
                {
                    distributr = cc2 as Distributor;
                }

                //company info
                CompanyName = cc.Name;
                Contact contact = null;
                if (distributr.Contact.Any())
                {
                    contact = distributr.Contact.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              distributr.Contact.FirstOrDefault();
                }
                if (contact == null)
                {
                    var contacts = Using<IContactRepository>(c).GetByContactsOwnerId(distributr.Id);
                    contact = contacts.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              contacts.FirstOrDefault();
                }
                if (contact != null)
                {
                    Address = contact.PhysicalAddress;
                    PhysicalAddress = contact.City == contact.PhysicalAddress
                                          ? contact.City
                                          : contact.City + " ,"
                                            + contact.PhysicalAddress;
                    TelNo = contact.BusinessPhone;
                    FaxNo = contact.Fax;
                    Cell = contact.MobilePhone;
                    Email = contact.Email;
                    WebSite = "";

                }


                VatNo = distributr != null ? distributr.VatRegistrationNo : "";
                PinNo = distributr != null ? distributr.PIN : "";
                

                InvoiceNo = TheInvoice.DocumentReference;
                InvoiceDate = TheInvoice.DocumentDateIssued.ToString("dd-MMM-yyyy");
                ChequePayableTo = TheInvoice.DocumentIssuerCostCentre.Name;
                PreparedByUserName = TheInvoice.DocumentIssuerUser.Username;
                PreparedByJobTitle = "Accountant";
                InvoiceRecipientCompanyName = TheInvoice.DocumentRecipientCostCentre.Name;

                TotalVat = TheInvoice.TotalVat.ToString("N2");

                var orderDoc = Using<IMainOrderRepository>(c).GetById(OrderId);
                OutletName = orderDoc.IssuedOnBehalfOf != null ? orderDoc.IssuedOnBehalfOf.Name : "";
                //TotalGross = pricingService.GetTotalGross(TheInvoice.TotalGross).ToString("N2");
                var invoiceGross =(TheInvoice.LineItems.Sum(x => x.LineItemTotal.GetTruncatedValue())).GetTotalGross();
                TotalGross = invoiceGross.ToString("N2");
                try
                {
                    SaleDiscount = Using<IMainOrderRepository>(c).GetById(OrderId).SaleDiscount;

                    var invoiceBalance = InvoiceBalance -= SaleDiscount;
                    //InvoiceBalance -= SaleDiscount;
                    InvoiceBalance = invoiceBalance < 0 ? 0 : invoiceBalance;
                }catch
                {
                    SaleDiscount = TheInvoice.SaleDiscount;
                }

                TotalNet = (TheInvoice.TotalNet-SaleDiscount).ToString("N2");
               
                TotalProductDiscount = TheInvoice.LineItems.Sum(n => n.ProductDiscount);
                InvoiceLineItemsList.Clear();
                var ili = TheInvoice.LineItems.Select((n, i) => new InvoiceLineItem
                                                                    {
                                                                        LineItemNumber = i + 1,
                                                                        LineItemId = n.Id,
                                                                        SequenceNo = n.LineItemSequenceNo,
                                                                        Description = n.Product.Description,
                                                                        Qantity = n.Qty,
                                                                        UnitPrice =
                                                                            (n.Value < 0 ? -n.Value : n.Value) +
                                                                            n.LineItemVatValue,
                                                                        Amount = n.LineItemTotal,
                                                                        ProductDiscount = n.ProductDiscount,
                                                                    }).ToList();

                ili.ForEach(x=>x.Amount=x.Amount.GetTruncatedValue());
                foreach (var item in ili) InvoiceLineItemsList.Add(item);
            }

        }

        public void LoadInvoiceAndReceipts()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                TheInvoice = Using<IInvoiceRepository>(c).GetInvoiceByOrderId(OrderId);
                var pricingService = Using<IDiscountProWorkflow>(c);

                TheInvoice.LineItems.ForEach(x =>x.LineItemTotal= x.LineItemTotal.GetTotalGross());
                var invoiceGross = (TheInvoice.LineItems.Sum(x =>x.LineItemTotal.GetTruncatedValue())).GetTotalGross();
                TotalGross = invoiceGross.ToString("N2");

                InvoiceReceipts.Clear();
                var rec = new Receipt(Guid.Empty)
                              {
                                  DocumentReference = GetLocalText("sl.invoice.receiptlist.default")
                                  /*"--Select Receipt To View--"*/
                              };
                InvoiceReceipts.Add(rec);
                SelectedReceipt = rec;
                var invReceipts = Using<IReceiptRepository>(c).GetByInvoiceId(TheInvoice.Id);
                invReceipts.Where(n => n.Total > 0).ToList().ForEach(n => InvoiceReceipts.Add(n));

                InvoicePaymentInfo.Clear();
                InvoicePaymentInfo =
                    invReceipts.Where(n => n.Total > 0).Where(n => n.Id != Guid.Empty).ToList()
                        .Select((n, i) => new InvoicePaymentInfoLineItem
                                              {
                                                  ReceiptLineItemNo = i + 1,
                                                  ReceiptId = n.Id,
                                                  ReceiptDate = n.DocumentDateIssued.ToString("dd-MMM-yyyy"),
                                                  ReceiptNo = n.DocumentReference,
                                                  AmountPaid = n.Total
                                              }).ToList();
                TotalAmountPaid = invReceipts.Sum(i => i.Total);

                LoadInvoiceDeductions(invoiceGross);

                InvoiceBalance = (InvoiceSubBalance - TotalAmountPaid).GetTotalGross();
            }
        }

        public void LoadInvoiceDeductions(decimal InvoiceGross)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var invoiceCreditNotes =
                    Using<ICreditNoteRepository>(c).GetAll().OfType<CreditNote>().Where(
                        n => n.InvoiceId == TheInvoice.Id);

                var pricingService = Using<IDiscountProWorkflow>(c);

                InvoiceDeductions.Clear();
                foreach (var cn in invoiceCreditNotes)
                {
                    cn.LineItems.Select((n, i) => new InvoiceDeductionLineItem
                                                      {
                                                          Amount = n.LineItemTotal,
                                                          CreditNoteId = cn.Id,
                                                          CreditNoteRef = cn.DocumentReference,
                                                          LineItemId = n.Id,
                                                          ProductId = n.Product.Id,
                                                          Qty = n.Qty,
                                                          Description = n.Product.Description,
                                                          SequenceNo = i + 1,
                                                          UnitPrice = n.Value + n.LineItemVatValue,
                                                      }).ToList().ForEach(InvoiceDeductions.Add);
                }

                TotalDeductions = invoiceCreditNotes.Sum(n => n.Total);
               
                //InvoiceSubBalance =pricingService.GetTotalGross(TheInvoice.TotalGross - TotalDeductions);
                InvoiceSubBalance = (InvoiceGross - TotalDeductions).GetTotalGross();
            }
        }

        private void ReturnToView()
        {
          using (StructureMap.IContainer c = NestedContainer)
            {
                Order o = Using<IOrderRepository>(c).GetById(OrderId);
                string url = "";
                if (o.OrderType == OrderType.DistributorPOS)
                {
                    url = "/views/Order_Pos/ViewPOS.xaml";

                }
                else if (o.OrderType == OrderType.OutletToDistributor)
                {
                    url = "/views/Orders/ViewOrder.xaml";

                }
                Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = OrderId });
                NavigateCommand.Execute(url);
            }
        }

        private void RunViewReceiptCommand()
        {
            if (SelectedReceipt.Id == Guid.Empty)
                MessageBox.Show("Select a receipt to view.", "Distributr: View Receipt", MessageBoxButton.OK);
            else
            {
                const string uri = "/views/receiptdocuments/receiptdocument.xaml";
                Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage {Id = SelectedReceipt.Id});
                NavigateCommand.Execute(uri);
            }
        }



        #endregion
        }

        #region Helper Classes

        public class InvoiceLineItem
        {
            public int LineItemNumber { get; set; }
            public Guid LineItemId { get; set; }
            public int SequenceNo { get; set; }
            public string Description { get; set; }
            public decimal Qantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal UnitVat { get; set; }
            public decimal Amount { get; set; }
            public decimal ProductDiscount { get; set; }
        }

        public class InvoiceDeductionLineItem
        {
            public Guid CreditNoteId { get; set; }
            public string CreditNoteRef { get; set; }
            public Guid LineItemId { get; set; }
            public Guid ProductId { get; set; }
            public decimal Qty { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public int SequenceNo { get; set; }
        }

        public class InvoicePaymentInfoLineItem
        {
            public Guid ReceiptId { get; set; }
            public int ReceiptLineItemNo { get; set; }
            public string ReceiptNo { get; set; }
            public string ReceiptDate { get; set; }
            public decimal AmountPaid { get; set; }
        }

        #endregion
    }
