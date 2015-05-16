using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;
using PaymentGateway.WSApi.Lib.MessageResults;

namespace Distributr.WPF.Lib.Services.Service.WSProxies
{
    public interface IPaymentGatewayProxy
    {
        Task<string> AboutPGBridgeWebAPI();
        Task<PaymentInstrumentResponse> GetPaymentInstrumentListAsync(PaymentInstrumentRequest request);
        Task<PaymentResponse> PaymentRequestAsync(PaymentRequest request);
        Task<PaymentNotificationResponse> GetPaymentNotificationAsync(PaymentNotificationRequest request);
        Task<BuyGoodsNotificationResponse> GetBuyGoodsNotificationAsync(BuyGoodsNotificationRequest request);
        Task<SDPPaymentNotificationResponse> ReceiveAsynchPaymentNotificationTest(SDPPaymentNotificationRequest request);
        Task<DocSMSResponse> SendDocSms(DocSMS docSms);
    }
}
