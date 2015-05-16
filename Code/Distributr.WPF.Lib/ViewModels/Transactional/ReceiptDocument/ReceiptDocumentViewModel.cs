using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.WPF.Lib.Services.Service;
using Distributr.WPF.Lib.ViewModels.Transactional.Orders;
using Distributr.WPF.Lib.ViewModels.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

//using System.Windows.Printing;

namespace Distributr.WPF.Lib.ViewModels.Transactional.ReceiptDocument
{

    public class ReceiptDocumentViewModel : DistributrViewModelBase
    {

        public ReceiptDocumentViewModel()
        {

            ReturnToListCommand = new RelayCommand(ReturnToView);
            ViewInvoiceCommand = new RelayCommand(RunViewInvoiceCommand);
            ViewReceiptCommand = new RelayCommand(RunViewReceiptCommand);
            PrintCommand = new RelayCommand<Panel>(OnPrintCommand);
            ReceiptDocumentLoadedCommand = new RelayCommand(PageLoaded);
            ReceiptLineItemsList = new ObservableCollection<ReceiptLineItem>();
            InvoiceReceipts = new ObservableCollection<Receipt>();
        }


        #region Properties
        public ObservableCollection<Receipt> InvoiceReceipts { get; set; }
        public ObservableCollection<ReceiptLineItem> ReceiptLineItemsList { get; set; }
       
        public RelayCommand ReturnToListCommand { get; set; }
        public RelayCommand ViewInvoiceCommand { get; set; }
        public RelayCommand ViewReceiptCommand { get; set; }
        public RelayCommand ReceiptDocumentLoadedCommand { get; set; }
        public ICommand PrintCommand { get; set; }



        public const string GenericReceiptVisibilityPropertyName = "GenericReceiptVisibility";
        private Visibility _genericReceiptVisibility = Visibility.Visible;
        public Visibility GenericReceiptVisibility
        {
            get { return _genericReceiptVisibility; }

            set
            {
                if (_genericReceiptVisibility == value)
                {
                    return;
                }
                _genericReceiptVisibility = value;
                RaisePropertyChanged(GenericReceiptVisibilityPropertyName);
            }
        }


        public const string DetailedReceiptVisibilityPropertyName = "DetailedReceiptVisibility";
        private Visibility _detailedReceiptVisibility = Visibility.Collapsed;
        public Visibility DetailedReceiptVisibility
        {
            get { return _detailedReceiptVisibility; }

            set
            {
                if (_detailedReceiptVisibility == value)
                {
                    return;
                }
                _detailedReceiptVisibility = value;
                RaisePropertyChanged(DetailedReceiptVisibilityPropertyName);
            }
        }


