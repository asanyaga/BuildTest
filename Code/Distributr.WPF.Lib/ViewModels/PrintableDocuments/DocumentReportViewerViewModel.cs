using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.WPF.Lib.Reporting;
using Distributr.WPF.Lib.Services.DocumentReports;
using Distributr.WPF.Lib.Services.DocumentReports.GRN;
using Distributr.WPF.Lib.Services.DocumentReports.Invoice;
using Distributr.WPF.Lib.Services.DocumentReports.Order;
using Distributr.WPF.Lib.Services.DocumentReports.Receipt;
using GalaSoft.MvvmLight.Command;
using Microsoft.Reporting.WinForms;

namespace Distributr.WPF.Lib.ViewModels.PrintableDocuments
{
    public partial class DocumentReportViewerViewModel : DistributrViewModelBase
    {
        public DocumentReportViewerViewModel()
        {
            OrderDispatchNotes = new ObservableCollection<DispatchNote>();
            InvoiceReceipts = new ObservableCollection<Receipt>();
        }

        #region properties

        public ObservableCollection<DispatchNote> OrderDispatchNotes { get; set; }
        public ObservableCollection<Receipt> InvoiceReceipts { get; set; }

        public const string SelectedDispatchNotePropertyName = "SelectedDispatchNote";
        private DispatchNote _selectedDispatchNote;

        public DispatchNote SelectedDispatchNote
        {
            get { return _selectedDispatchNote; }

            set
            {
                if (_selectedDispatchNote == value)
                {
                    return;
                }
                _selectedDispatchNote = value;
                RaisePropertyChanged(SelectedDispatchNotePropertyName);
            }
        }


        public const string TitlePropertyName = "Title";
        private string _title = "Printable Document View";

