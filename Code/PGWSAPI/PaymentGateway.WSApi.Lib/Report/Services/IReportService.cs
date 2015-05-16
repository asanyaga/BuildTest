using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Report.Domain;

namespace PaymentGateway.WSApi.Lib.Report.Services
{
   public interface IReportService
   {
       List<ServiceProviderReport> GetServiceProviderReport(DateTime startdate, DateTime enddate);
   }
}