        public const string InvoiceLookupIdPropertyName = "InvoiceLookupId";
        private Guid _invoiceLookupId = Guid.Empty;
        public Guid InvoiceLookupId
        {
            get
            {
                return _invoiceLookupId;
            }

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
        public const string InvoiceHasMoreThanOneReceiptPropertyName = "InvoiceHasMoreThanOneReceipt";
        private bool _invoiceHasMoreThanOneReceipt;
        public bool InvoiceHasMoreThanOneReceipt
        {
            get
            {
                return _invoiceHasMoreThanOneReceipt;
            }

            set
            {
                if (_invoiceHasMoreThanOneReceipt == value)
                {
                    return;
                }

                _invoiceHasMoreThanOneReceipt = value;
                RaisePropertyChanged(InvoiceHasMoreThanOneReceiptPropertyName);
            }
        }
        
        public const string TotalNetPropertyName = "TotalNet";
        private string _totalNet = "";
        public string TotalNet
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
        private string _totalVat = "";
        public string TotalVat
        {
            get
            {
                return _totalVat;
            }

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

        public const string DocumentTitlePropertyName = "DocumentTitle";
        private string _documentTitle = "";
        public string DocumentTitle
        {
            get
            {
                return _documentTitle;
            }

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

        public const string ReceiptNoPropertyName = "ReceiptNo";
        private string _receiptNo = "";
        public string ReceiptNo
        {
            get
            {
                return _receiptNo;
            }

            set
            {
                if (_receiptNo == value)
                {
                    return;
                }

                var oldValue = _receiptNo;
                _receiptNo = value;
                RaisePropertyChanged(ReceiptNoPropertyName);
            }
        }

        public const string ReceiptDatePropertyName = "ReceiptDate";
        private string _receiptDate = DateTime.Now.ToString("dd-MMM-yyyy");
        public string ReceiptDate
        {
            get
            {
                return _receiptDate;
            }

            set
            {
                if (_receiptDate == value)
                {
                    return;
                }

                var oldValue = _receiptDate;
                _receiptDate = value;
                RaisePropertyChanged(ReceiptDatePropertyName);
            }
        }

        public const string CreditTermsPropertyName = "CreditTerms";
        private string _creditTerms = "";
        public string CreditTerms
        {
            get
            {
                return _creditTerms;
            }

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
            get
            {
                return _chequePayableTo;
            }

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

        public const string ServedByUserNamePropertyName = "ServedByUserName";
        private string _servedByUserName = "";
        public string ServedByUserName
        {
            get
            {
                return _servedByUserName;
            }

            set
            {
                if (_servedByUserName == value)
                {
                    return;
                }

                var oldValue = _servedByUserName;
                _servedByUserName = value;
                RaisePropertyChanged(ServedByUserNamePropertyName);
            }
        }

        public const string PreparedByJobTitlePropertyName = "PreparedByJobTitle";
        private string _preparedByJobTitle = "";
        public string PreparedByJobTitle
        {
            get
            {
                return _preparedByJobTitle;
            }

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
            get
            {
                return _datePrepared;
            }

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
            get
            {
                return _approvedByUserName;
            }

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
            get
            {
                return _dateApproved;
            }

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
            get
            {
                return _companyName;
            }

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
            get
            {
                return _address;
            }

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
            get
            {
                return _physicalAddress;
            }

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
            get
            {
                return _telNo;
            }

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
            get
            {
                return _faxNo;
            }

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
            get
            {
                return _cell;
            }

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
            get
            {
                return _vatNo;
            }

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
            get
            {
                return _pinNo;
            }

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
            get
            {
                return _email;
            }

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
            get
            {
                return _webSite;
            }

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

        public const string ReceiptRecipientCompanyNamePropertyName = "ReceiptRecipientCompanyName";
        private string _receiptRecipientCompanyName = "";
        public string ReceiptRecipientCompanyName
        {
            get
            {
                return _receiptRecipientCompanyName;
            }

            set
            {
                if (_receiptRecipientCompanyName == value)
                {
                    return;
                }

                var oldValue = _receiptRecipientCompanyName;
                _receiptRecipientCompanyName = value;
                RaisePropertyChanged(ReceiptRecipientCompanyNamePropertyName);
            }
        }

        public const string InvoiceNoPropertyName = "InvoiceNo";
        private string _invoiceNo = "";
        public string InvoiceNo
        {
            get
            {
                return _invoiceNo;
            }

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

        public const string InvoiceDatePropertyName = "InvoiceDate";
        private string _invoiceDate = DateTime.Now.ToString("dd-MMM-yyyy");
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

        public const string TheInvoicePropertyName = "TheInvoice";
        private Invoice _theInvoice = null;
        public Invoice TheInvoice
        {
            get
            {
                return _theInvoice;
            }

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

        public const string SelectedReceiptPropertyName = "SelectedReceipt";
        private Receipt _selectedReceipt = null;
        public Receipt SelectedReceipt
        {
            get
            {
                return _selectedReceipt;
            }

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

        public const string ReceiptIdLookupPropertyName = "ReceiptIdLookup";
        private Guid _receiptIdLookup = Guid.Empty;
        public Guid ReceiptIdLookup
        {
            get
            {
                return _receiptIdLookup;
            }

            set
            {
                if (_receiptIdLookup == value)
                {
                    return;
                }

                var oldValue = _receiptIdLookup;
                _receiptIdLookup = value;
                RaisePropertyChanged(ReceiptIdLookupPropertyName);
            }
        }


        public const string FamerNoPropertyName = "FamerNo";
        private string _farmerNo = "";
        public string FamerNo
        {
            get
            {
                return _farmerNo;
            }

            set
            {
                if (_farmerNo == value)
                {
                    return;
                }

                var oldValue = _farmerNo;
                _farmerNo = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(FamerNoPropertyName);

            }
        }

        public const string DeliveredByPropertyName = "DeliveredBy";
        private string _deliveredBy = "";
        public string DeliveredBy
        {
            get
            {
                return _deliveredBy;
            }

            set
            {
                if (_deliveredBy == value)
                {
                    return;
                }

                var oldValue = _deliveredBy;
                _deliveredBy = value;

                // Remove one of the two calls below
                RaisePropertyChanged(DeliveredByPropertyName);

            }
        }

        public const string FarmerNamePropertyName = "FarmerName";
        private string _farmerName = "";
        public string FarmerName
        {
            get
            {
                return _farmerName;
            }

            set
            {
                if (_farmerName == value)
                {
                    return;
                }

                var oldValue = _farmerName;
                _farmerName = value;



                // Update bindings, no broadcast
                RaisePropertyChanged(FarmerNamePropertyName);

            }
        }

        public const string FarmerIdPropertyName = "FarmerId";
        private Guid _farmerId = Guid.Empty;
        public Guid FarmerId
        {
            get
            {
                return _farmerId;
            }

            set
            {
                if (_farmerId == value)
                {
                    return;
                }

                var oldValue = _farmerId;
                _farmerId = value;


                // Update bindings, no broadcast
                RaisePropertyChanged(FarmerIdPropertyName);

            }
        }


        public const string TotalWeightPropertyName = "TotalWeight";
        private decimal _totalWeight = 0;
        public decimal TotalWeight
        {
            get
            {
                return _totalWeight;
            }

            set
            {
                if (_totalWeight == value)
                {
                    return;
                }

                var oldValue = _totalWeight;
                _totalWeight = value;
                RaisePropertyChanged(TotalWeightPropertyName);
            }
        }

        #endregion

        #region Methods
        public void SetId(ViewModelMessage obj)
        {
            ReceiptIdLookup = obj.Id;

        }

        private void PageLoaded()
        {
            if(ReceiptIdLookup ==Guid.Empty)
            {
                MessageBox.Show("Error loading receipt.", "Distributr Error", MessageBoxButton.OK);
                ReturnToListCommand.Execute(null);
            }
            GenericReceiptVisibility=Visibility.Collapsed;
            DetailedReceiptVisibility=Visibility.Visible;
           Load();
            
        }

        void OnPrintCommand(Panel p)
        {
            if(p==null)return;
            using (var c =NestedContainer)
            {
                var reprint = Using<IPrintedReceiptsTrackerRepository>(c).IsReprint(ReceiptIdLookup);
                var control = FindVisualChildren<UserControl>(p).FirstOrDefault(n=>n.Visibility==Visibility.Visible);
               if(control !=null)
               {
                   var receiptLabel = control.FindName("lblOfficialReceipt") as Label;
                   if (receiptLabel != null)
                   {
                       receiptLabel.Content = reprint ? "OFFICIAL RECEIPT-REPRINT" : "OFFICIAL RECEIPT";
                  
                   }
               }

                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(p, "Print Receipt");
                    if (!reprint)
                        Using<IPrintedReceiptsTrackerRepository>(c).Log(ReceiptIdLookup);
                }
            }

        }

        public void Load()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var receipt = Using<IReceiptRepository>(c).GetById(ReceiptIdLookup);
                var costcentreRepository = Using<ICostCentreRepository>(c);
                TheInvoice = Using<IInvoiceRepository>(c).GetById(receipt.InvoiceId);
                OrderId = TheInvoice.OrderId;
                CostCentre cc = costcentreRepository.GetById(receipt.DocumentIssuerCostCentre.Id);
                CostCentre cc2 = costcentreRepository.GetById(receipt.DocumentRecipientCostCentre.Id);
                Distributor distributr = null;
                if (cc is Distributor)
                {
                    distributr = cc as Distributor;
                }
                if (cc2 is Distributor)
                {
                    distributr = cc2 as Distributor;
                }
                //contact info


                CompanyName = cc.Name;
                Contact contact = null;
                if (distributr!=null && distributr.Contact.Any())
                {
                    contact = distributr.Contact.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              distributr.Contact.FirstOrDefault();
                }
                if (contact == null && distributr!=null)
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
                WebSite = "";

                InvoiceNo = TheInvoice.DocumentReference;
                InvoiceDate = TheInvoice.DocumentDateIssued.ToString("dd-MMM-yyyy");

                ReceiptNo = receipt.DocumentReference;
                ReceiptDate = receipt.DocumentDateIssued.ToString("dd-MMM-yyyy");
                ChequePayableTo = receipt.DocumentIssuerCostCentre.Name;
                ServedByUserName = receipt.DocumentIssuerUser.Username;
                PreparedByJobTitle = "Accountant";
                ReceiptRecipientCompanyName = receipt.DocumentRecipientCostCentre.Name;

                var rli =
                    receipt.LineItems
                    .Where(n => n.LineItemType == OrderLineItemType.PostConfirmation)
                    .Select((n, i) =>
                        new ReceiptLineItem
                            {
                                LineItemNumber=i +1,LineItemId=n.Id,
                                LineItemSequenceNo=n.LineItemSequenceNo,
                                Description=n.Description,
                                InvoiceId=receipt.InvoiceId,
                                IsNew=n.IsNew,
                                PaymentType=n.PaymentType ==PaymentMode.MMoney? n.MMoneyPaymentType: (n.PaymentType).ToString(),
                                PaymentTypeReference=n.PaymentRefId,
                                Value=n.Value
                            });

                var invoiceTotalNet = TheInvoice.TotalNet;
                var invoiceTotalVat = TheInvoice.TotalVat;
                var invoiceTotalGross = TheInvoice.TotalGross;

                TotalGross = rli.Sum(n => n.Value).ToString("N2");
                TotalNet = ((Convert.ToDecimal(TotalGross) / invoiceTotalGross) * invoiceTotalNet).ToString("N2");
                TotalVat = ((Convert.ToDecimal(TotalGross) / invoiceTotalGross) * invoiceTotalVat).ToString("N2");

                ReceiptLineItemsList.Clear();
                foreach (var item in rli) ReceiptLineItemsList.Add(item);

                LoadRelatedReceipts(TheInvoice.Id);
            }
        }

        void LoadRelatedReceipts(Guid invoiceId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                if( InvoiceReceipts.Any())
                    InvoiceReceipts.Clear();
                Using<IReceiptRepository>(c).GetByInvoiceId(invoiceId)
                               .Where(n => n.Total > 0 && n.Id !=ReceiptIdLookup)
                               .ToList()
                               .ForEach(InvoiceReceipts.Add);
                InvoiceHasMoreThanOneReceipt = InvoiceReceipts.Count != 0;
                SelectedReceipt = new Receipt(Guid.Empty)
                {
                    DocumentReference = GetLocalText("sl.invoice.receiptlist.default"
                        /*"--Select Receipt To View--"*/)
                };
            }
        }

        void ReturnToView()
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
               Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id =OrderId });
                NavigateCommand.Execute(url);
            }
        }

        void RunViewInvoiceCommand()
        {
            const string uri = "/views/invoicedocument/invoicedocument.xaml";
            Messenger.Default.Send<ViewModelMessage>(new ViewModelMessage { Id = TheInvoice.OrderId });
            NavigateCommand.Execute(uri);
        }

   
        void RunViewReceiptCommand()
        {
            if (SelectedReceipt.Id == Guid.Empty)
                MessageBox.Show("Select a receipt to view.", "Distributr: View Receipt", MessageBoxButton.OK);
            else
            {
                ReceiptIdLookup = SelectedReceipt.Id;
                Load();
            }
        }

        #endregion
    }

    public class ReceiptLineItem
    {
        public int LineItemNumber { get; set; }
        public Guid LineItemId { get; set; } //Id
        public Guid InvoiceId { get; set; }
        public bool IsNew { get; set; }
        public int LineItemSequenceNo { get; set; }
        public string PaymentType { get; set; }//Mode
        public string PaymentTypeReference { get; set; }
        public decimal Value { get; set; }//Amount
        public string Description { get; set; }
    }
}