        public string Title 
        {
            get { return _title; }

            set
            {
                if (_title == value)
                {
                    return;
                }
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        public const string SelectedReceiptPropertyName = "SelectedReceipt";
        private Receipt _selectedReceipt;

        public Receipt SelectedReceipt
        {
            get { return _selectedReceipt; }

            set
            {
                if (_selectedReceipt == value)
                {
                    return;
                }
                _selectedReceipt = value;
                RaisePropertyChanged(SelectedReceiptPropertyName);
            }
        }

        public const string DispatchNotePanelVisibilityPropertyName = "DispatchNotePanelVisibility";
        private Visibility _dispatchNotePanelVisibility = Visibility.Collapsed;

        public Visibility DispatchNotePanelVisibility
        {
            get { return _dispatchNotePanelVisibility; }

            set
            {
                if (_dispatchNotePanelVisibility == value)
                {
                    return;
                }
                _dispatchNotePanelVisibility = value;
                RaisePropertyChanged(DispatchNotePanelVisibilityPropertyName);
            }
        }

        public const string SelectReceiptVisibilityPropertyName = "SelectReceiptVisibility";
        private Visibility _selectReceiptVisibility = Visibility.Collapsed;

        public Visibility SelectReceiptVisibility
        {
            get { return _dispatchNotePanelVisibility; }

            set
            {
                if (_selectReceiptVisibility == value)
                {
                    return;
                }
                _selectReceiptVisibility = value;
                RaisePropertyChanged(SelectReceiptVisibilityPropertyName);
            }
        }

        public const string ReceiptPanelVisibilityPropertyName = "ReceiptPanelVisibility";
        private Visibility _receiptPanelVisibility = Visibility.Collapsed;

        public Visibility ReceiptPanelVisibility
        {
            get { return _receiptPanelVisibility; }

            set
            {
                if (_receiptPanelVisibility == value)
                {
                    return;
                }
                _receiptPanelVisibility = value;
                RaisePropertyChanged(ReceiptPanelVisibilityPropertyName);
            }
        }

        public Guid DocumentId { get; set; }

        public DocumentType DocumentType { get; set; }

        public OrderType OrderType { get; set; }

        private RelayCommand<ReportViewer> _reportViewer_LoadCommand = null;
        public RelayCommand<ReportViewer> ReportViewer_LoadCommand
        {
            get
            {
                return _reportViewer_LoadCommand ??
                       (_reportViewer_LoadCommand = new RelayCommand<ReportViewer>(LoadDocumentReportViewer));
            }
        }

        private RelayCommand _loadSelectedReceiptCommand = null;
        public RelayCommand LoadSelectedReceiptCommand
        {
            get
            {
                return _loadSelectedReceiptCommand ??
                       (_loadSelectedReceiptCommand = new RelayCommand(LoadSelectedReceipt));
            }
        }

        private RelayCommand _loadInvoiceCommand = null;
        public RelayCommand LoadInvoiceCommand
        {
            get
            {
                return _loadInvoiceCommand ??
                       (_loadInvoiceCommand = new RelayCommand(LoadInvoice));
            }
        }

        private CompanyHeaderReport _companyHeader;
        private CompanyHeaderReport CompanyHeader
        {
            get
            {
                using (var c = NestedContainer)
                {
                    if (_companyHeader == null)
                    {
                        var hub = Using<ICostCentreRepository>(c).GetById(GetConfigParams().CostCentreId);
                        //var company = Using<ICostCentreRepository>(c).GetById(hub.ParentCostCentre.Id);
                        var compContacts = Using<IContactRepository>(c).GetByContactsOwnerId(hub.Id);
                        var defaultContact = compContacts.FirstOrDefault(
                            n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                                             compContacts.FirstOrDefault();
                        //var defaultContact = hub.Contact.FirstOrDefault(
                        //    n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                        //                     hub.Contact.FirstOrDefault();

                        _companyHeader = new CompanyHeaderReport
                                            {
                                                //CompanyName = company.Name
                                                CompanyName = hub.Name
                                            };
                        if (defaultContact != null)
                        {
                            _companyHeader.Telephone = defaultContact.BusinessPhone;
                            if (defaultContact.MobilePhone != "")
                                _companyHeader.CellNo += defaultContact.MobilePhone;
                            _companyHeader.PostalAddress = defaultContact.PostalAddress;
                            _companyHeader.PhysicalAddress = defaultContact.PhysicalAddress;
                            _companyHeader.Fax = defaultContact.Fax;
                            _companyHeader.Email = defaultContact.Email;
                        }
                        if (defaultContact == null)
                        {
                            _companyHeader.CompanyName = "Virtual City";
                            _companyHeader.Telephone = "+254 - 20 - 3873341 / 2191";
                            _companyHeader.Telephone = "+254 - 703 - 091300";
                            _companyHeader.PostalAddress = "P.O.Box 76460-00508 Nairobi, Kenya ";
                            _companyHeader.PhysicalAddress = "Virtual House, Ring Road, Kilimani";
                            _companyHeader.Fax = " +254 - 20 - 3876248";
                            _companyHeader.Email = "inforamtion-centre@virtualcity.co.ke";
                        }

                        _companyHeader.ContactsConcat = _companyHeader.PhysicalAddress + ", "
                                                       + _companyHeader.PostalAddress + ", "
                                                       + (_companyHeader.Fax == "" ? "" : "Fax: " + _companyHeader.Fax + ", ")
                                                       + "Tel: " + _companyHeader.Telephone + ","
                                                       + "Cell: " + _companyHeader.CellNo + ","
                                                       + "Email: "+_companyHeader.Email + "."
                                                       ;
                    }

                    return _companyHeader;
                }
            }
            set { _companyHeader = value; }
        }

        #endregion

        #region methods

        public void SetUp()
        {
            DispatchNotePanelVisibility = Visibility.Collapsed;
            ReceiptPanelVisibility = Visibility.Collapsed;
            OrderDispatchNotes.Clear();

            using (var c = NestedContainer)
            {
                if (DocumentType == DocumentType.DispatchNote)
                {
                    var dispatchNotes = Using<IDispatchNoteRepository>(c).GetByOrderId(DocumentId);
                    dispatchNotes.ForEach(OrderDispatchNotes.Add);
                    SelectedDispatchNote = OrderDispatchNotes.FirstOrDefault();
                }
            }
            if (OrderDispatchNotes.Count > 1) DispatchNotePanelVisibility = Visibility.Visible;
        }

        void LoadInvoiceReceipts(Guid invoiceId)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                InvoiceReceipts.Clear();
                Using<IReceiptRepository>(c).GetByInvoiceId(invoiceId)
                               .Where(n => n.Total > 0 && n.Id != SelectedReceipt.Id)//GO: I exclude current receipt
                               .ToList()
                               .ForEach(InvoiceReceipts.Add);
                SelectReceiptVisibility = InvoiceReceipts.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                
                SelectedReceipt = new Receipt(Guid.Empty)
                {
                    DocumentReference = GetLocalText("sl.invoice.receiptlist.default"
                        /*"--Select Receipt To View--"*/)
                };
            }
        }

        private void LoadInvoice()
        {
        }

        private void LoadSelectedReceipt()
        {
        }

        private void LoadDocumentReportViewer(ReportViewer reportViewer)
        {
            using (var c = NestedContainer)
            {
                ReportDataSource reportDataSourceHeader = null;
                ReportDataSource reportDataSourceLineItems = null;
                ReportDataSource reportDataSourcePayments = null;
                Stream stream = null;
                Stream subReportStream = null;
                string subReportName = null;

                reportViewer.Reset();

                switch (DocumentType)
                {
                    case DocumentType.Order:
                        OrderReportContainer order = GetOrderData(DocumentId);
                        reportDataSourceHeader = new ReportDataSource("dsOrder", new List<OrderHeader> { order.DocumentHeader });
                        reportDataSourceLineItems = new ReportDataSource("dsLineItems", order.LineItems);
                        reportDataSourcePayments=new ReportDataSource("dsPayments",order.PaymentInfoList);

                        if (order.DocumentHeader.OrderTypeStr == OrderType.DistributorToProducer.ToString())
                            stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.PurchaseOrderDocumentReport);
                        else if(order.DocumentHeader.OrderTypeStr == OrderType.SalesmanToDistributor.ToString())
                            stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.StockistOrderDocumentReport);
                        else
                            stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.OrderDocumentReport);

                        reportViewer.LocalReport.DisplayName = "Orders Report";

                        subReportName = "rptCompanyLetterHeadLandscape.rdlc";
                        subReportStream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.CompanyLetterHeadLandscape);

