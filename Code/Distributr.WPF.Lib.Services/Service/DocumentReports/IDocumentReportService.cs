using System;
using Distributr.WPF.Lib.Services.DocumentReports;
using Distributr.WPF.Lib.Services.DocumentReports.Receipt;

namespace Distributr.WPF.Lib.Services.Service.DocumentReports
{
    public interface IDocumentReportService
    {
        DocumentReportContainer GetOrderData(Guid orderId);
        DocumentReportContainer GetDispatchNote(Guid dispatchNoteId);

    }
}
