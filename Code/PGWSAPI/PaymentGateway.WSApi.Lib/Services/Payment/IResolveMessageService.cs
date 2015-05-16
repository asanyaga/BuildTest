using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.MasterData;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP;
using PaymentGateway.WSApi.Lib.MessageResults;

namespace PaymentGateway.WSApi.Lib.Services.Payment
{
    public interface IResolveMessageService
    {
        void ProcessClientRequest(ClientRequestResponseBase crrMessage, ServiceProvider serviceProvider, out ServerRequestBase sms);
        void ProcessSDPRequest(ServerRequestBase sdpRequest, out ClientRequestResponseBase crrRequest,out string subscriber,out Guid sip);
        ServiceProvider GetServiceProvider(Guid distributorCCId);
        ClientRequestResponseBase ProcessClientPaymentNotificationRequest(ClientRequestResponseBase crrReqMsg);
        List<PaymentNotification> ProcessPaymentReportRequest(Guid serviceProviderId, DateTime startDate, DateTime endDate);
    }
     public interface ISmsQueryResolverService
     {
         string SmsQuery(SmsQuery query);
     }

    public class SmsQuery
    {
        public string Code { get; set; }
        public string ClientKeyWord { get; set; }
        public string ActionKeyWord { get; set; }
        public string PhoneNumber { get; set; }
    }
}
