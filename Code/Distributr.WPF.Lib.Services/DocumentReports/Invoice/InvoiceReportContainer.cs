using System.Collections.Generic;

namespace Distributr.WPF.Lib.Services.DocumentReports.Invoice
{
    public class InvoiceReportContainer
    {
        public InvoiceReportContainer()
        {
            InvoiceHeader = new InvoiceReportHeader();
            InvoiceLineItems = new List<InvoiceReportLineItem>();
            InvoiceDeductionsLineItems = new List<InvoiceReportLineItem>();
            PaymentInformationLineItems = new List<InvoiceReportLineItem>();
        }
        
        public InvoiceReportHeader InvoiceHeader { get; set; }

        public CompanyHeaderReport CompanyHeader { get; set; }
        
        public List<InvoiceReportLineItem> InvoiceLineItems { get; set; }
        
        public List<InvoiceReportLineItem> InvoiceDeductionsLineItems { get; set; }
        
        public List<InvoiceReportLineItem> PaymentInformationLineItems { get; set; }
    }
}
