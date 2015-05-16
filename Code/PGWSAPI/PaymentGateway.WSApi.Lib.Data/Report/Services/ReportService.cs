using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Report.Domain;
using PaymentGateway.WSApi.Lib.Report.Services;

namespace PaymentGateway.WSApi.Lib.Data.Report.Services
{
    public class ReportService : IReportService
    {
        private PGDataContext _ctx;

        public ReportService(PGDataContext ctx)
        {
            _ctx = ctx;
        }

        public List<ServiceProviderReport> GetServiceProviderReport(DateTime startdate, DateTime enddate)
        {
            return _ctx.tblServiceProvider.Select(s=> new ServiceProviderReport
                                                           {
                                                               AllowOverPayment=s.AllowOverPayment,
                                                               AllowPartialPayment=s.AllowPartialPayment,
                                                               Code=s.SPCode,
                                                               Currency=s.Currecy,
                                                               Name=s.SPName,
                                                               SdpAppId=s.SDP_APP_ID,
                                                               SdpPassword=s.SDP_Password,
                                                               Sid=s.SPId,
                                                               SubscriberId=s.SubscriberId,
                                                           }).ToList();
        }
    }
}
