using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.Reporting
{
    public static class ReportCollective
    {
        private const string ReportDomain = "Distributr.WPF.UI.Views.DocumentReports.RdlcReports.";
        public static readonly string DispatchNoteReport = ReportDomain + "rptDispatchNote.rdlc";
        public static readonly string InvoiceReport = ReportDomain + "rptInvoice.rdlc";
        public static readonly string ReceiptReport = ReportDomain + "rptReceipt.rdlc";
        public static readonly string OrderDocumentReport = ReportDomain + "rptOrder.rdlc";
        public static readonly string GRNReport = ReportDomain + "rptGRN.rdlc";
        public static readonly string PurchaseOrderDocumentReport = ReportDomain + "rptPurchaseOrder.rdlc";
        public static readonly string StockistOrderDocumentReport = ReportDomain + "rptStockistOrder.rdlc";
        public static readonly string CompanyHeaderReport = ReportDomain + "rptCompany.rdlc";
        public static readonly string CompanyLetterHead = ReportDomain + "rptCompanyLetterHead.rdlc";
        public static readonly string CompanyLetterHeadLandscape = ReportDomain + "rptCompanyLetterHeadLandscape.rdlc";
        public static readonly string ApprovedOrdersReport = ReportDomain + "ApprovedOrdersReport.rdlc";
        public static readonly string PendingOrdersReport = ReportDomain + "PendingOrdersReport.rdlc";
        
    }
}
