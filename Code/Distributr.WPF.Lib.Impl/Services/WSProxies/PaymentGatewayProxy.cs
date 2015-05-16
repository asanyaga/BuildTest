using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Distributr.WPF.Lib.Services.Service.Payment;
using Distributr.WPF.Lib.Services.Service.WSProxies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client.Response;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Request;
using PaymentGateway.WSApi.Lib.Domain.Payments.SDP.Response;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;
using log4net;

namespace Distributr.WPF.Lib.Impl.Services.WSProxies
{
    public class PaymentGatewayProxy : IPaymentGatewayProxy
    {
        private ILog _log = LogManager.GetLogger("PaymentGatewayProxy");
        private IPaymentService _paymentService;

        public PaymentGatewayProxy(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private HttpClient setupHttpClient()
        {
            HttpClient client = new HttpClient();
            string url = _paymentService.GetPGWebApiUrl();
            if (!url.EndsWith("/")) url += "/";
            client.BaseAddress = new Uri(url);
            return client;
        }

        public async Task<string> AboutPGBridgeWebAPI()
        {
            string about = "";
            HttpClient httpClient = setupHttpClient();
            string url = "api/payment/getabout";
            try
            {
                var response = await httpClient.GetAsync(url);
                about = await response.Content.ReadAsAsync<string>();
            }
            catch(Exception ex)
            {
                string error = "Failed to retrive information about service.";
                _log.Error(error);
                about = error;
            }
            return about;
        }

        public async Task<PaymentInstrumentResponse> GetPaymentInstrumentListAsync(PaymentInstrumentRequest request)
        {
            PaymentInstrumentResponse _response = new PaymentInstrumentResponse();
            HttpClient httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "api/bridge/payment/getpaymentinstrumentlist";
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, request);
                _response = await response.Content.ReadAsAsync<PaymentInstrumentResponse>();
            }
            catch (Exception ex)
            {
                string error = "Failed to retrieve payment instrument list.\n" +
                               (ex.InnerException == null ? "" : ex.InnerException.Message);
                _log.Error(error);
                _response.StatusCode = "Error";
                _response.StatusDetail = error;
            }

            return _response;
        }

        public async Task<PaymentResponse> PaymentRequestAsync(PaymentRequest request)
        {

            PaymentResponse _response = new PaymentResponse();
            HttpClient httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "api/bridge/payment/paymentrequest";
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, request);
                _response = await response.Content.ReadAsAsync<PaymentResponse>();
            }
            catch (Exception ex)
            {
                string error = "Failed to retrieve payment response.\n" +
                               (ex.InnerException == null ? "" : ex.InnerException.Message);
                _log.Error(error);
                _response.StatusCode = "Error";
                _response.StatusDetail = error;
            }

            return _response;
        }

        public async Task<PaymentNotificationResponse> GetPaymentNotificationAsync(PaymentNotificationRequest request)
        {
            PaymentNotificationResponse _response = new PaymentNotificationResponse();
            HttpClient httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "api/bridge/payment/getpaymentnotification";
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, request);
                string data = await response.Content.ReadAsStringAsync();
                
                JObject jo =JsonConvert.DeserializeObject<JObject>(data);
                var respodata = jo["Response"];
                _response = respodata.ToObject<PaymentNotificationResponse>();
                var datelastsyncInfo = jo["Response"]["PaymentNotificationDetails"];
                var items = datelastsyncInfo.ToObject <List<PaymentNotificationListItem>>();// JsonConvert.DeserializeObject<List<PaymentNotificationListItem>>();
              
                _response.PaymentNotificationDetails = items;
            }
            catch (Exception ex)
            { 
                string error = "Failed to retrieve payment notification.\n" +
                               (ex.InnerException == null ? "" : ex.InnerException.Message);
                _log.Error(error);
                _response.StatusCode = "Error";
                _response.StatusDetail = error;
            }

            return _response;
        }

        public async Task<BuyGoodsNotificationResponse> GetBuyGoodsNotificationAsync(BuyGoodsNotificationRequest request)
        {
            BuyGoodsNotificationResponse _response = new BuyGoodsNotificationResponse();
            HttpClient httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "api/bridge/payment/getbuygoodsnotification";
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, request);
                _response = await response.Content.ReadAsAsync<BuyGoodsNotificationResponse>();
            }
            catch (Exception ex)
            { 
                string error = "Failed to retrieve payment notification.\n" +
                               (ex.InnerException == null ? "" : ex.InnerException.Message);
                _log.Error(error);
                _response.StatusCode = "Error";
                _response.StatusDetail = error;
            }

            return _response;
        }

        public async Task<SDPPaymentNotificationResponse> ReceiveAsynchPaymentNotificationTest(SDPPaymentNotificationRequest request)
        {
            SDPPaymentNotificationResponse _response = new SDPPaymentNotificationResponse();
            HttpClient httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "api/bridge/payment/postpaymentnotification";
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, request);
                _response = await response.Content.ReadAsAsync<SDPPaymentNotificationResponse>();
            }
            catch (Exception ex)
            {
                string error = "Failed to retrieve payment notification.\n" +
                               (ex.InnerException == null ? "" : ex.InnerException.Message);
                _log.Error(error);
                _response.statusCode = "Error";
                _response.statusDetail = error;
            }

            return _response;
        }

        public async Task<DocSMSResponse> SendDocSms(DocSMS docSms)
        {
            DocSMSResponse _response = new DocSMSResponse {SmsStatus = SmsStatuses.Pending};
            HttpClient httpClient = setupHttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string url = "api/gateway/sms/send";
            try
            {
                var response = await httpClient.PostAsJsonAsync(url, docSms);
                _response = await response.Content.ReadAsAsync<DocSMSResponse>();
            }
            catch (Exception ex)
            {
                string error = "Failed to forward sms for sending.\n" +
                               (ex.InnerException == null ? "" : ex.InnerException.Message);
                _response.SdpResponseCode = "Error";
                _response.SdpResponseStatus = error;
            }
            return _response;
        }
    }
}