                        break;
                    case DocumentType.DispatchNote:
                        OrderReportContainer dispatchNote = GetDispatchNote(SelectedDispatchNote.Id);
                        reportDataSourceHeader = new ReportDataSource("dsOrder", new List<OrderHeader> { dispatchNote.DocumentHeader });
                        reportDataSourceLineItems = new ReportDataSource("dsLineItems", dispatchNote.LineItems);
                        stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.DispatchNoteReport);
                        reportViewer.LocalReport.DisplayName = "Dispatch Note";

                        subReportName = "rptCompanyLetterHead.rdlc";
                        subReportStream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.CompanyLetterHead);

                        break;
                    case DocumentType.Invoice:
                        LoadInvoiceReportViewer(reportViewer);
                        return;
                    case DocumentType.Receipt:
                        ReceiptReportContainer receipt = GetReceipt(DocumentId);
                        reportDataSourceHeader = new ReportDataSource("dsReceiptHeader", new List<ReceiptReportHeader>{receipt.DocumentHeader});
                        reportDataSourceLineItems = new ReportDataSource("dsReceiptLineItems", receipt.LineItems);
                        stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.ReceiptReport);
                        reportViewer.LocalReport.DisplayName = "Receipt";

                        subReportName = "rptCompanyLetterHead.rdlc";
                        subReportStream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.CompanyLetterHead);
                        break;
                    case DocumentType.InventoryReceivedNote:
                        GRNReportContainer grn = GetGRNData(DocumentId);
                        reportDataSourceHeader = new ReportDataSource("dsGRN", new List<GRNHeader> {grn.DocumentHeader});
                        reportDataSourceLineItems = new ReportDataSource("dsLineItems", grn.LineItems);
                        stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.GRNReport);
                        reportViewer.LocalReport.DisplayName = "GRN";

                        subReportName = "rptCompanyLetterHead.rdlc";
                        subReportStream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.CompanyLetterHead);
                        break;
                }

                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.EnableHyperlinks = true;

                reportViewer.LocalReport.DataSources.Add(reportDataSourceHeader);
                reportViewer.LocalReport.DataSources.Add(reportDataSourceLineItems);
                if (reportDataSourcePayments !=null)
                reportViewer.LocalReport.DataSources.Add(reportDataSourcePayments);

                reportViewer.LocalReport.LoadSubreportDefinition(subReportName, subReportStream);

                reportViewer.LocalReport.ShowDetailedSubreportMessages = true;
                reportViewer.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;

                reportViewer.LocalReport.LoadReportDefinition(stream);

                reportViewer.LocalReport.Refresh();
                reportViewer.RefreshReport();

                Title = reportViewer.LocalReport.DisplayName;
            }
        }

        void LoadInvoiceReportViewer(ReportViewer reportViewer)
        {
            reportViewer.Reset();
            ReportDataSource reportDataSourceHeader = null;
            ReportDataSource reportDataSourceLineItems = null;
            Stream stream = null;
            Stream subReportStream = null;
            string subReportName = null;

            var invoiceReport = GetInvoice(DocumentId);
            reportDataSourceHeader = new ReportDataSource("dsInvoiceHeader",
                                                          new List<InvoiceReportHeader> { invoiceReport.InvoiceHeader });
            reportDataSourceLineItems = new ReportDataSource("dsInvoiceLineItems", invoiceReport.InvoiceLineItems);
            var reportDataSourcePaymentLineItems = new ReportDataSource("dsInvoicePayments", invoiceReport.PaymentInformationLineItems);
            var reportDataSourceDeductions = new ReportDataSource("dsInvoiceDeductions", invoiceReport.InvoiceDeductionsLineItems);

            stream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.InvoiceReport);
            reportViewer.LocalReport.DisplayName = "Invoice";

            subReportName = "rptCompanyLetterHead.rdlc";
            subReportStream = Assembly.GetEntryAssembly().GetManifestResourceStream(ReportCollective.CompanyLetterHead);

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.EnableHyperlinks = true;

            reportViewer.LocalReport.DataSources.Add(reportDataSourceHeader);
            reportViewer.LocalReport.DataSources.Add(reportDataSourceLineItems);
            reportViewer.LocalReport.DataSources.Add(reportDataSourcePaymentLineItems);
            reportViewer.LocalReport.DataSources.Add(reportDataSourceDeductions);

            reportViewer.LocalReport.LoadSubreportDefinition(subReportName, subReportStream);

            reportViewer.LocalReport.ShowDetailedSubreportMessages = true;
            reportViewer.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;

            reportViewer.LocalReport.LoadReportDefinition(stream);

            reportViewer.LocalReport.Refresh();
            reportViewer.RefreshReport();

            Title = reportViewer.LocalReport.DisplayName;
        }

        private void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            string dataSourceName = e.DataSourceNames[0];
            e.DataSources.Add(new ReportDataSource(dataSourceName, new List<CompanyHeaderReport> {CompanyHeader}));
        }

        #endregion

    }
}
